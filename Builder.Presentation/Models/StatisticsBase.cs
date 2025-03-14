using Builder.Core;
using Builder.Presentation.Models.Collections;

namespace Builder.Presentation.Models
{
    public class StatisticsBase : ObservableObject
    {
        private string _name;

        private string _displayName;

        private string _displayBuild;

        public AbilitiesCollection Abilities { get; }

        public SkillsCollection Skills { get; }

        public SavingThrowCollection SavingThrows { get; }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                SetProperty(ref _displayName, value, "DisplayName");
            }
        }

        public string DisplayBuild
        {
            get
            {
                return _displayBuild;
            }
            set
            {
                SetProperty(ref _displayBuild, value, "DisplayBuild");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value, "Name");
            }
        }

        public StatisticsBase()
        {
            Abilities = new AbilitiesCollection();
            Skills = new SkillsCollection(Abilities);
            SavingThrows = new SavingThrowCollection(Abilities);
        }

        public virtual void Reset()
        {
            Name = "";
            DisplayName = "";
            DisplayBuild = "";
            Abilities.Reset();
        }
    }
}
