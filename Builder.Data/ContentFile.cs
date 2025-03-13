using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Services
{
    public abstract class ContentFile
    {
        public ContentFileInformation Information { get; }

        protected ContentFile()
        {
            Information = new ContentFileInformation();
        }
    }
}
