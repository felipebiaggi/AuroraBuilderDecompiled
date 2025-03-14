namespace Builder.Presentation.Models.Sheet
{
    public class SkillField : Field
    {
        public string ProficientKey { get; }

        public bool IsProficient { get; set; }

        public SkillField(string key, string proficientKey, string value, bool isProficient, float fontSize = 8f)
            : base(key, value, fontSize)
        {
            ProficientKey = proficientKey;
            IsProficient = isProficient;
        }
    }
}
