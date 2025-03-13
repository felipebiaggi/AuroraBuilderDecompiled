using System.IO;
using Builder.Core.Events;
using Builder.Data.Elements;
using Builder.Presentation;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Models;
using Builder.Presentation.Models.NewFolder1;
using Builder.Presentation.Services.Data;

namespace Builder.Presentation.Models
{
    public class Companion : StatisticsBase, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>
    {
        private string _portrait;

        public CompanionStatistics Statistics { get; }

        public CompanionElement Element { get; private set; }

        public FillableField CompanionName { get; } = new FillableField();

        public FillableField Portrait { get; } = new FillableField();

        public FillableField Initiative { get; } = new FillableField();

        public FillableField ArmorClass { get; } = new FillableField();

        public FillableField Speed { get; } = new FillableField();

        public FillableField MaxHp { get; } = new FillableField();

        public Companion(CompanionElement element = null)
        {
            base.Abilities.DisablePointsCalculation = true;
            Statistics = new CompanionStatistics(this);
            if (element != null)
            {
                SetTemplate(element);
            }
            ApplicationManager.Current.EventAggregator.Subscribe(this);
        }

        public void SetTemplate(CompanionElement element)
        {
            Element = element;
            CompanionName.OriginalContent = element.Name;
            string[] files = Directory.GetFiles(DataManager.Current.UserDocumentsCompanionGalleryDirectory);
            foreach (string text in files)
            {
                if (text.ToLower().Contains(element.Name.ToLower()))
                {
                    Portrait.OriginalContent = text;
                    break;
                }
            }
            base.Abilities.Strength.BaseScore = element.Strength;
            base.Abilities.Dexterity.BaseScore = element.Dexterity;
            base.Abilities.Constitution.BaseScore = element.Constitution;
            base.Abilities.Intelligence.BaseScore = element.Intelligence;
            base.Abilities.Wisdom.BaseScore = element.Wisdom;
            base.Abilities.Charisma.BaseScore = element.Charisma;
            base.DisplayName = element.Name;
            base.DisplayBuild = element.Size + " " + element.CreatureType.ToLower() + ", " + element.Alignment.ToLower();
            Initiative.OriginalContent = base.Abilities.Dexterity.ModifierString;
            ArmorClass.OriginalContent = element.ArmorClass;
            Speed.OriginalContent = element.Speed;
            MaxHp.OriginalContent = element.ElementSetters.GetSetter("hp").Value;
            Statistics.Update(CharacterManager.Current.StatisticsCalculator.StatisticValues);
        }

        public override void Reset()
        {
            base.Reset();
            Element = null;
            CompanionName.Clear();
            Portrait.Clear();
            Initiative.Clear();
            ArmorClass.Clear();
            Speed.Clear();
            MaxHp.Clear();
            Statistics.Reset();
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            if (args.Element is CompanionElement template)
            {
                SetTemplate(template);
                CharacterManager.Current.Status.HasCompanion = Element != null;
            }
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            if (args.Element is CompanionElement)
            {
                Reset();
                CharacterManager.Current.Status.HasCompanion = Element != null;
            }
        }
    }
}
