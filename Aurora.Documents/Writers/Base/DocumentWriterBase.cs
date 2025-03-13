using Aurora.Documents.Helpers;
using Aurora.Documents.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Documents.Writers.Base
{
    public abstract class DocumentWriterBase
    {
        protected DocumentResources Resources { get; }

        protected FontsHelper Fonts { get; }

        protected DescriptionConverter DescriptionConverter { get; }

        protected DocumentWriterBase()
        {
            Resources = new DocumentResources();
            Fonts = new FontsHelper();
            DescriptionConverter = new DescriptionConverter(Fonts);
        }
    }
}
