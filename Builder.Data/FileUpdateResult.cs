using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Files.Updater
{
    public class FileUpdateResult<T>
    {
        public bool Success { get; }

        public T File { get; set; }

        public FileUpdateResult(bool success, T file)
        {
            Success = success;
            File = file;
        }
    }

}
