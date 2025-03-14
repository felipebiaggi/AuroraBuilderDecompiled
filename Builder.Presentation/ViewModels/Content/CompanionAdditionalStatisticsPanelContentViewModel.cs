using Builder.Core.Events;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Extensions;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Builder.Presentation.ViewModels.Content
{
    public sealed class CompanionAdditionalStatisticsPanelContentViewModel : ViewModelBase, ISubscriber<ReprocessCharacterEvent>, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>
    {
        public StatisticsPanelItem Speed { get; } = new StatisticsPanelItem("Speed");

        public StatisticsPanelItem SpeedFly { get; } = new StatisticsPanelItem("Fly Speed");

        public StatisticsPanelItem SpeedClimb { get; } = new StatisticsPanelItem("Climb Speed");

        public StatisticsPanelItem SpeedSwim { get; } = new StatisticsPanelItem("Swim Speed");

        public StatisticsPanelItem SpeedBurrow { get; } = new StatisticsPanelItem("Burrow Speed");

        public StatisticsPanelItem ArmorClass { get; } = new StatisticsPanelItem("AC");

        public StatisticsPanelItem Initiative { get; } = new StatisticsPanelItem("Initiative");

        public StatisticsPanelItem Hp { get; } = new StatisticsPanelItem("HP");

        public StatisticsPanelItem KiPoints { get; } = new StatisticsPanelItem("Ki Points");

        public StatisticsPanelItem SorceryPoints { get; } = new StatisticsPanelItem("Sorcery Points");

        public ObservableCollection<StatisticsPanelItem> AdditionalItems { get; }

        public StatisticsPanelItem SneakAttack { get; } = new StatisticsPanelItem("Sneak Attack");

        public StatisticsPanelItem SuperiorityDice { get; } = new StatisticsPanelItem("Superiority Dice");

        public IEnumerable<StatisticsPanelItem> All
        {
            get
            {
                yield return Speed;
                yield return SpeedFly;
                yield return SpeedClimb;
                yield return SpeedSwim;
                yield return SpeedBurrow;
                yield return ArmorClass;
                yield return Initiative;
                yield return Hp;
                yield return KiPoints;
                yield return SorceryPoints;
                yield return SneakAttack;
                yield return SuperiorityDice;
                foreach (StatisticsPanelItem additionalItem in AdditionalItems)
                {
                    yield return additionalItem;
                }
            }
        }

        public CompanionAdditionalStatisticsPanelContentViewModel()
        {
            AdditionalItems = new ObservableCollection<StatisticsPanelItem>();
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
            Speed.Value = 35;
            StatisticsPanelItem speed = Speed;
            bool exists = (Speed.IsUpdated = true);
            speed.Exists = exists;
            SpeedClimb.Value = 20;
            StatisticsPanelItem speedClimb = SpeedClimb;
            exists = (SpeedClimb.IsUpdated = true);
            speedClimb.Exists = exists;
            ArmorClass.Value = 14;
            StatisticsPanelItem armorClass = ArmorClass;
            exists = (ArmorClass.IsUpdated = true);
            armorClass.Exists = exists;
            Initiative.Value = 3;
            StatisticsPanelItem initiative = Initiative;
            exists = (Initiative.IsUpdated = true);
            initiative.Exists = exists;
            Hp.Value = 123;
            StatisticsPanelItem hp = Hp;
            exists = (Hp.IsUpdated = true);
            hp.Exists = exists;
            Hp.Summery = "All kinds of sources (123)";
            AdditionalItems.Add(Speed);
            AdditionalItems.Add(Hp);
        }

        private void Update()
        {
            StatisticValuesGroupCollection statisticValues = CharacterManager.Current.StatisticsCalculator.StatisticValues;
            if (statisticValues == null)
            {
                return;
            }
            foreach (StatisticsPanelItem item in All)
            {
                item.Exists = false;
                item.IsUpdated = false;
                item.Summery = "n/a";
            }
            AdditionalItems.Clear();
            foreach (StatisticValuesGroup item2 in statisticValues)
            {
                if (item2.GroupName.Equals("companion:speed", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Speed", item2);
                }
                if (item2.GroupName.Equals("companion:speed:fly", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Fly Speed", item2);
                }
                if (item2.GroupName.Equals("companion:speed:climb", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Climb Speed", item2);
                }
                if (item2.GroupName.Equals("companion:speed:swim", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Swim Speed", item2);
                }
                if (item2.GroupName.Equals("companion:speed:burrow", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Burrow Speed", item2);
                }
            }
            foreach (StatisticValuesGroup item3 in statisticValues)
            {
                if (item3.GroupName.Equals("companion:ac", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("AC", item3);
                }
                if (item3.GroupName.Equals("companion:initiative", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Initiative", item3, toValueString: true);
                }
                if (item3.GroupName.Equals("companion:hp:max", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("HP Max", item3);
                }
            }
        }

        private void AddItem(string displayName, StatisticValuesGroup group, bool toValueString = false)
        {
            StatisticsPanelItem statisticsPanelItem = new StatisticsPanelItem(displayName);
            statisticsPanelItem.Update(group);
            statisticsPanelItem.DisplayValue = (toValueString ? statisticsPanelItem.Value.ToValueString() : statisticsPanelItem.Value.ToString());
            AdditionalItems.Add(statisticsPanelItem);
        }

        public void OnHandleEvent(ReprocessCharacterEvent args)
        {
            Update();
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            Update();
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            Update();
        }
    }
}
