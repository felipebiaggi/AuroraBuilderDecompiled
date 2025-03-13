using System;
using System.Collections.Generic;
using Builder.Data;

namespace Builder.Presentation.Elements
{
    public class ElementCollectionEventArgs : EventArgs
    {
        public List<ElementBase> Elements { get; }

        public ElementCollectionEventArgs(List<ElementBase> elements)
        {
            Elements = elements;
        }
    }
}
