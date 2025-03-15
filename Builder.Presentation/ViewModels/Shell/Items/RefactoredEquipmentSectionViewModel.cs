using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Strings;
using Builder.Presentation;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.Models.Helpers;
using Builder.Presentation.Models.Sources;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.Sources;
using Builder.Presentation.Telemetry;
//using Builder.Presentation.UserControls;
using Builder.Presentation.ViewModels;
using Builder.Presentation.ViewModels.Base;
using Builder.Presentation.ViewModels.Shell.Items;
using Builder.Presentation.Views;
//using Builder.Presentation.Views.Dialogs;
using Builder.Presentation.Views.Sliders;

namespace Builder.Presentation.ViewModels.Shell.Items
{
    public class RefactoredEquipmentSectionViewModel : ViewModelBase, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>, ISubscriber<SettingsChangedEvent>, ISubscriber<CharacterLoadingCompletedEvent>
    {
        public class ItemColumns : ObservableObject
        {
            private bool _isPriceColumnVisible;

            private bool _isWeightColumnVisible;

            private bool _isWeaponDamageColumnVisible;

            private bool _isWeaponRangeColumnVisible;

            private bool _isWeaponCategoryVisible;

            private bool _isWeaponGroupVisible;

            private bool _isWeaponPropertiesVisible;

            private bool _isRarityColumnVisible;

            private bool _isAttunementColumnVisible;

            private bool _isSourceColumnVisible;

            private bool _isArmorClassColumnVisible;

            private bool _isArmorStrengthColumnVisible;

            private bool _isArmorStealthColumnVisible;

            private bool _isArmorGroupColumnVisible;

            private bool _isItemcardsColumnVisible;

            public bool IsPriceColumnVisible
            {
                get
                {
                    return _isPriceColumnVisible;
                }
                set
                {
                    SetProperty(ref _isPriceColumnVisible, value, "IsPriceColumnVisible");
                }
            }

            public bool IsWeightColumnVisible
            {
                get
                {
                    return _isWeightColumnVisible;
                }
                set
                {
                    SetProperty(ref _isWeightColumnVisible, value, "IsWeightColumnVisible");
                }
            }

            public bool IsRarityColumnVisible
            {
                get
                {
                    return _isRarityColumnVisible;
                }
                set
                {
                    SetProperty(ref _isRarityColumnVisible, value, "IsRarityColumnVisible");
                }
            }

            public bool IsAttunementColumnVisible
            {
                get
                {
                    return _isAttunementColumnVisible;
                }
                set
                {
                    SetProperty(ref _isAttunementColumnVisible, value, "IsAttunementColumnVisible");
                }
            }

            public bool IsArmorClassColumnVisible
            {
                get
                {
                    return _isArmorClassColumnVisible;
                }
                set
                {
                    SetProperty(ref _isArmorClassColumnVisible, value, "IsArmorClassColumnVisible");
                }
            }

            public bool IsArmorStrengthColumnVisible
            {
                get
                {
                    return _isArmorStrengthColumnVisible;
                }
                set
                {
                    SetProperty(ref _isArmorStrengthColumnVisible, value, "IsArmorStrengthColumnVisible");
                }
            }

            public bool IsArmorStealthColumnVisible
            {
                get
                {
                    return _isArmorStealthColumnVisible;
                }
                set
                {
                    SetProperty(ref _isArmorStealthColumnVisible, value, "IsArmorStealthColumnVisible");
                }
            }

            public bool IsArmorGroupColumnVisible
            {
                get
                {
                    return _isArmorGroupColumnVisible;
                }
                set
                {
                    SetProperty(ref _isArmorGroupColumnVisible, value, "IsArmorGroupColumnVisible");
                }
            }

            public bool IsWeaponDamageColumnVisible
            {
                get
                {
                    return _isWeaponDamageColumnVisible;
                }
                set
                {
                    SetProperty(ref _isWeaponDamageColumnVisible, value, "IsWeaponDamageColumnVisible");
                }
            }

            public bool IsWeaponRangeColumnVisible
            {
                get
                {
                    return _isWeaponRangeColumnVisible;
                }
                set
                {
                    SetProperty(ref _isWeaponRangeColumnVisible, value, "IsWeaponRangeColumnVisible");
                }
            }

            public bool IsWeaponCategoryVisible
            {
                get
                {
                    return _isWeaponCategoryVisible;
                }
                set
                {
                    SetProperty(ref _isWeaponCategoryVisible, value, "IsWeaponCategoryVisible");
                }
            }

            public bool IsWeaponGroupVisible
            {
                get
                {
                    return _isWeaponGroupVisible;
                }
                set
                {
                    SetProperty(ref _isWeaponGroupVisible, value, "IsWeaponGroupVisible");
                }
            }

            public bool IsWeaponPropertiesVisible
            {
                get
                {
                    return _isWeaponPropertiesVisible;
                }
                set
                {
                    SetProperty(ref _isWeaponPropertiesVisible, value, "IsWeaponPropertiesVisible");
                }
            }

            public bool IsSourceColumnVisible
            {
                get
                {
                    return _isSourceColumnVisible;
                }
                set
                {
                    SetProperty(ref _isSourceColumnVisible, value, "IsSourceColumnVisible");
                }
            }

            public bool IsItemcardsColumnVisible
            {
                get
                {
                    return _isItemcardsColumnVisible;
                }
                set
                {
                    SetProperty(ref _isItemcardsColumnVisible, value, "IsItemcardsColumnVisible");
                }
            }

            public ItemColumns()
            {
                Initialize();
            }

            public void Initialize()
            {
                IsPriceColumnVisible = true;
                IsWeightColumnVisible = true;
                IsRarityColumnVisible = false;
                IsAttunementColumnVisible = false;
                DisplayWeaponColumns(display: false);
                DisplayArmorColumns(display: false);
                IsSourceColumnVisible = true;
            }

            public void DisplayWeaponColumns(bool display = true)
            {
                IsWeaponDamageColumnVisible = display;
                IsWeaponRangeColumnVisible = display;
                IsWeaponCategoryVisible = display;
                IsWeaponGroupVisible = display;
                IsWeaponPropertiesVisible = display;
            }

            public void DisplayArmorColumns(bool display = true)
            {
                IsArmorClassColumnVisible = display;
                IsArmorStrengthColumnVisible = display;
                IsArmorStealthColumnVisible = display;
                IsArmorGroupColumnVisible = display;
            }

            public void CollapseColumns()
            {
                IsPriceColumnVisible = false;
                IsWeightColumnVisible = false;
                IsRarityColumnVisible = false;
                IsAttunementColumnVisible = false;
                DisplayWeaponColumns(display: false);
                DisplayArmorColumns(display: false);
                IsSourceColumnVisible = false;
            }
        }

        private readonly ExpressionInterpreter _expressionInterpreter;

        private readonly ElementBaseCollection _items = new ElementBaseCollection();

        private readonly ElementBaseCollection _a = new ElementBaseCollection();

        private readonly ElementBaseCollection _w = new ElementBaseCollection();

        private bool _isArmorSelection;

        private bool _isWeaponSelection;

        private EquipmentCategory _selectedEquipmentCategory;

        private ArmorElement _selectedArmorElement;

        private WeaponElement _selectedWeaponElement;

        private ElementBase _selectedItem;

        private int _buyAmount = 1;

        private string _equipmentWeight;

        private RefactoredEquipmentItem _selectedEquipmentItem;

        private int _attunedItemCount;

        private bool _allowActivateEquipmentItem;

        private bool _allowEquipEquipmentItem;

        private bool _allowEquipPrimaryEquipmentItem;

        private bool _allowEquipSecondaryEquipmentItem;

        private bool _allowEquipVersatileEquipmentItem;

        private bool _allowToggleAttunementEquipmentItem;

        private bool _allowEquipmentItemAddToAttacks;

        private bool _allowExtractEquipmentItem;

        private bool _allowSwitchWhenEquippingWeapons;

        private int _addAmount;

        private bool _isQuickEditEnabled;

        private bool _showInventoryItemCardColumn;

        private List<WeaponElement> _baseWeapons = new List<WeaponElement>();

        private List<ArmorElement> _baseArmors = new List<ArmorElement>();

        private CharacterManager Manager => CharacterManager.Current;

        public Character Character => Manager.Character;

        public CharacterInventory Inventory => Character.Inventory;

        public RelayCommand AddSelectedItemCommand { get; set; }

        public RelayCommand BuySelectedItemCommand { get; set; }

        public RelayCommand DeleteSelectedEquipmentCommand { get; set; }

        public RelayCommand DeleteAllSelectedEquipmentCommand { get; set; }

        public RelayCommand MoveSelectedEquipmentItemUpCommand { get; set; }

        public RelayCommand MoveSelectedEquipmentItemDownCommand { get; set; }

        public RelayCommand EquipSelectedEquipmentItemCommand { get; set; }

        public RelayCommand AttuneSelectedEquipmentItemCommand { get; set; }

        public RelayCommand ManageEquipmentItemCommand { get; }

        public RelayCommand AddToAttacksCommand { get; }

        public RelayCommand EquipmentItemSellCommand { get; set; }

        public RelayCommand EquipmentItemRemoveCommand { get; set; }

        public RelayCommand EquipmentItemRemoveAllCommand { get; set; }

        public RelayCommand ExtractEquipmentItemCommand { get; }

        public RelayCommand ShowManageStorageCommand => new RelayCommand(delegate
        {
            //InventoryStorageWindow inventoryStorageWindow = new InventoryStorageWindow();
            //inventoryStorageWindow.DataContext = this;
            //inventoryStorageWindow.Show();
        });

        public RelayCommand StoreSelectedEquipmentAsPrimaryCommand { get; }

        public RelayCommand StoreSelectedEquipmentAsSecondaryCommand { get; }

