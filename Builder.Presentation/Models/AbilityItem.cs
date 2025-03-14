using Builder.Core;
using Builder.Presentation.Properties;
using System;

namespace Builder.Presentation.Models
{
    public class AbilityItem : ObservableObject
    {
        private string _name;

        private int _baseScore;

        private int _additionalScore;

        private int _maximumScore;

        private bool _useAbilityScoreMaximum;

        private int _overrideScore;

        private string _additionalSummery = "";

        [Obsolete("needs to be implemented")]
        public bool UseAbilityScoreMaximum
        {
            get
            {
                return _useAbilityScoreMaximum;
            }
            set
            {
                SetProperty(ref _useAbilityScoreMaximum, value, "UseAbilityScoreMaximum");
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
                OnPropertyChanged("Abbreviation");
            }
        }

        public string Abbreviation
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    return Name.Substring(0, (Name.Length >= 3) ? 3 : Name.Length);
                }
                return string.Empty;
            }
        }

        public int BaseScore
        {
            get
            {
                return _baseScore;
            }
            set
            {
                SetProperty(ref _baseScore, value, "BaseScore");
                OnPropertyChanged("FinalScore", "Modifier", "ModifierString", "AbilityAndModifierString", "ExceedsMaximumScore");
            }
        }

        public int AdditionalScore
        {
            get
            {
                return _additionalScore;
            }
            set
            {
                SetProperty(ref _additionalScore, value, "AdditionalScore");
                OnPropertyChanged("FinalScore", "Modifier", "ModifierString", "AbilityAndModifierString", "ExceedsMaximumScore");
            }
        }

        public int FinalScore
        {
            get
            {
                int num = BaseScore + AdditionalScore;
                if (OverrideScore > num)
                {
                    return OverrideScore;
                }
                if (ExceedsMaximumScore && Settings.Default.UseDefaultAbilityScoreMaximum)
                {
                    return MaximumScore;
                }
                return num;
            }
        }

        public int Modifier
        {
            get
            {
                int finalScore = FinalScore;
                if (finalScore < 10)
                {
                    return (finalScore - 11) / 2;
                }
                return (finalScore - 10) / 2;
            }
        }

        public string ModifierString => string.Format("{0}{1}", (Modifier >= 0) ? "+" : "", Modifier);

        public string AbilityAndModifierString => string.Format("{0} ({1}{2})", FinalScore, (Modifier >= 0) ? "+" : "", Modifier);

        public int OverrideScore
        {
            get
            {
                return _overrideScore;
            }
            set
            {
                SetProperty(ref _overrideScore, value, "OverrideScore");
                OnPropertyChanged("FinalScore", "Modifier", "ModifierString", "AbilityAndModifierString", "ExceedsMaximumScore");
            }
        }

        public string AdditionalSummery
        {
            get
            {
                return _additionalSummery;
            }
            set
            {
                SetProperty(ref _additionalSummery, value, "AdditionalSummery");
            }
        }

        public bool ExceedsMaximumScore
        {
            get
            {
                int num = BaseScore + AdditionalScore;
                if (OverrideScore > num)
                {
                    num = OverrideScore;
                }
                return num > MaximumScore;
            }
        }

        public int MaximumScore
        {
            get
            {
                return _maximumScore;
            }
            set
            {
                SetProperty(ref _maximumScore, value, "MaximumScore");
                OnPropertyChanged("FinalScore", "Modifier", "ModifierString", "AbilityAndModifierString", "ExceedsMaximumScore");
            }
        }

        public AbilityItem(string name, int baseScore = 8)
        {
            _name = name;
            _baseScore = baseScore;
            _useAbilityScoreMaximum = true;
            _maximumScore = 20;
        }

        public bool UseOverrideScore()
        {
            return OverrideScore > BaseScore + AdditionalScore;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
