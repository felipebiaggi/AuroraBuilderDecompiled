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
    public sealed class AdditionalStatisticsPanelContentViewModel : ViewModelBase, ISubscriber<ReprocessCharacterEvent>, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>
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

        public AdditionalStatisticsPanelContentViewModel()
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
                if (item2.GroupName.Equals("speed", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Speed", item2);
                }
                if (item2.Sum() != 0)
                {
                    if (item2.GroupName.Equals("speed:fly", StringComparison.OrdinalIgnoreCase))
                    {
                        AddItem("Fly", item2);
                    }
                    if (item2.GroupName.Equals("speed:climb", StringComparison.OrdinalIgnoreCase))
                    {
                        AddItem("Climb", item2);
                    }
                    if (item2.GroupName.Equals("speed:swim", StringComparison.OrdinalIgnoreCase))
                    {
                        AddItem("Swim", item2);
                    }
                    if (item2.GroupName.Equals("speed:burrow", StringComparison.OrdinalIgnoreCase))
                    {
                        AddItem("Burrow", item2);
                    }
                }
            }
            foreach (StatisticValuesGroup item3 in statisticValues)
            {
                if (item3.GroupName.Equals("ac", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("AC", item3);
                }
                if (item3.GroupName.Equals("initiative", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Initiative", item3, toValueString: true);
                }
                if (item3.GroupName.Equals("hp", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("HP", item3);
                }
            }
            foreach (StatisticValuesGroup item4 in statisticValues)
            {
                if (item4.GroupName.Equals("ki:points", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Ki Points", item4);
                }
                if (item4.GroupName.Equals("sorcery-points", StringComparison.OrdinalIgnoreCase))
                {
                    AddItem("Sorcery Points", item4);
                }
            }
            if (statisticValues.ContainsGroup("sneak-attack:count") && statisticValues.ContainsGroup("sneak-attack:die"))
            {
                int value = statisticValues.GetValue("sneak-attack:count");
                int value2 = statisticValues.GetValue("sneak-attack:die");
                AdditionalItems.Add(new StatisticsPanelItem("Sneak Attack", $"{value}d{value2}")
                {
                    Exists = true,
                    Summery = "Sneak Attack"
                });
            }
            if (statisticValues.ContainsGroup("superiority dice:amount") && statisticValues.ContainsGroup("superiority dice:size"))
            {
                int value3 = statisticValues.GetValue("superiority dice:amount");
                int value4 = statisticValues.GetValue("superiority dice:size");
                AdditionalItems.Add(new StatisticsPanelItem("Superiority Dice", $"{value3}d{value4}")
                {
                    Exists = true,
                    Summery = "Combat Superiority"
                });
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
