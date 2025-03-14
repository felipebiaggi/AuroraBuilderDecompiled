using System;

namespace Builder.Presentation.Models.Sheet
{
    public class ContentLine
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public bool NewLineBefore { get; set; }

        public bool Indent { get; set; }

        public ContentLine(string name, string content)
            : this(name, content, newLineBefore: false, indent: false)
        {
        }

        public ContentLine(string name, string content, bool newLineBefore)
            : this(name, content, newLineBefore, indent: false)
        {
        }

        public ContentLine(string name, string content, bool newLineBefore, bool indent)
        {
            Name = name;
            Content = content;
            NewLineBefore = newLineBefore;
            Indent = indent;
        }

        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(Content);
        }

        public override string ToString()
        {
            return (NewLineBefore ? Environment.NewLine : "") + (Indent ? "    " : "") + Name + ". " + Content;
        }
    }
}
