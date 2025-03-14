namespace Builder.Presentation.Models.CharacterSheet
{
    public static class CharacterSheetResources
    {
        private const string SheetsDirectory = "Builder.Presentation.Resources.Sheets.";

        private const string PageDirectory = "Builder.Presentation.Resources.Sheets.Page.";

        private const string PartialsDirectory = "Builder.Presentation.Resources.Sheets.Partial.";

        private const string SpellcastingPartial = "Builder.Presentation.Resources.Sheets.Partial.Spellcasting.";

        public static CharacterSheetResourcePage GetDetailsPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Page.details_2.pdf");
        }

        public static CharacterSheetResourcePage GetBackgroundPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Page.background.pdf");
        }

        public static CharacterSheetResourcePage GetCompanionPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.aurorapagecompanion.pdf");
        }

        public static CharacterSheetResourcePage GetEquipmentPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Page.equipmentpage.pdf");
        }

        public static CharacterSheetResourcePage GetSpellcastingPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.aurora_spellcasting_flat.pdf");
        }

        public static CharacterSheetResourcePage GetGenericCardPartial()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.card.pdf");
        }

        public static CharacterSheetResourcePage GetAttackCardPartial()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.attackcard.pdf");
        }

        public static CharacterSheetResourcePage GetSpellCardPartial()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.spellcard.pdf");
        }

        public static CharacterSheetResourcePage GetSpellcastingHeaderPartial()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.Spellcasting.header.pdf");
        }

        public static CharacterSheetResourcePage GetSpellcastingSectionTopPartial()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.partial_spellcasting_section_top.pdf");
        }

        public static CharacterSheetResourcePage GetSpellcastingSectionCenterPartial()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.partial_spellcasting_section_center.pdf");
        }

        public static CharacterSheetResourcePage GetSpellcastingSectionBottomPartial()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.partial_spellcasting_section_bottom.pdf");
        }

        public static CharacterSheetResourcePage PartialSpellcastingHeader(string color = "red")
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.Spellcasting." + color + "_spells_header.pdf");
        }

        public static CharacterSheetResourcePage PartialSpellcastingTop(int level = 0, string color = "red")
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.Spellcasting." + $"{color}_spells_top{level}.pdf");
        }

        public static CharacterSheetResourcePage PartialSpellcastingMiddle(string color = "red")
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.Spellcasting." + color + "_spells_middle.pdf");
        }

        public static CharacterSheetResourcePage PartialSpellcastingBottom(string color = "red")
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.Spellcasting." + color + "_spells_bottom.pdf");
        }

        public static CharacterSheetResourcePage PartialColumnHeader()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.Spellcasting.column_header.pdf");
        }

        public static CharacterSheetResourcePage PartialColumnEntry()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.Spellcasting.column_entry.pdf");
        }

        public static CharacterSheetResourcePage GetLegacyDetailsPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.sheet_character.pdf");
        }

        public static CharacterSheetResourcePage GetLegacyBackgroundPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.sheet_details.pdf");
        }

        public static CharacterSheetResourcePage GetLegacySpellcastingPage()
        {
            return new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.sheet_spellcasting.pdf");
        }
    }
}
