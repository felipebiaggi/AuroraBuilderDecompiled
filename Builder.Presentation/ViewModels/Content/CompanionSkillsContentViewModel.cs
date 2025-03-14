using Builder.Presentation.Events.Character;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Services.Calculator;
using System.ComponentModel;

namespace Builder.Presentation.ViewModels.Content
{
    public class CompanionSkillsContentViewModel : SkillsContentViewModel
    {
        public new SkillsCollection Skills => CharacterManager.Current.Character.Companion.Skills;

        public bool IsAcrobaticsExpert => Skills.Acrobatics.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsAnimalHandlingExpert => Skills.AnimalHandling.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsArcanaExpert => Skills.Arcana.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsAthleticsExpert => Skills.Athletics.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsDeceptionExpert => Skills.Deception.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsHistoryExpert => Skills.History.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsInsightExpert => Skills.Insight.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsIntimidationExpert => Skills.Intimidation.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsInvestigationExpert => Skills.Investigation.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsMedicineExpert => Skills.Medicine.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsNatureExpert => Skills.Nature.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsPerceptionExpert => Skills.Perception.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsPerformanceExpert => Skills.Performance.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsPersuasionExpert => Skills.Persuasion.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsReligionExpert => Skills.Religion.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsSleightOfHandExpert => Skills.SleightOfHand.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsStealthExpert => Skills.Stealth.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public bool IsSurvivalExpert => Skills.Survival.IsExpertise(base.Character.Companion.Statistics.Proficiency);

        public CompanionSkillsContentViewModel()
        {
            base.Character.Companion.Statistics.PropertyChanged += Statistics_PropertyChanged;
        }

        private void Statistics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("IsAcrobaticsExpert");
            OnPropertyChanged("IsAnimalHandlingExpert");
            OnPropertyChanged("IsArcanaExpert");
            OnPropertyChanged("IsAthleticsExpert");
            OnPropertyChanged("IsDeceptionExpert");
            OnPropertyChanged("IsHistoryExpert");
            OnPropertyChanged("IsInsightExpert");
            OnPropertyChanged("IsIntimidationExpert");
            OnPropertyChanged("IsInvestigationExpert");
            OnPropertyChanged("IsMedicineExpert");
            OnPropertyChanged("IsNatureExpert");
            OnPropertyChanged("IsPerceptionExpert");
            OnPropertyChanged("IsPerformanceExpert");
            OnPropertyChanged("IsPersuasionExpert");
            OnPropertyChanged("IsReligionExpert");
            OnPropertyChanged("IsSleightOfHandExpert");
            OnPropertyChanged("IsStealthExpert");
            OnPropertyChanged("IsSurvivalExpert");
        }

        public override void OnHandleEvent(ReprocessCharacterEvent args)
        {
            StatisticValuesGroupCollection statisticValues = CharacterManager.Current.StatisticsCalculator.StatisticValues;
            if (statisticValues != null)
            {
                StatisticValuesGroup group = statisticValues.GetGroup("companion:perception:passive", createNonExisting: false);
                if (group != null)
                {
                    base.PassivePerception = group.Sum() + Skills.Perception.FinalBonus;
                }
                else
                {
                    base.PassivePerception = 10 + Skills.Perception.FinalBonus;
                }
                OnPropertyChanged("IsAcrobaticsExpert");
                OnPropertyChanged("IsAnimalHandlingExpert");
                OnPropertyChanged("IsArcanaExpert");
                OnPropertyChanged("IsAthleticsExpert");
                OnPropertyChanged("IsDeceptionExpert");
                OnPropertyChanged("IsHistoryExpert");
                OnPropertyChanged("IsInsightExpert");
                OnPropertyChanged("IsIntimidationExpert");
                OnPropertyChanged("IsInvestigationExpert");
                OnPropertyChanged("IsMedicineExpert");
                OnPropertyChanged("IsNatureExpert");
                OnPropertyChanged("IsPerceptionExpert");
                OnPropertyChanged("IsPerformanceExpert");
                OnPropertyChanged("IsPersuasionExpert");
                OnPropertyChanged("IsReligionExpert");
                OnPropertyChanged("IsSleightOfHandExpert");
                OnPropertyChanged("IsStealthExpert");
                OnPropertyChanged("IsSurvivalExpert");
            }
        }

        public override void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            StatisticValuesGroup group = CharacterManager.Current.StatisticsCalculator.StatisticValues.GetGroup("companion:perception:passive", createNonExisting: false);
            if (group != null)
            {
                base.PassivePerception = group.Sum() + Skills.Perception.FinalBonus;
            }
            else
            {
                base.PassivePerception = 10 + Skills.Perception.FinalBonus;
            }
            OnPropertyChanged("IsAcrobaticsExpert");
            OnPropertyChanged("IsAnimalHandlingExpert");
            OnPropertyChanged("IsArcanaExpert");
            OnPropertyChanged("IsAthleticsExpert");
            OnPropertyChanged("IsDeceptionExpert");
            OnPropertyChanged("IsHistoryExpert");
            OnPropertyChanged("IsInsightExpert");
            OnPropertyChanged("IsIntimidationExpert");
            OnPropertyChanged("IsInvestigationExpert");
            OnPropertyChanged("IsMedicineExpert");
            OnPropertyChanged("IsNatureExpert");
            OnPropertyChanged("IsPerceptionExpert");
            OnPropertyChanged("IsPerformanceExpert");
            OnPropertyChanged("IsPersuasionExpert");
            OnPropertyChanged("IsReligionExpert");
            OnPropertyChanged("IsSleightOfHandExpert");
            OnPropertyChanged("IsStealthExpert");
            OnPropertyChanged("IsSurvivalExpert");
        }

        public override void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            StatisticValuesGroup group = CharacterManager.Current.StatisticsCalculator.StatisticValues.GetGroup("companion:perception:passive", createNonExisting: false);
            if (group != null)
            {
                base.PassivePerception = group.Sum() + Skills.Perception.FinalBonus;
            }
            else
            {
                base.PassivePerception = 10 + Skills.Perception.FinalBonus;
            }
            OnPropertyChanged("IsAcrobaticsExpert");
            OnPropertyChanged("IsAnimalHandlingExpert");
            OnPropertyChanged("IsArcanaExpert");
            OnPropertyChanged("IsAthleticsExpert");
            OnPropertyChanged("IsDeceptionExpert");
            OnPropertyChanged("IsHistoryExpert");
            OnPropertyChanged("IsInsightExpert");
            OnPropertyChanged("IsIntimidationExpert");
            OnPropertyChanged("IsInvestigationExpert");
            OnPropertyChanged("IsMedicineExpert");
            OnPropertyChanged("IsNatureExpert");
            OnPropertyChanged("IsPerceptionExpert");
            OnPropertyChanged("IsPerformanceExpert");
            OnPropertyChanged("IsPersuasionExpert");
            OnPropertyChanged("IsReligionExpert");
            OnPropertyChanged("IsSleightOfHandExpert");
            OnPropertyChanged("IsStealthExpert");
            OnPropertyChanged("IsSurvivalExpert");
        }
    }
}
