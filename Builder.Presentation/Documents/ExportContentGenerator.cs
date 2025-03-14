using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Aurora.Documents.ExportContent;
using Aurora.Documents.ExportContent.Equipment;
using Aurora.Documents.ExportContent.Notes;
using Aurora.Documents.Sheets;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Strings;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.ViewModels.Shell.Items;

namespace Builder.Presentation.Documents
{
    public class ExportContentGenerator : IExportContentProvider
    {
        private readonly CharacterManager _manager;

        private readonly CharacterSheetConfiguration _configuration;

        public ExportContentGenerator(CharacterManager manager, CharacterSheetConfiguration configuration)
        {
            _manager = manager;
            _configuration = configuration;
        }

        public EquipmentExportContent GetEquipmentContent()
        {
            EquipmentExportContent equipmentExportContent = new EquipmentExportContent();
            CharacterInventory inventory = _manager.Character.Inventory;
            foreach (RefactoredEquipmentItem item2 in GetItems(inventory.Items.Where((RefactoredEquipmentItem x) => x.IncludeInEquipmentPageInventory)))
            {
                if (item2.IsStored)
                {
                    continue;
                }
                InventoryItemExportContent item = new InventoryItemExportContent(item2.GetEquipmentPageName())
                {
                    Amount = item2.Amount.ToString(),
                    Weight = item2.GetWeight().ToString("#.##", CultureInfo.InvariantCulture),
                    IsEquipped = item2.IsActivated
                };
                item.ReferenceId = item2.Item.Id;
                if (item2.Item.CalculableWeight == 0m)
                {
                    item.Weight = "—";
                }
                if (!item2.IsStackable && (item2.Item.Type == "Weapon" || item2.Item.Type == "Armor") && !item2.IsAdorned && !item2.IsEquipped)
                {
                    InventoryItemExportContent inventoryItemExportContent = equipmentExportContent.AdventuringGear.FirstOrDefault((InventoryItemExportContent x) => x.ReferenceId.Equals(item.ReferenceId) && !x.IsEquipped);
                    if (inventoryItemExportContent != null)
                    {
                        int num = int.Parse(inventoryItemExportContent.Amount);
                        inventoryItemExportContent.Amount = (num + 1).ToString();
                        inventoryItemExportContent.Weight = ((decimal)(num + 1) * item2.Item.CalculableWeight).ToString("#.##", CultureInfo.InvariantCulture);
                        continue;
                    }
                }
                if (_configuration.IsFormFillable && !_configuration.IncludeFormatting && item.IsEquipped)
                {
                    item.Name = item.Name ?? "";
                }
                if (item2.IncludeInTreasure)
                {
                    equipmentExportContent.Valuables.Add(item);
                    continue;
                }
                if (item2.Item.Type.Equals("Magic Item") || item2.IsAdorned)
                {
                    equipmentExportContent.MagicItems.Add(item);
                    continue;
                }
                switch (item2.Item.Category)
                {
                    case "Art":
                    case "Art Objects":
                    case "Gemstones":
                    case "Treasure":
                    case "Valuables":
                        Logger.Warning("should include " + item.Name + " in treasure, but it's not flagged as 'IncludeInTreasure'");
                        equipmentExportContent.Valuables.Add(item);
                        break;
                    default:
                        equipmentExportContent.AdventuringGear.Add(item);
                        break;
                }
            }
            equipmentExportContent.Coinage.Copper = inventory.Coins.Copper.ToString();
            equipmentExportContent.Coinage.Silver = inventory.Coins.Silver.ToString();
            equipmentExportContent.Coinage.Electrum = inventory.Coins.Electrum.ToString();
            equipmentExportContent.Coinage.Gold = inventory.Coins.Gold.ToString();
            equipmentExportContent.Coinage.Platinum = inventory.Coins.Platinum.ToString();
            decimal carryCapacity = CalculateCarryingCapacity();
            decimal num2 = CalculateDragCapacity(carryCapacity);
            equipmentExportContent.CarryingCapacity = carryCapacity.ToString("#.#", CultureInfo.InvariantCulture) + " lb";
            equipmentExportContent.DragCapacity = num2.ToString("#.#", CultureInfo.InvariantCulture) + " lb";
            equipmentExportContent.WeightCarried = inventory.EquipmentWeight.ToString("#.#", CultureInfo.InvariantCulture) + " lb";
            equipmentExportContent.AdditionalTreasure = inventory.Treasure;
            equipmentExportContent.AttunementCurrent = inventory.AttunedItemCount.ToString();
            equipmentExportContent.AttunementMaximum = inventory.MaxAttunedItemCount.ToString();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (RefactoredEquipmentItem item3 in inventory.Items.Where((RefactoredEquipmentItem x) => x.IncludeInEquipmentPageDescriptionSidebar))
            {
                if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()))
                {
                    stringBuilder.AppendLine("<p>&nbsp;</p>");
                }
                string equipmentPageName = item3.GetEquipmentPageName();
                string text = (item3.IsAdorned ? item3.AdornerItem.Description : item3.Item.Description);
                if (text.StartsWith("<p>"))
                {
                    text = text.Substring(3, text.Length - 3);
                }
                if (text.Contains("<p class=\"indent\">"))
                {
                    text = text.Replace("<p class=\"indent\">", "<p class=\"indent\">&nbsp;     &nbsp;");
                }
                if (!string.IsNullOrWhiteSpace(item3.Notes))
                {
                    StringBuilder stringBuilder2 = new StringBuilder();
                    string[] array = Regex.Split(item3.Notes, Environment.NewLine);
                    foreach (string text2 in array)
                    {
                        if (string.IsNullOrWhiteSpace(text2))
                        {
                            stringBuilder2.Append("<p>&nbsp;</p>");
                        }
                        else
                        {
                            stringBuilder2.Append("<p>" + text2 + "</p>");
                        }
                    }
                    text = stringBuilder2.ToString().Substring(3, stringBuilder2.Length - 3);
                }
                stringBuilder.Append("<p><b><i>" + equipmentPageName + ".</i></b> " + text);
            }
            equipmentExportContent.AttunedMagicItems = stringBuilder.ToString();
            StoredItemsExportContent storedItemsExportContent = new StoredItemsExportContent(inventory.StoredItems1.Name);
            foreach (RefactoredEquipmentItem storedItem in inventory.StoredItems1.StoredItems)
            {
                InventoryItemExportContent inventoryItemExportContent2 = new InventoryItemExportContent(storedItem.GetEquipmentPageName())
                {
                    Amount = storedItem.Amount.ToString(),
                    Weight = storedItem.GetWeight().ToString("#.##", CultureInfo.InvariantCulture)
                };
                if (string.IsNullOrWhiteSpace(inventoryItemExportContent2.Weight))
                {
                    inventoryItemExportContent2.Weight = "—";
                }
                storedItemsExportContent.Items.Add(inventoryItemExportContent2);
            }
            StoredItemsExportContent storedItemsExportContent2 = new StoredItemsExportContent(inventory.StoredItems2.Name);
            foreach (RefactoredEquipmentItem storedItem2 in inventory.StoredItems2.StoredItems)
            {
                InventoryItemExportContent inventoryItemExportContent3 = new InventoryItemExportContent(storedItem2.GetEquipmentPageName())
                {
                    Amount = storedItem2.Amount.ToString(),
                    Weight = (storedItem2.Item.CalculableWeight * (decimal)storedItem2.Amount).ToString("#.##", CultureInfo.InvariantCulture)
                };
                if (string.IsNullOrWhiteSpace(inventoryItemExportContent3.Weight))
                {
                    inventoryItemExportContent3.Weight = "—";
                }
                storedItemsExportContent2.Items.Add(inventoryItemExportContent3);
            }
            equipmentExportContent.StorageLocations.Add(storedItemsExportContent);
            equipmentExportContent.StorageLocations.Add(storedItemsExportContent2);
            equipmentExportContent.QuestItems = inventory.QuestItems;
            return equipmentExportContent;
        }

        public NotesExportContent GetNotesContent()
        {
            return new NotesExportContent
            {
                LeftNotesColumn = _manager.Character.Notes1,
                RightNotesColumn = _manager.Character.Notes2
            };
        }

        public string GetItemEquippedAffix(RefactoredEquipmentItem equipment)
        {
            List<string> list = new List<string>();
            if (equipment.IsEquipped)
            {
                list.Add("Equipped");
            }
            if (equipment.IsAttuned)
            {
                list.Add("Attuned");
            }
            return string.Join(", ", list);
        }

        private IEnumerable<RefactoredEquipmentItem> GetItems(IEnumerable<RefactoredEquipmentItem> inventoryItems)
        {
            List<RefactoredEquipmentItem> list = new List<RefactoredEquipmentItem>();
            List<RefactoredEquipmentItem> list2 = new List<RefactoredEquipmentItem>();
            List<RefactoredEquipmentItem> list3 = new List<RefactoredEquipmentItem>();
            List<RefactoredEquipmentItem> list4 = new List<RefactoredEquipmentItem>();
            List<RefactoredEquipmentItem> list5 = new List<RefactoredEquipmentItem>();
            List<RefactoredEquipmentItem> list6 = new List<RefactoredEquipmentItem>();
            foreach (RefactoredEquipmentItem inventoryItem in inventoryItems)
            {
                if (inventoryItem.IncludeInTreasure)
                {
                    list6.Add(inventoryItem);
                    continue;
                }
                if (inventoryItem.Item.Type.Equals("Magic Item"))
                {
                    switch (inventoryItem.Item.ItemType)
                    {
                        case "Potion":
                            list4.Add(inventoryItem);
                            break;
                        case "Scroll":
                            list5.Add(inventoryItem);
                            break;
                        default:
                            list3.Add(inventoryItem);
                            break;
                    }
                    continue;
                }
                if (inventoryItem.Item.Type.Equals("Item"))
                {
                    switch (inventoryItem.Item.Category)
                    {
                        case "Art":
                        case "Art Objects":
                        case "Gemstones":
                        case "Treasure":
                        case "Valuable":
                        case "Valuables":
                            list6.Add(inventoryItem);
                            continue;
                    }
                }
                list2.Add(inventoryItem);
            }
            list.AddRange(list2);
            list.AddRange(list3);
            list.AddRange(list4);
            list.AddRange(list5);
            list.AddRange(list6);
            return list;
        }

        public decimal GetCarryMultiplierBasedOnSize()
        {
            ElementBase elementBase = _manager.GetElements().FirstOrDefault((ElementBase x) => x.Type.Equals("Size"));
            if (elementBase != null)
            {
                bool flag = (from x in _manager.GetElements()
                             where x.Type.Equals("Grants")
                             select x).ToList().Any((ElementBase x) => x.Id.Equals(InternalGrants.WeightCapacityCountAsLargerSize));
                switch (elementBase.Id)
                {
                    case "ID_SIZE_TINY":
                        if (flag)
                        {
                            return 15m;
                        }
                        return 7.5m;
                    case "ID_SIZE_MEDIUM":
                        if (flag)
                        {
                            return 30m;
                        }
                        break;
                    case "ID_SIZE_LARGE":
                    case "ID_SIZE_HUGE":
                    case "ID_SIZE_GARGANTUAN":
                    case "ID_SIZE_COLOSSAL":
                        return 30m;
                }
            }
            else
            {
                Logger.Warning("no size element available to calculate carry capacity based on size, defaulting to 15 (medium)");
            }
            return 15m;
        }

        private decimal CalculateCarryingCapacity()
        {
            decimal carryMultiplierBasedOnSize = GetCarryMultiplierBasedOnSize();
            List<ElementBase> source = (from x in _manager.GetElements()
                                        where x.Type.Equals("Grants")
                                        select x).ToList();
            if (source.Any((ElementBase x) => x.Id.Equals(InternalGrants.WeightCapacityDoubled)))
            {
                carryMultiplierBasedOnSize *= 2m;
            }
            if (source.Any((ElementBase x) => x.Id.Equals(InternalGrants.WeightCapacityHalved)))
            {
                carryMultiplierBasedOnSize /= 2m;
            }
            return (decimal)_manager.Character.Abilities.Strength.FinalScore * carryMultiplierBasedOnSize;
        }

        private decimal CalculateDragCapacity(decimal carryCapacity)
        {
            return carryCapacity * 2m;
        }
    }
}
