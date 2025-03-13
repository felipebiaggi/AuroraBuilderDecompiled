using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.ExportContent.Equipment
{
    public class VehicleExportContent
    {
        public string Name { get; set; }

        public List<InventoryItemExportContent> Cargo { get; set; }

        public string WeightCarried { get; set; }

        public VehicleExportContent(string name = "")
        {
            Name = name;
            Cargo = new List<InventoryItemExportContent>();
        }
    }
}
