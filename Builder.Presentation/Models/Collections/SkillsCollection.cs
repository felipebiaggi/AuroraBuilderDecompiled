using System.Collections.Generic;

namespace Builder.Presentation.Models.Collections
{
    public class SkillsCollection
    {
        private readonly List<SkillItem> _collection;

        public SkillItem Acrobatics { get; }

        public SkillItem AnimalHandling { get; }

        public SkillItem Arcana { get; }

        public SkillItem Athletics { get; }

        public SkillItem Deception { get; }

        public SkillItem History { get; }

        public SkillItem Insight { get; }

        public SkillItem Intimidation { get; }

        public SkillItem Investigation { get; }

        public SkillItem Medicine { get; }

        public SkillItem Nature { get; }

        public SkillItem Perception { get; }

        public SkillItem Performance { get; }

        public SkillItem Persuasion { get; }

        public SkillItem Religion { get; }

        public SkillItem SleightOfHand { get; }

        public SkillItem Stealth { get; }

        public SkillItem Survival { get; }

        public SkillsCollection(AbilitiesCollection abilities)
        {
            Acrobatics = new SkillItem("Acrobatics", abilities.Dexterity);
            AnimalHandling = new SkillItem("Animal Handling", abilities.Wisdom);
            Arcana = new SkillItem("Arcana", abilities.Intelligence);
            Athletics = new SkillItem("Athletics", abilities.Strength);
            Deception = new SkillItem("Deception", abilities.Charisma);
            History = new SkillItem("History", abilities.Intelligence);
            Insight = new SkillItem("Insight", abilities.Wisdom);
            Intimidation = new SkillItem("Intimidation", abilities.Charisma);
            Investigation = new SkillItem("Investigation", abilities.Intelligence);
            Medicine = new SkillItem("Medicine", abilities.Wisdom);
            Nature = new SkillItem("Nature", abilities.Intelligence);
            Perception = new SkillItem("Perception", abilities.Wisdom);
            Performance = new SkillItem("Performance", abilities.Charisma);
            Persuasion = new SkillItem("Persuasion", abilities.Charisma);
            Religion = new SkillItem("Religion", abilities.Intelligence);
            SleightOfHand = new SkillItem("Sleight of Hand", abilities.Dexterity);
            Stealth = new SkillItem("Stealth", abilities.Dexterity);
            Survival = new SkillItem("Survival", abilities.Wisdom);
            _collection = new List<SkillItem>
        {
            Acrobatics, AnimalHandling, Arcana, Athletics, Deception, History, Insight, Intimidation, Investigation, Medicine,
            Nature, Perception, Performance, Persuasion, Religion, SleightOfHand, Stealth, Survival
        };
        }

        public IEnumerable<SkillItem> GetCollection()
        {
            return _collection;
        }
    }
}
