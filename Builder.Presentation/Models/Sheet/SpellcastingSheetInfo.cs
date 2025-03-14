using System;
using System.Collections.Generic;

namespace Builder.Presentation.Models.Sheet
{
    public class SpellcastingSheetInfo
    {
        public class SpellsContainer : List<SpellsContainer.SpellPlaceholder>
        {
            public class SpellPlaceholder
            {
                public string Name { get; set; }

                public bool IsPrepared { get; set; }

                public SpellPlaceholder(string name, bool isPrepared = false)
                {
                    Name = name;
                    IsPrepared = isPrepared;
                }
            }

            public int TotalSlots { get; set; }

            public int ExpendedSlots { get; set; }

            public int MaximumDisplayable { get; set; }

            public SpellsContainer(int totalSlots, int expendedSlots, int maximumDisplayable)
            {
                TotalSlots = totalSlots;
                ExpendedSlots = expendedSlots;
                MaximumDisplayable = maximumDisplayable;
            }

            public SpellPlaceholder GetSpell(int lineNumber)
            {
                try
                {
                    return base[lineNumber - 1];
                }
                catch (ArgumentOutOfRangeException)
                {
                    return new SpellPlaceholder(string.Empty);
                }
            }
        }

        public string SpellcastingClass { get; set; }

        public string SpellcastingAbility { get; set; }

        public string SpellSaveDifficultyClass { get; set; }

        public string SpellAttackBonus { get; set; }

        public SpellsContainer Cantrips { get; } = new SpellsContainer(0, 0, 8);

        public SpellsContainer Spells1st { get; } = new SpellsContainer(0, 0, 12);

        public SpellsContainer Spells2nd { get; } = new SpellsContainer(0, 0, 13);

        public SpellsContainer Spells3rd { get; } = new SpellsContainer(0, 0, 13);

        public SpellsContainer Spells4th { get; } = new SpellsContainer(0, 0, 13);

        public SpellsContainer Spells5th { get; } = new SpellsContainer(0, 0, 9);

        public SpellsContainer Spells6th { get; } = new SpellsContainer(0, 0, 9);

        public SpellsContainer Spells7th { get; } = new SpellsContainer(0, 0, 9);

        public SpellsContainer Spells8th { get; } = new SpellsContainer(0, 0, 7);

        public SpellsContainer Spells9th { get; } = new SpellsContainer(0, 0, 7);
    }
}
