using Builder.Core;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Models;
using Builder.Presentation.Models.NewFolder1;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace Builder.Presentation.ViewModels.Shell.Manage
{
    public sealed class ManageContentViewModel : ViewModelBase
    {
        private string _characterName;

        private string _playerName;

        private string _experience;

        private string _deity;

        private string _spellcastingClass;

        private string _spellcastingAbility;

        private string _spellSaveDc;

        private string _spellAttackBonus;

        private string _age;

        private string _height;

        private string _weight;

        private string _eyes;

        private string _skin;

        private string _hair;

        private string _characterBackstory;

        private Random _rnd = new Random();

        private string _additionalFeaturesAndTraits;

        private string _treasure;

        private string _copper;

        private string _silver;

        private string _electrum;

        private string _gold;

        private string _platinum;

        private string _equipment;

        private string _cantripSlot1;

        public CharacterManager CharacterManager => CharacterManager.Current;

        public Character Character => CharacterManager.Character;

        public string CharacterName
        {
            get
            {
                return _characterName;
            }
            set
            {
                SetProperty(ref _characterName, value, "CharacterName");
                base.EventAggregator.Send(new CharacterNameChangedEvent(_characterName));
            }
        }

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

        public string Experience
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

        public string SpellSaveDc
        {
            get
            {
                return _spellSaveDc;
            }
            set
            {
                SetProperty(ref _spellSaveDc, value, "SpellSaveDc");
            }
        }

        public string SpellAttackBonus
        {
            get
            {
                return _spellAttackBonus;
            }
            set
            {
                SetProperty(ref _spellAttackBonus, value, "SpellAttackBonus");
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

        public ICommand ShowSymbolsGalleryCommand => new RelayCommand(ShowSymbolsGallery);

        public ICommand ShowCompanionsGalleryCommand => new RelayCommand(ShowCompanionsGallery);

        public ICommand ClearSymbolCommand => new RelayCommand(ClearSymbol);

        public string CharacterBackstory
        {
            get
            {
                return _characterBackstory;
            }
            set
            {
                SetProperty(ref _characterBackstory, value, "CharacterBackstory");
            }
        }

        public FillableBackgroundCharacteristics FillableBackgroundCharacteristics { get; set; } = new FillableBackgroundCharacteristics();

        public ObservableCollection<string> Trinkets { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Organizations { get; } = new ObservableCollection<string>();

        public ICommand RandomizeTrinketCommand => new RelayCommand(RandomizeTrinket);

        public string AdditionalFeaturesAndTraits
        {
            get
            {
                return _additionalFeaturesAndTraits;
            }
            set
            {
                SetProperty(ref _additionalFeaturesAndTraits, value, "AdditionalFeaturesAndTraits");
            }
        }

        public string Treasure
        {
            get
            {
                return _treasure;
            }
            set
            {
                SetProperty(ref _treasure, value, "Treasure");
            }
        }

        public ICommand GenerateCharacterNameCommand { get; }

        public ICommand GenerateCharacterSheetCommand { get; }

        public string Copper
        {
            get
            {
                return _copper;
            }
            set
            {
                SetProperty(ref _copper, value, "Copper");
            }
        }

        public string Silver
        {
            get
            {
                return _silver;
            }
            set
            {
                SetProperty(ref _silver, value, "Silver");
            }
        }

        public string Electrum
        {
            get
            {
                return _electrum;
            }
            set
            {
                SetProperty(ref _electrum, value, "Electrum");
            }
        }

        public string Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                SetProperty(ref _gold, value, "Gold");
            }
        }

        public string Platinum
        {
            get
            {
                return _platinum;
            }
            set
            {
                SetProperty(ref _platinum, value, "Platinum");
            }
        }

        public string Equipment
        {
            get
            {
                return _equipment;
            }
            set
            {
                SetProperty(ref _equipment, value, "Equipment");
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

        public SpellContentViewModel SpellContentViewModel { get; set; } = new SpellContentViewModel();

        public ManageContentViewModel()
        {
            GenerateCharacterNameCommand = new RelayCommand(GenerateCharacterName);
            GenerateCharacterSheetCommand = new RelayCommand(GenerateCharacterSheet);
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            PlayerName = Builder.Presentation.Properties.Settings.Default.PlayerName;
            Trinkets.Add("A mummified goblin hand");
            Trinkets.Add("A piece of crystal that faintly glows in the moonlight");
            Trinkets.Add("A gold coin minted in a fallen civilization");
            Trinkets.Add("A diary written in a language you don’t know");
            Trinkets.Add("A brass ring that never tarnishes");
            Trinkets.Add("An old chess piece made from glass");
            Trinkets.Add("A pair of knucklebone dice, each with a skull symbol on the side that would normally show six pips");
            Trinkets.Add("A small idol depicting a nightmarish creature that gives you unsettling dreams when you sleep near it");
            Trinkets.Add("A rope necklace from which dangles four mummified elf fingers");
            Trinkets.Add("The deed for a parcel of land in a realm unknown to you");
            Trinkets.Add("A 1-ounce block made from an unknown material");
            Trinkets.Add("A small cloth doll skewered with needles");
            Trinkets.Add("A tooth from an unknown beast");
            Trinkets.Add("An enormous scale, perhaps from a dragon");
            Trinkets.Add("A bright green feather");
            Trinkets.Add("An old divination card bearing your likeness");
            Trinkets.Add("A glass orb filled with moving smoke");
            Trinkets.Add("A 1-pound egg with a bright red shell");
            Trinkets.Add("A pipe that blows bubbles");
            Trinkets.Add("A glass jar containing a weird bit of flesh floating in pickling fluid");
            Trinkets.Add("A tiny gnome-crafted music box that plays a song you dimly remember from your childhood");
            Trinkets.Add("A small wooden statuette of a smug halfling");
            Trinkets.Add("A brass orb etched with strange runes");
            Trinkets.Add("A multicolored stone disk");
            Trinkets.Add("A tiny silver icon of a raven");
            Trinkets.Add("A bag containing forty-seven humanoid teeth, one of which is rotten");
            Trinkets.Add("A shard of obsidian that always feels warm to the touch");
            Trinkets.Add("A dragon’s bony talon hanging from a plain leather necklace");
            Trinkets.Add("A pair of old socks");
            Trinkets.Add("A blank book whose pages refuse to hold ink, chalk, graphite, or any other substance or marking");
            Trinkets.Add("A silver badge in the shape of a five-pointed star");
            Trinkets.Add("A knife that belonged to a relative");
            Trinkets.Add("A glass vial filled with nail clippings");
            Trinkets.Add("A rectangular metal device with two tiny metal cups on one end that throws sparks when wet");
            Trinkets.Add("A white, sequined glove sized for a human");
            Trinkets.Add("A vest with one hundred tiny pockets");
            Trinkets.Add("A small, weightless stone block");
            Trinkets.Add("A tiny sketch portrait of a goblin");
            Trinkets.Add("An empty glass vial that smells of perfume when opened");
            Trinkets.Add("A gemstone that looks like a lump of coal when examined by anyone but you");
            Trinkets.Add("A scrap of cloth from an old banner");
            Trinkets.Add("A rank insignia from a lost legionnaire");
            Trinkets.Add("A tiny silver bell without a clapper");
            Trinkets.Add("A mechanical canary inside a gnome-crafted lamp");
            Trinkets.Add("A tiny chest carved to look like it has numerous feet on the bottom");
            Trinkets.Add("A dead sprite inside a clear glass bottle");
            Trinkets.Add("A metal can that has no opening but sounds as if it is filled with liquid, sand, spiders, or broken glass (your choice)");
            Trinkets.Add("A glass orb filled with water, in which swims a clockwork goldfish");
            Trinkets.Add("A silver spoon with an M engraved on the handle");
            Trinkets.Add("A whistle made from the gold-colored wood");
            Trinkets.Add("A dead scarab beetle the size of your hand");
            Trinkets.Add("Two toy soldiers, one with a missing head");
            Trinkets.Add("A small box filled with different-sized buttons");
            Trinkets.Add("A candle that can’t be lit");
            Trinkets.Add("A tiny cage with no door");
            Trinkets.Add("An old key");
            Trinkets.Add("An indecipherable treasure map");
            Trinkets.Add("A hilt from a broken sword");
            Trinkets.Add("A rabbit’s foot");
            Trinkets.Add("A glass eye");
            Trinkets.Add("A cameo carved in the likeness of a hideous person");
            Trinkets.Add("A silver skull the size of a coin");
            Trinkets.Add("An alabaster mask");
            Trinkets.Add("A pyramid of sticky black incense that smells very bad");
            Trinkets.Add("A nightcap that, when worn, gives you pleasant dreams");
            Trinkets.Add("A single caltrop made from bone");
            Trinkets.Add("A gold monocle frame without the lens");
            Trinkets.Add("A 1 inch cube, each side painted a different color");
            Trinkets.Add("A crystal knob from a door");
            Trinkets.Add("A small packet filled with pink dust");
            Trinkets.Add("A fragment of a beautiful song, written as musical notes on two pieces of parchment");
            Trinkets.Add("A silver teardrop earring made from a real teardrop");
            Trinkets.Add("The shell of an egg painted with scenes of human misery in disturbing detail");
            Trinkets.Add("A fan that, when unfolded, shows a sleeping cat");
            Trinkets.Add("A set of bone pipes");
            Trinkets.Add("A four-leaf clover pressed inside a book discussing manners and etiquette");
            Trinkets.Add("A sheet of parchment upon which is drawn a complex mechanical contraption");
            Trinkets.Add("An ornate scabbard that fits no blade you have found so far");
            Trinkets.Add("An invitation to a party where a murder happened");
            Trinkets.Add("A bronze pentacle with an etching of a rat’s head in its center");
            Trinkets.Add("A purple handkerchief embroidered with the name of a powerful archmage");
            Trinkets.Add("Half of a floorplan for a temple, castle, or some other structure");
            Trinkets.Add("A bit of folded cloth that, when unfolded, turns into a stylish cap");
            Trinkets.Add("A receipt of deposit at a bank in a far-flung city");
            Trinkets.Add("A diary with seven missing pages");
            Trinkets.Add("An empty silver snuffbox bearing an inscription on the surface that says “dreams”");
            Trinkets.Add("An iron holy symbol devoted to an unknown god");
            Trinkets.Add("A book that tells the story of a legendary hero’s rise and fall, with the last chapter missing");
            Trinkets.Add("A vial of dragon blood");
            Trinkets.Add("An ancient arrow of elven design");
            Trinkets.Add("A needle that never bends");
            Trinkets.Add("An ornate brooch of dwarven design");
            Trinkets.Add("An empty wine bottle bearing a pretty label that says, “The Wizard of Wines Winery, Red Dragon Crush, 33142 - W”");
            Trinkets.Add("A mosaic tile with a multicolored, glazed surface");
            Trinkets.Add("A petrified mouse");
            Trinkets.Add("A black pirate flag adorned with a dragon’s skull");
            Trinkets.Add("A tiny mechanical crab or spider that moves about when it’s not being observed");
            Trinkets.Add("A glass jar containing lard with a label that reads, “Griffon Grease”");
            Trinkets.Add("A wooden box with a ceramic bottom that holds a living worm with a head on each end of its body");
            Trinkets.Add("A metal urn containing the ashes of a hero");
            Trinkets.Add("An hourglass decorated with emeralds which is filled with acid instead of sand");
            Organizations.Add("The Harpers");
            Organizations.Add("The Emerald Enclave");
            Organizations.Add("The Lords’ Alliance");
            Organizations.Add("The Order of the Gauntlet");
            Organizations.Add("The Zhentarim");
            Organizations.Add("House Cannith");
            Organizations.Add("House Deneith");
            Organizations.Add("House Ghallanda");
            Organizations.Add("House Jorasco");
            Organizations.Add("House Kundarak");
            Organizations.Add("House Lyrandar");
            Organizations.Add("House Medani");
            Organizations.Add("House Orien");
            Organizations.Add("House Phiarlan");
            Organizations.Add("House Sivis");
            Organizations.Add("House Tharashk");
            Organizations.Add("House Thuranni");
            Organizations.Add("House Vadalis");
            Organizations.Add("Azorius Senate");
            Organizations.Add("Boros Legion");
            Organizations.Add("Cult of Rakdos");
            Organizations.Add("Golgari Swarm");
            Organizations.Add("Gruul Clans");
            Organizations.Add("House Dimir");
            Organizations.Add("Izzet League");
            Organizations.Add("Orzhov Syndicate");
            Organizations.Add("Selesnya Conclave");
            Organizations.Add("Simic Combine");
            foreach (ElementBase item in DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Organization")))
            {
                if (!Organizations.Contains(item.Name))
                {
                    Organizations.Add(item.Name);
                }
            }
            base.EventAggregator.Subscribe(this);
        }

        private void ShowSymbolsGallery()
        {
            ApplicationManager.Current.EventAggregator.Send(new ShowSliderEvent(Slider.OrganizationSymbolsGallery));
        }

        private void ShowCompanionsGallery()
        {
            ApplicationManager.Current.EventAggregator.Send(new ShowSliderEvent(Slider.CompanionGallery));
        }

        private void ClearSymbol()
        {
            Character.OrganisationSymbol = string.Empty;
        }

        private void RandomizeTrinket()
        {
            int num = _rnd.Next(Trinkets.Count);
            if (num >= Trinkets.Count)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                num = _rnd.Next(Trinkets.Count);
            }
            Character.Trinket.Clear();
            Character.Trinket.OriginalContent = Trinkets[num];
        }

        private void GenerateCharacterName()
        {
            Random random = new Random(Environment.TickCount);
            if (CharacterManager.Current.Elements.FirstOrDefault((ElementBase x) => x.Type == "Race") is Race race)
            {
                string text = CharacterManager.Current.Character.Gender.ToLower();
                if (race.Names != null)
                {
                    if (text == "male" || text == "female")
                    {
                        CharacterName = race.Names.GenerateRandomName(text);
                    }
                    else
                    {
                        CharacterName = race.Names.GenerateRandomName();
                    }
                }
                else
                {
                    List<string> list = ((text == "male") ? race.MaleNames : race.FemaleNames);
                    CharacterName = list[random.Next(list.Count)];
                }
            }
            else
            {
                string[] array = new string[4] { "Dr. Ustabil", "Beguil the Bard", "Kurald Emurlahn", "Flaem" };
                CharacterName = array[random.Next(array.Length)];
            }
        }

        private void GenerateCharacterSheet()
        {
            try
            {
                CharacterManager.Current.GenerateCharacterSheet();
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowException(ex);
            }
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            CharacterName = "Dr. Ustabil";
            Experience = "6500";
            Age = "63";
            Eyes = "Blue";
            Hair = "White";
        }
    }
}
