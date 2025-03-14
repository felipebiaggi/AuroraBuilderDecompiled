using Builder.Core;
using System.ComponentModel;

namespace Builder.Presentation.Models
{
    public class SkillItem : ObservableObject
    {
        private int _proficiencyBonus;

        private int _miscBonus;

        public string Name { get; }

        public AbilityItem KeyAbility { get; }

        public int ProficiencyBonus
        {
            get
            {
                return _proficiencyBonus;
            }
            set
            {
                SetProperty(ref _proficiencyBonus, value, "ProficiencyBonus");
                OnPropertyChanged("FinalBonus", "FinalPassiveBonus", "IsProficient");
            }
        }

        public int MiscBonus
        {
            get
            {
                return _miscBonus;
            }
            set
            {
                SetProperty(ref _miscBonus, value, "MiscBonus");
                OnPropertyChanged("FinalBonus");
                OnPropertyChanged("FinalPassiveBonus");
            }
        }

        public int FinalBonus => ProficiencyBonus + KeyAbility.Modifier + MiscBonus;

        public int FinalPassiveBonus => 10 + FinalBonus;

        public bool IsProficient => _proficiencyBonus > 0;

        public SkillItem(string name, AbilityItem abilityItem)
        {
            Name = name;
            KeyAbility = abilityItem;
            KeyAbility.PropertyChanged += AbilityPropertyChanged;
        }

        public bool IsExpertise(int proficiencyBonus)
        {
            if (IsProficient)
            {
                return ProficiencyBonus >= proficiencyBonus * 2;
            }
            return false;
        }

        private void AbilityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Modifier")
            {
                OnPropertyChanged("FinalBonus");
                OnPropertyChanged("FinalPassiveBonus");
            }
        }
    }
}
