using Builder.Core;
using Builder.Data.Strings;

namespace Builder.Presentation.Models
{
    public class CompanionStatistics : ObservableObject
    {
        private readonly Companion _companion;

        private int _initiative;

        private int _armorClass;

        private int _proficiency;

        private int _maxHp;

        private int _speed;

        private int _speedFly;

        private int _speedClimb;

        private int _speedSwim;

        private int _speedBurrow;

        private int _attackBonus;

        private int _damageBonus;

        public int Initiative
        {
            get
            {
                return _initiative;
            }
            set
            {
                SetProperty(ref _initiative, value, "Initiative");
            }
        }

        public int ArmorClass
        {
            get
            {
                return _armorClass;
            }
            set
            {
                SetProperty(ref _armorClass, value, "ArmorClass");
            }
        }

        public int Proficiency
        {
            get
            {
                return _proficiency;
            }
            set
            {
                SetProperty(ref _proficiency, value, "Proficiency");
            }
        }

        public int MaxHp
        {
            get
            {
                return _maxHp;
            }
            set
            {
                SetProperty(ref _maxHp, value, "MaxHp");
            }
        }

        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                SetProperty(ref _speed, value, "Speed");
            }
        }

        public int SpeedFly
        {
            get
            {
                return _speedFly;
            }
            set
            {
                SetProperty(ref _speedFly, value, "SpeedFly");
            }
        }

        public int SpeedClimb
        {
            get
            {
                return _speedClimb;
            }
            set
            {
                SetProperty(ref _speedClimb, value, "SpeedClimb");
            }
        }

        public int SpeedSwim
        {
            get
            {
                return _speedSwim;
            }
            set
            {
                SetProperty(ref _speedSwim, value, "SpeedSwim");
            }
        }

        public int SpeedBurrow
        {
            get
            {
                return _speedBurrow;
            }
            set
            {
                SetProperty(ref _speedBurrow, value, "SpeedBurrow");
            }
        }

        public int AttackBonus
        {
            get
            {
                return _attackBonus;
            }
            set
            {
                SetProperty(ref _attackBonus, value, "AttackBonus");
            }
        }

        public int DamageBonus
        {
            get
            {
                return _damageBonus;
            }
            set
            {
                SetProperty(ref _damageBonus, value, "DamageBonus");
            }
        }

        public CompanionStatistics(Companion companion)
        {
            _companion = companion;
        }

        public void Reset()
        {
            Initiative = 0;
            ArmorClass = 0;
            Proficiency = 0;
            MaxHp = 0;
            Speed = 0;
            SpeedFly = 0;
            SpeedClimb = 0;
            SpeedSwim = 0;
            SpeedBurrow = 0;
        }

        public void Update(StatisticValuesGroupCollection statistics)
        {
            Initiative = _companion.Abilities.Dexterity.Modifier;
            Proficiency = _companion.Element.Proficiency;
            Initiative += statistics.GetValue("companion:initiative");
            Initiative += statistics.GetValue("companion:initiative:misc");
            ArmorClass = statistics.GetValue("companion:ac");
            MaxHp = statistics.GetValue("companion:hp:max");
            Speed = statistics.GetValue("companion:speed");
            SpeedFly = statistics.GetValue("companion:speed:fly");
            SpeedClimb = statistics.GetValue("companion:speed:climb");
            SpeedSwim = statistics.GetValue("companion:speed:swim");
            SpeedBurrow = statistics.GetValue("companion:speed:burrow");
            AuroraStatisticStrings auroraStatisticStrings = new AuroraStatisticStrings();
            _companion.Skills.Acrobatics.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.AcrobaticsProficiency);
            _companion.Skills.AnimalHandling.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.AnimalHandlingProficiency);
            _companion.Skills.Arcana.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.ArcanaProficiency);
            _companion.Skills.Athletics.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.AthleticsProficiency);
            _companion.Skills.Deception.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.DeceptionProficiency);
            _companion.Skills.History.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.HistoryProficiency);
            _companion.Skills.Insight.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.InsightProficiency);
            _companion.Skills.Intimidation.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.IntimidationProficiency);
            _companion.Skills.Investigation.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.InvestigationProficiency);
            _companion.Skills.Medicine.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.MedicineProficiency);
            _companion.Skills.Nature.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.NatureProficiency);
            _companion.Skills.Perception.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.PerceptionProficiency);
            _companion.Skills.Performance.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.PerformanceProficiency);
            _companion.Skills.Persuasion.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.PersuasionProficiency);
            _companion.Skills.Religion.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.ReligionProficiency);
            _companion.Skills.SleightOfHand.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.SleightOfHandProficiency);
            _companion.Skills.Stealth.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.StealthProficiency);
            _companion.Skills.Survival.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.SurvivalProficiency);
            _companion.Skills.Acrobatics.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.AcrobaticsMisc);
            _companion.Skills.AnimalHandling.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.AnimalHandlingMisc);
            _companion.Skills.Arcana.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.ArcanaMisc);
            _companion.Skills.Athletics.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.AthleticsMisc);
            _companion.Skills.Deception.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.DeceptionMisc);
            _companion.Skills.History.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.HistoryMisc);
            _companion.Skills.Insight.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.InsightMisc);
            _companion.Skills.Intimidation.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.IntimidationMisc);
            _companion.Skills.Investigation.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.InvestigationMisc);
            _companion.Skills.Medicine.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.MedicineMisc);
            _companion.Skills.Nature.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.NatureMisc);
            _companion.Skills.Perception.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.PerceptionMisc);
            _companion.Skills.Performance.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.PerformanceMisc);
            _companion.Skills.Persuasion.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.PersuasionMisc);
            _companion.Skills.Religion.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.ReligionMisc);
            _companion.Skills.SleightOfHand.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.SleightOfHandMisc);
            _companion.Skills.Stealth.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.StealthMisc);
            _companion.Skills.Survival.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.SurvivalMisc);
            _companion.SavingThrows.Strength.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.StrengthSaveProficiency);
            _companion.SavingThrows.Dexterity.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.DexteritySaveProficiency);
            _companion.SavingThrows.Constitution.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.ConstitutionSaveProficiency);
            _companion.SavingThrows.Intelligence.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.IntelligenceSaveProficiency);
            _companion.SavingThrows.Wisdom.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.WisdomSaveProficiency);
            _companion.SavingThrows.Charisma.ProficiencyBonus = statistics.GetValue("companion:" + auroraStatisticStrings.CharismaSaveProficiency);
            _companion.SavingThrows.Strength.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.StrengthSaveMisc);
            _companion.SavingThrows.Dexterity.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.DexteritySaveMisc);
            _companion.SavingThrows.Constitution.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.ConstitutionSaveMisc);
            _companion.SavingThrows.Intelligence.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.IntelligenceSaveMisc);
            _companion.SavingThrows.Wisdom.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.WisdomSaveMisc);
            _companion.SavingThrows.Charisma.MiscBonus = statistics.GetValue("companion:" + auroraStatisticStrings.CharismaSaveMisc);
            AttackBonus = statistics.GetValue("companion:attack");
            DamageBonus = statistics.GetValue("companion:damage");
        }
    }
}
