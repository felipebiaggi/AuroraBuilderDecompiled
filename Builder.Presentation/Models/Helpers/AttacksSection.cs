using Builder.Core;
using System.Collections.ObjectModel;

namespace Builder.Presentation.Models.Helpers
{
    public class AttacksSection : ObservableObject
    {
        public class AttackObject : ObservableObject
        {
            private string _name;

            private string _bonus;

            private string _damage;

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

            public string Bonus
            {
                get
                {
                    return _bonus;
                }
                set
                {
                    SetProperty(ref _bonus, value, "Bonus");
                }
            }

            public string Damage
            {
                get
                {
                    return _damage;
                }
                set
                {
                    SetProperty(ref _damage, value, "Damage");
                }
            }

            public AttackObject(string name, string bonus, string damage)
            {
                Name = name;
                Bonus = bonus;
                Damage = damage;
            }

            public void Reset()
            {
                Name = "";
                Bonus = "";
                Damage = "";
            }
        }

        private AttackObject _attackObject1;

        private AttackObject _attackObject2;

        private AttackObject _attackObject3;

        private string _attacksAndSpellcasting;

        public AttackObject AttackObject1
        {
            get
            {
                return _attackObject1;
            }
            set
            {
                SetProperty(ref _attackObject1, value, "AttackObject1");
            }
        }

        public AttackObject AttackObject2
        {
            get
            {
                return _attackObject2;
            }
            set
            {
                SetProperty(ref _attackObject2, value, "AttackObject2");
            }
        }

        public AttackObject AttackObject3
        {
            get
            {
                return _attackObject3;
            }
            set
            {
                SetProperty(ref _attackObject3, value, "AttackObject3");
            }
        }

        public string AttacksAndSpellcasting
        {
            get
            {
                return _attacksAndSpellcasting;
            }
            set
            {
                SetProperty(ref _attacksAndSpellcasting, value, "AttacksAndSpellcasting");
            }
        }

        public ObservableCollection<AttackSectionItem> Items { get; } = new ObservableCollection<AttackSectionItem>();

        public AttacksSection()
        {
            AttackObject1 = new AttackObject("", "", "");
            AttackObject2 = new AttackObject("", "", "");
            AttackObject3 = new AttackObject("", "", "");
            AttacksAndSpellcasting = "";
        }

        public void Reset()
        {
            AttackObject1.Reset();
            AttackObject2.Reset();
            AttackObject3.Reset();
            AttacksAndSpellcasting = string.Empty;
            Items.Clear();
        }
    }
}
