using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Files.Updater
{
    public class ElementsFileUpdateResult : FileUpdateResult<ElementsFile>
    {
        public Version RequiredAppVersion { get; set; }

        public ElementsFileUpdateResult(bool success, ElementsFile file)
            : base(success, file)
        {
        }
    }
}
