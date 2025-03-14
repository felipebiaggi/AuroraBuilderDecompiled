using System.Collections.Generic;

namespace Builder.Presentation.Models.Collections
{
    public class SavingThrowCollection
    {
        private readonly List<SavingThrowItem> _collection;

        public SavingThrowItem Strength { get; }

        public SavingThrowItem Dexterity { get; }

        public SavingThrowItem Constitution { get; }

        public SavingThrowItem Intelligence { get; }

        public SavingThrowItem Wisdom { get; }

        public SavingThrowItem Charisma { get; }

        public SavingThrowCollection(AbilitiesCollection abilities)
        {
            Strength = new SavingThrowItem(abilities.Strength);
            Dexterity = new SavingThrowItem(abilities.Dexterity);
            Constitution = new SavingThrowItem(abilities.Constitution);
            Intelligence = new SavingThrowItem(abilities.Intelligence);
            Wisdom = new SavingThrowItem(abilities.Wisdom);
            Charisma = new SavingThrowItem(abilities.Charisma);
            _collection = new List<SavingThrowItem> { Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma };
        }

        public IEnumerable<SavingThrowItem> GetCollection()
        {
            return _collection;
        }
    }
}
