using Builder.Core;

namespace Builder.Presentation.Models
{
    public sealed class Character : ObservableObject
    {
        private string _name;

        private string _playerName;

        private int _level;

        private int _experience;

        private string _class;

        private string _archetype;

        private string _race;

        private string _background;

        private string _alignment;

        private int _proficiency;

        private string _age;

        private string _height;

        private string _weight;

        private string _eyes;

        private string _skin;

        private string _hair;

        private int _speed;

        private int _armorClass;

        private int _initiative;

        private int _maxHp;

        private string _backstory;

        private string _portraitFilename;

        private string _gender;

        private string _deity;

        private string _dragonmark;

        private string _allies;

        private string _organisationName;

        private string _organisationSymbol;

        private string _additonalFeatures;

        private int _multiclassSpellcasterLevel;

        private string _notes1;

        private string _notes2;

        public AbilitiesCollection Abilities { get; }

        public SkillsCollection Skills { get; }

        public SavingThrowCollection SavingThrows { get; }

        public CharacterInventory Inventory { get; }

        public Companion Companion { get; }

        public string PlayerName
        {
            get
            {
                return _playerName;
            }
            set
            {
                SetProperty(ref _playerName, value, "PlayerName");
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

        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                SetProperty(ref _level, value, "Level");
            }
        }

        public int Experience
        {
            get
            {
                return _experience;
            }
            set
            {
                SetProperty(ref _experience, value, "Experience");
            }
        }

        public string Class
        {
            get
            {
                return _class;
            }
            set
            {
                SetProperty(ref _class, value, "Class");
            }
        }

        public string Archetype
        {
            get
            {
                return _archetype;
            }
            set
            {
                SetProperty(ref _archetype, value, "Archetype");
            }
        }

        public string Race
        {
            get
            {
                return _race;
            }
            set
            {
                SetProperty(ref _race, value, "Race");
            }
        }

        public string Background
        {
            get
            {
                return _background;
            }
            set
            {
                SetProperty(ref _background, value, "Background");
            }
        }

