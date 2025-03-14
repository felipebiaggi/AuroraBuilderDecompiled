using Builder.Core;
using Builder.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.Models.Collections
{
    public class AbilitiesCollection : ObservableObject
    {
        private const int StartingPoints = 27;

        private int _minimumAbilityBaseScore;

        private int _maximumAbilityBaseScore;

        private readonly List<AbilityItem> _collection;

        private readonly Dictionary<int, int> _pointCost;

        private int _availablePoints;

        public int MinimumAbilityBaseScore
        {
            get
            {
                return _minimumAbilityBaseScore;
            }
            set
            {
                SetProperty(ref _minimumAbilityBaseScore, value, "MinimumAbilityBaseScore");
            }
        }

        public int MaximumAbilityBaseScore
        {
            get
            {
                return _maximumAbilityBaseScore;
            }
            set
            {
                SetProperty(ref _maximumAbilityBaseScore, value, "MaximumAbilityBaseScore");
            }
        }

        public int AvailablePoints
        {
            get
            {
                return _availablePoints;
            }
            set
            {
                SetProperty(ref _availablePoints, value, "AvailablePoints");
            }
        }

        public AbilityItem Strength { get; }

        public AbilityItem Dexterity { get; }

        public AbilityItem Constitution { get; }

        public AbilityItem Intelligence { get; }

        public AbilityItem Wisdom { get; }

        public AbilityItem Charisma { get; }

        public FirstFloor.ModernUI.Presentation.RelayCommand IncreaseAbilityCommand => new FirstFloor.ModernUI.Presentation.RelayCommand(IncreaseAbility, CanIncreaseAbility);

        public FirstFloor.ModernUI.Presentation.RelayCommand DecreaseAbilityCommand => new FirstFloor.ModernUI.Presentation.RelayCommand(DecreaseAbility, CanDecreaseAbility);

        public FirstFloor.ModernUI.Presentation.RelayCommand ResetScoresCommand => new FirstFloor.ModernUI.Presentation.RelayCommand(delegate
        {
            ResetScores();
        });

        public bool DisablePointsCalculation { get; set; }

        public AbilitiesCollection()
        {
            _minimumAbilityBaseScore = 3;
            _maximumAbilityBaseScore = 30;
            _pointCost = new Dictionary<int, int>
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
            { 6, 0 },
            { 7, 0 },
            { 8, 0 },
            { 9, 1 },
            { 10, 2 },
            { 11, 3 },
            { 12, 4 },
            { 13, 5 },
            { 14, 7 },
            { 15, 9 },
            { 16, 11 },
            { 17, 13 },
            { 18, 15 },
            { 19, 17 },
            { 20, 19 },
            { 21, 21 },
            { 22, 23 },
            { 23, 25 },
            { 24, 27 },
            { 25, 29 },
            { 26, 31 },
            { 27, 33 },
            { 28, 35 },
            { 29, 37 },
            { 30, 39 },
            { 31, 41 },
            { 32, 43 },
            { 33, 45 },
            { 34, 47 },
            { 35, 49 },
            { 36, 51 },
            { 37, 53 },
            { 38, 55 },
            { 39, 57 },
            { 40, 59 },
            { 41, 61 }
        };
            while (!_pointCost.ContainsKey(20))
            {
                _pointCost.Add(_pointCost.Last().Key + 1, _pointCost.Last().Value + 2);
            }
            _availablePoints = 27;
            Strength = new AbilityItem("Strength", 10);
            Dexterity = new AbilityItem("Dexterity", 10);
            Constitution = new AbilityItem("Constitution", 10);
            Intelligence = new AbilityItem("Intelligence", 10);
            Wisdom = new AbilityItem("Wisdom", 10);
            Charisma = new AbilityItem("Charisma", 10);
            _collection = new List<AbilityItem> { Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma };
            CalculateAvailablePoints();
        }

        private bool CanIncreaseAbility(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }
            return ((AbilityItem)parameter).BaseScore < MaximumAbilityBaseScore;
        }

        private bool CanDecreaseAbility(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }
            return ((AbilityItem)parameter).BaseScore > MinimumAbilityBaseScore;
        }

        private void IncreaseAbility(object parameter)
        {
            ((AbilityItem)parameter).BaseScore++;
            CalculateAvailablePoints();
            IncreaseAbilityCommand.OnCanExecuteChanged();
        }

        private void DecreaseAbility(object parameter)
        {
            ((AbilityItem)parameter).BaseScore--;
            CalculateAvailablePoints();
            DecreaseAbilityCommand.OnCanExecuteChanged();
        }

        private void ResetScores()
        {
            foreach (AbilityItem item in _collection)
            {
                item.BaseScore = 10;
            }
            CalculateAvailablePoints();
            IncreaseAbilityCommand.OnCanExecuteChanged();
            DecreaseAbilityCommand.OnCanExecuteChanged();
        }

        public int CalculateAvailablePoints()
        {
            if (DisablePointsCalculation)
            {
                if (CharacterManager.Current != null && CharacterManager.Current.Status.IsLoaded)
                {
                    CharacterManager.Current.ReprocessCharacter();
                }
                return 0;
            }
            try
            {
                int num = _pointCost[Strength.BaseScore];
                int num2 = _pointCost[Dexterity.BaseScore];
                int num3 = _pointCost[Constitution.BaseScore];
                int num4 = _pointCost[Intelligence.BaseScore];
                int num5 = _pointCost[Wisdom.BaseScore];
                int num6 = _pointCost[Charisma.BaseScore];
                int num7 = num + num2 + num3 + num4 + num5 + num6;
                AvailablePoints = 27 - num7;
                IncreaseAbilityCommand.OnCanExecuteChanged();
                DecreaseAbilityCommand.OnCanExecuteChanged();
                if (CharacterManager.Current != null && CharacterManager.Current.Status.IsLoaded)
                {
                    CharacterManager.Current.ReprocessCharacter();
                }
                return AvailablePoints;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "CalculateAvailablePoints");
            }
            finally
            {
                IncreaseAbilityCommand.OnCanExecuteChanged();
                DecreaseAbilityCommand.OnCanExecuteChanged();
            }
            return AvailablePoints;
        }

        public void Reset()
        {
            ResetScores();
        }

        public IEnumerable<AbilityItem> GetCollection()
        {
            return _collection;
        }
    }
}
