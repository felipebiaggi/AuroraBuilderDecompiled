using Builder.Core;
using Builder.Core;
using Builder.Presentation.Models.CharacterSheet.Content;

namespace Builder.Presentation.Models.CharacterSheet.Content
{
    public class SpellcastingSheetContent : ObservableObject
    {
        public string SpellcastingClass { get; set; }

        public string SpellcastingAbility { get; set; }

        public string SpellcastingAttackModifier { get; set; }

        public string SpellcastingSave { get; set; }

        public string SpellcastingPrepareCount { get; set; }

        public string SpellcastingNotes { get; set; }

        public SpellcastingSpellsContent Cantrips { get; } = new SpellcastingSpellsContent(8);

        public SpellcastingSpellsContent Spells1 { get; } = new SpellcastingSpellsContent(12);

        public SpellcastingSpellsContent Spells2 { get; } = new SpellcastingSpellsContent(13);

        public SpellcastingSpellsContent Spells3 { get; } = new SpellcastingSpellsContent(13);

        public SpellcastingSpellsContent Spells4 { get; } = new SpellcastingSpellsContent(13);

        public SpellcastingSpellsContent Spells5 { get; } = new SpellcastingSpellsContent(9);

        public SpellcastingSpellsContent Spells6 { get; } = new SpellcastingSpellsContent(9);

        public SpellcastingSpellsContent Spells7 { get; } = new SpellcastingSpellsContent(9);

        public SpellcastingSpellsContent Spells8 { get; } = new SpellcastingSpellsContent(7);

        public SpellcastingSpellsContent Spells9 { get; } = new SpellcastingSpellsContent(7);
    }
}
