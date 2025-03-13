using Aurora.Documents.ExportContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.Sheets
{
    public abstract class CharacterSheetBase
    {
        public CharacterSheetConfiguration Configuration { get; }

        protected CharacterSheetBase(CharacterSheetConfiguration configuration)
        {
            Configuration = configuration;
        }

        public abstract void Generate(IExportContentProvider contentProvider);
    }
}
