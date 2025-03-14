using Builder.Core;
using Builder.Core.Events;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Models.Helpers;
using Builder.Presentation.ViewModels.Base;
using System.Linq;
using System.Windows.Input;

namespace Builder.Presentation.ViewModels.Shell.Manage
{
    public sealed class ManageAttacksViewModel : ViewModelBase, ISubscriber<CharacterManagerElementsUpdated>, ISubscriber<ReprocessCharacterEvent>
    {
        private AttackSectionItem _selectedAttackItem;

        public CharacterManager Manager => CharacterManager.Current;

        public AttacksSection Attacks => Manager.Character.AttacksSection;

        public AttackSectionItem SelectedAttackItem
        {
            get
            {
                return _selectedAttackItem;
            }
            set
            {
                SetProperty(ref _selectedAttackItem, value, "SelectedAttackItem");
                MoveAttackUpCommand.RaiseCanExecuteChanged();
                MoveAttackDownCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddAttackCommand => new RelayCommand(AddAttack);

        public ICommand EditAttackCommand => new RelayCommand<AttackSectionItem>(EditAttack);

        public ICommand RemoveAttackCommand => new RelayCommand<AttackSectionItem>(RemoveAttack);

        public ICommand ToggleDisplayAttackCommand => new RelayCommand<AttackSectionItem>(ToggleDisplayAttack);

        public RelayCommand<AttackSectionItem> MoveAttackUpCommand { get; }

        public RelayCommand<AttackSectionItem> MoveAttackDownCommand { get; }

        public ManageAttacksViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            MoveAttackUpCommand = new RelayCommand<AttackSectionItem>(MoveAttackUp, CanMoveAttackUp);
            MoveAttackDownCommand = new RelayCommand<AttackSectionItem>(MoveAttackDown, CanMoveAttackDown);
            SubscribeWithEventAggregator();
        }

        private void MoveAttackUp(AttackSectionItem parameter)
        {
            if (parameter != null && !parameter.Equals(Attacks.Items.FirstOrDefault()))
            {
                int num = Attacks.Items.IndexOf(parameter);
                Attacks.Items.Move(num, num - 1);
                MoveAttackUpCommand.RaiseCanExecuteChanged();
                MoveAttackDownCommand.RaiseCanExecuteChanged();
            }
        }

        private void MoveAttackDown(AttackSectionItem parameter)
        {
            if (parameter != null && !parameter.Equals(Attacks.Items.LastOrDefault()))
            {
                int num = Attacks.Items.IndexOf(parameter);
                Attacks.Items.Move(num, num + 1);
                MoveAttackUpCommand.RaiseCanExecuteChanged();
                MoveAttackDownCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanMoveAttackUp(AttackSectionItem parameter)
        {
            if (parameter == null || parameter.Equals(Attacks.Items.FirstOrDefault()))
            {
                return false;
            }
            return Attacks.Items.IndexOf(parameter) != 0;
        }

        private bool CanMoveAttackDown(AttackSectionItem parameter)
        {
            if (parameter == null || parameter.Equals(Attacks.Items.LastOrDefault()))
            {
                return false;
            }
            return Attacks.Items.IndexOf(parameter) != Attacks.Items.Count - 1;
        }

        private void AddAttack()
        {
            AttackSectionItem attackSectionItem = new AttackSectionItem("New Attack");
            bool? flag = new CreateAttackWindow
            {
                DataContext = attackSectionItem
            }.ShowDialog();
            if (flag.HasValue && flag.Value)
            {
                Attacks.Items.Add(attackSectionItem);
            }
        }

        private void EditAttack(AttackSectionItem parameter)
        {
            if (parameter != null)
            {
                CreateAttackWindow createAttackWindow = new CreateAttackWindow();
                createAttackWindow.DataContext = parameter;
                createAttackWindow.ShowDialog();
            }
        }

        private void RemoveAttack(AttackSectionItem parameter)
        {
            if (parameter != null)
            {
                Attacks.Items.Remove(parameter);
            }
        }

        private void ToggleDisplayAttack(AttackSectionItem parameter)
        {
            if (parameter != null)
            {
                parameter.IsDisplayed = !parameter.IsDisplayed;
            }
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            Attacks.Items.Add(new AttackSectionItem("Attack 1", "Description 1"));
            Attacks.Items.Add(new AttackSectionItem("Attack 2", "Description 2")
            {
                IsDisplayed = false
            });
            Attacks.Items.Add(new AttackSectionItem("Attack 3", "Description 3"));
            Attacks.Items.Add(new AttackSectionItem("Attack 4", "Description 4")
            {
                IsDisplayed = false
            });
            SelectedAttackItem = Attacks.Items.FirstOrDefault();
        }

        private void UpdateLinkedAttacks()
        {
            if (!Manager.Status.IsLoaded)
            {
                return;
            }
            foreach (AttackSectionItem item in Attacks.Items)
            {
                if (item.EquipmentItem != null && item.IsAutomaticAddition)
                {
                    item.UpdateCalculations();
                }
            }
        }

        public void OnHandleEvent(CharacterManagerElementsUpdated args)
        {
            UpdateLinkedAttacks();
        }

        public void OnHandleEvent(ReprocessCharacterEvent args)
        {
            UpdateLinkedAttacks();
        }
    }
}
