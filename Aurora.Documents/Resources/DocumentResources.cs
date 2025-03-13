using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.Resources
{
    public class DocumentResources
    {
        public Stream GetResource(string resource)
        {
            return Assembly.GetAssembly(typeof(DocumentResources)).GetManifestResourceStream(resource);
        }
    }
}
