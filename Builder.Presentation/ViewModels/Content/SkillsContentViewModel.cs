using Builder.Core.Events;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.ViewModels.Content
{
    public class SkillsContentViewModel : ViewModelBase, ISubscriber<ReprocessCharacterEvent>, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>
    {
        private int _passivePerception;

        private int _passiveInsight;

        public Character Character => CharacterManager.Current.Character;

        public SkillsCollection Skills => CharacterManager.Current.Character.Skills;

        public int PassivePerception
        {
            get
            {
                return _passivePerception;
            }
            set
            {
                SetProperty(ref _passivePerception, value, "PassivePerception");
            }
        }

        public int PassiveInsight
        {
            get
            {
                return _passiveInsight;
            }
            set
            {
                SetProperty(ref _passiveInsight, value, "PassiveInsight");
            }
        }

        public SkillsContentViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
            }
            else
            {
                base.EventAggregator.Subscribe(this);
            }
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            Skills.Acrobatics.ProficiencyBonus = 2;
            Skills.History.ProficiencyBonus = 2;
            Skills.Arcana.ProficiencyBonus = 2;
            Skills.Perception.ProficiencyBonus = 2;
            Skills.Perception.MiscBonus = 2;
            PassivePerception = Skills.Perception.FinalBonus + 10;
            PassiveInsight = 10;
        }

        public virtual void OnHandleEvent(ReprocessCharacterEvent args)
        {
            StatisticValuesGroupCollection statisticValues = CharacterManager.Current.StatisticsCalculator.StatisticValues;
            if (statisticValues != null)
            {
                StatisticValuesGroup group = statisticValues.GetGroup("perception:passive", createNonExisting: false);
                StatisticValuesGroup group2 = statisticValues.GetGroup("insight:passive", createNonExisting: false);
                PassivePerception = group.Sum() + Skills.Perception.FinalBonus;
                PassiveInsight = group2.Sum() + Skills.Insight.FinalBonus;
            }
        }

        public virtual void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            StatisticValuesGroupCollection statisticValues = CharacterManager.Current.StatisticsCalculator.StatisticValues;
            if (statisticValues != null)
            {
                StatisticValuesGroup group = statisticValues.GetGroup("perception:passive", createNonExisting: false);
                StatisticValuesGroup group2 = statisticValues.GetGroup("insight:passive", createNonExisting: false);
                PassivePerception = group.Sum() + Skills.Perception.FinalBonus;
                PassiveInsight = group2.Sum() + Skills.Insight.FinalBonus;
            }
        }

        public virtual void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            StatisticValuesGroupCollection statisticValues = CharacterManager.Current.StatisticsCalculator.StatisticValues;
            if (statisticValues != null)
            {
                StatisticValuesGroup group = statisticValues.GetGroup("perception:passive", createNonExisting: false);
                StatisticValuesGroup group2 = statisticValues.GetGroup("insight:passive", createNonExisting: false);
                PassivePerception = group.Sum() + Skills.Perception.FinalBonus;
                PassiveInsight = group2.Sum() + Skills.Insight.FinalBonus;
            }
        }
    }
}
