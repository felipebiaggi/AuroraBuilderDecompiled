using Builder.Core;


namespace Builder.Presentation.ViewModels.Shell.Manage
{
    public class SpellcastingCollection : ObservableObject
    {
        private string _cantripSlot1;

        private string _cantripSlot2;

        private string _cantripSlot3;

        private string _cantripSlot4;

        private string _cantripSlot5;

        private string _cantripSlot6;

        private string _cantripSlot7;

        private string _cantripSlot8;

        private string _spellcastingClass;

        private string _spellcastingAbility;

        private string _spellcastingDifficultyClass;

        private string _spellcastingAttackBonus;

        public string SpellcastingClass
        {
            get
            {
                return _spellcastingClass;
            }
            set
            {
                SetProperty(ref _spellcastingClass, value, "SpellcastingClass");
            }
        }

        public string SpellcastingAbility
        {
            get
            {
                return _spellcastingAbility;
            }
            set
            {
                SetProperty(ref _spellcastingAbility, value, "SpellcastingAbility");
            }
        }

        public string SpellcastingDifficultyClass
        {
            get
            {
                return _spellcastingDifficultyClass;
            }
            set
            {
                SetProperty(ref _spellcastingDifficultyClass, value, "SpellcastingDifficultyClass");
            }
        }

        public string SpellcastingAttackBonus
        {
            get
            {
                return _spellcastingAttackBonus;
            }
            set
            {
                SetProperty(ref _spellcastingAttackBonus, value, "SpellcastingAttackBonus");
            }
        }

        public string CantripSlot1
        {
            get
            {
                return _cantripSlot1;
            }
            set
            {
                SetProperty(ref _cantripSlot1, value, "CantripSlot1");
            }
        }

        public string CantripSlot2
        {
            get
            {
                return _cantripSlot2;
            }
            set
            {
                SetProperty(ref _cantripSlot2, value, "CantripSlot2");
            }
        }

        public string CantripSlot3
        {
            get
            {
                return _cantripSlot3;
            }
            set
            {
                SetProperty(ref _cantripSlot3, value, "CantripSlot3");
            }
        }

        public string CantripSlot4
        {
            get
            {
                return _cantripSlot4;
            }
            set
            {
                SetProperty(ref _cantripSlot4, value, "CantripSlot4");
            }
        }

        public string CantripSlot5
        {
            get
            {
                return _cantripSlot5;
            }
            set
            {
                SetProperty(ref _cantripSlot5, value, "CantripSlot5");
            }
        }

        public string CantripSlot6
        {
            get
            {
                return _cantripSlot6;
            }
            set
            {
                SetProperty(ref _cantripSlot6, value, "CantripSlot6");
            }
        }

        public string CantripSlot7
        {
            get
            {
                return _cantripSlot7;
            }
            set
            {
                SetProperty(ref _cantripSlot7, value, "CantripSlot7");
            }
        }

        public string CantripSlot8
        {
            get
            {
                return _cantripSlot8;
            }
            set
            {
                SetProperty(ref _cantripSlot8, value, "CantripSlot8");
            }
        }

        public ObservableSpell Spell1Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot8 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot9 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot10 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot11 { get; } = new ObservableSpell();

        public ObservableSpell Spell1Slot12 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot8 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot9 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot10 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot11 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot12 { get; } = new ObservableSpell();

        public ObservableSpell Spell2Slot13 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot8 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot9 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot10 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot11 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot12 { get; } = new ObservableSpell();

        public ObservableSpell Spell3Slot13 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot8 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot9 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot10 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot11 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot12 { get; } = new ObservableSpell();

        public ObservableSpell Spell4Slot13 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot8 { get; } = new ObservableSpell();

        public ObservableSpell Spell5Slot9 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot8 { get; } = new ObservableSpell();

        public ObservableSpell Spell6Slot9 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot8 { get; } = new ObservableSpell();

        public ObservableSpell Spell7Slot9 { get; } = new ObservableSpell();

        public ObservableSpell Spell8Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell8Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell8Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell8Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell8Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell8Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell8Slot7 { get; } = new ObservableSpell();

        public ObservableSpell Spell9Slot1 { get; } = new ObservableSpell();

        public ObservableSpell Spell9Slot2 { get; } = new ObservableSpell();