        public RelayCommand RetrieveStoredSelectedEquipmentCommand { get; }

        public RelayCommand ActivateEquipmentItemCommand { get; set; }

        public RelayCommand EquipEquipmentItemCommand { get; set; }

        public RelayCommand EquipPrimaryEquipmentItemCommand { get; set; }

        public RelayCommand EquipSecondaryEquipmentItemCommand { get; set; }

        public RelayCommand EquipVersatileEquipmentItemCommand { get; set; }

        public RelayCommand ToggleAttunementEquipmentItemCommand { get; set; }

        public bool AllowActivateEquipmentItem
        {
            get
            {
                return _allowActivateEquipmentItem;
            }
            set
            {
                SetProperty(ref _allowActivateEquipmentItem, value, "AllowActivateEquipmentItem");
            }
        }

        public bool AllowEquipEquipmentItem
        {
            get
            {
                return _allowEquipEquipmentItem;
            }
            set
            {
                SetProperty(ref _allowEquipEquipmentItem, value, "AllowEquipEquipmentItem");
            }
        }

        public bool AllowEquipPrimaryEquipmentItem
        {
            get
            {
                return _allowEquipPrimaryEquipmentItem;
            }
            set
            {
                SetProperty(ref _allowEquipPrimaryEquipmentItem, value, "AllowEquipPrimaryEquipmentItem");
            }
        }

        public bool AllowEquipSecondaryEquipmentItem
        {
            get
            {
                return _allowEquipSecondaryEquipmentItem;
            }
            set
            {
                SetProperty(ref _allowEquipSecondaryEquipmentItem, value, "AllowEquipSecondaryEquipmentItem");
            }
        }

        public bool AllowEquipVersatileEquipmentItem
        {
            get
            {
                return _allowEquipVersatileEquipmentItem;
            }
            set
            {
                SetProperty(ref _allowEquipVersatileEquipmentItem, value, "AllowEquipVersatileEquipmentItem");
            }
        }

        public bool AllowToggleAttunementEquipmentItem
        {
            get
            {
                return _allowToggleAttunementEquipmentItem;
            }
            set
            {
                SetProperty(ref _allowToggleAttunementEquipmentItem, value, "AllowToggleAttunementEquipmentItem");
            }
        }

        public bool AllowEquipmentItemAddToAttacks
        {
            get
            {
                return _allowEquipmentItemAddToAttacks;
            }
            set
            {
                SetProperty(ref _allowEquipmentItemAddToAttacks, value, "AllowEquipmentItemAddToAttacks");
            }
        }

        public ICommand ShowManageCoinageSliderCommand => new RelayCommand(delegate
        {
            base.EventAggregator.Send(new ShowSliderEvent(Slider.ManageCoinage));
        });

        public RelayCommand IncrementSelectedEquipmentItemAmountCommand { get; set; }

        public RelayCommand DecrementSelectedEquipmentItemAmountCommand { get; set; }

        public bool AllowExtractEquipmentItem
        {
            get
            {
                return _allowExtractEquipmentItem;
            }
            set
            {
                SetProperty(ref _allowExtractEquipmentItem, value, "AllowExtractEquipmentItem");
            }
        }

        public bool AllowSwitchWhenEquippingWeapons
        {
            get
            {
                return _allowSwitchWhenEquippingWeapons;
            }
            set
            {
                SetProperty(ref _allowSwitchWhenEquippingWeapons, value, "AllowSwitchWhenEquippingWeapons");
            }
        }

        public ObservableCollection<EquipmentCategory> EquipmentCategories { get; } = new ObservableCollection<EquipmentCategory>();

        public ObservableCollection<ArmorElement> ArmorCollection { get; } = new ObservableCollection<ArmorElement>();

        public ObservableCollection<WeaponElement> WeaponsCollection { get; } = new ObservableCollection<WeaponElement>();

        public ElementBaseCollection ItemsCollection { get; } = new ElementBaseCollection();

        public bool IsArmorSelection
        {
            get
            {
                return _isArmorSelection;
            }
            set
            {
                SetProperty(ref _isArmorSelection, value, "IsArmorSelection");
            }
        }

        public bool IsWeaponSelection
        {
            get
            {
                return _isWeaponSelection;
            }
            set
            {
                SetProperty(ref _isWeaponSelection, value, "IsWeaponSelection");
            }
        }

        public EquipmentCategory SelectedEquipmentCategory
        {
            get
            {
                return _selectedEquipmentCategory;
            }
            set
            {
                SetProperty(ref _selectedEquipmentCategory, value, "SelectedEquipmentCategory");
                if (_selectedEquipmentCategory != null)
                {
                    IsArmorSelection = _selectedEquipmentCategory.DisplayName.Equals("Magic Armor");
                    IsWeaponSelection = _selectedEquipmentCategory.DisplayName.Equals("Magic Weapons");
                    UpdateItemsCollections();
                }
            }
        }

        public ArmorElement SelectedArmorElement
        {
            get
            {
                return _selectedArmorElement;
            }
            set
            {
                SetProperty(ref _selectedArmorElement, value, "SelectedArmorElement");
                if (_selectedArmorElement != null)
                {
                    base.EventAggregator.Send(new EquipmentElementDescriptionDisplayRequestEvent(_selectedArmorElement));
                    UpdateItemsCollections();
                }
            }
        }

        public WeaponElement SelectedWeaponElement
        {
            get
            {
                return _selectedWeaponElement;
            }
            set
            {
                SetProperty(ref _selectedWeaponElement, value, "SelectedWeaponElement");
                if (_selectedWeaponElement != null)
                {
                    base.EventAggregator.Send(new EquipmentElementDescriptionDisplayRequestEvent(_selectedWeaponElement));
                    UpdateItemsCollections();
                }
            }
        }

