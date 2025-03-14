using Builder.Core;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Data.Rules;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Extensions;
using Builder.Presentation.Models.Sources;
using Builder.Presentation.Properties;
using Builder.Presentation.Services.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Builder.Presentation.Models
{
    public class CharacterFile : ObservableObject
    {
        public class LoadResult
        {
            public bool Success { get; }

            public string Message { get; }

            public LoadResult(bool success, string message = "")
            {
                Success = success;
                Message = message;
            }
        }

        private const string RootNodeName = "character";

        private const string DisplayPropertiesNodeName = "display-properties";

        private const string BuildNodeName = "build";

        private const string AbilitiesNodeName = "abilities";

        private const string AppearanceNodeName = "appearance";

        private const string ElementsNodeName = "elements";

        private const string SumNodeName = "sum";

        private const string DisplayPropertiesIsFavorite = "favorite";

        private const string DisplayPropertiesName = "name";

        private const string DisplayPropertiesRace = "race";

        private const string DisplayPropertiesClass = "class";

        private const string DisplayPropertiesBackground = "background";

        private const string DisplayPropertiesLevel = "level";

        private const string DisplayPropertiesLocalPortrait = "local-portrait";

        private const string DisplayPropertiesBase64Portrait = "base64-portrait";

        private XmlDocument _document;

        private string _filepath;

        private bool _isInitialized;

        private bool _isNew;

        private bool _isFavorite;

        private string _displayName;

        private string _displayRace;

        private string _displayClass;

        private string _displayArchetype;

        private string _displayBackground;

        private string _displayLevel;

        private string _displayPortraitFilePath;

        private string _displayPortraitBase64;

        private string _fileName;

        private string _displayVersion;

        private string _collectionGroupName;

        public string FilePath
        {
            get
            {
                return _filepath;
            }
            set
            {
                SetProperty(ref _filepath, value, "FilePath");
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                SetProperty(ref _fileName, value, "FileName");
            }
        }

        public string DisplayVersion
        {
            get
            {
                return _displayVersion;
            }
            set
            {
                SetProperty(ref _displayVersion, value, "DisplayVersion");
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                SetProperty(ref _isInitialized, value, "IsInitialized");
            }
        }

        public bool IsNew
        {
            get
            {
                return _isNew;
            }
            set
            {
                SetProperty(ref _isNew, value, "IsNew");
            }
        }

        public bool IsFavorite
        {
            get
            {
                return _isFavorite;
            }
            set
            {
                SetProperty(ref _isFavorite, value, "IsFavorite");
            }
        }

        public string CollectionGroupName
        {
            get
            {
                return _collectionGroupName;
            }
            set
            {
                SetProperty(ref _collectionGroupName, value, "CollectionGroupName");
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                SetProperty(ref _displayName, value, "DisplayName");
            }
        }

        public string DisplayRace
        {
            get
            {
                return _displayRace;
            }
            set
            {
                SetProperty(ref _displayRace, value, "DisplayRace");
                OnPropertyChanged("DisplayBuild");
            }
        }

        public string DisplayClass
        {
            get
            {
                return _displayClass;
            }
            set
            {
                SetProperty(ref _displayClass, value, "DisplayClass");
                OnPropertyChanged("DisplayBuild");
            }
        }

        public string DisplayArchetype
        {
            get
            {
                return _displayArchetype;
            }
            set
            {
                SetProperty(ref _displayArchetype, value, "DisplayArchetype");
            }
        }

        public string DisplayBackground
        {
            get
            {
                return _displayBackground;
            }
            set
            {
                SetProperty(ref _displayBackground, value, "DisplayBackground");
            }
        }

        public string DisplayLevel
        {
            get
            {
                return _displayLevel;
            }
            set
            {
                SetProperty(ref _displayLevel, value, "DisplayLevel");
                OnPropertyChanged("DisplayBuild");
            }
        }

        public string DisplayPortraitFilePath
        {
            get
            {
                return _displayPortraitFilePath;
            }
            set
            {
                SetProperty(ref _displayPortraitFilePath, value, "DisplayPortraitFilePath");
            }
        }

        public string DisplayPortraitBase64
        {
            get
            {
                return _displayPortraitBase64;
            }
            set
            {
                SetProperty(ref _displayPortraitBase64, value, "DisplayPortraitBase64");
            }
        }

        public string DisplayBuild => "Level " + DisplayLevel + " " + DisplayRace + " " + DisplayClass;

        public CharacterFile(string filepath)
        {
            _filepath = filepath;
            _displayVersion = Resources.ApplicationVersion;
        }

        public void InitializeDisplayPropertiesFromCharacter(Character character)
        {
            DisplayName = character.Name;
            DisplayRace = character.Race;
            DisplayClass = character.Class;
            DisplayArchetype = character.Archetype;
            DisplayBackground = character.Background;
            DisplayLevel = character.Level.ToString();
            DisplayPortraitFilePath = character.PortraitFilename;
        }

        public void InitializeDisplayPropertiesFromFilePath()
        {
            try
            {
                IsInitialized = false;
                _document = new XmlDocument();
                _document.Load(_filepath);
                XmlElement xmlElement = _document["character"];
                XmlElement xmlElement2 = xmlElement?["display-properties"];
                ReadInformationNode(xmlElement);
                if (xmlElement.ContainsAttribute("version"))
                {
                    string attributeValue = xmlElement.GetAttributeValue("version");
                    DisplayVersion = attributeValue;
                }
                if (xmlElement2 == null)
                {
                    MessageDialogService.Show("unable to load display properties for " + _filepath);
                    IsInitialized = false;
                    return;
                }
                foreach (XmlAttribute attribute in xmlElement2.Attributes)
                {
                    string name = attribute.Name;
                    if (name != null && name == "favorite")
                    {
                        IsFavorite = Convert.ToBoolean(attribute.Value);
                        continue;
                    }
                    Logger.Warning("unhandled display property attribute {0} in character file '{1}'", attribute.Name, _filepath);
                }
                foreach (XmlNode childNode in xmlElement2.ChildNodes)
                {
                    string innerText = childNode.InnerText;
                    switch (childNode.Name)
                    {
                        case "name":
                            DisplayName = innerText;
                            break;
                        case "race":
                            DisplayRace = innerText;
                            break;
                        case "class":
                            DisplayClass = innerText;
                            break;
                        case "archetype":
                            DisplayArchetype = innerText;
                            break;
                        case "background":
                            DisplayBackground = innerText;
                            break;
                        case "level":
                            DisplayLevel = innerText;
                            break;
                        case "portrait":
                            try
                            {
                                DisplayPortraitFilePath = childNode["local"].GetInnerText();
                                DisplayPortraitBase64 = childNode["base64"].GetInnerText();
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "InitializeDisplayPropertiesFromFilePath");
                            }
                            break;
                        default:
                            Logger.Warning("unhandled display property element {0} in character file '{1}'", childNode.Name, _filepath);
                            break;
                    }
                }
                FileInfo fileInfo = new FileInfo(_filepath);
                FileName = fileInfo.Name;
                SaveRemotePortrait();
                IsInitialized = true;
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "InitializeDisplayPropertiesFromFilePath");
                MessageDialogService.ShowException(ex2);
            }
        }

        public void UpdateGroupName(string newGroup)
        {
            try
            {
                _document = new XmlDocument();
                _document.Load(_filepath);
                XmlNode xmlNode = _document.DocumentElement.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("information"));
                if (xmlNode == null)
                {
                    xmlNode = _document.DocumentElement.AppendChild(_document.CreateNode(XmlNodeType.Element, "information", null));
                    xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "group", null));
                }
                if (xmlNode.ContainsChildNode("group"))
                {
                    xmlNode.GetChildNode("group").InnerText = newGroup;
                }
                _document.Save(_filepath);
                CollectionGroupName = newGroup;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "UpdateGroupName");
                MessageDialogService.ShowException(ex);
            }
        }

        public bool Save()
        {
            return Save(CharacterManager.Current.Character);
        }

        public bool Save(Character character)
        {
            _document = new XmlDocument();
            XmlNode xmlNode = _document.AppendChild(_document.CreateNode(XmlNodeType.Element, "character", null));
            Dictionary<string, string> attributesDictionary = new Dictionary<string, string>
        {
            {
                "version",
                Resources.ApplicationVersion
            },
            { "preview", "false" }
        };
            xmlNode.AppendAttributes(attributesDictionary);
            xmlNode.AppendChild(_document.CreateComment(" Aurora - https://www.aurorabuilder.com "));
            xmlNode.AppendChild(_document.CreateComment(" information "));
            WriteInformationNode(xmlNode);
            xmlNode.AppendChild(_document.CreateComment(" display data "));
            xmlNode.AppendChild(CreateDisplayPropertiesNode(character));
            xmlNode.AppendChild(_document.CreateComment(" build data "));
            xmlNode.AppendChild(CreateBuildNode(character));
            xmlNode.AppendChild(_document.CreateComment(" restricted sources "));
            xmlNode.AppendChild(CreateRestrictedSourcesNode());
            string.IsNullOrWhiteSpace(_filepath);
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(_filepath, Encoding.UTF8))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.IndentChar = '\t';
                xmlTextWriter.Indentation = 1;
                _document.Save(xmlTextWriter);
                return true;
            }
        }

        public async Task<LoadResult> Load()
        {
            return await Load(_filepath);
        }

        public async Task<LoadResult> Load(string filepath)
        {
            int currentProgress = 0;
            int progressMax = 8;
            await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
            await SendCharacterLoadingScreenStatusUpdate("loading character file");
            FilePath = filepath;
            InitializeDisplayPropertiesFromFilePath();
            progressMax += int.Parse(DisplayLevel);
            currentProgress++;
            await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
            if (!IsInitialized)
            {
                return new LoadResult(success: false, "character file not initialized");
            }
            if (_document.DocumentElement == null)
            {
                throw new NullReferenceException("DocumentElement not found on " + _filepath);
            }
            ReadInformationNode(_document.DocumentElement);
            XmlNode buildNode = _document.DocumentElement["build"];
            if (buildNode == null)
            {
                throw new NullReferenceException("buildNode not found on " + _filepath);
            }
            string attributeValue = _document.DocumentElement.GetAttributeValue("version");
            _document.DocumentElement.GetAttributeAsBoolean("preview");
            new Version(attributeValue);
            Stopwatch sw = Stopwatch.StartNew();
            Character character = await CharacterManager.Current.New(initializeFirstLevel: false);
            CharacterManager.Current.Status.IsLoaded = false;
            currentProgress++;
            await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
            XmlNode inputNode = buildNode["input"];
            if (inputNode == null)
            {
                throw new NullReferenceException("inputNode not found on " + _filepath);
            }
            character.Name = inputNode["name"].GetInnerText();
            character.PlayerName = inputNode["player-name"].GetInnerText();
            character.Gender = inputNode["gender"].GetInnerText();
            character.Experience = (int.TryParse(inputNode["experience"].GetInnerText(), out var result) ? result : 0);
            character.Backstory = inputNode["backstory"].GetInnerText();
            XmlNode xmlNode = inputNode["currency"];
            if (xmlNode == null)
            {
                throw new NullReferenceException("currencyNode not found on " + _filepath);
            }
            character.Inventory.Coins.Set(int.Parse(xmlNode["copper"].GetInnerText()), int.Parse(xmlNode["silver"].GetInnerText()), int.Parse(xmlNode["electrum"].GetInnerText()), int.Parse(xmlNode["gold"].GetInnerText()), int.Parse(xmlNode["platinum"].GetInnerText()));
            character.Inventory.Equipment = xmlNode["equipment"].GetInnerText();
            character.Inventory.Treasure = xmlNode["treasure"].GetInnerText();
            XmlNode xmlNode2 = inputNode["organization"];
            if (xmlNode2 == null)
            {
                throw new NullReferenceException("organizationNode not found on " + _filepath);
            }
            character.OrganisationName = xmlNode2["name"].GetInnerText();
            character.OrganisationSymbol = xmlNode2["symbol"].GetInnerText();
            character.Allies = xmlNode2["allies"].GetInnerText();
            XmlNode xmlNode3 = inputNode["additional-features"];
            if (xmlNode3 == null)
            {
                throw new NullReferenceException("featuresNode not found on " + _filepath);
            }
            character.AdditionalFeatures = xmlNode3.GetInnerText();
            ReadCharacterNotesNode(inputNode, character);
            ReadQuestItemsNode(inputNode, character);
            XmlNode xmlNode4 = buildNode["appearance"];
            if (xmlNode4 == null)
            {
                throw new NullReferenceException("appearanceNode not found on " + _filepath);
            }
            ReadAppearanceNode(xmlNode4, character);
            XmlNode xmlNode5 = buildNode["abilities"];
            if (xmlNode5 == null)
            {
                throw new NullReferenceException("abilitiesNode not found on " + _filepath);
            }
            AbilitiesCollection abilities = character.Abilities;
            abilities.Strength.BaseScore = Convert.ToInt32(xmlNode5[abilities.Strength.Name.ToLowerInvariant()].GetInnerText());
            abilities.Dexterity.BaseScore = Convert.ToInt32(xmlNode5[abilities.Dexterity.Name.ToLowerInvariant()].GetInnerText());
            abilities.Constitution.BaseScore = Convert.ToInt32(xmlNode5[abilities.Constitution.Name.ToLowerInvariant()].GetInnerText());
            if (xmlNode5.ContainsChildNode(abilities.Intelligence.Name.ToLower()))
            {
                abilities.Intelligence.BaseScore = Convert.ToInt32(xmlNode5[abilities.Intelligence.Name.ToLower()].GetInnerText());
            }
            else
            {
                abilities.Intelligence.BaseScore = Convert.ToInt32(xmlNode5[abilities.Intelligence.Name.ToLowerInvariant()].GetInnerText());
            }
            abilities.Wisdom.BaseScore = Convert.ToInt32(xmlNode5[abilities.Wisdom.Name.ToLowerInvariant()].GetInnerText());
            abilities.Charisma.BaseScore = Convert.ToInt32(xmlNode5[abilities.Charisma.Name.ToLowerInvariant()].GetInnerText());
            int num = abilities.CalculateAvailablePoints();
            int num2 = Convert.ToInt32(xmlNode5.GetAttributeValue("available-points"));
            if (num2 != num)
            {
                Logger.Warning("availablePoints ({0}) differs from the calculatedAvailablePoints ({1})", num2, num);
            }
            XmlElement xmlElement = _document.DocumentElement["sources"];
            XmlNode sourcesNode = xmlElement;
            if (sourcesNode != null)
            {
                currentProgress++;
                await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
                await SendCharacterLoadingScreenStatusUpdate("Setting Source Restrictions");
                ReadRestrictedSourcesNodes(sourcesNode);
            }
            else
            {
                Logger.Warning("no sources in " + FileName);
            }
            currentProgress++;
            await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
            await SendCharacterLoadingScreenStatusUpdate("preparing character");
            XmlNode elementsNode = buildNode["elements"];
            if (elementsNode == null)
            {
                throw new NullReferenceException("elementsNode not found on " + _filepath);
            }
            int.Parse(elementsNode.GetAttributeValue("level-count"));
            int optionCount = 0;
            currentProgress++;
            await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
            foreach (XmlNode elementNode in elementsNode.ChildNodes)
            {
                string type = elementNode.GetAttributeValue("type");
                if (type == "Option")
                {
                    optionCount++;
                    await SendCharacterLoadingScreenStatusUpdate("applying character options");
                    string attributeValue2 = elementNode.GetAttributeValue("id");
                    ElementBase element = DataManager.Current.ElementsCollection.GetElement(attributeValue2);
                    if (element != null)
                    {
                        CharacterManager.Current.RegisterElement(element);
                    }
                    else
                    {
                        Logger.Warning("unable to load the option with the id: " + attributeValue2);
                    }
                }
                if (!(type == "Level"))
                {
                    continue;
                }
                string attributeValue3 = elementNode.GetAttributeValue("id");
                LevelElement element2 = DataManager.Current.ElementsCollection.GetElement(attributeValue3).AsElement<LevelElement>();
                await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
                await SendCharacterLoadingScreenStatusUpdate("applying character elements");
                if (element2.Level == 1)
                {
                    CharacterManager.Current.RegisterElement(element2);
                    await ReadChildElements(elementNode, element2);
                }
                else
                {
                    bool flag = false;
                    if (elementNode.ContainsAttribute("multiclass"))
                    {
                        flag = Convert.ToBoolean(elementNode.GetAttributeValue("multiclass"));
                    }
                    if (flag)
                    {
                        bool flag2 = false;
                        if (elementNode.ContainsAttribute("starting"))
                        {
                            flag2 = Convert.ToBoolean(elementNode.GetAttributeValue("starting"));
                        }
                        if (flag2)
                        {
                            CharacterManager.Current.NewMulticlass();
                            await ReadChildElements(elementNode, element2);
                        }
                        else
                        {
                            string attributeValue4 = elementNode.GetAttributeValue("class");
                            Multiclass element3 = DataManager.Current.ElementsCollection.GetElement(attributeValue4).AsElement<Multiclass>();
                            CharacterManager.Current.LevelUpMulti(element3);
                        }
                    }
                    else
                    {
                        CharacterManager.Current.LevelUpMain();
                    }
                }
                foreach (XmlNode childNode in elementsNode.ChildNodes)
                {
                    if (childNode.GetAttributeValue("type") == "Level" || type == "Option")
                    {
                        string attributeValue5 = childNode.GetAttributeValue("id");
                        ElementBase element4 = DataManager.Current.ElementsCollection.GetElement(attributeValue5);
                        await ReadChildElements(childNode, element4);
                    }
                }
                currentProgress++;
                await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
            }
            foreach (XmlNode elementNode in elementsNode.ChildNodes)
            {
                string attributeValue6 = elementNode.GetAttributeValue("type");
                if (!(attributeValue6 == "Level") && !(attributeValue6 == "Option"))
                {
                    continue;
                }
                string attributeValue7 = elementNode.GetAttributeValue("id");
                ElementBase element5 = DataManager.Current.ElementsCollection.GetElement(attributeValue7);
                await ReadChildElements(elementNode, element5);
                if (elementNode.ContainsAttribute("rndhp"))
                {
                    int[] randomHitPointsArray = elementNode.GetAttributeValue("rndhp").Split(',').Select(int.Parse)
                        .ToArray();
                    CharacterManager.Current.ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.LevelElements.Contains(element5))?.SetRandomHitPointsArray(randomHitPointsArray);
                }
            }
            character.Experience = (int.TryParse(inputNode["experience"].GetInnerText(), out var result2) ? result2 : 0);
            ReadDefensesNode(buildNode, character);
            ReadCompanionNode(buildNode, character);
            await SendCharacterLoadingScreenStatusUpdate("Writing Background");
            if (!character.BackgroundStory.EqualsOriginalContent(inputNode["backstory"].GetInnerText()))
            {
                character.BackgroundStory.Content = inputNode["backstory"].GetInnerText();
            }
            XmlNode xmlNode7 = inputNode["background"];
            if (xmlNode7 != null)
            {
                ParseBackgroundInput(xmlNode7, character);
            }
            ParseBackgroundCharacteristicsInput(inputNode, character);
            await SendCharacterLoadingScreenStatusUpdate("Scribing Spells");
            XmlNode xmlNode8 = buildNode["magic"];
            if (xmlNode8 != null)
            {
                IEnumerable<XmlNode> enumerable = from x in xmlNode8.NonCommentChildNodes()
                                                  where x.Name.Equals("spellcasting")
                                                  select x;
                List<SpellcastingInformation> source = CharacterManager.Current.GetSpellcastingInformations().ToList();
                foreach (XmlNode item2 in enumerable)
                {
                    string name = item2.GetAttributeValue("name");
                    string source2 = item2.GetAttributeValue("source");
                    SpellcastingInformation spellcastingInformation = source.FirstOrDefault((SpellcastingInformation x) => x.Name.Equals(name) && x.ElementHeader.Id.Equals(source2));
                    if (spellcastingInformation == null || !spellcastingInformation.Prepare)
                    {
                        continue;
                    }
                    XmlNode xmlNode9 = item2.NonCommentChildNodes().FirstOrDefault((XmlNode x) => x.Name.Equals("spells"));
                    if (xmlNode9 == null)
                    {
                        continue;
                    }
                    foreach (XmlNode item3 in from x in xmlNode9.NonCommentChildNodes()
                                              where x.Name.Equals("spell") && x.ContainsAttribute("prepared")
                                              select x)
                    {
                        SpellcastingSectionHandler.Current.SetPrepareSpell(spellcastingInformation, item3.GetAttributeValue("id"));
                    }
                }
                xmlNode8.NonCommentChildNodes().FirstOrDefault((XmlNode x) => x.Name.Equals("additional"));
            }
            CharacterManager.Current.ReprocessCharacter();
            await SendCharacterLoadingScreenStatusUpdate("preparing inventory");
            XmlNode xmlNode10 = buildNode["equipment"];
            if (xmlNode10 != null)
            {
                int count = xmlNode10.ChildNodes.Count;
                int num3 = 0;
                foreach (XmlNode item4 in from XmlNode x in xmlNode10.ChildNodes
                                          where x.Name.Equals("storage")
                                          select x)
                {
                    string attributeValue8 = item4.GetAttributeValue("name");
                    if (num3 == 0)
                    {
                        character.Inventory.StoredItems1.Name = attributeValue8;
                    }
                    if (num3 == 1)
                    {
                        character.Inventory.StoredItems2.Name = attributeValue8;
                    }
                    num3++;
                }
                foreach (XmlNode item5 in from XmlNode x in xmlNode10.ChildNodes
                                          where x.Name.Equals("item")
                                          select x)
                {
                    string attributeValue9 = item5.GetAttributeValue("id");
                    ElementBase element6 = DataManager.Current.ElementsCollection.GetElement(attributeValue9);
                    if (element6 == null && attributeValue9.Contains("ID_WOTC_ITEM"))
                    {
                        element6 = DataManager.Current.ElementsCollection.GetElement(attributeValue9.Replace("ID_WOTC_ITEM", "ID_WOTC_PHB_ITEM"));
                    }
                    if (element6 == null && attributeValue9.Contains("ID_WOTC_WEAPON"))
                    {
                        element6 = DataManager.Current.ElementsCollection.GetElement(attributeValue9.Replace("ID_WOTC_WEAPON", "ID_WOTC_PHB_WEAPON"));
                    }
                    if (element6 == null)
                    {
                        Logger.Warning("unable to add " + item5.GetAttributeValue("name"));
                        continue;
                    }
                    ElementBase elementBase = null;
                    XmlNode xmlNode11 = item5.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("items"));
                    if (xmlNode11 != null)
                    {
                        XmlNode xmlNode12 = xmlNode11.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("adorner"));
                        if (xmlNode12 != null)
                        {
                            elementBase = DataManager.Current.ElementsCollection.GetElement(xmlNode12.GetAttributeValue("id"));
                        }
                    }
                    RefactoredEquipmentItem refactoredEquipmentItem = new RefactoredEquipmentItem(element6 as Item, elementBase as Item);
                    if (item5.ContainsAttribute("identifier"))
                    {
                        refactoredEquipmentItem.SetIdentifier(item5.GetAttributeValue("identifier"));
                    }
                    if (item5.ContainsAttribute("amount"))
                    {
                        refactoredEquipmentItem.Amount = Convert.ToInt32(item5.GetAttributeValue("amount"));
                    }
                    XmlNode xmlNode13 = item5.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("equipped"));
                    if (xmlNode13 != null)
                    {
                        refactoredEquipmentItem.IsEquipped = Convert.ToBoolean(xmlNode13.InnerText);
                        if (xmlNode13.ContainsAttribute("location"))
                        {
                            refactoredEquipmentItem.EquippedLocation = xmlNode13.GetAttributeValue("location");
                        }
                    }
                    XmlNode xmlNode14 = item5.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("attunement"));
                    if (xmlNode14 != null)
                    {
                        refactoredEquipmentItem.IsAttuned = Convert.ToBoolean(xmlNode14.InnerText);
                    }
                    XmlNode xmlNode15 = item5.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("details"));
                    if (xmlNode15 != null)
                    {
                        if (xmlNode15.ContainsAttribute("card"))
                        {
                            refactoredEquipmentItem.ShowCard = Convert.ToBoolean(xmlNode15.GetAttributeValue("card"));
                        }
                        refactoredEquipmentItem.AlternativeName = xmlNode15["name"].GetInnerText();
                        refactoredEquipmentItem.Notes = xmlNode15["notes"].GetInnerText();
                    }
                    if (item5.ContainsAttribute("aquired"))
                    {
                        refactoredEquipmentItem.AquisitionParent = new ElementHeader(item5.GetAttributeValue("aquired"), "", "", "");
                        refactoredEquipmentItem.HasAquisitionParent = true;
                    }
                    if (item5.ContainsAttribute("hidden"))
                    {
                        refactoredEquipmentItem.IncludeInEquipmentPageInventory = !item5.GetAttributeAsBoolean("hidden");
                    }
                    if (item5.ContainsAttribute("sidebar"))
                    {
                        refactoredEquipmentItem.IncludeInEquipmentPageDescriptionSidebar = item5.GetAttributeAsBoolean("sidebar");
                    }
                    else
                    {
                        refactoredEquipmentItem.IncludeInEquipmentPageDescriptionSidebar = false;
                    }
                    XmlNode xmlNode16 = item5.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("storage"));
                    if (xmlNode16 != null)
                    {
                        string innerText = xmlNode16.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("location")).GetInnerText();
                        InventoryStorage storage = character.Inventory.GetStorage(innerText);
                        refactoredEquipmentItem.Store(storage);
                    }
                    character.Inventory.Items.Add(refactoredEquipmentItem);
                    num3++;
                }
                if (count != num3)
                {
                    Logger.Warning("======= NOT ALL EQUIPMENT LOADED");
                }
            }
            XmlNode xmlNode17 = inputNode["attacks"];
            if (xmlNode17 != null)
            {
                ParseAttacksSection(xmlNode17, character);
            }
            int index = 0;
            foreach (XmlNode childNode2 in elementsNode.ChildNodes)
            {
                switch (childNode2.GetAttributeValue("type"))
                {
                    case "Weapon":
                    case "Armor":
                    case "Item":
                    case "Magic Item":
                        {
                            string id = childNode2.GetAttributeValue("id");
                            List<RefactoredEquipmentItem> items = character.Inventory.Items.Where((RefactoredEquipmentItem x) => x.Item.Id.Equals(id)).ToList();
                            if (items.Count > 0)
                            {
                                if (items.Count == 1 && index > 0)
                                {
                                    index = 0;
                                }
                                Item item = items[index].Item;
                                await ReadChildElements(childNode2, item);
                                index++;
                                if (items.Count <= index)
                                {
                                    index = 0;
                                }
                            }
                            else
                            {
                                ElementBase element7 = DataManager.Current.ElementsCollection.GetElement(id);
                                await ReadChildElements(childNode2, element7);
                                index = 0;
                            }
                            break;
                        }
                }
            }
            currentProgress++;
            await SendCharacterLoadingScreenProgressUpdate(currentProgress.IsPercetageOf(progressMax));
            await SendCharacterLoadingScreenStatusUpdate("performing validation");
            XmlNode xmlNode19 = buildNode["sum"];
            if (xmlNode19 == null)
            {
                throw new NullReferenceException("sumNode not found on " + _filepath);
            }
            int elementSaveCount = Convert.ToInt32(xmlNode19.GetAttributeValue("element-count"));
            int count2 = CharacterManager.Current.GetElements().Count;
            if (elementSaveCount != count2)
            {
                int difference = elementSaveCount - count2;
                Logger.Warning($"the sum of the saved elements ({elementSaveCount}) differs from the sum that is loaded ({count2})");
                bool validCount = false;
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(250);
                    int count3 = CharacterManager.Current.GetElements().Count;
                    if (elementSaveCount == count3)
                    {
                        validCount = true;
                        break;
                    }
                    Logger.Info("waiting for trailing elements to be registered ");
                }
                if (validCount)
                {
                    sw.Stop();
                    Logger.Warning($"{character} loaded in {sw.ElapsedMilliseconds}ms");
                }
                else
                {
                    sw.Stop();
                    Logger.Warning($"{character} loaded in {sw.ElapsedMilliseconds}ms without all elements ({difference})");
                }
                if (difference > 0)
                {
                    await SendCharacterLoadingScreenProgressUpdate(100);
                    await SendCharacterLoadingScreenStatusUpdate(WebUtility.HtmlDecode("&#xE10A;"), success: false);
                    return new LoadResult(success: false, string.Format("character not fully prepared, {0} item{1} could not be set", difference, (difference > 1) ? "(s)" : ""));
                }
                await SendCharacterLoadingScreenProgressUpdate(100);
                await SendCharacterLoadingScreenStatusUpdate(WebUtility.HtmlDecode("&#xE10B;"));
                return new LoadResult(success: true);
            }
            await SendCharacterLoadingScreenProgressUpdate(100);
            await SendCharacterLoadingScreenStatusUpdate(WebUtility.HtmlDecode("&#xE10B;"));
            sw.Stop();
            Logger.Warning($"{character} loaded in {sw.ElapsedMilliseconds}ms");
            return new LoadResult(success: true, WebUtility.HtmlDecode("&#xE10B;"));
        }

        private async Task SendCharacterLoadingScreenStatusUpdate(string message, bool success = true)
        {
            ApplicationManager.Current.EventAggregator.Send(new CharacterLoadingSliderStatusUpdateEvent(message, success));
            await Task.Delay(50);
        }

        private async Task SendCharacterLoadingScreenProgressUpdate(int progress)
        {
            ApplicationManager.Current.EventAggregator.Send(new CharacterLoadingSliderProgressEvent(progress));
            await Task.Delay(50);
        }

        private void LoadSpellOverride(ObservableSpell spellslot, XmlNode node)
        {
            spellslot.Name = node.GetInnerText();
            spellslot.IsPrepared = Convert.ToBoolean(node.GetAttributeValue("prepared"));
        }

        private XmlNode CreateDisplayPropertiesNode(Character character)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "display-properties", null);
            Dictionary<string, string> attributesDictionary = new Dictionary<string, string> {
        {
            "favorite",
            IsFavorite.ToString().ToLowerInvariant()
        } };
            AppendAttributes(xmlNode, attributesDictionary);
            XmlNode xmlNode2 = _document.CreateNode(XmlNodeType.Element, "name", null);
            xmlNode2.InnerText = character.Name;
            XmlNode xmlNode3 = _document.CreateNode(XmlNodeType.Element, "race", null);
            xmlNode3.InnerText = character.Race;
            XmlNode xmlNode4 = _document.CreateNode(XmlNodeType.Element, "class", null);
            xmlNode4.InnerText = character.Class;
            XmlNode xmlNode5 = _document.CreateNode(XmlNodeType.Element, "archetype", null);
            xmlNode5.InnerText = character.Archetype;
            XmlNode xmlNode6 = _document.CreateNode(XmlNodeType.Element, "background", null);
            xmlNode6.InnerText = character.Background;
            XmlNode xmlNode7 = _document.CreateNode(XmlNodeType.Element, "level", null);
            xmlNode7.InnerText = character.Level.ToString();
            XmlNode xmlNode8 = _document.CreateNode(XmlNodeType.Element, "portrait", null);
            xmlNode8.AppendChild(_document.CreateElement("companion")).InnerText = character.Companion.Portrait.ToString();
            xmlNode8.AppendChild(_document.CreateElement("local")).InnerText = character.PortraitFilename;
            Path.GetFileName(character.PortraitFilename);
            if (File.Exists(character.PortraitFilename))
            {
                XmlNode xmlNode9 = _document.CreateNode(XmlNodeType.Element, "base64", null);
                string data = Convert.ToBase64String(File.ReadAllBytes(character.PortraitFilename));
                XmlCDataSection newChild = _document.CreateCDataSection(data);
                xmlNode9.AppendChild(newChild);
                xmlNode8.AppendChild(xmlNode9);
            }
            xmlNode.AppendChild(xmlNode2);
            xmlNode.AppendChild(xmlNode3);
            xmlNode.AppendChild(xmlNode4);
            xmlNode.AppendChild(xmlNode5);
            xmlNode.AppendChild(xmlNode6);
            xmlNode.AppendChild(xmlNode7);
            xmlNode.AppendChild(xmlNode8);
            return xmlNode;
        }

        private XmlNode CreateBuildNode(Character character)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "build", null);
            xmlNode.AppendChild(CreateInputNode(character));
            xmlNode.AppendChild(CreateAppearanceNode(character));
            xmlNode.AppendChild(CreateAbilitiesNode(character));
            XmlNode xmlNode2 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "elements", null));
            AppendAttribute(xmlNode2, "level-count", CharacterManager.Current.Elements.Count((ElementBase x) => x.Type == "Level").ToString());
            AppendAttribute(xmlNode2, "registered-count", CharacterManager.Current.Elements.Count.ToString());
            foreach (ElementBase item in CharacterManager.Current.Elements.Where((ElementBase x) => x.Type.Equals("Option")))
            {
                XmlNode xmlNode3 = _document.CreateNode(XmlNodeType.Element, "element", null);
                Dictionary<string, string> attributesDictionary = new Dictionary<string, string>
            {
                { "type", item.Type },
                { "name", item.Name },
                { "id", item.Id }
            };
                AppendAttributes(xmlNode3, attributesDictionary);
                xmlNode2.AppendChild(xmlNode3);
                CreateRuleNodes(item, xmlNode3);
            }
            foreach (ElementBase item2 in CharacterManager.Current.Elements.Where((ElementBase e) => e.Type == "Level"))
            {
                XmlNode xmlNode4 = _document.CreateNode(XmlNodeType.Element, "element", null);
                Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "type", item2.Type },
                { "name", item2.Name },
                { "id", item2.Id }
            };
                foreach (ClassProgressionManager classProgressionManager in CharacterManager.Current.ClassProgressionManagers)
                {
                    LevelElement levelElement = item2.AsElement<LevelElement>();
                    _ = classProgressionManager.IsMainClass;
                    if (!classProgressionManager.LevelElements.Contains(item2))
                    {
                        continue;
                    }
                    if (classProgressionManager.IsMulticlass)
                    {
                        dictionary.Add("multiclass", "true");
                        if (levelElement.Level == classProgressionManager.StartingLevel)
                        {
                            dictionary.Add("starting", "true");
                            dictionary.Add("rndhp", string.Join(",", classProgressionManager.GetRandomHitPointsArrayAsync()));
                        }
                        dictionary.Add("class", classProgressionManager.ClassElement.Id);
                    }
                    else if (classProgressionManager.IsMainClass && levelElement.Level == classProgressionManager.StartingLevel)
                    {
                        dictionary.Add("rndhp", string.Join(",", classProgressionManager.GetRandomHitPointsArrayAsync()));
                    }
                }
                AppendAttributes(xmlNode4, dictionary);
                xmlNode2.AppendChild(xmlNode4);
                CreateRuleNodes(item2, xmlNode4);
            }
            foreach (ElementBase item3 in CharacterManager.Current.Elements.Where((ElementBase x) => x.Type.Equals("Weapon") || x.Type.Equals("Armor") || x.Type.Equals("Item") || x.Type.Equals("Magic Item")))
            {
                if (item3.ContainsSelectRules || item3.ContainsGrantRules)
                {
                    XmlNode xmlNode5 = _document.CreateNode(XmlNodeType.Element, "element", null);
                    Dictionary<string, string> attributesDictionary2 = new Dictionary<string, string>
                {
                    { "type", item3.Type },
                    { "name", item3.Name },
                    { "id", item3.Id }
                };
                    xmlNode5.AppendAttributes(attributesDictionary2);
                    xmlNode2.AppendChild(xmlNode5);
                    CreateRuleNodes(item3, xmlNode5);
                }
            }
            WriteDefensesNode(xmlNode, character);
            WriteCompanionNode(xmlNode, character);
            xmlNode.AppendChild(CreateEquipmentNode());
            xmlNode.AppendChild(CreateSumNode());
            xmlNode.AppendChild(CreateMagicNode());
            return xmlNode;
        }

        private XmlNode CreateRestrictedSourcesNode()
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "sources", null);
            XmlNode parentNode = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "restricted", null));
            foreach (SourceItem restrictedSource in CharacterManager.Current.SourcesManager.RestrictedSources)
            {
                parentNode.AppendChild("source", restrictedSource.Source.Name).AppendAttribute("id", restrictedSource.Source.Id);
            }
            foreach (string restrictedElementId in CharacterManager.Current.SourcesManager.GetRestrictedElementIds())
            {
                parentNode.AppendChild("element", restrictedElementId);
            }
            return xmlNode;
        }

        private void ReadRestrictedSourcesNodes(XmlNode sourcesNode)
        {
            XmlNode xmlNode = sourcesNode["restricted"];
            if (xmlNode == null)
            {
                return;
            }
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            foreach (XmlNode item in xmlNode.ChildNodes.Cast<XmlNode>())
            {
                if (item.NodeType == XmlNodeType.Element)
                {
                    switch (item.Name)
                    {
                        case "source":
                            list.Add(item.GetAttributeValue("id"));
                            break;
                        case "element":
                            list2.Add(item.GetInnerText());
                            break;
                    }
                }
            }
            CharacterManager.Current.SourcesManager.Load(list);
            CharacterManager.Current.SourcesManager.ApplyRestrictions();
        }

        private XmlNode CreateInputNode(Character character)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "input", null);
            XmlNode xmlNode2 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "name", null));
            XmlNode xmlNode3 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "gender", null));
            XmlNode xmlNode4 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "player-name", null));
            XmlNode xmlNode5 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "experience", null));
            xmlNode2.InnerText = character.Name;
            xmlNode3.InnerText = character.Gender;
            xmlNode4.InnerText = character.PlayerName;
            xmlNode5.InnerText = character.Experience.ToString();
            XmlNode xmlNode6 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "attacks", null));
            xmlNode6.AppendChild("description", character.AttacksSection.AttacksAndSpellcasting, isCData: true);
            foreach (AttackSectionItem item in character.AttacksSection.Items)
            {
                XmlNode parentNode = xmlNode6.AppendChild(_document.CreateNode(XmlNodeType.Element, "attack", null));
                parentNode.AppendAttribute("identifier", item.EquipmentItem?.Identifier ?? "");
                parentNode.AppendAttribute("name", item.Name.Content);
                parentNode.AppendAttribute("range", item.Range.Content);
                parentNode.AppendAttribute("attack", item.Attack.Content);
                parentNode.AppendAttribute("damage", item.Damage.Content);
                parentNode.AppendAttribute("displayed", item.IsDisplayed ? "true" : "false");
                if (item.LinkedAbility != null)
                {
                    parentNode.AppendAttribute("ability", item.LinkedAbility?.Name);
                }
                parentNode.AppendChild("description", item.Description.Content, isCData: true);
            }
            xmlNode.AppendChild("backstory", character.BackgroundStory.Content, isCData: true);
            xmlNode.AppendChild("background-trinket", character.Trinket.Content);
            xmlNode.AppendChild("background-traits", character.FillableBackgroundCharacteristics.Traits.Content);
            xmlNode.AppendChild("background-ideals", character.FillableBackgroundCharacteristics.Ideals.Content);
            xmlNode.AppendChild("background-bonds", character.FillableBackgroundCharacteristics.Bonds.Content);
            xmlNode.AppendChild("background-flaws", character.FillableBackgroundCharacteristics.Flaws.Content);
            XmlNode parentNode2 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "background", null)).AppendChild(_document.CreateNode(XmlNodeType.Element, "feature", null));
            parentNode2.AppendAttribute("name", character.BackgroundFeatureName.Content);
            parentNode2.AppendChild("description", character.BackgroundFeatureDescription.Content, isCData: true);
            XmlNode parentNode3 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "organization", null));
            parentNode3.AppendChild("name", character.OrganisationName);
            parentNode3.AppendChild("symbol", character.OrganisationSymbol);
            parentNode3.AppendChild("allies", character.Allies, isCData: true);
            xmlNode.AppendChild("additional-features", character.AdditionalFeatures, isCData: true);
            XmlNode parentNode4 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "currency", null));
            parentNode4.AppendChild("copper", character.Inventory.Coins.Copper.ToString());
            parentNode4.AppendChild("silver", character.Inventory.Coins.Silver.ToString());
            parentNode4.AppendChild("electrum", character.Inventory.Coins.Electrum.ToString());
            parentNode4.AppendChild("gold", character.Inventory.Coins.Gold.ToString());
            parentNode4.AppendChild("platinum", character.Inventory.Coins.Platinum.ToString());
            parentNode4.AppendChild("equipment", character.Inventory.Equipment, isCData: true);
            parentNode4.AppendChild("treasure", character.Inventory.Treasure, isCData: true);
            WriteCharacterNotesNode(xmlNode, character);
            WriteQuestItemsNode(xmlNode, character);
            return xmlNode;
        }

        private XmlNode CreateAppearanceNode(Character character)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "appearance", null);
            xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "portrait", null)).InnerText = character.PortraitFilename;
            XmlNode xmlNode2 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "age", null));
            XmlNode xmlNode3 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "height", null));
            XmlNode xmlNode4 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "weight", null));
            XmlNode xmlNode5 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "eyes", null));
            XmlNode xmlNode6 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "skin", null));
            XmlNode xmlNode7 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "hair", null));
            xmlNode2.InnerText = character.AgeField.Content;
            xmlNode3.InnerText = character.HeightField.Content;
            xmlNode4.InnerText = character.WeightField.Content;
            xmlNode5.InnerText = character.Eyes;
            xmlNode6.InnerText = character.Skin;
            xmlNode7.InnerText = character.Hair;
            return xmlNode;
        }

        private XmlNode CreateAbilitiesNode(Character character)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "abilities", null);
            XmlNode xmlNode2 = _document.CreateNode(XmlNodeType.Element, character.Abilities.Strength.Name.ToLowerInvariant(), null);
            xmlNode2.InnerText = character.Abilities.Strength.BaseScore.ToString();
            XmlNode xmlNode3 = _document.CreateNode(XmlNodeType.Element, character.Abilities.Dexterity.Name.ToLowerInvariant(), null);
            xmlNode3.InnerText = character.Abilities.Dexterity.BaseScore.ToString();
            XmlNode xmlNode4 = _document.CreateNode(XmlNodeType.Element, character.Abilities.Constitution.Name.ToLowerInvariant(), null);
            xmlNode4.InnerText = character.Abilities.Constitution.BaseScore.ToString();
            XmlNode xmlNode5 = _document.CreateNode(XmlNodeType.Element, character.Abilities.Intelligence.Name.ToLowerInvariant(), null);
            xmlNode5.InnerText = character.Abilities.Intelligence.BaseScore.ToString();
            XmlNode xmlNode6 = _document.CreateNode(XmlNodeType.Element, character.Abilities.Wisdom.Name.ToLowerInvariant(), null);
            xmlNode6.InnerText = character.Abilities.Wisdom.BaseScore.ToString();
            XmlNode xmlNode7 = _document.CreateNode(XmlNodeType.Element, character.Abilities.Charisma.Name.ToLowerInvariant(), null);
            xmlNode7.InnerText = character.Abilities.Charisma.BaseScore.ToString();
            xmlNode.AppendChild(xmlNode2);
            xmlNode.AppendChild(xmlNode3);
            xmlNode.AppendChild(xmlNode4);
            xmlNode.AppendChild(xmlNode5);
            xmlNode.AppendChild(xmlNode6);
            xmlNode.AppendChild(xmlNode7);
            AppendAttribute(xmlNode, "available-points", character.Abilities.AvailablePoints.ToString());
            return xmlNode;
        }

        [Obsolete]
        private XmlNode CreateSpellsNode(Character character)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "spells", null);
            AppendAttribute(xmlNode, "class", character.SpellcastingCollection.SpellcastingClass);
            AppendAttribute(xmlNode, "ability", character.SpellcastingCollection.SpellcastingAbility);
            AppendAttribute(xmlNode, "dc", character.SpellcastingCollection.SpellcastingDifficultyClass);
            AppendAttribute(xmlNode, "attack", character.SpellcastingCollection.SpellcastingAttackBonus);
            XmlNode xmlNode2 = _document.CreateNode(XmlNodeType.Element, "cantrips", null);
            XmlNode xmlNode3 = _document.CreateNode(XmlNodeType.Element, "spells-1st", null);
            XmlNode xmlNode4 = _document.CreateNode(XmlNodeType.Element, "spells-2nd", null);
            XmlNode xmlNode5 = _document.CreateNode(XmlNodeType.Element, "spells-3rd", null);
            XmlNode xmlNode6 = _document.CreateNode(XmlNodeType.Element, "spells-4th", null);
            XmlNode xmlNode7 = _document.CreateNode(XmlNodeType.Element, "spells-5th", null);
            XmlNode xmlNode8 = _document.CreateNode(XmlNodeType.Element, "spells-6th", null);
            XmlNode xmlNode9 = _document.CreateNode(XmlNodeType.Element, "spells-7th", null);
            XmlNode xmlNode10 = _document.CreateNode(XmlNodeType.Element, "spells-8th", null);
            XmlNode xmlNode11 = _document.CreateNode(XmlNodeType.Element, "spells-9th", null);
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot1));
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot2));
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot3));
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot4));
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot5));
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot6));
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot7));
            xmlNode2.AppendChild(CreateCantripNode(character.SpellcastingCollection.CantripSlot8));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot1));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot2));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot3));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot4));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot5));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot6));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot7));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot8));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot9));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot10));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot11));
            xmlNode3.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell1Slot12));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot1));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot2));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot3));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot4));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot5));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot6));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot7));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot8));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot9));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot10));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot11));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot12));
            xmlNode4.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell2Slot13));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot1));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot2));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot3));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot4));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot5));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot6));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot7));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot8));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot9));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot10));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot11));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot12));
            xmlNode5.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell3Slot13));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot1));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot2));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot3));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot4));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot5));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot6));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot7));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot8));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot9));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot10));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot11));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot12));
            xmlNode6.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell4Slot13));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot1));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot2));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot3));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot4));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot5));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot6));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot7));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot8));
            xmlNode7.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell5Slot9));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot1));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot2));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot3));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot4));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot5));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot6));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot7));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot8));
            xmlNode8.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell6Slot9));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot1));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot2));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot3));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot4));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot5));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot6));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot7));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot8));
            xmlNode9.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell7Slot9));
            xmlNode10.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell8Slot1));
            xmlNode10.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell8Slot2));
            xmlNode10.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell8Slot3));
            xmlNode10.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell8Slot4));
            xmlNode10.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell8Slot5));
            xmlNode10.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell8Slot6));
            xmlNode10.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell8Slot7));
            xmlNode11.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell9Slot1));
            xmlNode11.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell9Slot2));
            xmlNode11.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell9Slot3));
            xmlNode11.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell9Slot4));
            xmlNode11.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell9Slot5));
            xmlNode11.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell9Slot6));
            xmlNode11.AppendChild(CreateSpellNode(character.SpellcastingCollection.Spell9Slot7));
            AppendAttribute(xmlNode3, "slots", "0");
            AppendAttribute(xmlNode4, "slots", "0");
            AppendAttribute(xmlNode5, "slots", "0");
            AppendAttribute(xmlNode6, "slots", "0");
            AppendAttribute(xmlNode7, "slots", "0");
            AppendAttribute(xmlNode8, "slots", "0");
            AppendAttribute(xmlNode9, "slots", "0");
            AppendAttribute(xmlNode10, "slots", "0");
            AppendAttribute(xmlNode11, "slots", "0");
            xmlNode.AppendChild(xmlNode2);
            xmlNode.AppendChild(xmlNode3);
            xmlNode.AppendChild(xmlNode4);
            xmlNode.AppendChild(xmlNode5);
            xmlNode.AppendChild(xmlNode6);
            xmlNode.AppendChild(xmlNode7);
            xmlNode.AppendChild(xmlNode8);
            xmlNode.AppendChild(xmlNode9);
            xmlNode.AppendChild(xmlNode10);
            xmlNode.AppendChild(xmlNode11);
            return xmlNode;
        }

        [Obsolete]
        private XmlNode CreateCantripNode(string cantrip)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "cantrip", null);
            xmlNode.InnerText = (string.IsNullOrWhiteSpace(cantrip) ? " " : cantrip);
            return xmlNode;
        }

        [Obsolete]
        private XmlNode CreateSpellNode(ObservableSpell spell)
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "spell", null);
            AppendAttribute(xmlNode, "prepared", spell.IsPrepared ? "true" : "false");
            xmlNode.InnerText = (string.IsNullOrWhiteSpace(spell.Name) ? " " : spell.Name);
            return xmlNode;
        }

        private void CreateRuleNodes(ElementBase element, XmlNode parentNode)
        {
            foreach (SelectRule selectRule in element.GetSelectRules())
            {
                ProgressionManager progressManager = CharacterManager.Current.GetProgressManager(selectRule);
                if (progressManager == null)
                {
                    Logger.Warning($"not creating rules for selectionrule: {selectRule}, not registed with any manager req:({selectRule.Attributes.Requirements})");
                }
                else
                {
                    if (!selectRule.Attributes.MeetsLevelRequirement(progressManager.ProgressionLevel))
                    {
                        continue;
                    }
                    for (int i = 1; i <= selectRule.Attributes.Number; i++)
                    {
                        XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "element", null);
                        parentNode.AppendChild(xmlNode);
                        Dictionary<string, string> dictionary = new Dictionary<string, string>
                    {
                        {
                            "type",
                            selectRule.Attributes.Type
                        },
                        {
                            "name",
                            selectRule.Attributes.Name
                        },
                        {
                            "requiredLevel",
                            selectRule.Attributes.RequiredLevel.ToString()
                        }
                    };
                        if (selectRule.Attributes.MultipleNumberCount)
                        {
                            dictionary.Add("number", i.ToString());
                        }
                        dictionary.Add("checksum", CharacterFileVerification.GenerateCrC(selectRule, i));
                        if (selectRule.Attributes.IsList)
                        {
                            dictionary.Add("isList", "true");
                            SelectionRuleListItem selectionRuleListItem = SelectionRuleExpanderHandler.Current.GetRegisteredElement(selectRule, i) as SelectionRuleListItem;
                            dictionary.Add("registered", selectionRuleListItem?.ID.ToString());
                            if (selectionRuleListItem != null)
                            {
                                xmlNode.InnerText = selectionRuleListItem.Text;
                            }
                            AppendAttributes(xmlNode, dictionary);
                            continue;
                        }
                        try
                        {
                            int retrainLevel = SelectionRuleExpanderHandler.Current.GetRetrainLevel(selectRule, i);
                            if (retrainLevel > 0)
                            {
                                dictionary.Add("replaceLevel", retrainLevel.ToString());
                            }
                            ElementBase elementBase = SelectionRuleExpanderHandler.Current.GetRegisteredElement(selectRule, i) as ElementBase;
                            dictionary.Add("registered", elementBase?.Id);
                            AppendAttributes(xmlNode, dictionary);
                            if (elementBase != null)
                            {
                                CreateRuleNodes(elementBase, xmlNode);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warning($"error trying to get registered element for rule: {selectRule}");
                            Logger.Exception(ex, "CreateRuleNodes");
                            throw ex;
                        }
                    }
                }
            }
            try
            {
                foreach (ElementBase ruleElement in element.RuleElements)
                {
                    XmlNode xmlNode2 = _document.CreateNode(XmlNodeType.Element, "element", null);
                    parentNode.AppendChild(xmlNode2);
                    Dictionary<string, string> attributesDictionary = new Dictionary<string, string>
                {
                    { "type", ruleElement.Type },
                    { "name", ruleElement.Name },
                    { "id", ruleElement.Id }
                };
                    AppendAttributes(xmlNode2, attributesDictionary);
                    CreateRuleNodes(ruleElement, xmlNode2);
                }
            }
            catch (Exception ex2)
            {
                Logger.Warning($"error trying to get registered element for: {element}");
                Logger.Exception(ex2, "CreateRuleNodes");
            }
        }

        private XmlNode CreateEquipmentNode()
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "equipment", null);
            if (CharacterManager.Current.Character.Inventory.StoredItems1.IsInUse())
            {
                XmlNode xmlNode2 = _document.CreateNode(XmlNodeType.Element, "storage", null);
                xmlNode2.AppendAttribute("name", CharacterManager.Current.Character.Inventory.StoredItems1.Name);
                xmlNode.AppendChild(xmlNode2);
            }
            if (CharacterManager.Current.Character.Inventory.StoredItems2.IsInUse())
            {
                XmlNode xmlNode3 = _document.CreateNode(XmlNodeType.Element, "storage", null);
                xmlNode3.AppendAttribute("name", CharacterManager.Current.Character.Inventory.StoredItems2.Name);
                xmlNode.AppendChild(xmlNode3);
            }
            foreach (RefactoredEquipmentItem item in CharacterManager.Current.Character.Inventory.Items)
            {
                XmlNode xmlNode4 = _document.CreateNode(XmlNodeType.Element, "item", null);
                Dictionary<string, string> attributesDictionary = new Dictionary<string, string>
            {
                { "identifier", item.Identifier },
                { "name", item.Name },
                {
                    "id",
                    item.Item.Id
                }
            };
                AppendAttributes(xmlNode4, attributesDictionary);
                if (item.Amount > 1)
                {
                    AppendAttribute(xmlNode4, "amount", item.Amount.ToString());
                }
                if (item.HasAquisitionParent)
                {
                    AppendAttribute(xmlNode4, "aquired", item.AquisitionParent.Name);
                }
                if (!item.IncludeInEquipmentPageInventory)
                {
                    AppendAttribute(xmlNode4, "hidden", "true");
                }
                if (item.IncludeInEquipmentPageDescriptionSidebar)
                {
                    AppendAttribute(xmlNode4, "sidebar", "true");
                }
                if (item.IsEquipped)
                {
                    XmlNode xmlNode5 = _document.CreateNode(XmlNodeType.Element, "equipped", null);
                    xmlNode5.InnerText = item.IsEquipped.ToString().ToLowerInvariant();
                    if (!string.IsNullOrWhiteSpace(item.EquippedLocation) && item.EquippedLocation.Contains("Versatile"))
                    {
                        AppendAttribute(xmlNode5, "versatile", "true");
                    }
                    if (!string.IsNullOrWhiteSpace(item.EquippedLocation))
                    {
                        AppendAttribute(xmlNode5, "location", item.EquippedLocation);
                    }
                    xmlNode4.AppendChild(xmlNode5);
                }
                if (item.IsAttuned)
                {
                    XmlNode xmlNode6 = _document.CreateNode(XmlNodeType.Element, "attunement", null);
                    xmlNode6.InnerText = item.IsAttuned.ToString().ToLowerInvariant();
                    xmlNode4.AppendChild(xmlNode6);
                }
                XmlNode xmlNode7 = _document.CreateNode(XmlNodeType.Element, "items", null);
                if (item.IsAdorned)
                {
                    XmlNode parentNode = xmlNode7.AppendChild(_document.CreateNode(XmlNodeType.Element, "adorner", null));
                    parentNode.AppendAttribute("name", item.AdornerItem.Name);
                    parentNode.AppendAttribute("id", item.AdornerItem.Id);
                    xmlNode4.AppendChild(xmlNode7);
                }
                XmlNode xmlNode8 = xmlNode4.AppendChild(_document.CreateNode(XmlNodeType.Element, "details", null));
                xmlNode8.AppendAttribute("card", item.ShowCard ? "true" : "false");
                XmlNode xmlNode9 = xmlNode8.AppendChild(_document.CreateNode(XmlNodeType.Element, "name", null));
                XmlNode xmlNode10 = xmlNode8.AppendChild(_document.CreateNode(XmlNodeType.Element, "notes", null));
                xmlNode9.InnerText = item.AlternativeName ?? "";
                xmlNode10.InnerText = item.Notes ?? "";
                if (item.IsStored)
                {
                    xmlNode4.AppendChild(_document.CreateNode(XmlNodeType.Element, "storage", null)).AppendChild(_document.CreateNode(XmlNodeType.Element, "location", null)).InnerText = item.Storage.Name;
                }
                xmlNode.AppendChild(xmlNode4);
            }
            return xmlNode;
        }

        private XmlNode CreateSumNode()
        {
            XmlNode xmlNode = _document.CreateNode(XmlNodeType.Element, "sum", null);
            List<ElementBase> list = CharacterManager.Current.GetElements().ToList();
            AppendAttribute(xmlNode, "element-count", list.Count.ToString());
            foreach (ElementBase item in list)
            {
                XmlNode xmlNode2 = _document.CreateNode(XmlNodeType.Element, "element", null);
                Dictionary<string, string> attributesDictionary = new Dictionary<string, string>
            {
                { "type", item.Type },
                { "id", item.Id }
            };
                AppendAttributes(xmlNode2, attributesDictionary);
                xmlNode.AppendChild(xmlNode2);
            }
            return xmlNode;
        }

        private XmlNode CreateMagicNode()
        {
            CharacterManager current = CharacterManager.Current;
            SpellcastingSectionHandler current2 = SpellcastingSectionHandler.Current;
            List<ElementBase> list = (from x in current.GetElements()
                                      where x.Type.Equals("Spell")
                                      select x).ToList();
            _ = list.Count;
            XmlElement xmlElement = _document.CreateElement("magic");
            if (current.Status.HasMulticlass)
            {
                xmlElement.AppendAttribute("multiclass", "true");
                StatisticValuesGroup group = current.StatisticsCalculator.StatisticValues.GetGroup("multiclass:spellcasting:level", createNonExisting: false);
                if (group != null)
                {
                    xmlElement.AppendAttribute("level", group.Sum().ToString());
                }
            }
            foreach (SpellcastingInformation spellcastingInformation in current.GetSpellcastingInformations())
            {
                if (spellcastingInformation.IsExtension)
                {
                    continue;
                }
                SpellcasterSelectionControlViewModel viewModel = current2.GetSpellcasterSection(spellcastingInformation.UniqueIdentifier).GetViewModel<SpellcasterSelectionControlViewModel>();
                XmlNode xmlNode = xmlElement.AppendChild(_document.CreateElement("spellcasting"));
                xmlNode.AppendAttribute("name", spellcastingInformation.Name);
                xmlNode.AppendAttribute("ability", spellcastingInformation.AbilityName);
                xmlNode.AppendAttribute("attack", viewModel.InformationHeader.SpellAttackModifier.ToString());
                xmlNode.AppendAttribute("dc", viewModel.InformationHeader.SpellSaveDc.ToString());
                xmlNode.AppendAttribute("source", spellcastingInformation.ElementHeader.Id);
                XmlNode parentNode = xmlNode.AppendChild(_document.CreateElement("slots"));
                Dictionary<string, string> attributesDictionary = new Dictionary<string, string>
            {
                {
                    "s1",
                    viewModel.InformationHeader.Slot1.ToString()
                },
                {
                    "s2",
                    viewModel.InformationHeader.Slot2.ToString()
                },
                {
                    "s3",
                    viewModel.InformationHeader.Slot3.ToString()
                },
                {
                    "s4",
                    viewModel.InformationHeader.Slot4.ToString()
                },
                {
                    "s5",
                    viewModel.InformationHeader.Slot5.ToString()
                },
                {
                    "s6",
                    viewModel.InformationHeader.Slot6.ToString()
                },
                {
                    "s7",
                    viewModel.InformationHeader.Slot7.ToString()
                },
                {
                    "s8",
                    viewModel.InformationHeader.Slot8.ToString()
                },
                {
                    "s9",
                    viewModel.InformationHeader.Slot9.ToString()
                }
            };
                parentNode.AppendAttributes(attributesDictionary);
                XmlNode xmlNode2 = xmlNode.AppendChild(_document.CreateElement("cantrips"));
                XmlNode xmlNode3 = xmlNode.AppendChild(_document.CreateElement("spells"));
                foreach (SelectionElement item in from x in viewModel.KnownSpells
                                                  orderby x.IsChosen descending, x.Element.AsElement<Spell>().Level, x.Element.Name
                                                  select x)
                {
                    bool flag = list.Remove(item.Element);
                    Spell spell = item.Element.AsElement<Spell>();
                    XmlElement xmlElement2 = _document.CreateElement("spell");
                    xmlElement2.AppendAttribute("name", spell.Name);
                    xmlElement2.AppendAttribute("level", spell.Level.ToString());
                    xmlElement2.AppendAttribute("id", spell.Id);
                    if (spell.Level == 0)
                    {
                        xmlNode2.AppendChild(xmlElement2);
                        continue;
                    }
                    if (viewModel.IsPrepareSpellsRequired)
                    {
                        bool isChosen = viewModel.PreparedSpells.Contains(item.Element);
                        if (!item.IsChosen)
                        {
                            item.IsChosen = isChosen;
                        }
                        if (item.IsChosen)
                        {
                            xmlElement2.AppendAttribute("prepared", item.IsChosen ? "true" : "false");
                        }
                        if (flag)
                        {
                            xmlElement2.AppendAttribute("always-prepared", "true");
                        }
                    }
                    if (flag)
                    {
                        xmlElement2.AppendAttribute("known", "true");
                    }
                    xmlNode3.AppendChild(xmlElement2);
                }
            }
            if (list.Any())
            {
                XmlNode xmlNode4 = xmlElement.AppendChild(_document.CreateElement("additional"));
                foreach (Spell item2 in list.Cast<Spell>())
                {
                    XmlElement xmlElement3 = _document.CreateElement("spell");
                    xmlElement3.AppendAttribute("name", item2.Name);
                    xmlElement3.AppendAttribute("level", item2.Level.ToString());
                    xmlElement3.AppendAttribute("id", item2.Id);
                    if (item2.Aquisition.WasGranted)
                    {
                        xmlElement3.AppendAttribute("source", item2.Aquisition.GrantRule.ElementHeader.Name);
                    }
                    else if (item2.Aquisition.WasSelected)
                    {
                        xmlElement3.AppendAttribute("source", item2.Aquisition.SelectRule.ElementHeader.Name);
                    }
                    xmlNode4.AppendChild(xmlElement3);
                }
            }
            return xmlElement;
        }

        private XmlNode WriteInformationNode(XmlNode parentNode)
        {
            XmlNode xmlNode = parentNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "information", null));
            xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "group", null)).InnerText = CollectionGroupName;
            xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "generationOption", null)).InnerText = $"{ApplicationManager.Current.Settings.Settings.AbilitiesGenerationOption}";
            return xmlNode;
        }

        private void ReadInformationNode(XmlNode parentNode)
        {
            XmlNode xmlNode = parentNode.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("information"));
            if (xmlNode == null)
            {
                CollectionGroupName = "Characters";
                return;
            }
            if (xmlNode.ContainsChildNode("group"))
            {
                CollectionGroupName = xmlNode.GetChildNode("group").GetInnerText();
            }
            if (!xmlNode.ContainsChildNode("generationOption") || !int.TryParse(xmlNode.GetChildNode("generationOption").GetInnerText(), out var result))
            {
                return;
            }
            try
            {
                ApplicationManager.Current.Settings.Settings.AbilitiesGenerationOption = result;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ReadInformationNode");
                AnalyticsErrorHelper.Exception(ex, null, null, "ReadInformationNode", 2444);
            }
        }

        private XmlNode WriteDefensesNode(XmlNode parentNode, Character character)
        {
            XmlNode xmlNode = parentNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "defenses", null));
            xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "conditional", null)).InnerText = character.ConditionalArmorClassField.Content;
            return xmlNode;
        }

        private void ReadDefensesNode(XmlNode parentNode, Character character)
        {
            XmlNode xmlNode = parentNode.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("defenses"));
            if (xmlNode != null)
            {
                string ifNotEqualOriginalContent = xmlNode["conditional"]?.GetInnerText();
                character.ConditionalArmorClassField.SetIfNotEqualOriginalContent(ifNotEqualOriginalContent);
            }
        }

        private XmlNode WriteCompanionNode(XmlNode parentNode, Character character)
        {
            Companion companion = character.Companion;
            XmlNode xmlNode = parentNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "companion", null));
            xmlNode.AppendAttribute("name", companion.CompanionName.Content);
            XmlNode parentNode2 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "attributes", null));
            foreach (AbilityItem item in companion.Abilities.GetCollection())
            {
                parentNode2.AppendChild(item.Name.ToLowerInvariant(), item.FinalScore.ToString());
            }
            XmlNode parentNode3 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "saves", null));
            foreach (SavingThrowItem item2 in companion.SavingThrows.GetCollection())
            {
                parentNode3.AppendChild("save", item2.FinalBonus.ToString()).AppendAttribute("ability", item2.KeyAbility.Name.ToLowerInvariant());
            }
            XmlNode parentNode4 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "skills", null));
            foreach (SkillItem item3 in companion.Skills.GetCollection())
            {
                parentNode4.AppendChild("skill", item3.FinalBonus.ToString()).AppendAttribute("name", item3.Name);
            }
            XmlNode xmlNode2 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "portrait", null));
            xmlNode2.AppendAttribute("location", "local");
            xmlNode2.InnerText = companion.Portrait.ToString();
            return xmlNode;
        }

        private void ReadCompanionNode(XmlNode parentNode, Character character)
        {
            XmlNode xmlNode = parentNode.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("companion"));
            if (xmlNode != null)
            {
                Companion companion = character.Companion;
                if (!companion.CompanionName.EqualsOriginalContent(xmlNode.GetAttributeValue("name")))
                {
                    companion.CompanionName.Content = xmlNode.GetAttributeValue("name");
                }
                XmlNode node = xmlNode.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("portrait"));
                if (node.HasAttributes() && node.GetAttributeValue("location").Equals("local") && !companion.Portrait.EqualsOriginalContent(node.GetInnerText()))
                {
                    companion.Portrait.Content = node.GetInnerText();
                }
            }
        }

        private XmlNode WriteCharacterNotesNode(XmlNode parentNode, Character character)
        {
            XmlNode xmlNode = parentNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "notes", null));
            XmlNode xmlNode2 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "note", null));
            xmlNode2.AppendAttribute("column", "left");
            xmlNode2.InnerText = character.Notes1;
            XmlNode xmlNode3 = xmlNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "note", null));
            xmlNode3.AppendAttribute("column", "right");
            xmlNode3.InnerText = character.Notes2;
            return xmlNode;
        }

        private void ReadCharacterNotesNode(XmlNode parentNode, Character character)
        {
            XmlNode xmlNode = parentNode.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("notes"));
            if (xmlNode == null)
            {
                return;
            }
            foreach (XmlNode item in from XmlNode x in xmlNode.ChildNodes
                                     where x.Name.Equals("note")
                                     select x)
            {
                if (item.ContainsAttribute("column"))
                {
                    switch (item.GetAttributeValue("column"))
                    {
                        case "left":
                            character.Notes1 = item.GetInnerText();
                            break;
                        case "right":
                            character.Notes2 = item.GetInnerText();
                            break;
                    }
                }
            }
        }

        private XmlNode WriteQuestItemsNode(XmlNode parentNode, Character character)
        {
            XmlNode xmlNode = parentNode.AppendChild(_document.CreateNode(XmlNodeType.Element, "quest", null));
            xmlNode.InnerText = character.Inventory.QuestItems;
            return xmlNode;
        }

        private void ReadQuestItemsNode(XmlNode parentNode, Character character)
        {
            XmlNode xmlNode = parentNode.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("quest"));
            if (xmlNode != null)
            {
                character.Inventory.QuestItems = xmlNode.GetInnerText();
            }
        }

        private void ReadAppearanceNode(XmlNode appearanceNode, Character character)
        {
            character.PortraitFilename = appearanceNode["portrait"].GetInnerText();
            character.AgeField.Content = appearanceNode["age"].GetInnerText();
            character.HeightField.Content = appearanceNode["height"].GetInnerText();
            character.WeightField.Content = appearanceNode["weight"].GetInnerText();
            character.Eyes = appearanceNode["eyes"].GetInnerText();
            character.Skin = appearanceNode["skin"].GetInnerText();
            character.Hair = appearanceNode["hair"].GetInnerText();
        }

        private async Task ReadChildElements(XmlNode elementNode, ElementBase element)
        {
            foreach (XmlNode childNode in elementNode.ChildNodes)
            {
                if (childNode.ContainsAttribute("registered"))
                {
                    string registeredElementId = childNode.GetAttributeValue("registered");
                    if (string.IsNullOrWhiteSpace(registeredElementId))
                    {
                        continue;
                    }
                    string type = childNode.GetAttributeValue("type");
                    string name = childNode.GetAttributeValue("name");
                    bool flag = childNode.ContainsAttribute("isList") && Convert.ToBoolean(childNode.GetAttributeValue("isList"));
                    bool hasNumber = childNode.ContainsAttribute("number");
                    int num = int.Parse(childNode.GetAttributeValue("requiredLevel"));
                    bool isRetrained = childNode.ContainsAttribute("replaceLevel");
                    int retrainLevel = -1;
                    if (isRetrained)
                    {
                        isRetrained = int.TryParse(childNode.GetAttributeValue("replaceLevel"), out retrainLevel);
                    }
                    int number = -1;
                    if (hasNumber)
                    {
                        hasNumber = int.TryParse(childNode.GetAttributeValue("number"), out number);
                    }
                    if ((num > 1 && CharacterManager.Current.Character.Level < num) || (isRetrained && CharacterManager.Current.Character.Level < retrainLevel))
                    {
                        continue;
                    }
                    try
                    {
                        if (flag)
                        {
                            SelectRule listRule = element.GetSelectRules().Single((SelectRule x) => x.Attributes.IsList && x.Attributes.Name.Equals(name));
                            await AwaitExpanderCreationAsync(listRule, (!hasNumber) ? 1 : number);
                            SelectionRuleExpanderHandler.Current.SetRegisteredElement(listRule, registeredElementId, (!hasNumber) ? 1 : number);
                            continue;
                        }
                        if (hasNumber)
                        {
                            string existingChecksum = childNode.GetAttributeValue("checksum");
                            List<SelectRule> list = (from x in element.GetSelectRules()
                                                     where x.Attributes.Type == type && x.Attributes.Number > 1 && x.Attributes.Name == name && x.GetCrC(number) == existingChecksum
                                                     select x).ToList();
                            if (!list.Any())
                            {
                                continue;
                            }
                            if (list.Count > 1 && Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                            SelectRule listRule = list.First();
                            if (await AwaitExpanderCreationAsync(listRule, number))
                            {
                                if (isRetrained)
                                {
                                    SelectionRuleExpanderHandler.Current.RetrainSpellExpander(listRule, number, retrainLevel);
                                }
                                SelectionRuleExpanderHandler.Current.SetRegisteredElement(listRule, registeredElementId, number);
                                goto IL_0697;
                            }
                            continue;
                        }
                        List<SelectRule> list2 = (from x in element.GetSelectRules()
                                                  where x.Attributes.Type == type && x.Attributes.Name == name
                                                  select x).ToList();
                        _ = list2.Count;
                        _ = 1;
                        if (list2.Count != 1 && childNode.ContainsAttribute("checksum"))
                        {
                            string existingChecksum2 = childNode.GetAttributeValue("checksum");
                            SelectRule listRule = list2.Single((SelectRule x) => x.GetCrC(1) == existingChecksum2);
                            if (!(await AwaitExpanderCreationAsync(listRule)))
                            {
                                return;
                            }
                            if (isRetrained)
                            {
                                SelectionRuleExpanderHandler.Current.RetrainSpellExpander(listRule, 1, retrainLevel);
                            }
                            SelectionRuleExpanderHandler.Current.SetRegisteredElement(listRule, registeredElementId);
                        }
                        else
                        {
                            SelectRule listRule = element.GetSelectRules().FirstOrDefault((SelectRule x) => x.Attributes.Type == type && x.Attributes.Name == name);
                            if (listRule == null && Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                            if (!(await AwaitExpanderCreationAsync(listRule)))
                            {
                                return;
                            }
                            if (isRetrained)
                            {
                                SelectionRuleExpanderHandler.Current.RetrainSpellExpander(listRule, 1, retrainLevel);
                            }
                            SelectionRuleExpanderHandler.Current.SetRegisteredElement(listRule, registeredElementId);
                        }
                        goto IL_0697;
                    IL_0697:
                        Logger.Debug("--Registered:" + registeredElementId);
                        if (!CharacterManager.Current.Elements.Any((ElementBase x) => x.Id == registeredElementId))
                        {
                            Logger.Warning("--not yet registered!" + registeredElementId);
                            await Task.Delay(500);
                            if (!CharacterManager.Current.Elements.Any((ElementBase x) => x.Id == registeredElementId))
                            {
                                Logger.Warning("--not yet registered!" + registeredElementId);
                            }
                            else
                            {
                                Logger.Warning("--yep! after 500ms it is now registered!" + registeredElementId);
                            }
                        }
                        ElementBase elementBase = CharacterManager.Current.Elements.LastOrDefault((ElementBase x) => x.Id == registeredElementId);
                        if (elementBase != null)
                        {
                            await ReadChildElements(childNode, elementBase);
                            continue;
                        }
                        Logger.Warning("unable to get element from character elements: {0}", registeredElementId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning("error while reading child element " + name + " (" + type + ") -> " + registeredElementId);
                        Logger.Exception(ex, "ReadChildElements");
                    }
                }
                else
                {
                    string id = childNode.GetAttributeValue("id");
                    if (string.IsNullOrWhiteSpace(id) && Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    ElementBase elementBase2 = CharacterManager.Current.GetElements().LastOrDefault((ElementBase x) => x.Id == id);
                    if (elementBase2 != null)
                    {
                        await ReadChildElements(childNode, elementBase2);
                        continue;
                    }
                    Logger.Warning("unable to get element from character elements: {0}", id);
                }
            }
        }

        private void ParseAttacksSection(XmlNode attacksNode, Character character)
        {
            character.AttacksSection.AttacksAndSpellcasting = attacksNode.FirstChild.GetInnerText();
            if (!IsPostCharacterSheetUpdate())
            {
                return;
            }
            List<XmlNode> list = (from XmlNode x in attacksNode.ChildNodes
                                  where x.Name.Equals("attack")
                                  select x).ToList();
            character.AttacksSection.Items.Clear();
            foreach (XmlNode item in list)
            {
                try
                {
                    AttackSectionItem attackSectionItem = null;
                    if (item.ContainsAttribute("identifier"))
                    {
                        string id = item.GetAttributeValue("identifier");
                        RefactoredEquipmentItem refactoredEquipmentItem = character.Inventory.Items.FirstOrDefault((RefactoredEquipmentItem x) => x.Identifier.Equals(id, StringComparison.OrdinalIgnoreCase));
                        if (refactoredEquipmentItem != null)
                        {
                            attackSectionItem = new AttackSectionItem(refactoredEquipmentItem, initializeAbility: false);
                        }
                    }
                    if (attackSectionItem == null)
                    {
                        attackSectionItem = new AttackSectionItem(item.GetAttributeValue("name"));
                    }
                    if (item.ContainsAttribute("name"))
                    {
                        attackSectionItem.Name.SetIfNotEqualOriginalContent(item.GetAttributeValue("name"));
                    }
                    if (item.ContainsAttribute("range"))
                    {
                        attackSectionItem.Range.SetIfNotEqualOriginalContent(item.GetAttributeValue("range"));
                    }
                    if (item.ContainsAttribute("attack"))
                    {
                        attackSectionItem.Attack.SetIfNotEqualOriginalContent(item.GetAttributeValue("attack"));
                    }
                    if (item.ContainsAttribute("damage"))
                    {
                        attackSectionItem.Damage.SetIfNotEqualOriginalContent(item.GetAttributeValue("damage"));
                    }
                    if (item.ContainsAttribute("ability") && attackSectionItem.IsAutomaticAddition)
                    {
                        string attributeValue = item.GetAttributeValue("ability");
                        if (!string.IsNullOrWhiteSpace(attributeValue))
                        {
                            attackSectionItem.SetLinkedAbility(attributeValue);
                        }
                    }
                    if (item.FirstChild != null)
                    {
                        attackSectionItem.Description.SetIfNotEqualOriginalContent(item.FirstChild.GetInnerText());
                    }
                    attackSectionItem.IsDisplayed = item.GetAttributeAsBoolean("displayed");
                    character.AttacksSection.Items.Add(attackSectionItem);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "ParseAttacksSection");
                }
            }
        }

        private void ParseBackgroundInput(XmlNode backgroundNode, Character character)
        {
            if (!IsPostCharacterSheetUpdate())
            {
                return;
            }
            XmlNode xmlNode = backgroundNode.ChildNodes.Cast<XmlNode>().FirstOrDefault((XmlNode x) => x.Name.Equals("feature"));
            if (xmlNode != null)
            {
                string attributeValue = xmlNode.GetAttributeValue("name");
                string innerText = xmlNode.FirstChild.GetInnerText();
                if (!character.BackgroundFeatureName.EqualsOriginalContent(attributeValue))
                {
                    character.BackgroundFeatureName.Content = attributeValue;
                }
                if (!character.BackgroundFeatureDescription.EqualsOriginalContent(innerText))
                {
                    character.BackgroundFeatureDescription.Content = innerText;
                }
            }
        }

        private void ParseBackgroundCharacteristicsInput(XmlNode inputNode, Character character)
        {
            string content = inputNode["background-traits"]?.GetInnerText();
            string content2 = inputNode["background-ideals"]?.GetInnerText();
            string content3 = inputNode["background-bonds"]?.GetInnerText();
            string content4 = inputNode["background-flaws"]?.GetInnerText();
            FillableBackgroundCharacteristics fillableBackgroundCharacteristics = character.FillableBackgroundCharacteristics;
            if (!fillableBackgroundCharacteristics.Traits.EqualsOriginalContent(content))
            {
                fillableBackgroundCharacteristics.Traits.Content = content;
            }
            if (!fillableBackgroundCharacteristics.Ideals.EqualsOriginalContent(content2))
            {
                fillableBackgroundCharacteristics.Ideals.Content = content2;
            }
            if (!fillableBackgroundCharacteristics.Bonds.EqualsOriginalContent(content3))
            {
                fillableBackgroundCharacteristics.Bonds.Content = content3;
            }
            if (!fillableBackgroundCharacteristics.Flaws.EqualsOriginalContent(content4))
            {
                fillableBackgroundCharacteristics.Flaws.Content = content4;
            }
            string content5 = inputNode["background-trinket"]?.GetInnerText();
            if (!character.Trinket.EqualsOriginalContent(content5))
            {
                character.Trinket.Content = content5;
            }
        }

        private static async Task<bool> AwaitExpanderCreationAsync(SelectRule selectionRule, int number = 1)
        {
            int loopCount = 0;
            do
            {
                if (SelectionRuleExpanderHandler.Current.HasExpander(selectionRule.UniqueIdentifier, number))
                {
                    return true;
                }
                await Task.Delay(10);
                loopCount++;
                if (loopCount >= 5)
                {
                    Logger.Warning($"expander for {selectionRule.Attributes.Name} still does not exist after {loopCount} loops, waiting for it to get added");
                }
            }
            while (loopCount < 10);
            return false;
        }

        [Obsolete]
        private void AppendAttribute(XmlNode node, string name, string value)
        {
            if (node.OwnerDocument == null)
            {
                throw new NullReferenceException("OwnerDocument");
            }
            XmlAttribute xmlAttribute = node.OwnerDocument.CreateAttribute(name);
            xmlAttribute.Value = value;
            node.Attributes?.Append(xmlAttribute);
        }

        [Obsolete]
        private void AppendAttributes(XmlNode node, Dictionary<string, string> attributesDictionary)
        {
            if (node.OwnerDocument == null)
            {
                throw new NullReferenceException("OwnerDocument");
            }
            foreach (KeyValuePair<string, string> item in attributesDictionary)
            {
                XmlAttribute xmlAttribute = node.OwnerDocument.CreateAttribute(item.Key);
                xmlAttribute.Value = item.Value;
                node.Attributes?.Append(xmlAttribute);
            }
        }

        [Obsolete]
        private XmlNode AppendChild(XmlNode parentNode, string nodeName, string innertext, bool isCData)
        {
            XmlNode xmlNode = parentNode.AppendChild(_document.CreateNode(XmlNodeType.Element, nodeName, null));
            if (isCData)
            {
                xmlNode.AppendChild(_document.CreateCDataSection(innertext));
            }
            else
            {
                xmlNode.InnerText = innertext;
            }
            return xmlNode;
        }

        public override string ToString()
        {
            return DisplayName + ", Level " + DisplayLevel + " " + DisplayRace + " " + DisplayClass;
        }

        public void SaveRemotePortrait()
        {
            try
            {
                if (!File.Exists(DisplayPortraitFilePath))
                {
                    string fileName = Path.GetFileName(DisplayPortraitFilePath);
                    string text = Path.Combine(DataManager.Current.UserDocumentsPortraitsDirectory, fileName);
                    if (File.Exists(text))
                    {
                        DisplayPortraitFilePath = text;
                        return;
                    }
                    GalleryUtilities.SaveBase64AsImage(DisplayPortraitBase64, text);
                    DisplayPortraitFilePath = text;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SaveRemotePortrait");
                MessageDialogService.ShowException(ex, ex.Message, "Unable to save remote portrait.");
            }
        }

        private bool IsPostCharacterSheetUpdate()
        {
            string attributeValue = _document.DocumentElement.GetAttributeValue("version");
            bool attributeAsBoolean = _document.DocumentElement.GetAttributeAsBoolean("preview");
            Version version = new Version(attributeValue);
            if (attributeAsBoolean && version.CompareTo(new Version("1.18.822")) < 0)
            {
                return false;
            }
            return true;
        }
    }

}
