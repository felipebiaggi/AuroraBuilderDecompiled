using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.ExportContent.Equipment
{
    public class StoredItemsExportContent
    {
        public string Name { get; set; }

        public List<InventoryItemExportContent> Items { get; set; }

        public StoredItemsExportContent(string name = "")
        {
            Name = name;
            Items = new List<InventoryItemExportContent>();
        }
    }
}
