﻿namespace Builder.Presentation.Models.Sheet
{
    public class CharacterSheetInfo
    {
        public string CharacterName { get; set; }

        public string PlayerName { get; set; }

        public string Race { get; set; }

        public string Class { get; set; }

        public string Level { get; set; }

        public string Alignment { get; set; }

        public string Experience { get; set; }

        public string Strength { get; set; }

        public string Dexterity { get; set; }

        public string Constitution { get; set; }

        public string Intelligence { get; set; }

        public string Wisdom { get; set; }

        public string Charisma { get; set; }

        public string StrengthModifier { get; set; }

        public string DexterityModifier { get; set; }

        public string ConstitutionModifier { get; set; }

        public string IntelligenceModifier { get; set; }

        public string WisdomModifier { get; set; }

        public string CharismaModifier { get; set; }

        public string StrengthSavingThrow { get; set; }

        public string DexteritySavingThrow { get; set; }

        public string ConstitutionSavingThrow { get; set; }

        public string IntelligenceSavingThrow { get; set; }

        public string WisdomSavingThrow { get; set; }

        public string CharismaSavingThrow { get; set; }

        public bool StrengthSavingThrowProficient { get; set; }

        public bool DexteritySavingThrowProficient { get; set; }

        public bool ConstitutionSavingThrowProficient { get; set; }

        public bool IntelligenceSavingThrowProficient { get; set; }

        public bool WisdomSavingThrowProficient { get; set; }

        public bool CharismaSavingThrowProficient { get; set; }

        public string Acrobatics { get; set; }

        public string AnimalHandling { get; set; }

        public string Athletics { get; set; }

        public string Deception { get; set; }

        public string History { get; set; }

        public string Insight { get; set; }

        public string Intimidation { get; set; }

        public string Investigation { get; set; }

        public string Arcana { get; set; }

        public string Perception { get; set; }

        public string Nature { get; set; }

        public string Performance { get; set; }

        public string Medicine { get; set; }

        public string Religion { get; set; }

        public string Stealth { get; set; }

        public string Persuasion { get; set; }

        public string SleightOfHand { get; set; }

        public string Survival { get; set; }

        public bool AcrobaticsProficient { get; set; }

        public bool AnimalHandlingProficient { get; set; }

        public bool AthleticsProficient { get; set; }

        public bool DeceptionProficient { get; set; }

        public bool HistoryProficient { get; set; }

        public bool InsightProficient { get; set; }

        public bool IntimidationProficient { get; set; }

        public bool InvestigationProficient { get; set; }

        public bool ArcanaProficient { get; set; }

        public bool PerceptionProficient { get; set; }

        public bool NatureProficient { get; set; }

        public bool PerformanceProficient { get; set; }

        public bool MedicineProficient { get; set; }

        public bool ReligionProficient { get; set; }

        public bool StealthProficient { get; set; }

        public bool PersuasionProficient { get; set; }

        public bool SleightOfHandProficient { get; set; }

        public bool SurvivalProficient { get; set; }

        public string PassiveWisdomPerception { get; set; }

        public string ProficienciesAndLanguages { get; set; }

        public bool Inspiration { get; set; }

        public string ProficiencyBonus { get; set; }

        public string ArmorClass { get; set; }

        public string Initiative { get; set; }

        public string Speed { get; set; }

        public string MaximumHitPoints { get; set; }

        public string CurrentHitPoints { get; set; }

        public string TemporaryHitPoints { get; set; }

        public string HitDice { get; set; }

        public string TotalHitDice { get; set; }

        public bool DeathSavingThrowSuccess1 { get; set; }

        public bool DeathSavingThrowSuccess2 { get; set; }

        public bool DeathSavingThrowSuccess3 { get; set; }

        public bool DeathSavingThrowFailure1 { get; set; }

        public bool DeathSavingThrowFailure2 { get; set; }

        public bool DeathSavingThrowFailure3 { get; set; }

        public string Background { get; set; }

        public string PersonalityTraits { get; set; }

        public string Ideals { get; set; }

        public string Bonds { get; set; }

        public string Flaws { get; set; }

        public string Name1 { get; set; }

        public string AttackBonus1 { get; set; }

        public string DamageType1 { get; set; }

        public string Name2 { get; set; }

        public string AttackBonus2 { get; set; }

        public string DamageType2 { get; set; }

        public string Name3 { get; set; }

        public string AttackBonus3 { get; set; }

        public string DamageType3 { get; set; }

        public string AttackAndSpellcastingField { get; set; }

        public string Copper { get; set; }

        public string Silver { get; set; }

        public string Electrum { get; set; }

        public string Gold { get; set; }

        public string Platinum { get; set; }

        public string Equipment { get; set; }

        public string FeaturesAndTraitsField { get; set; }
        public ContentField ProficienciesAndLanguagesFieldContent { get; set; } = new ContentField();

        public ContentField FeaturesFieldContent { get; set; } = new ContentField();
    }
}
