using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Files
{
    [Serializable]
    public class ElementsFileLoadException : Exception
    {
        public ElementsFileLoadException()
        {
        }

        public ElementsFileLoadException(string message)
            : base(message)
        {
        }

        public ElementsFileLoadException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ElementsFileLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
