using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.ExportContent.Notes
{
    public class NotesExportContent
    {
        public string LeftNotesColumn { get; set; }

        public string RightNotesColumn { get; set; }

        public NotesExportContent()
        {
            LeftNotesColumn = "";
            RightNotesColumn = "";
        }
    }
}
