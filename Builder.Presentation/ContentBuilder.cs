using Builder.Presentation.Models.Sheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation
{
    public class ContentBuilder
    {
        private readonly ContentField _field;

        public string Key => _field.Key;

        public ContentBuilder(string key = "")
        {
            _field = new ContentField(key);
        }

        public ContentBuilder Append(string name, string content, bool indent)
        {
            ContentLine item = new ContentLine(name, content, newLineBefore: false, indent);
            _field.Lines.Add(item);
            return this;
        }

        public ContentBuilder AppendNewLine()
        {
            return this;
        }

        public ContentField GetContentField()
        {
            return _field;
        }
    }
}