        public ElementBase SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                SetProperty(ref _selectedItem, value, "SelectedItem");
                if (IsArmorSelection)
                {
                    base.EventAggregator.Send(new EquipmentElementDescriptionDisplayRequestEvent(_selectedItem, _selectedArmorElement)
                    {
                        IgnoreGeneratedDescription = true
                    });
                }
                else if (IsWeaponSelection)
                {
                    base.EventAggregator.Send(new EquipmentElementDescriptionDisplayRequestEvent(_selectedItem, _selectedWeaponElement)
                    {
                        IgnoreGeneratedDescription = true
                    });
                }
                else
                {
                    base.EventAggregator.Send(new EquipmentElementDescriptionDisplayRequestEvent(_selectedItem));
                }
                AddSelectedItemCommand.RaiseCanExecuteChanged();
                RaiseEquipmentItemCommands();
            }
        }

        public int BuyAmount
        {
            get
            {
                return _buyAmount;
            }
            set
            {
                SetProperty(ref _buyAmount, value, "BuyAmount");
            }
        }

        public int AddAmount
        {
            get
            {
                return _addAmount;
            }
            set
            {
                SetProperty(ref _addAmount, value, "AddAmount");
            }
        }

        public string EquipmentWeight
        {
            get
            {
                return _equipmentWeight;
            }
            set
            {
                SetProperty(ref _equipmentWeight, value, "EquipmentWeight");
            }
        }

        public RefactoredEquipmentItem SelectedEquipmentItem
        {
            get
            {
                return _selectedEquipmentItem;
            }
            set
            {
                SetProperty(ref _selectedEquipmentItem, value, "SelectedEquipmentItem");
                if (_selectedEquipmentItem != null)
                {
                    base.EventAggregator.Send(_selectedEquipmentItem.IsAdorned ? new EquipmentElementDescriptionDisplayRequestEvent(_selectedEquipmentItem.AdornerItem, _selectedEquipmentItem.Item) : new EquipmentElementDescriptionDisplayRequestEvent(_selectedEquipmentItem.Item));
                }
                RaiseEquipmentItemCommands();
                OnPropertyChanged("HasSelectedEquipmentItem");
            }
        }

        public bool HasSelectedEquipmentItem => SelectedEquipmentItem != null;

        // public ElementFilter ItemFilter { get; }

        public ItemColumns Columns { get; } = new ItemColumns();

        public bool IsQuickEditEnabled
        {
            get
            {
                return _isQuickEditEnabled;
            }
            set
            {
                SetProperty(ref _isQuickEditEnabled, value, "IsQuickEditEnabled");
            }
        }

        public bool ShowInventoryItemCardColumn
        {
            get
            {
                return _showInventoryItemCardColumn;
            }
            set
            {
                SetProperty(ref _showInventoryItemCardColumn, value, "ShowInventoryItemCardColumn");
            }
        }

        public ICollectionView CategoriesCollectionView { get; set; }

        public ICollectionView WeaponElementsCollectionView { get; set; }

        public ICollectionView ArmorElementsCollectionView { get; set; }

        public ICollectionView ItemsCollectionView { get; set; }

        private void StoreSelectedEquipmentInPrimary()
        {
            RefactoredEquipmentItem selectedEquipmentItem = SelectedEquipmentItem;
            SelectedEquipmentItem.Store(Inventory.StoredItems1);
            SelectedEquipmentItem = selectedEquipmentItem;
            Inventory.CalculateWeight();
        }

        private bool CanStoreSelectedEquipmentInPrimary()
        {
            if (SelectedEquipmentItem != null)
            {
                if (SelectedEquipmentItem.IsStored && SelectedEquipmentItem.Storage == Inventory.StoredItems1)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private void StoreSelectedEquipmentInSecondary()
        {
            RefactoredEquipmentItem selectedEquipmentItem = SelectedEquipmentItem;
            SelectedEquipmentItem.Store(Inventory.StoredItems2);
            SelectedEquipmentItem = selectedEquipmentItem;
            Inventory.CalculateWeight();
        }

        private bool CanStoreSelectedEquipmentInSecondary()
        {
            if (SelectedEquipmentItem != null)
            {
                if (SelectedEquipmentItem.IsStored && SelectedEquipmentItem.Storage == Inventory.StoredItems2)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private void RetrieveStoredSelectedEquipment()
        {
            SelectedEquipmentItem.Retrieve();
            OnPropertyChanged("SelectedEquipmentItem");
            Inventory.CalculateWeight();
        }

        private void AddSelectedItem()
        {
            Item item = SelectedItem as Item;
            if (item == null)
            {
                return;
            }
            try
            {
                int num = Math.Max(1, AddAmount);
                AnalyticsEventHelper.EquipmentAdd(SelectedEquipmentCategory.DisplayName, item.Name, item.Source);
                if (item.IsStackable)
                {
                    RefactoredEquipmentItem refactoredEquipmentItem = Inventory.Items.FirstOrDefault((RefactoredEquipmentItem x) => x.Item.Id.Equals(item.Id));
                    if (refactoredEquipmentItem != null)
                    {
                        refactoredEquipmentItem.Amount += num;
                        base.EventAggregator.Send(new MainWindowStatusUpdateEvent("You have added " + ((num > 1) ? (num + " x") : "another") + " '" + item.Name + "' to your inventory."));
                        return;
                    }
                }
                string arg = "";
                for (int i = 0; i < num; i++)
                {
                    RefactoredEquipmentItem refactoredEquipmentItem2 = null;
                    if (IsArmorSelection)
                    {
                        SelectedArmorElement.Copy();
                        refactoredEquipmentItem2 = new RefactoredEquipmentItem(SelectedArmorElement, item);
                    }
                    if (IsWeaponSelection)
                    {
                        refactoredEquipmentItem2 = new RefactoredEquipmentItem(SelectedWeaponElement, item);
                    }
                    if (item.Type.Equals("Magic Item") || item.Type.Equals("Item"))
                    {
                        if (item.ElementSetters.ContainsSetter("weapon"))
                        {
                            List<WeaponElement> source = Inventory.GetSupportedWeaponElements(item.ElementSetters.GetSetter("weapon").Value).ToList();
                            if (source.Count() == 1)
                            {
                                refactoredEquipmentItem2 = new RefactoredEquipmentItem(source.FirstOrDefault(), item);
                            }
                        }
                        else if (item.ElementSetters.ContainsSetter("armor"))
                        {
                            List<ArmorElement> source2 = Inventory.GetSupportedArmorElements(item.ElementSetters.GetSetter("armor").Value).ToList();
                            if (source2.Count() == 1)
                            {
                                refactoredEquipmentItem2 = new RefactoredEquipmentItem(source2.FirstOrDefault(), item);
                            }
                        }
                    }
                    if (refactoredEquipmentItem2 == null)
                    {
                        refactoredEquipmentItem2 = new RefactoredEquipmentItem(item);
                    }
                    if (item.IsStackable)
                    {
                        RefactoredEquipmentItem refactoredEquipmentItem3 = Inventory.Items.FirstOrDefault((RefactoredEquipmentItem x) => x.Item.Id.Equals(item.Id));
                        if (refactoredEquipmentItem3 != null)
                        {
                            refactoredEquipmentItem3.Amount++;
                            arg = item.Name;
                            continue;
                        }
                    }
                    Inventory.Items.Add(refactoredEquipmentItem2);
                    arg = refactoredEquipmentItem2.DisplayName;
                }
                base.EventAggregator.Send(new MainWindowStatusUpdateEvent($"You have added {num} x '{arg}' to your inventory."));
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "AddSelectedItem");
                Dictionary<string, string> additionalProperties = AnalyticsErrorHelper.CreateProperties("item", item.ToString());
                AnalyticsErrorHelper.Exception(ex, additionalProperties, item.ElementNodeString, "AddSelectedItem", 332);
                MessageDialogService.ShowException(ex);
            }
        }

        private bool CanAddSelectedItem()
        {
            return _selectedItem != null;
        }

        private void BuySelectedItem()
        {
            if (!(SelectedItem is Item item))
            {
                return;
            }
            try
            {
                AnalyticsEventHelper.EquipmentBuy(SelectedEquipmentCategory.DisplayName, item.Name, item.Source);
                Coinage coinage = new Coinage();
                bool flag = false;
                if (SelectedItem is MagicItemElement magicItemElement)
                {
                    flag = magicItemElement.OverrideCost;
                }
                if (IsArmorSelection)
                {
                    if (!flag)
                    {
                        for (int i = 0; i < BuyAmount; i++)
                        {
                            coinage.Deposit(Coinage.GetCurrencyCoinFromAbbreviation(SelectedArmorElement.CurrencyAbbreviation), SelectedArmorElement.Cost);
                        }
                    }
                }
                else if (IsWeaponSelection && !flag)
                {
                    for (int j = 0; j < BuyAmount; j++)
                    {
                        coinage.Deposit(Coinage.GetCurrencyCoinFromAbbreviation(SelectedWeaponElement.CurrencyAbbreviation), SelectedWeaponElement.Cost);
                    }
                }
                for (int k = 0; k < BuyAmount; k++)
                {
                    coinage.Deposit(Coinage.GetCurrencyCoinFromAbbreviation(item.CurrencyAbbreviation), item.Cost);
                }
                long calculationBase = coinage.CalculationBase;
                if (Inventory.Coins.HasSufficienctFunds(Coinage.CurrencyCoin.Copper, calculationBase))
                {
                    if (!IsArmorSelection && !IsWeaponSelection)
                    {
                        if (coinage.Platinum > 0)
                        {
                            Inventory.Coins.Withdraw(Coinage.CurrencyCoin.Platinum, coinage.Platinum);
                        }
                        else if (coinage.Gold > 0)
                        {
                            Inventory.Coins.Withdraw(Coinage.CurrencyCoin.Gold, coinage.Gold);
                        }
                        else if (coinage.Electrum > 0)
                        {
                            Inventory.Coins.Withdraw(Coinage.CurrencyCoin.Electrum, coinage.Electrum);
                        }
                        else if (coinage.Silver > 0)
                        {
                            Inventory.Coins.Withdraw(Coinage.CurrencyCoin.Silver, coinage.Silver);
                        }
                        else if (coinage.Copper > 0)
                        {
                            Inventory.Coins.Withdraw(Coinage.CurrencyCoin.Copper, coinage.Copper);
                        }
                    }
                    else
                    {
                        Inventory.Coins.Withdraw(Coinage.CurrencyCoin.Copper, calculationBase);
                    }
                    int addAmount = AddAmount;
                    AddAmount = 1;
                    for (int l = 0; l < BuyAmount; l++)
                    {
                        AddSelectedItem();
                    }
                    AddAmount = addAmount;
                }
                else if (MessageBox.Show($"This purchase costs {coinage.DisplayCoinage}gp, you only have {Inventory.Coins.DisplayCoinage}gp." + " Do you want to add the goods instead of buying?", "Insufficienct Coin", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int addAmount2 = AddAmount;
                    AddAmount = 1;
                    for (int m = 0; m < BuyAmount; m++)
                    {
                        AddSelectedItem();
                    }
                    AddAmount = addAmount2;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuySelectedItem");
                Dictionary<string, string> additionalProperties = AnalyticsErrorHelper.CreateProperties("item", item.ToString());
                AnalyticsErrorHelper.Exception(ex, additionalProperties, item.ElementNodeString, "BuySelectedItem", 480);
                MessageDialogService.ShowException(ex);
            }
        }

        private bool BuyAddSelectedItem()
        {
            return CanAddSelectedItem();
        }

        private void MoveSelectedEquipmentItemUp()
        {
            if (CanMoveSelectedEquipmentItemUp())
            {
                int num = Inventory.Items.IndexOf(_selectedEquipmentItem);
                Inventory.Items.Move(num, num - 1);
                MoveSelectedEquipmentItemUpCommand.RaiseCanExecuteChanged();
                MoveSelectedEquipmentItemDownCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanMoveSelectedEquipmentItemUp()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            return Inventory.Items.IndexOf(_selectedEquipmentItem) != 0;
        }

        private void MoveSelectedEquipmentItemDown()
        {
            if (CanMoveSelectedEquipmentItemDown())
            {
                int num = Inventory.Items.IndexOf(_selectedEquipmentItem);
                Inventory.Items.Move(num, num + 1);
                MoveSelectedEquipmentItemUpCommand.RaiseCanExecuteChanged();
                MoveSelectedEquipmentItemDownCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanMoveSelectedEquipmentItemDown()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            return Inventory.Items.IndexOf(_selectedEquipmentItem) != Inventory.Items.Count - 1;
        }

        private void DeleteSelectedEquipment()
        {
            if (!CanDeleteSelectedEquipment())
            {
                return;
            }
            int num = Inventory.Items.IndexOf(_selectedEquipmentItem);
            if (SelectedEquipmentItem.IsEquipped)
            {
                bool num2 = Inventory.IsEquippedPrimary(SelectedEquipmentItem);
                bool flag = Inventory.IsEquippedSecondary(SelectedEquipmentItem);
                if (num2)
                {
                    Inventory.UnequipPrimary();
                }
                else if (flag)
                {
                    Inventory.UnequipSecondary();
                }
                else if (SelectedEquipmentItem.IsArmorTarget())
                {
                    Inventory.UnequipArmor();
                }
                else
                {
                    SelectedEquipmentItem.Deactivate();
                }
            }
            string statusMessage = string.Format("You have removed{0} '{1}' from your inventory.", (SelectedEquipmentItem.Amount > 1) ? " 1" : "", SelectedEquipmentItem);
            if (SelectedEquipmentItem.IsStackable)
            {
                if (SelectedEquipmentItem.Amount > 1)
                {
                    SelectedEquipmentItem.Amount--;
                }
                else
                {
                    Inventory.Items.Remove(SelectedEquipmentItem);
                }
            }
            else
            {
                Inventory.Items.Remove(SelectedEquipmentItem);
            }
            base.EventAggregator.Send(new MainWindowStatusUpdateEvent(statusMessage));
            if (Inventory.Items.Any() && SelectedEquipmentItem == null)
            {
                SelectedEquipmentItem = ((num != 0) ? Inventory.Items[num - 1] : Inventory.Items[0]);
            }
            CalculateWeight();
        }

        private bool CanDeleteSelectedEquipment()
        {
            return _selectedEquipmentItem != null;
        }

        private void DeleteAllSelectedEquipment()
        {
            if (CanDeleteSelectedEquipment())
            {
                int num = Inventory.Items.IndexOf(_selectedEquipmentItem);
                if (SelectedEquipmentItem.IsEquipped)
                {
                    SelectedEquipmentItem.Deactivate();
                    SelectedEquipmentItem.EquippedLocation = "";
                }
                if (SelectedEquipmentItem.IsStored)
                {
                    SelectedEquipmentItem.Retrieve();
                }
                string statusMessage = $"You have removed '{SelectedEquipmentItem}' from your inventory.";
                Inventory.Items.Remove(SelectedEquipmentItem);
                base.EventAggregator.Send(new MainWindowStatusUpdateEvent(statusMessage));
                if (Inventory.Items.Any() && SelectedEquipmentItem == null)
                {
                    SelectedEquipmentItem = ((num != 0) ? Inventory.Items[num - 1] : Inventory.Items[0]);
                }
                CalculateWeight();
            }
        }

        private bool CanDeleteAllSelectedEquipment()
        {
            if (_selectedEquipmentItem != null)
            {
                return _selectedEquipmentItem.IsStackable;
            }
            return false;
        }

        private void ManageEquipmentItem()
        {
            if (_selectedEquipmentItem != null)
            {
                //ManageEquipmentItemWindow manageEquipmentItemWindow = new ManageEquipmentItemWindow();
                //manageEquipmentItemWindow.DataContext = this;
                //manageEquipmentItemWindow.ShowDialog();
            }
        }

        private bool CanManageEquipmentItem()
        {
            return _selectedEquipmentItem != null;
        }

        private void ExtractEquipmentItem()
        {
            //if (_selectedEquipmentItem == null || !_selectedEquipmentItem.Item.IsExtractable || new ExtractItemWindow(_selectedEquipmentItem.Item, _items).ShowDialog() != true)
            //{
            //    return;
            //}
            int count = _selectedEquipmentItem.Item.Extractables.Count;
            int num = 0;
            foreach (KeyValuePair<string, int> extractable2 in _selectedEquipmentItem.Item.Extractables)
            {
                string id = extractable2.Key;
                int value = extractable2.Value;
                Item extractable = _items.FirstOrDefault((ElementBase x) => x.Id.Equals(id)) as Item;
                if (extractable != null)
                {
                    RefactoredEquipmentItem refactoredEquipmentItem = new RefactoredEquipmentItem(extractable);
                    if (extractable.IsStackable)
                    {
                        RefactoredEquipmentItem refactoredEquipmentItem2 = Inventory.Items.FirstOrDefault((RefactoredEquipmentItem x) => x.Item.Id.Equals(extractable.Id));
                        if (refactoredEquipmentItem2 != null)
                        {
                            refactoredEquipmentItem2.Amount += value;
                            num++;
                            continue;
                        }
                    }
                    refactoredEquipmentItem.Amount = value;
                    Inventory.Items.Add(refactoredEquipmentItem);
                    num++;
                }
                else
                {
                    string text = $"unable to extract item with id: {id} from {_selectedEquipmentItem.Item} with id {_selectedEquipmentItem.Item.Id}";
                    Logger.Warning(text);
                    ApplicationManager.Current.SendStatusMessage(text);
                }
            }
            string displayName = _selectedEquipmentItem.DisplayName;
            DeleteSelectedEquipment();
            if (count == num)
            {
                ApplicationManager.Current.SendStatusMessage("You extracted " + displayName);
            }
        }

        private bool CanExtractEquipmentItem()
        {
            if (_selectedEquipmentItem != null)
            {
                return _selectedEquipmentItem.Item.IsExtractable;
            }
            return false;
        }

        private void EquipSelectedEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return;
            }
            throw new NotImplementedException();
        }

        private bool CanEquipSelectedEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            return _selectedEquipmentItem.Item.IsEquippable;
        }

        private void AttuneSelectedEquipmentItemUp()
        {
            if (_selectedEquipmentItem != null && CanAttuneSelectedEquipmentItem())
            {
                if (!_selectedEquipmentItem.IsAttuned)
                {
                    _selectedEquipmentItem.AttuneItem();
                }
                else
                {
                    _selectedEquipmentItem.UnAttuneItem();
                }
                CalculateAttunementCount();
            }
        }

        private bool CanAttuneSelectedEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            return _selectedEquipmentItem.IsAttunable;
        }

        private void ActivateEquipmentItem()
        {
            if (_selectedEquipmentItem != null)
            {
                _ = _selectedEquipmentItem.IsActivated;
                if (CanEquipEquipmentItem())
                {
                    EquipEquipmentItem();
                    return;
                }
                base.EventAggregator.Send(new MainWindowStatusUpdateEvent(_selectedEquipmentItem.DisplayName + " activated clicked."));
                base.EventAggregator.Send(_selectedEquipmentItem.IsAdorned ? new EquipmentElementDescriptionDisplayRequestEvent(_selectedEquipmentItem.AdornerItem, _selectedEquipmentItem.Item) : new EquipmentElementDescriptionDisplayRequestEvent(_selectedEquipmentItem.Item));
                CalculateAttunementCount();
            }
        }

        private void EquipEquipmentItem()
        {
            if (_selectedEquipmentItem.IsArmorTarget())
            {
                if (Inventory.EquippedArmor != null)
                {
                    bool num = Inventory.EquippedArmor.Identifier.Equals(_selectedEquipmentItem.Identifier);
                    Inventory.UnequipArmor();
                    if (num)
                    {
                        return;
                    }
                }
                Inventory.EquipArmor(_selectedEquipmentItem);
                return;
            }
            if (_selectedEquipmentItem.IsOneHandTarget() && _selectedEquipmentItem.IsSecondaryTarget())
            {
                EquipSecondaryEquipmentItem();
                return;
            }
            if (_selectedEquipmentItem.IsTwoHandTarget())
            {
                if (Inventory.EquippedSecondary != null)
                {
                    bool num2 = Inventory.EquippedSecondary.Identifier.Equals(_selectedEquipmentItem.Identifier);
                    Inventory.UnequipSecondary();
                    if (num2)
                    {
                        return;
                    }
                }
                if (Inventory.EquippedPrimary != null)
                {
                    bool num3 = Inventory.EquippedPrimary.Identifier.Equals(_selectedEquipmentItem.Identifier);
                    Inventory.UnequipPrimary();
                    if (num3)
                    {
                        return;
                    }
                }
                _selectedEquipmentItem.Activate(equip: true, _selectedEquipmentItem.IsAttunable);
                Inventory.EquipPrimary(_selectedEquipmentItem, twohanded: true);
                return;
            }
            if (_selectedEquipmentItem.IsOneHandTarget())
            {
                if (Inventory.IsEquippedTwoHanded())
                {
                    Inventory.UnequipPrimary();
                    Inventory.UnequipSecondary();
                }
                if (IsEquippedPrimary(_selectedEquipmentItem))
                {
                    Inventory.UnequipPrimary();
                    return;
                }
                if (Inventory.EquippedPrimary != null && !IsEquippedSecondary(Inventory.EquippedPrimary))
                {
                    Inventory.UnequipPrimary();
                }
                if (!_selectedEquipmentItem.IsEquipped && Inventory.EquippedPrimary == null)
                {
                    Inventory.EquipPrimary(_selectedEquipmentItem);
                }
                return;
            }
            if (_selectedEquipmentItem.IsPrimaryTarget() || _selectedEquipmentItem.IsSecondaryTarget())
            {
                if (_selectedEquipmentItem.IsPrimaryTarget())
                {
                    EquipPrimaryEquipmentItem();
                }
                else if (_selectedEquipmentItem.IsSecondaryTarget())
                {
                    EquipSecondaryEquipmentItem();
                }
                return;
            }
            if (_selectedEquipmentItem.IsEquippable)
            {
                if (_selectedEquipmentItem.IsEquipped)
                {
                    _selectedEquipmentItem.Deactivate();
                }
                else
                {
                    _selectedEquipmentItem.Activate(equip: true, _selectedEquipmentItem.IsAttunable);
                }
            }
            else if (_selectedEquipmentItem.IsAttunable)
            {
                if (_selectedEquipmentItem.IsAttuned)
                {
                    _selectedEquipmentItem.DeactivateAttunement();
                }
                else
                {
                    _selectedEquipmentItem.Activate(equip: false, _selectedEquipmentItem.IsAttunable);
                }
            }
            CalculateAttunementCount();
        }

        private bool IsEquippedPrimary(EquipmentItem equipment)
        {
            return Inventory.IsEquippedPrimary(equipment);
        }

        private bool IsEquippedSecondary(EquipmentItem equipment)
        {
            return Inventory.IsEquippedSecondary(equipment);
        }

        private void EquipPrimaryEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return;
            }
            bool allowSwitchWhenEquippingWeapons = AllowSwitchWhenEquippingWeapons;
            bool flag = false;
            if (_selectedEquipmentItem.IsEquipped)
            {
                bool flag2 = IsEquippedPrimary(_selectedEquipmentItem);
                bool flag3 = IsEquippedSecondary(_selectedEquipmentItem);
                if (flag2 && flag3)
                {
                    Inventory.EquippedSecondary = null;
                    Inventory.EquippedPrimary.EquippedLocation = "Primary Hand";
                    return;
                }
                if (flag3)
                {
                    Inventory.EquippedSecondary = null;
                    flag = allowSwitchWhenEquippingWeapons;
                }
                else if (flag2)
                {
                    Inventory.UnequipPrimary();
                    return;
                }
            }
            if (Inventory.EquippedPrimary != null)
            {
                RefactoredEquipmentItem equippedPrimary = Inventory.EquippedPrimary;
                if (flag)
                {
                    Inventory.EquippedSecondary = equippedPrimary;
                    equippedPrimary.EquippedLocation = "Secondary Hand";
                }
                else
                {
                    if (Inventory.IsEquippedTwoHanded() || Inventory.IsEquippedVersatile())
                    {
                        Inventory.EquippedSecondary = null;
                    }
                    Inventory.EquippedPrimary.Deactivate();
                }
                Inventory.EquippedPrimary = null;
            }
            Inventory.EquipPrimary(_selectedEquipmentItem);
        }

        private void EquipSecondaryEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return;
            }
            bool allowSwitchWhenEquippingWeapons = AllowSwitchWhenEquippingWeapons;
            bool flag = false;
            if (_selectedEquipmentItem.IsEquipped)
            {
                bool flag2 = IsEquippedPrimary(_selectedEquipmentItem);
                bool flag3 = IsEquippedSecondary(_selectedEquipmentItem);
                if (flag2 && flag3)
                {
                    if (_selectedEquipmentItem.Item.HasVersatile)
                    {
                        Inventory.UnequipPrimary();
                        Inventory.UnequipSecondary();
                    }
                }
                else if (flag2)
                {
                    Inventory.EquippedPrimary = null;
                    flag = allowSwitchWhenEquippingWeapons;
                }
                else if (flag3)
                {
                    Inventory.EquippedSecondary.Deactivate();
                    Inventory.EquippedSecondary = null;
                    return;
                }
            }
            if (Inventory.EquippedSecondary != null)
            {
                RefactoredEquipmentItem equippedSecondary = Inventory.EquippedSecondary;
                if (flag && !Inventory.EquippedSecondary.IsSecondaryTarget())
                {
                    Inventory.EquippedPrimary = equippedSecondary;
                    equippedSecondary.EquippedLocation = "Primary Hand";
                }
                else if (Inventory.IsEquippedTwoHanded())
                {
                    Inventory.EquippedPrimary = null;
                    Inventory.EquippedSecondary.Deactivate();
                }
                else if (Inventory.IsEquippedVersatile())
                {
                    Inventory.EquippedPrimary.EquippedLocation = "Primary Hand";
                }
                else
                {
                    Inventory.UnequipSecondary();
                }
                Inventory.EquippedSecondary = null;
            }
            Inventory.EquipSecondary(_selectedEquipmentItem);
        }

        private void EquipVersatileEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return;
            }
            if (_selectedEquipmentItem.IsEquipped)
            {
                bool flag = IsEquippedPrimary(_selectedEquipmentItem);
                bool flag2 = IsEquippedSecondary(_selectedEquipmentItem);
                if (flag && flag2)
                {
                    Inventory.UnequipSecondary();
                }
                else if (flag)
                {
                    if (Inventory.EquippedSecondary != null)
                    {
                        Inventory.UnequipSecondary();
                    }
                }
                else if (flag2 && Inventory.EquippedPrimary != null)
                {
                    Inventory.UnequipPrimary();
                }
            }
            else
            {
                if (Inventory.EquippedPrimary != null)
                {
                    Inventory.UnequipPrimary();
                }
                if (Inventory.EquippedSecondary != null)
                {
                    Inventory.UnequipSecondary();
                }
            }
            Inventory.EquipPrimary(_selectedEquipmentItem, twohanded: true);
        }

        private void ToggleAttunementEquipmentItem()
        {
            if (_selectedEquipmentItem.IsAttunable)
            {
                if (_selectedEquipmentItem.IsAttuned)
                {
                    _selectedEquipmentItem.DeactivateAttunement();
                }
                else
                {
                    _selectedEquipmentItem.Activate(equip: false, attune: true);
                }
            }
            CalculateAttunementCount();
        }

        private void AddToAttacks()
        {
            Character.AttacksSection.Items.Add(new AttackSectionItem(SelectedEquipmentItem));
            ApplicationManager.Current.SendStatusMessage($"{SelectedEquipmentItem} added to your attacks.");
        }

        private bool CanActivateEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            if (_selectedEquipmentItem.IsEquippable || _selectedEquipmentItem.IsAttunable)
            {
                return true;
            }
            return false;
        }

        private bool CanEquipEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            if (!_selectedEquipmentItem.IsEquippable)
            {
                return false;
            }
            if (_selectedEquipmentItem.IsArmorTarget() || _selectedEquipmentItem.IsTwoHandTarget())
            {
                return true;
            }
            if (_selectedEquipmentItem.IsOneHandTarget())
            {
                if (_selectedEquipmentItem.IsSecondaryTarget())
                {
                    return false;
                }
                if (Inventory.IsEquippedTwoHanded())
                {
                    return false;
                }
                _ = Inventory.EquippedPrimary;
                return false;
            }
            if (_selectedEquipmentItem.IsEquippable)
            {
                return true;
            }
            return false;
        }

        private bool CanEquipPrimaryEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            if (!_selectedEquipmentItem.IsEquippable)
            {
                return false;
            }
            if (_selectedEquipmentItem.IsOneHandTarget())
            {
                if (_selectedEquipmentItem.IsSecondaryTarget())
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool CanEquipSecondaryEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            if (!_selectedEquipmentItem.IsEquippable)
            {
                return false;
            }
            if (_selectedEquipmentItem.IsOneHandTarget())
            {
                if (_selectedEquipmentItem.IsPrimaryTarget())
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool CanEquipVersatileEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            if (!_selectedEquipmentItem.IsEquippable)
            {
                return false;
            }
            if (_selectedEquipmentItem.Item.HasVersatile)
            {
                return true;
            }
            return false;
        }

        private bool CanToggleAttunementEquipmentItem()
        {
            if (_selectedEquipmentItem == null)
            {
                return false;
            }
            if (_selectedEquipmentItem.IsAttunable)
            {
                return true;
            }
            return false;
        }

        private bool CanAddToAttacks()
        {
            if (SelectedEquipmentItem == null)
            {
                return false;
            }
            if (SelectedEquipmentItem.Item.Type == "Weapon")
            {
                return true;
            }
            return false;
        }

        public RefactoredEquipmentSectionViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            _expressionInterpreter = new ExpressionInterpreter();
            _items.AddRange(DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Item") || x.Type.Equals("Magic Item") || x.Type.Equals("Armor") || x.Type.Equals("Weapon")));
            List<string> list = (from x in _items.Select((ElementBase x) => x.Source).Distinct()
                                 orderby x
                                 select x).ToList();
            list.Insert(0, "--");
            //ItemFilter = new ElementFilter(list)
            //{
            //    IncludeKeywords = true,
            //    IsSourceFilterAvailable = true
            //};
            //ItemFilter.PropertyChanged += ItemFilter_PropertyChanged;
            AddSelectedItemCommand = new RelayCommand(AddSelectedItem, CanAddSelectedItem);
            BuySelectedItemCommand = new RelayCommand(BuySelectedItem, BuyAddSelectedItem);
            DeleteSelectedEquipmentCommand = new RelayCommand(DeleteSelectedEquipment, CanDeleteSelectedEquipment);
            DeleteAllSelectedEquipmentCommand = new RelayCommand(DeleteAllSelectedEquipment, CanDeleteAllSelectedEquipment);
            MoveSelectedEquipmentItemUpCommand = new RelayCommand(MoveSelectedEquipmentItemUp, CanMoveSelectedEquipmentItemUp);
            MoveSelectedEquipmentItemDownCommand = new RelayCommand(MoveSelectedEquipmentItemDown, CanMoveSelectedEquipmentItemDown);
            ManageEquipmentItemCommand = new RelayCommand(ManageEquipmentItem, CanManageEquipmentItem);
            ActivateEquipmentItemCommand = new RelayCommand(ActivateEquipmentItem);
            EquipEquipmentItemCommand = new RelayCommand(EquipEquipmentItem, CanEquipEquipmentItem);
            EquipPrimaryEquipmentItemCommand = new RelayCommand(EquipPrimaryEquipmentItem, CanEquipPrimaryEquipmentItem);
            EquipSecondaryEquipmentItemCommand = new RelayCommand(EquipSecondaryEquipmentItem, CanEquipSecondaryEquipmentItem);
            EquipVersatileEquipmentItemCommand = new RelayCommand(EquipVersatileEquipmentItem, CanEquipVersatileEquipmentItem);
            ToggleAttunementEquipmentItemCommand = new RelayCommand(ToggleAttunementEquipmentItem, CanToggleAttunementEquipmentItem);
            AddToAttacksCommand = new RelayCommand(AddToAttacks, CanAddToAttacks);
            ExtractEquipmentItemCommand = new RelayCommand(ExtractEquipmentItem, CanExtractEquipmentItem);
            IncrementSelectedEquipmentItemAmountCommand = new RelayCommand(delegate
            {
                SelectedEquipmentItem.Amount++;
                Inventory.CalculateWeight();
            }, () => SelectedEquipmentItem != null && SelectedEquipmentItem.IsStackable);
            DecrementSelectedEquipmentItemAmountCommand = new RelayCommand(delegate
            {
                SelectedEquipmentItem.Amount--;
                Inventory.CalculateWeight();
            }, () => SelectedEquipmentItem != null && SelectedEquipmentItem.IsStackable && SelectedEquipmentItem.Amount > 1);
            StoreSelectedEquipmentAsPrimaryCommand = new RelayCommand(StoreSelectedEquipmentInPrimary, CanStoreSelectedEquipmentInPrimary);
            StoreSelectedEquipmentAsSecondaryCommand = new RelayCommand(StoreSelectedEquipmentInSecondary, CanStoreSelectedEquipmentInSecondary);
            RetrieveStoredSelectedEquipmentCommand = new RelayCommand(RetrieveStoredSelectedEquipment, () => SelectedEquipmentItem != null && SelectedEquipmentItem.IsStored);
            InitializeCollections();
            foreach (Item item in from Item x in _items
                                  orderby x.Name
                                  select x)
            {
                EquipmentCategory equipmentCategory = EquipmentCategories.FirstOrDefault();
                switch (item.Type)
                {
                    case "Item":
                        equipmentCategory = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals(item.Category)) ?? equipmentCategory;
                        break;
                    case "Magic Item":
                        equipmentCategory = EquipmentCategories.First((EquipmentCategory x) => x.DisplayName.Equals("Wondrous Items", StringComparison.OrdinalIgnoreCase));
                        equipmentCategory = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals(item.Category)) ?? equipmentCategory;
                        break;
                    case "Armor":
                    case "Weapon":
                        continue;
                }
                if (equipmentCategory == null)
                {
                    Logger.Warning($"unable to add {item} with category: {item.Category} to non-existing category");
                }
                else
                {
                    equipmentCategory.Items.Add(item);
                }
            }
            Inventory.Items.CollectionChanged += EquipmentItems_CollectionChanged;
            Logger.Warning($"{_items.Count} items loaded");
            SelectedEquipmentCategory = EquipmentCategories.FirstOrDefault();
            Inventory.Coins.PropertyChanged += Coinage_PropertyChanged;
            Columns.IsItemcardsColumnVisible = base.Settings.IncludeItemcards;
            ShowInventoryItemCardColumn = base.Settings.IncludeItemcards;
            Manager.SourcesManager.SourceRestrictionsApplied += SourceRestrictionsApplied;
            SubscribeWithEventAggregator();
        }

        private void ItemFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name") || e.PropertyName.Equals("Source"))
            {
                //if (string.IsNullOrWhiteSpace(ItemFilter.Name) && ItemFilter.Source != null && ItemFilter.Source.Equals("--"))
                //{
                //    UpdateItemsCollections();
                //}
                //else
                //{
                //    UpdateItemsCollections(filter: true);
                //}
            }
        }

        private void InitializeEquipmentCategories()
        {
            EquipmentCategories.Add(new EquipmentCategory("Adventuring Gear"));
            EquipmentCategories.Add(new EquipmentCategory("Treasure"));
            EquipmentCategories.Add(new EquipmentCategory("Equipment Packs"));
            EquipmentCategories.Add(new EquipmentCategory("Tools"));
            EquipmentCategories.Add(new EquipmentCategory("Musical Instruments"));
            EquipmentCategories.Add(new EquipmentCategory("Armor"));
            EquipmentCategories.Add(new EquipmentCategory("Magic Armor"));
            EquipmentCategories.Add(new EquipmentCategory("Weapons"));
            EquipmentCategories.Add(new EquipmentCategory("Magic Weapons"));
            EquipmentCategories.Add(new EquipmentCategory("Ammunition"));
            EquipmentCategories.Add(new EquipmentCategory("Spellcasting Focus"));
            EquipmentCategories.Add(new EquipmentCategory("Wondrous Items"));
            EquipmentCategories.Add(new EquipmentCategory("Artificer Infusions"));
            EquipmentCategories.Add(new EquipmentCategory("Supernatural Gifts"));
            EquipmentCategories.Add(new EquipmentCategory("Staffs"));
            EquipmentCategories.Add(new EquipmentCategory("Rods"));
            EquipmentCategories.Add(new EquipmentCategory("Wands"));
            EquipmentCategories.Add(new EquipmentCategory("Rings"));
            EquipmentCategories.Add(new EquipmentCategory("Potions"));
            EquipmentCategories.Add(new EquipmentCategory("Poison"));
            EquipmentCategories.Add(new EquipmentCategory("Scrolls"));
            EquipmentCategories.Add(new EquipmentCategory("Spell Scrolls"));
            foreach (string item in from x in (from Item x in _items
                                               where !x.Category.StartsWith("Additional ")
                                               select x.Category).Distinct()
                                    orderby x
                                    select x)
            {
                if (!string.IsNullOrWhiteSpace(item) && !EquipmentCategories.Select((EquipmentCategory x) => x.DisplayName).Contains(item))
                {
                    EquipmentCategories.Add(new EquipmentCategory(item));
                }
            }
            foreach (string item2 in (from Item x in _items
                                      where x.Category.StartsWith("Additional ")
                                      select x.Category).Distinct())
            {
                if (!string.IsNullOrWhiteSpace(item2) && !EquipmentCategories.Select((EquipmentCategory x) => x.DisplayName).Contains(item2))
                {
                    EquipmentCategories.Add(new EquipmentCategory(item2));
                }
            }
        }

        private void Coinage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalculateWeight();
        }

        private void RaiseEquipmentItemCommands()
        {
            AddSelectedItemCommand.RaiseCanExecuteChanged();
            BuySelectedItemCommand.RaiseCanExecuteChanged();
            DeleteSelectedEquipmentCommand.RaiseCanExecuteChanged();
            DeleteAllSelectedEquipmentCommand.RaiseCanExecuteChanged();
            MoveSelectedEquipmentItemUpCommand.RaiseCanExecuteChanged();
            MoveSelectedEquipmentItemDownCommand.RaiseCanExecuteChanged();
            ManageEquipmentItemCommand.RaiseCanExecuteChanged();
            ActivateEquipmentItemCommand.RaiseCanExecuteChanged();
            EquipEquipmentItemCommand.RaiseCanExecuteChanged();
            EquipPrimaryEquipmentItemCommand.RaiseCanExecuteChanged();
            EquipSecondaryEquipmentItemCommand.RaiseCanExecuteChanged();
            EquipVersatileEquipmentItemCommand.RaiseCanExecuteChanged();
            ToggleAttunementEquipmentItemCommand.RaiseCanExecuteChanged();
            AddToAttacksCommand.RaiseCanExecuteChanged();
            ExtractEquipmentItemCommand.RaiseCanExecuteChanged();
            AllowExtractEquipmentItem = CanExtractEquipmentItem();
            IncrementSelectedEquipmentItemAmountCommand.RaiseCanExecuteChanged();
            DecrementSelectedEquipmentItemAmountCommand.RaiseCanExecuteChanged();
            StoreSelectedEquipmentAsPrimaryCommand.RaiseCanExecuteChanged();
            StoreSelectedEquipmentAsSecondaryCommand.RaiseCanExecuteChanged();
            RetrieveStoredSelectedEquipmentCommand.RaiseCanExecuteChanged();
            AllowActivateEquipmentItem = CanActivateEquipmentItem();
            AllowEquipEquipmentItem = CanEquipEquipmentItem();
            AllowEquipPrimaryEquipmentItem = CanEquipPrimaryEquipmentItem();
            AllowEquipSecondaryEquipmentItem = CanEquipSecondaryEquipmentItem();
            AllowEquipVersatileEquipmentItem = CanEquipVersatileEquipmentItem();
            AllowToggleAttunementEquipmentItem = CanToggleAttunementEquipmentItem();
            AllowEquipmentItemAddToAttacks = CanAddToAttacks();
        }

        private decimal CalculateWeight()
        {
            return CalculateWeight(Inventory.Items);
        }

        private decimal CalculateWeight(IEnumerable<RefactoredEquipmentItem> equipmentItems)
        {
            decimal num = Inventory.CalculateWeight(equipmentItems);
            EquipmentWeight = $"{num}";
            return num;
        }

        private int CalculateAttunementCount()
        {
            Inventory.CalculateAttunedItemCount();
            return Inventory.AttunedItemCount;
        }

        private void EquipmentItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Manager.Status.IsLoaded)
            {
                CalculateWeight();
            }
            RefactoredEquipmentItem selectedEquipmentItem = _selectedEquipmentItem;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count == 0)
                {
                    return;
                }
                RefactoredEquipmentItem refactoredEquipmentItem = (RefactoredEquipmentItem)e.NewItems[0];
                if (!refactoredEquipmentItem.Item.IsEquippable && !refactoredEquipmentItem.IsActivated)
                {
                    return;
                }
                if (refactoredEquipmentItem.IsActivated)
                {
                    bool isEquipped = refactoredEquipmentItem.IsEquipped;
                    bool isAttuned = refactoredEquipmentItem.IsAttuned;
                    string equippedLocation = refactoredEquipmentItem.EquippedLocation;
                    refactoredEquipmentItem.Deactivate();
                    SelectedEquipmentItem = refactoredEquipmentItem;
                    if (isEquipped)
                    {
                        if (!string.IsNullOrWhiteSpace(equippedLocation))
                        {
                            switch (equippedLocation)
                            {
                                case "One-Hand":
                                case "Primary":
                                case "Primary Hand":
                                case "mainhand":
                                    EquipPrimaryEquipmentItem();
                                    break;
                                case "Secondary":
                                case "Secondary Hand":
                                case "offhand":
                                    EquipSecondaryEquipmentItem();
                                    break;
                                case "body":
                                case "Armor":
                                    EquipEquipmentItem();
                                    break;
                                case "Two-Handed":
                                    EquipEquipmentItem();
                                    break;
                                case "Two-Handed (Versatile)":
                                    EquipVersatileEquipmentItem();
                                    break;
                            }
                        }
                        else
                        {
                            EquipEquipmentItem();
                        }
                        if (isAttuned && !SelectedEquipmentItem.IsAttuned)
                        {
                            ToggleAttunementEquipmentItem();
                        }
                    }
                    else if (isAttuned)
                    {
                        ToggleAttunementEquipmentItem();
                    }
                    SelectedEquipmentItem = selectedEquipmentItem;
                    return;
                }
                if (Manager.Status.IsLoaded)
                {
                    if (!refactoredEquipmentItem.Item.HasMultipleSlots)
                    {
                        _ = refactoredEquipmentItem.Item.Slot;
                    }
                    else
                    {
                        refactoredEquipmentItem.Item.Slots.First();
                    }
                    if (refactoredEquipmentItem.IsArmorTarget() && Inventory.EquippedArmor == null)
                    {
                        SelectedEquipmentItem = refactoredEquipmentItem;
                        EquipEquipmentItem();
                    }
                    else if (refactoredEquipmentItem.IsTwoHandTarget() && Inventory.EquippedPrimary == null && Inventory.EquippedSecondary == null)
                    {
                        SelectedEquipmentItem = refactoredEquipmentItem;
                        EquipEquipmentItem();
                    }
                    else if (refactoredEquipmentItem.IsOneHandTarget() && refactoredEquipmentItem.IsSecondaryTarget() && Inventory.EquippedSecondary == null)
                    {
                        SelectedEquipmentItem = refactoredEquipmentItem;
                        EquipEquipmentItem();
                    }
                    else if (refactoredEquipmentItem.IsOneHandTarget() && Inventory.EquippedPrimary == null && !refactoredEquipmentItem.IsSecondaryTarget())
                    {
                        SelectedEquipmentItem = refactoredEquipmentItem;
                        EquipEquipmentItem();
                    }
                    else if (!refactoredEquipmentItem.IsArmorTarget() && !refactoredEquipmentItem.IsOneHandTarget() && !refactoredEquipmentItem.IsTwoHandTarget() && !refactoredEquipmentItem.IsPrimaryTarget() && !refactoredEquipmentItem.IsSecondaryTarget() && refactoredEquipmentItem.IsAttunable && !Inventory.AllowMoreAttunement())
                    {
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count == 0)
                {
                    return;
                }
                RefactoredEquipmentItem refactoredEquipmentItem2 = (RefactoredEquipmentItem)e.OldItems[0];
                if (!refactoredEquipmentItem2.Item.IsEquippable)
                {
                    return;
                }
                refactoredEquipmentItem2.Deactivate();
            }
            if (Manager.Status.IsLoaded)
            {
                CalculateWeight();
            }
            SelectedEquipmentItem = selectedEquipmentItem;
        }

        private void UpdateItemsCollections(bool filter = false)
        {
            ItemsCollection.Clear();
            Columns.CollapseColumns();
            if (IsArmorSelection)
            {
                if (_selectedArmorElement == null)
                {
                    _selectedArmorElement = ArmorCollection.First();
                }
                foreach (ElementBase item in _a.OrderBy((ElementBase x) => x.Name))
                {
                    if (item.ElementSetters.ContainsSetter("armor"))
                    {
                        string value = item.ElementSetters.GetSetter("armor").Value;
                        if (!string.IsNullOrWhiteSpace(value) && _expressionInterpreter.EvaluateSupportsExpression(value, ArmorCollection).Contains(SelectedArmorElement))
                        {
                            ItemsCollection.Add(item);
                        }
                    }
                }
            }
            else if (IsWeaponSelection)
            {
                if (_selectedWeaponElement == null)
                {
                    _selectedWeaponElement = WeaponsCollection.First();
                }
                foreach (ElementBase item2 in _w.OrderBy((ElementBase x) => x.Name))
                {
                    if (item2.ElementSetters.ContainsSetter("weapon"))
                    {
                        string value2 = item2.ElementSetters.GetSetter("weapon").Value;
                        if (!string.IsNullOrWhiteSpace(value2) && _expressionInterpreter.EvaluateSupportsExpression(value2, WeaponsCollection).Contains(SelectedWeaponElement))
                        {
                            ItemsCollection.Add(item2);
                        }
                    }
                }
            }
            else
            {
                ItemsCollection.AddRange(_selectedEquipmentCategory.Items);
            }
            //if (filter || ItemFilter.IsLocked)
            //{
            //    List<ElementBase> elements = ItemFilter.Filter(ItemsCollection);
            //    ItemsCollection.Clear();
            //    ItemsCollection.AddRange(elements);
            //}
            SourcesManager sourcesManager = CharacterManager.Current.SourcesManager;
            List<string> list = sourcesManager.GetUndefinedRestrictedSourceNames().ToList();
            List<string> list2 = sourcesManager.GetRestrictedElementIds().ToList();
            ElementBaseCollection elementBaseCollection = new ElementBaseCollection();
            foreach (ElementBase item3 in ItemsCollection)
            {
                if (list2.Contains(item3.Id))
                {
                    elementBaseCollection.Add(item3);
                }
                else if (list.Contains(item3.Source))
                {
                    elementBaseCollection.Add(item3);
                }
            }
            foreach (ElementBase item4 in elementBaseCollection)
            {
                ItemsCollection.Remove(item4);
            }
            Columns.IsPriceColumnVisible = true;
            Columns.IsWeightColumnVisible = true;
            Columns.IsSourceColumnVisible = true;
            string displayName = SelectedEquipmentCategory.DisplayName;
            if (displayName != null)
            {
                switch (displayName)
                {
                    case "Armor":
                        Columns.DisplayArmorColumns();
                        break;
                    case "Weapons":
                        Columns.DisplayWeaponColumns();
                        break;
                    case "Magic Armor":
                    case "Magic Weapons":
                    case "Staffs":
                    case "Wands":
                    case "Rods":
                        Columns.IsRarityColumnVisible = true;
                        Columns.IsAttunementColumnVisible = true;
                        break;
                    case "Rings":
                    case "Wondrous Items":
                        Columns.IsRarityColumnVisible = true;
                        Columns.IsAttunementColumnVisible = true;
                        break;
                    case "Potion":
                    case "Potions":
                    case "Poison":
                    case "Scrolls":
                    case "Spell Scrolls":
                        Columns.IsRarityColumnVisible = true;
                        break;
                }
            }
        }

        public void UpdateSubCollections()
        {
            ArmorElement selectedArmorElement = _selectedArmorElement;
            List<ArmorElement> list = ArmorCollection.ToList();
            ArmorCollection.Clear();
            foreach (ArmorElement item in list)
            {
                ArmorCollection.Add(item);
            }
            _selectedArmorElement = selectedArmorElement;
            WeaponElement selectedWeaponElement = _selectedWeaponElement;
            List<WeaponElement> list2 = WeaponsCollection.ToList();
            WeaponsCollection.Clear();
            foreach (WeaponElement item2 in list2)
            {
                WeaponsCollection.Add(item2);
            }
            _selectedWeaponElement = selectedWeaponElement;
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            UpdateCollections();
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            UpdateCollections();
        }

        void ISubscriber<SettingsChangedEvent>.OnHandleEvent(SettingsChangedEvent args)
        {
            Columns.IsItemcardsColumnVisible = args.Settings.IncludeItemcards;
            OnSettingsChanged();
        }

        public void OnSettingsChanged()
        {
            ShowInventoryItemCardColumn = base.Settings.IncludeItemcards;
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            AllowActivateEquipmentItem = true;
            AllowEquipEquipmentItem = true;
            AllowEquipPrimaryEquipmentItem = true;
            AllowEquipSecondaryEquipmentItem = true;
            AllowEquipVersatileEquipmentItem = true;
            AllowToggleAttunementEquipmentItem = true;
            EquipmentWeight = "1349 lb.";
            EquipmentCategories.Add(new EquipmentCategory("Adventuring Gear"));
            EquipmentCategories.Add(new EquipmentCategory("Armor"));
            EquipmentCategories.Add(new EquipmentCategory("Weapons"));
            EquipmentCategories.Add(new EquipmentCategory("Shields"));
            EquipmentCategories.Add(new EquipmentCategory("Trinkets"));
        }

        public void OnHandleEvent(CharacterLoadingCompletedEvent args)
        {
            UpdateCollections();
        }

        private void SourceRestrictionsApplied(object sender, EventArgs e)
        {
            UpdateCollections(updateItemsCollection: true);
        }

        public void InitializeCollections()
        {
            CategoriesCollectionView = CollectionViewSource.GetDefaultView(EquipmentCategories);
            CategoriesCollectionView.Filter = (object item) => item is EquipmentCategory equipmentCategory && equipmentCategory.IsEnabled;
            WeaponElementsCollectionView = CollectionViewSource.GetDefaultView(WeaponsCollection);
            ArmorElementsCollectionView = CollectionViewSource.GetDefaultView(ArmorCollection);
            InitializeCategoryCollection();
            InitializeWeaponCollection();
            InitializeArmorCollection();
        }

        private void InitializeCategoryCollection()
        {
            string selectedCategoryName = _selectedEquipmentCategory?.DisplayName;
            EquipmentCategories.Clear();
            List<string> orderedCategories = new List<string>
        {
            "Adventuring Gear", "Treasure", "Trade Goods", "Equipment Packs", "Tools", "Musical Instruments", "Armor", "Magic Armor", "Weapons", "Magic Weapons",
            "Ammunition", "Spellcasting Focus", "Wondrous Items", "Supernatural Gifts", "Staffs", "Rods", "Wands", "Rings", "Potions", "Poison",
            "Scrolls", "Spell Scrolls"
        };
            List<string> list = (from Item x in _items
                                 select x.Category).Distinct().ToList();
            list.RemoveAll((string cat) => orderedCategories.Contains(cat));
            foreach (string item in list.Where((string x) => !x.StartsWith("Additional ")))
            {
                if (!string.IsNullOrWhiteSpace(item) && !orderedCategories.Contains(item))
                {
                    orderedCategories.Add(item);
                }
            }
            list.RemoveAll((string cat) => orderedCategories.Contains(cat));
            foreach (string item2 in list.Where((string x) => x.StartsWith("Additional ")))
            {
                if (!string.IsNullOrWhiteSpace(item2) && !orderedCategories.Contains(item2))
                {
                    orderedCategories.Add(item2);
                }
            }
            list.RemoveAll((string cat) => orderedCategories.Contains(cat));
            foreach (string item3 in list)
            {
                if (!string.IsNullOrWhiteSpace(item3) && !orderedCategories.Contains(item3))
                {
                    orderedCategories.Add(item3);
                }
            }
            foreach (string item4 in orderedCategories)
            {
                if (!string.IsNullOrWhiteSpace(item4))
                {
                    EquipmentCategories.Add(new EquipmentCategory(item4));
                }
            }
            SelectedEquipmentCategory = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals(selectedCategoryName));
        }

        private void InitializeWeaponCollection()
        {
            EquipmentCategory equipmentCategory = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals("Weapons"));
            _baseWeapons = (from WeaponElement x in _items.Where((ElementBase x) => x.Type == "Weapon")
                            orderby x.Name
                            select x).ToList();
            foreach (WeaponElement baseWeapon in _baseWeapons)
            {
                WeaponsCollection.Add(baseWeapon);
                equipmentCategory.Items.Add(baseWeapon);
            }
            List<ElementBase> elements = (from x in _items
                                          where x.Type == "Magic Item"
                                          where x.ElementSetters.ContainsSetter("type") && x.ElementSetters.GetSetter("type").Value.Equals("weapon", StringComparison.OrdinalIgnoreCase)
                                          select x).ToList();
            _w.AddRange(elements);
        }

        private void InitializeArmorCollection()
        {
            EquipmentCategory equipmentCategory = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals("Armor"));
            List<ArmorElement> source = (from ArmorElement x in _items.Where((ElementBase x) => x.Type == "Armor")
                                         orderby x.Name
                                         select x).ToList();
            IEnumerable<ArmorElement> collection = source.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Light));
            IEnumerable<ArmorElement> collection2 = source.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Medium));
            IEnumerable<ArmorElement> collection3 = source.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Heavy));
            IEnumerable<ArmorElement> collection4 = source.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Shield));
            _baseArmors.AddRange(collection);
            _baseArmors.AddRange(collection2);
            _baseArmors.AddRange(collection3);
            _baseArmors.AddRange(collection4);
            foreach (ArmorElement baseArmor in _baseArmors)
            {
                ArmorCollection.Add(baseArmor);
            }
            equipmentCategory.Items.AddRange(ArmorCollection);
            List<ElementBase> elements = (from x in _items
                                          where x.Type == "Magic Item"
                                          where x.ElementSetters.ContainsSetter("type") && x.ElementSetters.GetSetter("type").Value.Equals("armor", StringComparison.OrdinalIgnoreCase)
                                          select x).ToList();
            _a.AddRange(elements);
        }

        public async Task UpdateCollections(bool updateItemsCollection = false)
        {
            await Task.Run((Action)UpdateCategoryCollection);
            CategoriesCollectionView.Refresh();
            UpdateWeaponCollection();
            WeaponElementsCollectionView.Refresh();
            UpdateArmorCollection();
            ArmorElementsCollectionView.Refresh();
            UpdateItemFilterSources();
            if (updateItemsCollection)
            {
                UpdateItemsCollections();
            }
        }

        public void UpdateCategoryCollection()
        {
            if (!Manager.Status.IsLoaded)
            {
                return;
            }
            List<string> list = (from x in Manager.GetSpellcastingInformations()
                                 select "Additional " + x.Name + " Spell").ToList();
            foreach (EquipmentCategory equipmentCategory in EquipmentCategories)
            {
                equipmentCategory.IsEnabled = true;
                if (equipmentCategory.DisplayName.Contains("Additional ") && equipmentCategory.DisplayName.Contains(" Spell") && !equipmentCategory.DisplayName.Equals("Additional Spell"))
                {
                    equipmentCategory.IsEnabled = list.Contains(equipmentCategory.DisplayName);
                }
            }
            IEnumerable<string> restricted = Manager.SourcesManager.RestrictedSources.Select((SourceItem x) => x.Source.Name);
            foreach (EquipmentCategory equipmentCategory2 in EquipmentCategories)
            {
                if (equipmentCategory2.Items.Select((ElementBase x) => x.Source).Distinct().All((string x) => restricted.Contains(x)))
                {
                    equipmentCategory2.IsEnabled = false;
                }
                int enabledItemCount = equipmentCategory2.Items.Count((ElementBase x) => !restricted.Contains(x.Source));
                equipmentCategory2.EnabledItemCount = enabledItemCount;
            }
        }

        public void UpdateWeaponCollection()
        {
            List<string> list = Manager.SourcesManager.RestrictedSources.Select((SourceItem x) => x.Source.Name).ToList();
            WeaponElement selectedWeaponElement = _selectedWeaponElement;
            WeaponsCollection.Clear();
            foreach (WeaponElement baseWeapon in _baseWeapons)
            {
                if (!list.Contains(baseWeapon.Source))
                {
                    WeaponsCollection.Add(baseWeapon);
                }
            }
            _selectedWeaponElement = selectedWeaponElement;
        }

        public void UpdateArmorCollection()
        {
            List<string> list = Manager.SourcesManager.RestrictedSources.Select((SourceItem x) => x.Source.Name).ToList();
            ArmorElement selectedArmorElement = _selectedArmorElement;
            ArmorCollection.Clear();
            foreach (ArmorElement baseArmor in _baseArmors)
            {
                if (!list.Contains(baseArmor.Source))
                {
                    ArmorCollection.Add(baseArmor);
                }
            }
            _selectedArmorElement = selectedArmorElement;
        }

        public void UpdateItemFilterSources()
        {
            List<string> list = (from x in _items.Select((ElementBase x) => x.Source).Distinct()
                                 orderby x
                                 select x).ToList();
            List<string> restricted = Manager.SourcesManager.RestrictedSources.Select((SourceItem x) => x.Source.Name).ToList();
            list.RemoveAll((string x) => restricted.Contains(x));
            list.Insert(0, "--");
            //ItemFilter.SourceCollection.Clear();
            //foreach (string item in list)
            //{
            //    ItemFilter.SourceCollection.Add(item);
            //}
            //ItemFilter.Clear();
        }

        public void UpdateItemsCollection()
        {
            foreach (Item item in from Item x in _items
                                  orderby x.Name
                                  select x)
            {
                EquipmentCategory equipmentCategory = EquipmentCategories.FirstOrDefault();
                switch (item.Type)
                {
                    case "Item":
                        equipmentCategory = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals(item.Category)) ?? equipmentCategory;
                        break;
                    case "Magic Item":
                        equipmentCategory = EquipmentCategories.First((EquipmentCategory x) => x.DisplayName.Equals("Wondrous Items", StringComparison.OrdinalIgnoreCase));
                        equipmentCategory = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals(item.Category)) ?? equipmentCategory;
                        break;
                    case "Armor":
                    case "Weapon":
                        continue;
                }
                if (equipmentCategory == null)
                {
                    Logger.Warning($"unable to add {item} with category: {item.Category} to non-existing category");
                }
                else
                {
                    equipmentCategory.Items.Add(item);
                }
            }
            EquipmentCategory equipmentCategory2 = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals("Armor"));
            EquipmentCategory equipmentCategory3 = EquipmentCategories.FirstOrDefault((EquipmentCategory x) => x.DisplayName.Equals("Weapons"));
            List<ArmorElement> list = _items.Where((ElementBase x) => x.Type == "Armor").Cast<ArmorElement>().ToList();
            IEnumerable<ArmorElement> enumerable = list.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Light));
            IEnumerable<ArmorElement> enumerable2 = list.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Medium));
            IEnumerable<ArmorElement> enumerable3 = list.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Heavy));
            IEnumerable<ArmorElement> enumerable4 = list.Where((ArmorElement x) => x.Supports.Contains(InternalArmorGroups.Shield));
            foreach (ArmorElement item2 in enumerable)
            {
                ArmorCollection.Add(item2);
            }
            foreach (ArmorElement item3 in enumerable2)
            {
                ArmorCollection.Add(item3);
            }
            foreach (ArmorElement item4 in enumerable3)
            {
                ArmorCollection.Add(item4);
            }
            foreach (ArmorElement item5 in enumerable4)
            {
                ArmorCollection.Add(item5);
            }
            foreach (ArmorElement item6 in list)
            {
                if (!ArmorCollection.Contains(item6))
                {
                    ArmorCollection.Add(item6);
                }
            }
            equipmentCategory2.Items.AddRange(ArmorCollection);
            foreach (WeaponElement item7 in (from WeaponElement x in _items.Where((ElementBase x) => x.Type == "Weapon")
                                             orderby x.Name
                                             select x).ToList())
            {
                WeaponsCollection.Add(item7);
                equipmentCategory3.Items.Add(item7);
            }
            List<ElementBase> elements = (from x in _items
                                          where x.Type == "Magic Item"
                                          where x.ElementSetters.ContainsSetter("type") && x.ElementSetters.GetSetter("type").Value.Equals("armor", StringComparison.OrdinalIgnoreCase)
                                          select x).ToList();
            _a.AddRange(elements);
            List<ElementBase> elements2 = (from x in _items
                                           where x.Type == "Magic Item"
                                           where x.ElementSetters.ContainsSetter("type") && x.ElementSetters.GetSetter("type").Value.Equals("weapon", StringComparison.OrdinalIgnoreCase)
                                           select x).ToList();
            _w.AddRange(elements2);
        }
    }
}