        public ObservableSpell Spell9Slot3 { get; } = new ObservableSpell();

        public ObservableSpell Spell9Slot4 { get; } = new ObservableSpell();

        public ObservableSpell Spell9Slot5 { get; } = new ObservableSpell();

        public ObservableSpell Spell9Slot6 { get; } = new ObservableSpell();

        public ObservableSpell Spell9Slot7 { get; } = new ObservableSpell();

        public void Reset()
        {
            SpellcastingClass = "";
            SpellcastingAbility = "";
            SpellcastingDifficultyClass = "";
            SpellcastingAttackBonus = "";
            CantripSlot1 = "";
            CantripSlot2 = "";
            CantripSlot3 = "";
            CantripSlot4 = "";
            CantripSlot5 = "";
            CantripSlot6 = "";
            CantripSlot7 = "";
            CantripSlot8 = "";
            Spell1Slot1.Reset();
            Spell1Slot2.Reset();
            Spell1Slot3.Reset();
            Spell1Slot4.Reset();
            Spell1Slot5.Reset();
            Spell1Slot6.Reset();
            Spell1Slot7.Reset();
            Spell1Slot8.Reset();
            Spell1Slot9.Reset();
            Spell1Slot10.Reset();
            Spell1Slot11.Reset();
            Spell1Slot12.Reset();
            Spell2Slot1.Reset();
            Spell2Slot2.Reset();
            Spell2Slot3.Reset();
            Spell2Slot4.Reset();
            Spell2Slot5.Reset();
            Spell2Slot6.Reset();
            Spell2Slot7.Reset();
            Spell2Slot8.Reset();
            Spell2Slot9.Reset();
            Spell2Slot10.Reset();
            Spell2Slot11.Reset();
            Spell2Slot12.Reset();
            Spell2Slot13.Reset();
            Spell3Slot1.Reset();
            Spell3Slot2.Reset();
            Spell3Slot3.Reset();
            Spell3Slot4.Reset();
            Spell3Slot5.Reset();
            Spell3Slot6.Reset();
            Spell3Slot7.Reset();
            Spell3Slot8.Reset();
            Spell3Slot9.Reset();
            Spell3Slot10.Reset();
            Spell3Slot11.Reset();
            Spell3Slot12.Reset();
            Spell3Slot13.Reset();
            Spell4Slot1.Reset();
            Spell4Slot2.Reset();
            Spell4Slot3.Reset();
            Spell4Slot4.Reset();
            Spell4Slot5.Reset();
            Spell4Slot6.Reset();
            Spell4Slot7.Reset();
            Spell4Slot8.Reset();
            Spell4Slot9.Reset();
            Spell4Slot10.Reset();
            Spell4Slot11.Reset();
            Spell4Slot12.Reset();
            Spell4Slot13.Reset();
            Spell5Slot1.Reset();
            Spell5Slot2.Reset();
            Spell5Slot3.Reset();
            Spell5Slot4.Reset();
            Spell5Slot5.Reset();
            Spell5Slot6.Reset();
            Spell5Slot7.Reset();
            Spell5Slot8.Reset();
            Spell5Slot9.Reset();
            Spell6Slot1.Reset();
            Spell6Slot2.Reset();
            Spell6Slot3.Reset();
            Spell6Slot4.Reset();
            Spell6Slot5.Reset();
            Spell6Slot6.Reset();
            Spell6Slot7.Reset();
            Spell6Slot8.Reset();
            Spell6Slot9.Reset();
            Spell7Slot1.Reset();
            Spell7Slot2.Reset();
            Spell7Slot3.Reset();
            Spell7Slot4.Reset();
            Spell7Slot5.Reset();
            Spell7Slot6.Reset();
            Spell7Slot7.Reset();
            Spell7Slot8.Reset();
            Spell7Slot9.Reset();
            Spell8Slot1.Reset();
            Spell8Slot2.Reset();
            Spell8Slot3.Reset();
            Spell8Slot4.Reset();
            Spell8Slot5.Reset();
            Spell8Slot6.Reset();
            Spell8Slot7.Reset();
            Spell9Slot1.Reset();
            Spell9Slot2.Reset();
            Spell9Slot3.Reset();
            Spell9Slot4.Reset();
            Spell9Slot5.Reset();
            Spell9Slot6.Reset();
            Spell9Slot7.Reset();
        }
    }
}
