namespace Builder.Presentation.Models.Sheet
{
    public class Field
    {
        public string Key { get; }

        public string Value { get; set; }

        public float FontSize { get; set; }

        public Field(string key, string value, float fontSize = 8f)
        {
            Key = key;
            Value = value;
            FontSize = fontSize;
        }

        public override string ToString()
        {
            return "[" + Key + "]:[" + Value + "]";
        }
    }
}
