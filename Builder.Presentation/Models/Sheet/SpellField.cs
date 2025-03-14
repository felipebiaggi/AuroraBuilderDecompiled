namespace Builder.Presentation.Models.Sheet
{
    public class SpellField : Field
    {
        public string PreparedKey { get; }

        public bool IsPrepared { get; set; }

        public SpellField(string key, string preparedKey, string value = "", bool isPrepared = false, float fontSize = 8f)
            : base(key, value, fontSize)
        {
            PreparedKey = preparedKey;
            IsPrepared = isPrepared;
        }

        public override string ToString()
        {
            return base.ToString() + $" | [{PreparedKey}]:[{IsPrepared}]";
        }
    }
}
