using System.Collections.Generic;
using Builder.Presentation.Models.CharacterSheet.Content;

namespace Builder.Presentation.Models.CharacterSheet.Content
{
    public class CharacterSheetSpellcastingPageExportContent
    {
        public class SpellcastingLevelExportContent
        {
            public int Level { get; set; }

            public int AvailableSlots { get; set; }

            public int ExpendedSlots { get; set; }

            public List<SpellExportContent> Spells { get; set; } = new List<SpellExportContent>();

            public SpellExportContent Get(int index)
            {
                if (Spells.Count == 0)
                {
                    return null;
                }
                if (Spells.Count - 1 < index)
                {
                    return null;
                }
                return Spells[index];
            }
        }

        public class SpellExportContent
        {
            public bool IsPrepared { get; set; }

            public bool AlwaysPrepared { get; set; }

            public string Name { get; set; }

            public string Subtitle { get; set; }

            public string Description { get; set; }

            public string CastingTime { get; set; }

            public string Range { get; set; }

            public string Duration { get; set; }

            public string Components { get; set; }

            public string Level { get; set; }

            public string School { get; set; }

            public bool Ritual { get; set; }

            public bool Concentration { get; set; }

            public string GetDisplayName()
            {
                return ToString();
            }

            public override string ToString()
            {
                if (!AlwaysPrepared)
                {
                    return Name;
                }
                return Name + " (Always Prepared)";
            }
        }

        public bool IncludeHeader { get; set; }

        public string SpellcastingClass { get; set; }

        public string SpellcastingArchetype { get; set; }

        public string Ability { get; set; }

        public string AttackBonus { get; set; }

        public string Save { get; set; }

        public string PrepareCount { get; set; }

        public string Notes { get; set; }

        public bool IsMulticlassSpellcaster { get; set; }

        public SpellcastingLevelExportContent Cantrips { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells1 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells2 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells3 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells4 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells5 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells6 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells7 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells8 { get; set; } = new SpellcastingLevelExportContent();

        public SpellcastingLevelExportContent Spells9 { get; set; } = new SpellcastingLevelExportContent();

        public CharacterSheetSpellcastingPageExportContent()
        {
            SpellcastingClass = "";
            SpellcastingArchetype = "";
            Ability = "";
            AttackBonus = "";
            Save = "";
            PrepareCount = "";
            Notes = "";
            Notes = "";
            IncludeHeader = true;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(SpellcastingArchetype) || SpellcastingClass.Equals(SpellcastingArchetype))
            {
                return SpellcastingClass;
            }
            return SpellcastingClass + ", " + SpellcastingArchetype;
        }
    }
}
