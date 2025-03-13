using Builder.Data.Files.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Files.Updater
{
    public class IndexUpdateResult : FileUpdateResult<IndexFile>
    {
        public Version RequiredAppVersion { get; set; }

        public IndexUpdateResult(bool success, IndexFile file)
            : base(success, file)
        {
        }
    }

}
