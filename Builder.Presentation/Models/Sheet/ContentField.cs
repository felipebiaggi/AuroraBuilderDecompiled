using System.Collections.Generic;

namespace Builder.Presentation.Models.Sheet
{
    public class ContentField
    {
        public string Key { get; set; }

        public List<ContentLine> Lines { get; set; }

        public ContentField(string key = "")
        {
            Key = key;
            Lines = new List<ContentLine>();
        }
    }

}
