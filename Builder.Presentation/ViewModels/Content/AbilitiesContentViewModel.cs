using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Services;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Builder.Presentation.ViewModels.Content
{
    public class AbilitiesContentViewModel : ViewModelBase, ISubscriber<SettingsChangedEvent>
    {
        private readonly DiceService _dice;

        private AbilitiesGenerationOption _option;

        private bool _isRandomizeGeneration;

        private bool _isArrayGeneration;

        private bool _isPointsGeneration;

        private string _generationDisplayName;

        public virtual AbilitiesCollection Abilities => CharacterManager.Current.Character.Abilities;

        public bool IsRandomizeGeneration
        {
            get
            {
                return _isRandomizeGeneration;
            }
            set
            {
                SetProperty(ref _isRandomizeGeneration, value, "IsRandomizeGeneration");
            }
        }

        public bool IsArrayGeneration
        {
            get
            {
                return _isArrayGeneration;
            }
            set
            {
                SetProperty(ref _isArrayGeneration, value, "IsArrayGeneration");
            }
        }

        public bool IsPointsGeneration
        {
            get
            {
                return _isPointsGeneration;
            }
            set
            {
                SetProperty(ref _isPointsGeneration, value, "IsPointsGeneration");
            }
        }

        public string GenerationDisplayName
        {
            get
            {
                return _generationDisplayName;
            }
            set
            {
                SetProperty(ref _generationDisplayName, value, "GenerationDisplayName");
            }
        }

        public ICommand GenerateRandomAbilityScoreCommand => new RelayCommand<AbilityItem>(GenerateRandomAbilityScore);

        public ICommand InitializeStandardArrayCommand => new RelayCommand(InitializeStandardArray);

        public ICommand RandomizeCommand => new RelayCommand(RandomizeAbilitiesStatistic);

        public ICommand RandomizeSingleCommand => new RelayCommand<AbilityItem>(RandomizeSingle);

        public AbilitiesContentViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            _dice = new DiceService();
            SetGenerationOption();
            base.EventAggregator.Subscribe(this);
        }

        private void InitializeStandardArray()
        {
            Abilities.Strength.BaseScore = 15;
            Abilities.Strength.BaseScore = 14;
            Abilities.Strength.BaseScore = 13;
            Abilities.Strength.BaseScore = 12;
            Abilities.Strength.BaseScore = 10;
            Abilities.Strength.BaseScore = 8;
        }

        private async void GenerateRandomAbilityScore(AbilityItem parameter)
        {
            _ = 4;
            try
            {
                switch (_option)
                {
                    case AbilitiesGenerationOption.Roll3D6:
                        parameter.BaseScore = await _dice.D6(3);
                        break;
                    case AbilitiesGenerationOption.Roll4D6DiscardLowest:
                        {
                            List<int> list = new List<int>(4);
                            List<int> list2 = list;
                            list2.Add(await _dice.D6());
                            List<int> list3 = list;
                            list3.Add(await _dice.D6());
                            List<int> list4 = list;
                            list4.Add(await _dice.D6());
                            List<int> list5 = list;
                            list5.Add(await _dice.D6());
                            List<int> source = list;
                            parameter.BaseScore = source.Sum() - source.Min();
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Abilities.IncreaseAbilityCommand.OnCanExecuteChanged();
                Abilities.DecreaseAbilityCommand.OnCanExecuteChanged();
                base.EventAggregator.Send(new MainWindowStatusUpdateEvent($"You rolled {parameter.BaseScore} on your {parameter.Name}"));
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GenerateRandomAbilityScore");
            }
            try
            {
                Abilities.CalculateAvailablePoints();
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "GenerateRandomAbilityScore");
            }
        }

        private async void RandomizeSingle(AbilityItem parameter)
        {
            parameter.BaseScore = await _dice.RandomizeAbilityScore();
            Abilities.IncreaseAbilityCommand.OnCanExecuteChanged();
            Abilities.DecreaseAbilityCommand.OnCanExecuteChanged();
            string statusMessage = $"You rolled {parameter.BaseScore} on your {parameter.Name}";
            base.EventAggregator.Send(new MainWindowStatusUpdateEvent(statusMessage));
            try
            {
                Abilities.CalculateAvailablePoints();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "RandomizeSingle");
            }
        }

        private async void RandomizeAbilitiesStatistic()
        {
            AbilityItem strength = Abilities.Strength;
            strength.BaseScore = await _dice.RandomizeAbilityScore();
            strength = Abilities.Dexterity;
            strength.BaseScore = await _dice.RandomizeAbilityScore();
            strength = Abilities.Constitution;
            strength.BaseScore = await _dice.RandomizeAbilityScore();
            strength = Abilities.Intelligence;
            strength.BaseScore = await _dice.RandomizeAbilityScore();
            strength = Abilities.Wisdom;
            strength.BaseScore = await _dice.RandomizeAbilityScore();
            strength = Abilities.Charisma;
            strength.BaseScore = await _dice.RandomizeAbilityScore();
            Abilities.IncreaseAbilityCommand.OnCanExecuteChanged();
            Abilities.DecreaseAbilityCommand.OnCanExecuteChanged();
            try
            {
                Abilities.CalculateAvailablePoints();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "RandomizeAbilitiesStatistic");
            }
        }

        private void SetGenerationOption()
        {
            try
            {
                _option = (AbilitiesGenerationOption)Builder.Presentation.Properties.Settings.Default.AbilitiesGenerationOption;
            }
            catch (Exception ex)
            {
                _option = AbilitiesGenerationOption.Roll4D6DiscardLowest;
                Logger.Exception(ex, "SetGenerationOption");
            }
            GenerationDisplayName = "N/A";
            IsRandomizeGeneration = false;
            IsArrayGeneration = false;
            IsPointsGeneration = false;
            Abilities.DisablePointsCalculation = true;
            switch (_option)
            {
                case AbilitiesGenerationOption.Roll3D6:
                    IsRandomizeGeneration = true;
                    GenerationDisplayName = "Roll 3D6";
                    break;
                case AbilitiesGenerationOption.Roll4D6DiscardLowest:
                    IsRandomizeGeneration = true;
                    GenerationDisplayName = "Roll 4D6, Discard Lowest";
                    break;
                case AbilitiesGenerationOption.Array:
                    IsArrayGeneration = true;
                    GenerationDisplayName = "Array (Drag & Drop to Switch Scores)";
                    break;
                case AbilitiesGenerationOption.Points:
                    IsPointsGeneration = true;
                    GenerationDisplayName = "Point Buy";
                    Abilities.DisablePointsCalculation = false;
                    Abilities.CalculateAvailablePoints();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ISubscriber<SettingsChangedEvent>.OnHandleEvent(SettingsChangedEvent args)
        {
            if (CharacterManager.Current != null && CharacterManager.Current.Status.IsLoaded)
            {
                bool hasChanges = CharacterManager.Current.Status.HasChanges;
                CharacterManager.Current.ReprocessCharacter();
                if (!hasChanges)
                {
                    CharacterManager.Current.Status.HasChanges = false;
                }
            }
            if (_option != (AbilitiesGenerationOption)args.Settings.AbilitiesGenerationOption)
            {
                SetGenerationOption();
            }
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            Abilities.Strength.BaseScore = 8;
            Abilities.Dexterity.BaseScore = 15;
            Abilities.Constitution.BaseScore = 13;
            Abilities.Intelligence.BaseScore = 12;
            Abilities.Wisdom.BaseScore = 10;
            Abilities.Charisma.BaseScore = 15;
            Abilities.Dexterity.AdditionalScore = 3;
            Abilities.Constitution.AdditionalScore = 1;
            Abilities.CalculateAvailablePoints();
            GenerationDisplayName = "Design 4D6";
            IsRandomizeGeneration = true;
        }
    }
}
