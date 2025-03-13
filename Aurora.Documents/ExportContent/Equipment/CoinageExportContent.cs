using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.ExportContent.Equipment
{
    public class CoinageExportContent
    {
        public string Copper { get; set; }

        public string Silver { get; set; }

        public string Electrum { get; set; }

        public string Gold { get; set; }

        public string Platinum { get; set; }

        public CoinageExportContent()
        {
            Copper = "";
            Silver = "";
            Electrum = "";
            Gold = "";
            Platinum = "";
        }
    }
}
