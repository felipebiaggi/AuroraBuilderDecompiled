using Aurora.Documents.ExportContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.Sheets
{
    public class AuroraCharacterSheet : CharacterSheetBase
    {
        public AuroraCharacterSheet(CharacterSheetConfiguration configuration)
            : base(configuration)
        {
        }

        public override void Generate(IExportContentProvider provider)
        {
            if (base.Configuration.IncludeEquipmentPage)
            {
                provider.GetEquipmentContent();
            }
            if (base.Configuration.IncludeNotesPage)
            {
                provider.GetNotesContent();
            }
        }
    }
}