        public string Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                SetProperty(ref _alignment, value, "Alignment");
            }
        }

        public int Proficiency
        {
            get
            {
                return _proficiency;
            }
            set
            {
                SetProperty(ref _proficiency, value, "Proficiency");
            }
        }

        public string Age
        {
            get
            {
                return _age;
            }
            set
            {
                SetProperty(ref _age, value, "Age");
            }
        }

        public string Height
        {
            get
            {
                return _height;
            }
            set
            {
                SetProperty(ref _height, value, "Height");
            }
        }

        public string Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                SetProperty(ref _weight, value, "Weight");
            }
        }

        public string Eyes
        {
            get
            {
                return _eyes;
            }
            set
            {
                SetProperty(ref _eyes, value, "Eyes");
            }
        }

        public string Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                SetProperty(ref _skin, value, "Skin");
            }
        }

        public string Hair
        {
            get
            {
                return _hair;
            }
            set
            {
                SetProperty(ref _hair, value, "Hair");
            }
        }

        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                SetProperty(ref _speed, value, "Speed");
            }
        }

        public int ArmorClass
        {
            get
            {
                return _armorClass;
            }
            set
            {
                SetProperty(ref _armorClass, value, "ArmorClass");
            }
        }

        public int Initiative
        {
            get
            {
                return _initiative;
            }
            set
            {
                SetProperty(ref _initiative, value, "Initiative");
            }
        }

        public int MaxHp
        {
            get
            {
                return _maxHp;
            }
            set
            {
                SetProperty(ref _maxHp, value, "MaxHp");
            }
        }

        public string PortraitFilename
        {
            get
            {
                return _portraitFilename;
            }
            set
            {
                SetProperty(ref _portraitFilename, value, "PortraitFilename");
            }
        }

        public string Backstory
        {
            get
            {
                return _backstory;
            }
            set
            {
                SetProperty(ref _backstory, value, "Backstory");
            }
        }

        public string Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                SetProperty(ref _gender, value, "Gender");
            }
        }

        public string Deity
        {
            get
            {
                return _deity;
            }
            set
            {
                SetProperty(ref _deity, value, "Deity");
            }
        }

        public string Dragonmark
        {
            get
            {
                return _dragonmark;
            }
            set
            {
                SetProperty(ref _dragonmark, value, "Dragonmark");
            }
        }

        public string Allies
        {
            get
            {
                return _allies;
            }
            set
            {
                SetProperty(ref _allies, value, "Allies");
            }
        }

        public string OrganisationName
        {
            get
            {
                return _organisationName;
            }
            set
            {
                SetProperty(ref _organisationName, value, "OrganisationName");
            }
        }

        public string OrganisationSymbol
        {
            get
            {
                return _organisationSymbol;
            }
            set
            {
                SetProperty(ref _organisationSymbol, value, "OrganisationSymbol");
            }
        }

        public string AdditionalFeatures
        {
            get
            {
                return _additonalFeatures;
            }
            set
            {
                SetProperty(ref _additonalFeatures, value, "AdditionalFeatures");
            }
        }

        public SpellcastingCollection SpellcastingCollection { get; } = new SpellcastingCollection();

        public AttacksSection AttacksSection { get; set; }

        public string CharacterBuildString => ToBuildString();

        public FillableField HeightField { get; set; } = new FillableField();

        public FillableField WeightField { get; set; } = new FillableField();

        public FillableField AgeField { get; set; } = new FillableField();

        public FillableField BackgroundStory { get; set; } = new FillableField();

        public FillableField Trinket { get; set; } = new FillableField();

        public FillableBackgroundCharacteristics FillableBackgroundCharacteristics { get; set; } = new FillableBackgroundCharacteristics();

        public FillableField BackgroundFeatureName { get; set; } = new FillableField();

        public FillableField BackgroundFeatureDescription { get; set; } = new FillableField();

        public FillableField ConditionalArmorClassField { get; set; } = new FillableField();

        public FillableField ConditionalSavingThrowsField { get; set; } = new FillableField();

        public int MulticlassSpellcasterLevel
        {
            get
            {
                return _multiclassSpellcasterLevel;
            }
            set
            {
                SetProperty(ref _multiclassSpellcasterLevel, value, "MulticlassSpellcasterLevel");
            }
        }

        public SpellcastingSectionSlots MulticlassSpellSlots { get; } = new SpellcastingSectionSlots();

        public string Notes1
        {
            get
            {
                return _notes1;
            }
            set
            {
                SetProperty(ref _notes1, value, "Notes1");
            }
        }

        public string Notes2
        {
            get
            {
                return _notes2;
            }
            set
            {
                SetProperty(ref _notes2, value, "Notes2");
            }
        }

        public Character()
        {
            Abilities = new AbilitiesCollection();
            Skills = new SkillsCollection(Abilities);
            SavingThrows = new SavingThrowCollection(Abilities);
            Inventory = new CharacterInventory();
            Companion = new Companion();
            AttacksSection = new AttacksSection();
        }

        public bool MeetsLevelRequirement(int requiredLevel)
        {
            return Level >= requiredLevel;
        }

        public void ResetEntryFields()
        {
            Name = "";
            PortraitFilename = "";
            Experience = 0;
            PlayerName = "";
            Level = 0;
            Class = "";
            Archetype = "";
            Race = "";
            Background = "";
            Alignment = "";
            Age = "";
            Height = "";
            Weight = "";
            Eyes = "";
            Skin = "";
            Hair = "";
            Backstory = "";
            BackgroundStory.Clear(clearOriginalContent: true);
            Trinket.Clear();
            FillableBackgroundCharacteristics.Clear(clearOriginalContent: true);
            BackgroundFeatureName.OriginalContent = "";
            BackgroundFeatureDescription.OriginalContent = "";
            BackgroundFeatureName.Clear();
            BackgroundFeatureDescription.Clear();
            OrganisationName = "";
            OrganisationSymbol = "";
            Allies = "";
            AdditionalFeatures = "";
            Gender = "Male";
            Deity = "";
            Dragonmark = "";
            Abilities.Reset();
            Inventory.ClearInventory();
            AttacksSection.Reset();
            SpellcastingCollection.Reset();
            AgeField.Clear();
            HeightField.Clear();
            WeightField.Clear();
            ConditionalArmorClassField.Clear(clearOriginalContent: true);
            ConditionalSavingThrowsField.Clear(clearOriginalContent: true);
            MulticlassSpellcasterLevel = 0;
            MulticlassSpellSlots.Clear();
            Companion.Reset();
            Notes1 = "";
            Notes2 = "";
        }

        public override string ToString()
        {
            return $"{_name}, Level {_level} {_race} {_class}";
        }

        public string ToBuildString()
        {
            if (string.IsNullOrWhiteSpace(Race) && !string.IsNullOrWhiteSpace(Class))
            {
                return $"Level {Level} {Class}";
            }
            if (!string.IsNullOrWhiteSpace(Race) && string.IsNullOrWhiteSpace(Class))
            {
                return $"Level {Level} {Race}";
            }
            return $"Level {Level} {Race} {Class}";
        }
    }

}
