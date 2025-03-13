using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.Resources.Aurora
{
    public class AuroraDocumentResources : DocumentResources
    {
        private const string Root = "Aurora.Documents.Resources.Aurora.";

        public Stream GetEquipmentPage()
        {
            return GetResource("Aurora.Documents.Resources.Aurora.Pages.equipment_page.pdf");
        }

        public Stream GetNotesPage()
        {
            return GetResource("Aurora.Documents.Resources.Aurora.Pages.notes_page.pdf");
        }
    }
}
