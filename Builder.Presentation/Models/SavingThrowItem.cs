using Builder.Core;
using System.ComponentModel;

namespace Builder.Presentation.Models
{
    public class SavingThrowItem : ObservableObject
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
                OnPropertyChanged("FinalBonus", "FinalBonusModifierString", "IsProficient");
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
                OnPropertyChanged("FinalBonus", "FinalBonusModifierString");
            }
        }

        public int FinalBonus => ProficiencyBonus + KeyAbility.Modifier + MiscBonus;

        public bool IsProficient => _proficiencyBonus > 0;

        public string FinalBonusModifierString => string.Format("{0}{1}", (FinalBonus >= 0) ? "+" : "", FinalBonus);

        public SavingThrowItem(AbilityItem abilityItem)
        {
            Name = abilityItem.Name + " Saving Throw";
            KeyAbility = abilityItem;
            KeyAbility.PropertyChanged += AbilityPropertyChanged;
        }

        private void AbilityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Modifier")
            {
                OnPropertyChanged("FinalBonus");
            }
        }
    }
}
