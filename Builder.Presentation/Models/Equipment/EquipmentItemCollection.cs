using Builder.Data.Elements;
using System.Collections.ObjectModel;
using System.Linq;


namespace Builder.Presentation.Models.Equipment
{
    public class EquipmentItemCollection : ObservableCollection<EquipmentItem>
    {
        public bool Contains(Item item)
        {
            return this.Any((EquipmentItem equipmentItem) => equipmentItem.Item.Id == item.Id);
        }

        public EquipmentItem GetEquipmentItem(string id)
        {
            return this.First((EquipmentItem x) => x.Item.Id == id);
        }

        public void AddItem(Item itemElement, int amount = 1)
        {
            if (Contains(itemElement))
            {
                if (itemElement.IsStackable)
                {
                    GetEquipmentItem(itemElement.Id).Amount += amount;
                    return;
                }
                for (int i = 0; i < amount; i++)
                {
                    Add(new EquipmentItem(itemElement));
                }
            }
            else if (itemElement.IsStackable)
            {
                Add(new EquipmentItem(itemElement)
                {
                    Amount = amount
                });
            }
            else
            {
                for (int j = 0; j < amount; j++)
                {
                    Add(new EquipmentItem(itemElement));
                }
            }
        }

        public void DeleteOne(string id)
        {
            EquipmentItem equipmentItem = GetEquipmentItem(id);
            if (equipmentItem.Amount > 1)
            {
                equipmentItem.Amount--;
            }
            else
            {
                Remove(equipmentItem);
            }
        }

        public void DeleteAll(string id)
        {
            Remove(GetEquipmentItem(id));
        }
    }
}
