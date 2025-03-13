using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Data.Files;
using Builder.Data.Rules;
using Builder.Data.Strings;
using Builder.Presentation;
using Builder.Presentation.Events.Data;
using Builder.Presentation.Logging;
using Builder.Presentation.Models;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;

namespace Builder.Presentation.Services.Data
{
    public sealed class DataManager
    {
        private const string LocalAppDataRootDirectoryName = "5e Character Builder";

        private const string LocalAppDataApplicationElementsDirectoryName = "elements";

        private const string LocalAppDataLogsDirectoryName = "logs";

        private const string LocalAppDataAssetsDirectoryName = "assets";

        private const string UserDocumentsRootDirectoryName = "5e Character Builder";

        private const string UserDocumentsPortraitsDirectoryName = "portraits";

        private const string UserDocumentsCustomElementsDirectoryName = "custom";

        private const string UserDocumentsCompanionGalleryDirectoryName = "gallery\\companions";

        private const string UserDocumentsSymbolsGalleryDirectoryName = "gallery\\symbols";

        private const string IndexFileExtension = ".index";

        private const string ElementsDataFileExtension = ".xml";

        private const string CharacterFileExtension = ".dnd5e";

        private readonly IEventAggregator _eventAggregator;

        public static DataManager Current { get; } = new DataManager();

        public ElementBaseCollection ElementsCollection { get; } = new ElementBaseCollection();

        public string LocalAppDataRootDirectory { get; private set; }

        public string LocalAppDataApplicationElementsDirectory { get; private set; }

        public string LocalAppDataLogsDirectory { get; private set; }

        public string LocalAppDataAssetsDirectory { get; private set; }

        public string UserDocumentsRootDirectory { get; private set; }

        public string UserDocumentsPortraitsDirectory { get; private set; }

        public string UserDocumentsCustomElementsDirectory { get; private set; }

        public string UserDocumentsCompanionGalleryDirectory { get; private set; }

        public string UserDocumentsSymbolsGalleryDirectory { get; private set; }

        public bool IsElementsCollectionPopulated { get; private set; }

        public event EventHandler<DataManagerProgressChanged> ProgressChanged;

        public event EventHandler InitializingData;

        public event EventHandler InitializingDataCompleted;

        private DataManager()
        {
            _eventAggregator = ApplicationManager.Current.EventAggregator;
        }

        private void OnProgressChanged(DataManagerProgressChanged e)
        {
            this.ProgressChanged?.Invoke(this, e);
        }

        private void OnInitializingData()
        {
            this.InitializingData?.Invoke(this, EventArgs.Empty);
        }

        private void OnInitializingDataCompleted()
        {
            this.InitializingDataCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void InitializeDirectories()
        {
            try
            {
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                LocalAppDataRootDirectory = Path.Combine(folderPath, "5e Character Builder");
                LocalAppDataApplicationElementsDirectory = Path.Combine(LocalAppDataRootDirectory, "elements");
                LocalAppDataLogsDirectory = Path.Combine(LocalAppDataRootDirectory, "logs");
                LocalAppDataAssetsDirectory = Path.Combine(LocalAppDataRootDirectory, "assets");
                CreateDirectory(LocalAppDataRootDirectory);
                CreateDirectory(LocalAppDataApplicationElementsDirectory);
                CreateDirectory(LocalAppDataLogsDirectory);
                CreateDirectory(LocalAppDataAssetsDirectory);
            }
            catch (Exception ex)
            {
                Logger.Warning("unable to create appdata folders");
                Logger.Exception(ex, "InitializeDirectories");
            }
            try
            {
                string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                UserDocumentsRootDirectory = Path.Combine(folderPath2, "5e Character Builder");
                if (!string.IsNullOrWhiteSpace(Settings.Default.DocumentsRootDirectory))
                {
                    if (Directory.Exists(Settings.Default.DocumentsRootDirectory))
                    {
                        UserDocumentsRootDirectory = Settings.Default.DocumentsRootDirectory;
                    }
                    else
                    {
                        MessageDialogService.Show("The root " + Settings.Default.DocumentsRootDirectory + " does not exist. Falling back to the default directory.");
                        Settings.Default.DocumentsRootDirectory = "";
                    }
                }
                UserDocumentsCompanionGalleryDirectory = Path.Combine(UserDocumentsRootDirectory, "gallery\\companions");
                UserDocumentsSymbolsGalleryDirectory = Path.Combine(UserDocumentsRootDirectory, "gallery\\symbols");
                UserDocumentsPortraitsDirectory = Path.Combine(UserDocumentsRootDirectory, "portraits");
                UserDocumentsCustomElementsDirectory = Path.Combine(UserDocumentsRootDirectory, "custom");
                CreateDirectory(UserDocumentsRootDirectory);
                CreateDirectory(UserDocumentsPortraitsDirectory);
                CreateDirectory(UserDocumentsCustomElementsDirectory);
                CreateDirectory(UserDocumentsCompanionGalleryDirectory);
                CreateDirectory(UserDocumentsSymbolsGalleryDirectory);
                CreateDirectory(Path.Combine(UserDocumentsCustomElementsDirectory, "user"));
                if (string.IsNullOrWhiteSpace(Settings.Default.DocumentsRootDirectory))
                {
                    Settings.Default.DocumentsRootDirectory = UserDocumentsRootDirectory;
                }
            }
            catch (Exception ex2)
            {
                Logger.Warning("unable to create user documents folders");
                Logger.Exception(ex2, "InitializeDirectories");
            }
            Logger.Info("Directories Initialized");
        }

        public void InitializeFileLogger()
        {
            Logger.RegisterLogger(new FileLogger(LocalAppDataLogsDirectory));
        }

        public void InitializeFileWatcher()
        {
            if (Directory.Exists(UserDocumentsRootDirectory))
            {
                new FileSystemWatcher(UserDocumentsRootDirectory)
                {
                    Filter = "*.dnd5e",
                    EnableRaisingEvents = true
                };
                return;
            }
            throw new Exception("Directories are not initialized. Call InitializeFolders before InitializeFileWatcher");
        }

        public async Task<IEnumerable<ElementBase>> InitializeElementDataAsync()
        {
            IsElementsCollectionPopulated = false;
            new List<ElementBase>();
            List<ElementParser> elementParserCollection = ElementParserFactory.GetParsers().ToList();
            ElementParser defaultParser = new ElementParser();
            ElementParser elementParser = new ElementParser();
            List<Exception> exceptions = new List<Exception>();
            ElementBaseCollection coreElements = new ElementBaseCollection();
            int elementNodeCount = 0;
            DataManagerProgressChanged args = new DataManagerProgressChanged("Initializing Core", 0, inProgress: true);
            _eventAggregator.Send(args);
            int num = 0;
            List<XmlDocument> list = LoadElementDocumentsFromResource();
            foreach (XmlDocument item in list)
            {
                ElementHeader arg = null;
                try
                {
                    num++;
                    args.ProgressPercentage = GetPercentage(num, list.Count);
                    _eventAggregator.Send(args);
                    List<XmlNode> list2 = (from XmlNode x in item.DocumentElement.ChildNodes
                                           where x.NodeType != XmlNodeType.Comment && x.Name.Equals("element")
                                           select x).ToList();
                    elementNodeCount += list2.Count;
                    foreach (XmlNode item2 in list2)
                    {
                        try
                        {
                            ElementHeader header = elementParser.ParseElementHeader(item2);
                            arg = header;
                            if (elementParser.ParserType != header.Type)
                            {
                                elementParser = elementParserCollection.FirstOrDefault((ElementParser x) => x.ParserType == header.Type) ?? defaultParser;
                            }
                            ElementBase elementBase = elementParser.ParseElement(item2);
                            if (coreElements.Select((ElementBase e) => e.Id).Contains(elementBase.Id))
                            {
                                exceptions.Add(new DuplicateElementException(elementBase.Name, "resource filename"));
                            }
                            else
                            {
                                coreElements.Add(elementBase);
                            }
                            _eventAggregator.Send(args);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warning($"'{ex.GetType()}' in parsing the resource data files on {arg}");
                            Logger.Exception(ex, "InitializeElementDataAsync");
                            ex.Data["warning"] = $"'{ex.GetType()}' in parsing the resource data files on {arg}";
                            exceptions.Add(ex);
                        }
                    }
                    arg = null;
                }
                catch (Exception ex2)
                {
                    Logger.Warning($"'{ex2.GetType()}' in parsing the resource data files on {arg}");
                    Logger.Exception(ex2, "InitializeElementDataAsync");
                    ex2.Data["warning"] = $"'{ex2.GetType()}' in parsing the resource data files on {arg}";
                    exceptions.Add(ex2);
                }
            }
            Logger.Info("loaded {0} core elements from {1} element nodes", coreElements.Count, elementNodeCount);
            args.ProgressMessage = "Initializing Custom Elements";
            args.ProgressPercentage = 0;
            _eventAggregator.Send(args);
            await Task.Delay(50);
            int currentFileCount = 0;
            List<FileInfo> customFiles = GetCustomFiles();
            List<XmlNode> appendNotes = new List<XmlNode>();
            foreach (FileInfo file in customFiles)
            {
                try
                {
                    Logger.Info($"parsing {file}");
                    ElementsFile ef = ElementsFile.FromFile(file);
                    currentFileCount++;
                    args.ProgressMessage = ef.Info.DisplayName ?? "";
                    args.ProgressPercentage = GetPercentage(currentFileCount, customFiles.Count);
                    _eventAggregator.Send(args);
                    if (ef.Ignore)
                    {
                        Logger.Warning($"ignore {file}");
                        continue;
                    }
                    ElementBaseCollection applicationElements = new ElementBaseCollection();
                    XmlDocument xmlDocument = await CreateXmlDocument(file.FullName);
                    _ = ef.ElementNodes;
                    if (xmlDocument.DocumentElement != null)
                    {
                        List<XmlNode> list3 = (from XmlNode x in xmlDocument.DocumentElement.ChildNodes
                                               where x.NodeType != XmlNodeType.Comment && x.Name.Equals("element")
                                               select x).ToList();
                        elementNodeCount += list3.Count;
                        foreach (XmlNode item3 in list3)
                        {
                            ElementHeader header2 = elementParser.ParseElementHeader(item3);
                            if (elementParser.ParserType != header2.Type)
                            {
                                elementParser = elementParserCollection.FirstOrDefault((ElementParser p) => p.ParserType == header2.Type) ?? defaultParser;
                            }
                            applicationElements.Add(elementParser.ParseElement(item3));
                        }
                    }
                    foreach (ElementBase element in applicationElements)
                    {
                        if (coreElements.Select((ElementBase e) => e.Id).Contains(element.Id))
                        {
                            ElementBase elementBase2 = coreElements.Single((ElementBase x) => x.Id == element.Id);
                            if (elementBase2.Type != element.Type)
                            {
                                exceptions.Add(new DuplicateElementException(element.Name, file.Name));
                            }
                            coreElements.Remove(elementBase2);
                        }
                        coreElements.Add(element);
                    }
                    appendNotes.AddRange(ef.ExtendNodes);
                }
                catch (ElementsFileLoadException ex3)
                {
                    Logger.Warning(ex3.Message);
                    Logger.Exception(ex3, "InitializeElementDataAsync");
                    ex3.Data.Add("filename", file.FullName);
                    exceptions.Add(ex3);
                }
                catch (Exception ex4)
                {
                    Logger.Warning("'{0}' in parsing {1}", ex4.GetType(), file.FullName);
                    Logger.Exception(ex4, "InitializeElementDataAsync");
                    ex4.Data.Add("filename", file.FullName);
                    exceptions.Add(ex4);
                }
            }
            args.ProgressMessage = "Processing elements...";
            _eventAggregator.Send(args);
            await Task.Delay(10);
            AppendElements(appendNotes, coreElements, elementParser, defaultParser, elementParserCollection);
            args.ProgressMessage = $"{coreElements.Count}/{elementNodeCount} elements loaded";
            args.ProgressPercentage = 100;
            Logger.Info(args.ProgressMessage);
            if (exceptions.Any())
            {
                List<Exception> list4 = exceptions.Where((Exception x) => x.GetType() == typeof(MissingSetterException)).ToList();
                if (list4.Any())
                {
                    foreach (Exception item4 in list4)
                    {
                        exceptions.Remove(item4);
                    }
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("There are missing setter nodes on the following elements:").AppendLine();
                    foreach (Exception item5 in list4)
                    {
                        stringBuilder.AppendLine("\t" + item5.Message);
                    }
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("These will not be available until these missing setters have been added.").AppendLine().AppendLine();
                    MessageDialogService.Show(stringBuilder.ToString());
                }
                List<Exception> list5 = exceptions.Where((Exception x) => x.GetType() == typeof(DuplicateElementException)).ToList();
                if (list5.Any())
                {
                    foreach (Exception item6 in list5)
                    {
                        exceptions.Remove(item6);
                    }
                    StringBuilder stringBuilder2 = new StringBuilder();
                    stringBuilder2.AppendLine("There are duplicated element id's on the following elements:").AppendLine();
                    foreach (Exception item7 in list5)
                    {
                        stringBuilder2.AppendLine("\t" + item7.Message);
                    }
                    stringBuilder2.AppendLine("original elements have been replaced with the custom elements with the same id");
                    if (Debugger.IsAttached)
                    {
                        MessageDialogService.Show(stringBuilder2.ToString());
                    }
                }
                if (exceptions.Any())
                {
                    Exception ex5 = exceptions.First();
                    object obj = (ex5.Data.Contains("filename") ? ex5.Data["filename"] : "internal");
                    object obj2 = (ex5.Data.Contains("warning") ? ex5.Data["warning"] : "");
                    MessageDialogService.ShowException(ex5, "Error(s) parsing data files", (exceptions.Count > 1) ? $"{exceptions.Count} exceptions occurred while parsing the data files. The first one is shown below, the others can be found in the logs.\r\nFile: {obj}\r\nInfo: {obj2}" : $"An exception occurred while parsing the data files.\r\nFile: {obj}\r\nInfo: {obj2}");
                }
            }
            ElementsCollection.Clear();
            ElementsCollection.AddRange(coreElements);
            _eventAggregator.Send(new ElementsCollectionPopulatedEvent());
            args.ProgressMessage = "Finalizing Content";
            _eventAggregator.Send(args);
            List<ElementBase> source = ElementsCollection.Where((ElementBase x) => x.Type.Equals("Support")).ToList();
            string[] array = new string[11]
            {
            "(", ")", ",", "&", "|", "!", "[", "]", ":", "'",
            "’"
            };
            foreach (ElementBase item8 in ElementsCollection)
            {
                string[] array2 = array;
                foreach (string value in array2)
                {
                    if (item8.Id.Contains(value))
                    {
                        Logger.Warning($"INVALID ID ON: {item8} ({item8.Id})");
                        break;
                    }
                }
                List<string> list6 = new List<string>();
                foreach (string support in item8.Supports)
                {
                    if (support.StartsWith("ID_INTERNAL_SUPPORT"))
                    {
                        ElementBase elementBase3 = source.FirstOrDefault((ElementBase x) => x.Id.Equals(support));
                        if (elementBase3 != null)
                        {
                            list6.Add(elementBase3.Name);
                        }
                    }
                }
                foreach (string item9 in list6)
                {
                    if (!item8.Supports.Contains(item9))
                    {
                        item8.Supports.Add(item9);
                    }
                }
            }
            IEnumerable<ElementBase> source2 = ElementsCollection.Where((ElementBase x) => x.Type == "Class Feature");
            IEnumerable<ElementBase> source3 = source2.Where((ElementBase x) => x.Id.StartsWith("ID_INTERNAL_TEMPLATE_CLASS_FEATURE_ABILITY_4"));
            IEnumerable<ElementBase> source4 = source2.Where((ElementBase x) => x.Id.StartsWith("ID_INTERNAL_TEMPLATE_CLASS_FEATURE_FEAT_4"));
            ElementBase original = source3.FirstOrDefault();
            ElementBase original2 = source4.FirstOrDefault();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<string> list7 = new List<string>();
            foreach (Class item10 in ElementsCollection.Where((ElementBase x) => x.Type == "Class").Cast<Class>().ToList())
            {
                if (item10.CanMulticlass)
                {
                    ElementBase elementBase4 = elementParserCollection.FirstOrDefault((ElementParser x) => x.ParserType == "Multiclass").ParseElement(item10.ElementNode);
                    ElementsCollection.Add(elementBase4);
                    item10.Requirements = (item10.HasRequirements ? ("(" + item10.Requirements + ")&&!" + elementBase4.Id) : ("!" + elementBase4.Id));
                    GrantRule grantRule = new GrantRule(item10.ElementHeader);
                    grantRule.Attributes.Type = "Grants";
                    grantRule.Attributes.Name = InternalGrants.MulticlassPrerequisite;
                    grantRule.Attributes.Requirements = InternalOptions.AllowMulticlassing + "&&(" + elementBase4.Requirements + ")&&!" + elementBase4.Id;
                    item10.Rules.Add(grantRule);
                    elementBase4.Requirements = "!" + item10.Id + "&&(" + elementBase4.Requirements + ")";
                    dictionary.Add(item10.Id, elementBase4.Id);
                }
                if (list7.Contains(item10.Name))
                {
                    continue;
                }
                ElementBaseCollection elementBaseCollection = new ElementBaseCollection();
                int[] array3 = new int[9] { 4, 6, 8, 10, 12, 14, 16, 18, 19 };
                for (int i = 0; i < array3.Length; i++)
                {
                    int num2 = array3[i];
                    string name = item10.Name;
                    if (name.Contains(","))
                    {
                        Logger.Warning(name + " contains ','");
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                    }
                    ElementBase elementBase5 = original.Copy();
                    string text = $"ID_INTERNAL_CLASS_FEATURE_ASI_{num2}_{name.ToUpperInvariant()}";
                    if (!ElementsHelper.ValidateID(text))
                    {
                        text = ElementsHelper.SanitizeID(text);
                    }
                    elementBase5.ElementHeader = new ElementHeader($"Ability Score Improvement ({num2})", "Class Feature", "Player’s Handbook", text);
                    elementBase5.GetSelectRules().First().Attributes.Name = $"Ability Score Increase ({name.ToUpperInvariant()} {num2})";
                    elementBase5.GetSelectRules().First().Attributes.RequiredLevel = num2;
                    elementBase5.GetSelectRules().First().RenewIdentifier();
                    elementBase5.GetSelectRules().First().ElementHeader = elementBase5.ElementHeader;
                    elementBase5.Supports.Add("Improvement Option");
                    elementBase5.Supports.Add(name);
                    elementBase5.Supports.Add(num2.ToString());
                    elementBase5.IsExtended = true;
                    elementBase5.IncludeInCompendium = false;
                    elementBaseCollection.Add(elementBase5);
                    ElementBase elementBase6 = original2.Copy();
                    string text2 = $"ID_INTERNAL_CLASS_FEATURE_FEAT_{num2}_{name.ToUpperInvariant()}";
                    if (!ElementsHelper.ValidateID(text2))
                    {
                        text2 = ElementsHelper.SanitizeID(text2);
                    }
                    elementBase6.ElementHeader = new ElementHeader($"Feat ({num2})", "Class Feature", "Player’s Handbook", text2);
                    elementBase6.GetSelectRules().First().Attributes.Name = $"Feat ({name.ToUpperInvariant()} {num2})";
                    elementBase6.GetSelectRules().First().Attributes.RequiredLevel = num2;
                    elementBase6.GetSelectRules().First().RenewIdentifier();
                    elementBase6.GetSelectRules().First().ElementHeader = elementBase6.ElementHeader;
                    elementBase6.Supports.Add("Improvement Option");
                    elementBase6.Supports.Add(name);
                    elementBase6.Supports.Add(num2.ToString());
                    elementBase6.IsExtended = true;
                    elementBase6.IncludeInCompendium = false;
                    elementBaseCollection.Add(elementBase6);
                }
                ElementsCollection.AddRange(elementBaseCollection);
                list7.Add(item10.Name);
            }
            if (true)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                List<ElementBase> list8 = new SpellScrollContentGenerator().Generate(template: ElementsCollection.FirstOrDefault((ElementBase x) => x.Id.Equals("ID_WOTC_DMG_MAGIC_ITEM_SPELL_SCROLL_CANTRIP")) as MagicItemElement, content: ElementsCollection);
                ElementsCollection.AddRange(list8);
                stopwatch.Stop();
                Logger.Warning($"generating {list8.Count} scrolls took {stopwatch.ElapsedMilliseconds}ms");
            }
            InternalElementsGenerator internalElementsGenerator = new InternalElementsGenerator();
            ElementsCollection.AddRange(internalElementsGenerator.GenerateInternalFeats(ElementsCollection));
            ElementsCollection.AddRange(internalElementsGenerator.GenerateInternalLanguages(ElementsCollection));
            ElementsCollection.AddRange(internalElementsGenerator.GenerateInternalProficiency(ElementsCollection));
            ElementsCollection.AddRange(internalElementsGenerator.GenerateInternalAsi(ElementsCollection));
            ElementsCollection.AddRange(internalElementsGenerator.GenerateInternalSpells(ElementsCollection));
            if (Debugger.IsAttached || ApplicationManager.Current.IsInDeveloperMode)
            {
                ElementsCollection.AddRange(internalElementsGenerator.GenerateInternalIgnore(ElementsCollection));
            }
            InitializeItemDetails(coreElements);
            if (Debugger.IsAttached)
            {
                IEnumerable<ElementBase> enumerable = ElementsCollection.Where((ElementBase x) => !ElementsHelper.ValidateID(x.Id) && !x.Type.Equals("Ignore"));
                Logger.Warning($"found {enumerable.Count()} invalid IDs");
                foreach (ElementBase item11 in enumerable)
                {
                    Logger.Warning($"invalid ID on {item11} [{item11.Id}]");
                }
            }
            IsElementsCollectionPopulated = true;
            return coreElements;
        }

        private void InitializeItemDetails(ElementBaseCollection collection)
        {
            foreach (Item item in collection.Where((ElementBase x) => x.Type.Equals("Weapon")).Cast<Item>())
            {
                List<string> list = new List<string>();
                List<string> list2 = new List<string>();
                List<string> list3 = new List<string>();
                foreach (string support in item.Supports)
                {
                    ElementBase elementBase = collection.FirstOrDefault((ElementBase x) => x.Id.Equals(support));
                    if (elementBase == null)
                    {
                        continue;
                    }
                    switch (elementBase.Type)
                    {
                        case "Weapon Category":
                            list.Add(elementBase.Name);
                            break;
                        case "Weapon Group":
                            list2.Add(elementBase.Name.Replace("Weapon Group (", "").Trim(')', ' '));
                            break;
                        case "Weapon Property":
                            if (!elementBase.Name.Equals("Special"))
                            {
                                list3.Add(elementBase.Name);
                            }
                            break;
                    }
                }
                item.WeaponGroups.AddRange(list2.OrderBy((string x) => x));
                item.WeaponProperties.AddRange(list3.OrderBy((string x) => x));
                item.Keywords.AddRange(from x in list3
                                       orderby x
                                       select x.Trim().ToLower());
                item.Keywords.AddRange(from x in list
                                       orderby x
                                       select x.Trim().ToLower());
            }
            foreach (Item item2 in collection.Where((ElementBase x) => x.Type.Equals("Armor")).Cast<Item>())
            {
                List<string> list4 = new List<string>();
                foreach (string support2 in item2.Supports)
                {
                    ElementBase elementBase2 = collection.FirstOrDefault((ElementBase x) => x.Id.Equals(support2));
                    if (elementBase2 != null)
                    {
                        string type = elementBase2.Type;
                        if (type != null && type == "Armor Group")
                        {
                            list4.Add(elementBase2.Name.Replace("Armor Group (", "").Trim(')', ' '));
                        }
                    }
                }
                item2.ArmorGroups.AddRange(list4.OrderBy((string x) => x));
            }
        }

        [Obsolete("this method creates copies which is not used yet")]
        public IEnumerable<ElementBase> GetDataElements()
        {
            return ElementsCollection.Select((ElementBase element) => element.Copy()).ToList();
        }

        public CharacterFile LoadCharacterFile(string filepath)
        {
            CharacterFile characterFile = new CharacterFile(filepath);
            characterFile.InitializeDisplayPropertiesFromFilePath();
            return characterFile;
        }

        public IEnumerable<CharacterFile> LoadCharacterFiles()
        {
            List<FileInfo> files = GetFiles(UserDocumentsRootDirectory, "*.dnd5e", includeSubdirectories: false);
            foreach (string characterFileLocation in GetCharacterFileLocations())
            {
                FileInfo fileInfo = new FileInfo(characterFileLocation);
                if (fileInfo.Exists && !files.Select((FileInfo x) => x.FullName).Contains(fileInfo.FullName))
                {
                    files.Add(new FileInfo(characterFileLocation));
                }
            }
            List<CharacterFile> list = new List<CharacterFile>();
            foreach (FileInfo item in files)
            {
                try
                {
                    list.Add(LoadCharacterFile(item.FullName));
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "LoadCharacterFiles");
                    MessageDialogService.ShowException(ex);
                }
            }
            return list;
        }

        public string GetCombinedCharacterFilePath(string filename, string directory = null)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidFileNameChars)
            {
                filename = filename.Replace(c.ToString(), "");
            }
            if (directory == null)
            {
                return Path.Combine(UserDocumentsRootDirectory, filename + ".dnd5e");
            }
            return Path.Combine(directory, filename + ".dnd5e");
        }

        private List<string> GetCharacterFileLocations()
        {
            string path = Path.Combine(LocalAppDataRootDirectory, "characters.aurora");
            if (File.Exists(path))
            {
                return File.ReadAllLines(path).ToList();
            }
            return new List<string>();
        }

        private void SaveCharacterFileLocations(IEnumerable<string> paths)
        {
            File.WriteAllLines(Path.Combine(LocalAppDataRootDirectory, "characters.aurora"), paths);
        }

        public void AppendCharacterFileLocation(string path)
        {
            List<string> characterFileLocations = GetCharacterFileLocations();
            if (!characterFileLocations.Contains(path))
            {
                characterFileLocations.Add(path);
            }
            SaveCharacterFileLocations(characterFileLocations);
        }

        public void RemoveNonExistingCharacterFileLocations()
        {
            List<string> characterFileLocations = GetCharacterFileLocations();
            foreach (string item in characterFileLocations.ToList())
            {
                if (!File.Exists(item))
                {
                    characterFileLocations.Remove(item);
                }
            }
            SaveCharacterFileLocations(characterFileLocations);
        }

        public string GetResourceWebDocument(string resourceFilename)
        {
            string name = "Builder.Presentation.Resources.Documents." + resourceFilename;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    return null;
                }
                using (StreamReader streamReader = new StreamReader(stream, Encoding.Default))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public void CopyElementsFromResources()
        {
            foreach (string item in from x in Assembly.GetExecutingAssembly().GetManifestResourceNames()
                                    where x.StartsWith("Builder.Presentation.Resources.Data.ApplicationElements")
                                    select x)
            {
                Logger.Info("Getting Resource Stream: {0}", item);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(item))
                {
                    if (stream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(stream, Encoding.Default))
                        {
                            string contents = streamReader.ReadToEnd();
                            string path = item.Replace("Builder.Presentation.Resources.Data.ApplicationElements.", "");
                            File.WriteAllText(Path.Combine(LocalAppDataApplicationElementsDirectory, path), contents);
                        }
                    }
                }
            }
        }

        public void CopyPortraitsFromResources()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (string item in from x in executingAssembly.GetManifestResourceNames()
                                    where x.StartsWith("Builder.Presentation.Resources.SamplePortraits")
                                    select x)
            {
                string text = item.Replace("Builder.Presentation.Resources.SamplePortraits.", "");
                string text2 = Path.Combine(UserDocumentsPortraitsDirectory, text);
                if (File.Exists(text2))
                {
                    continue;
                }
                using (Stream stream = executingAssembly.GetManifestResourceStream(item))
                {
                    if (stream != null)
                    {
                        using (Image image = Image.FromStream(stream))
                        {
                            image.Save(text2);
                        }
                        Logger.Info("Embedded portrait image '{0}' copied to the portraits directory", text);
                    }
                }
            }
            CopyNewPortraitsFromResources();
        }

        public void CopyNewPortraitsFromResources()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (string item in from x in executingAssembly.GetManifestResourceNames()
                                    where x.StartsWith("Builder.Presentation.Resources.Portraits")
                                    select x)
            {
                string text = item.Replace("Builder.Presentation.Resources.Portraits.", "");
                string text2 = Path.Combine(UserDocumentsPortraitsDirectory, text);
                if (File.Exists(text2))
                {
                    continue;
                }
                using (Stream stream = executingAssembly.GetManifestResourceStream(item))
                {
                    if (stream != null)
                    {
                        using (Image image = Image.FromStream(stream))
                        {
                            image.Save(text2);
                        }
                        Logger.Info("Embedded portrait image '{0}' copied to the portraits directory", text);
                    }
                }
            }
        }

        public void CopyCompanionPortraitsFromResources()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (string item in from x in executingAssembly.GetManifestResourceNames()
                                    where x.StartsWith("Builder.Presentation.Resources.Gallery.CompanionPortraits")
                                    select x)
            {
                string text = item.Replace("Builder.Presentation.Resources.Gallery.CompanionPortraits.", "");
                string text2 = Path.Combine(UserDocumentsCompanionGalleryDirectory, text);
                if (File.Exists(text2))
                {
                    continue;
                }
                using (Stream stream = executingAssembly.GetManifestResourceStream(item))
                {
                    if (stream != null)
                    {
                        using (Image image = Image.FromStream(stream))
                        {
                            image.Save(text2);
                        }
                        Logger.Info("Embedded companion portrait image '{0}' copied to the portraits directory", text);
                    }
                }
            }
        }

        public void CopySymbolsFromResources()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (string item in from x in executingAssembly.GetManifestResourceNames()
                                    where x.StartsWith("Builder.Presentation.Resources.Gallery.OrganizationSymbols")
                                    select x)
            {
                string text = item.Replace("Builder.Presentation.Resources.Gallery.OrganizationSymbols.", "");
                string text2 = Path.Combine(UserDocumentsSymbolsGalleryDirectory, text);
                if (File.Exists(text2))
                {
                    continue;
                }
                using (Stream stream = executingAssembly.GetManifestResourceStream(item))
                {
                    if (stream != null)
                    {
                        using (Image image = Image.FromStream(stream))
                        {
                            image.Save(text2);
                        }
                        Logger.Info("Embedded organization symbol image '{0}' copied to the gallery directory", text);
                    }
                }
            }
        }

        public void CopyDragonmarksFromResources()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (string item in from x in executingAssembly.GetManifestResourceNames()
                                    where x.StartsWith("Builder.Presentation.Resources.Gallery.Dragonmarks")
                                    select x)
            {
                string text = item.Replace("Builder.Presentation.Resources.Gallery.Dragonmarks.", "");
                string text2 = Path.Combine(UserDocumentsSymbolsGalleryDirectory, text);
                if (File.Exists(text2))
                {
                    continue;
                }
                using (Stream stream = executingAssembly.GetManifestResourceStream(item))
                {
                    if (stream != null)
                    {
                        using (Image image = Image.FromStream(stream))
                        {
                            image.Save(text2);
                        }
                        Logger.Info("Embedded dragonmark image '{0}' copied to the gallery directory", text);
                    }
                }
            }
        }

        [Obsolete]
        private void CopySpellcardBackground(Assembly assembly)
        {
            string filename = Path.Combine(LocalAppDataRootDirectory, "spellcard-background.jpg");
            using (Stream stream = assembly.GetManifestResourceStream("Builder.Presentation.Resources.spellcard-background.jpg"))
            {
                if (stream != null)
                {
                    using (Image image = Image.FromStream(stream))
                    {
                        image.Save(filename);
                    }
                    Logger.Info("Embedded spellcard background image '{0}' copied to the local app directory", "spellcard-background.jpg");
                }
            }
        }

        public List<XmlDocument> LoadElementDocumentsFromResource()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            IEnumerable<string> enumerable = from x in executingAssembly.GetManifestResourceNames()
                                             where x.StartsWith("Builder.Presentation.Resources.Data")
                                             select x;
            List<XmlDocument> list = new List<XmlDocument>();
            foreach (string item in enumerable)
            {
                if (!item.EndsWith(".xml"))
                {
                    Logger.Warning("loading non-elements resource file: '{0}'", item);
                }
                using (Stream stream = executingAssembly.GetManifestResourceStream(item))
                {
                    if (stream != null)
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(stream);
                        list.Add(xmlDocument);
                    }
                }
            }
            return list;
        }

        private static List<FileInfo> GetFiles(string directory, string pattern = "*.*", bool includeSubdirectories = true)
        {
            Logger.Info("getting files from {0} using the {1} pattern", directory, pattern);
            List<FileInfo> list = new List<FileInfo>();
            list.AddRange(from f in Directory.GetFiles(directory, pattern)
                          select new FileInfo(f));
            if (includeSubdirectories)
            {
                string[] directories = Directory.GetDirectories(directory);
                foreach (string directory2 in directories)
                {
                    list.AddRange(GetFiles(directory2, pattern));
                }
            }
            return list;
        }

        private List<FileInfo> GetCustomFiles()
        {
            List<FileInfo> customFiles = GetCustomFiles(UserDocumentsCustomElementsDirectory);
            string additionalCustomDirectory = Settings.Default.AdditionalCustomDirectory;
            if (!string.IsNullOrWhiteSpace(additionalCustomDirectory) && Directory.Exists(additionalCustomDirectory))
            {
                List<FileInfo> customFiles2 = GetCustomFiles(additionalCustomDirectory);
                customFiles.AddRange(customFiles2);
            }
            return customFiles;
        }

        private List<FileInfo> GetCustomFiles(string path)
        {
            try
            {
                List<FileInfo> files = GetFiles(path, "*.xml");
                List<FileInfo> includedFiles = new List<FileInfo>();
                List<FileInfo> list = files.Where((FileInfo x) => !x.FullName.Contains("custom\\ignore")).ToList();
                includedFiles.AddRange(list.Where((FileInfo x) => x.FullName.StartsWith(Path.Combine(path, "srd"))));
                includedFiles.AddRange(list.Where((FileInfo x) => x.FullName.StartsWith(Path.Combine(path, "system-reference-document"))));
                list.RemoveAll((FileInfo info) => includedFiles.Contains(info));
                includedFiles.AddRange(list.Where((FileInfo x) => x.FullName.StartsWith(Path.Combine(path, "core"))));
                includedFiles.AddRange(list.Where((FileInfo x) => x.FullName.StartsWith(Path.Combine(path, "supplements"))));
                includedFiles.AddRange(list.Where((FileInfo x) => x.FullName.StartsWith(Path.Combine(path, "unearthed-arcana"))));
                includedFiles.AddRange(list.Where((FileInfo x) => x.FullName.StartsWith(Path.Combine(path, "third-party"))));
                includedFiles.AddRange(list.Where((FileInfo x) => x.FullName.StartsWith(Path.Combine(path, "homebrew"))));
                list.RemoveAll((FileInfo info) => includedFiles.Contains(info));
                List<FileInfo> user = list.Where((FileInfo x) => x.Directory != null && x.FullName.StartsWith(Path.Combine(path, "user")) && x.Directory.Name.Equals("user")).ToList();
                list.RemoveAll((FileInfo info) => user.Contains(info));
                List<FileInfo> userIndices = list.Where((FileInfo x) => x.Directory != null && x.FullName.StartsWith(Path.Combine(path, "user"))).ToList();
                list.RemoveAll((FileInfo info) => userIndices.Contains(info));
                List<FileInfo> root = list.Where((FileInfo x) => x.DirectoryName != null && x.DirectoryName.EndsWith("custom", StringComparison.OrdinalIgnoreCase)).ToList();
                list.RemoveAll((FileInfo info) => root.Contains(info));
                includedFiles.AddRange(list);
                list.RemoveAll((FileInfo info) => includedFiles.Contains(info));
                includedFiles.AddRange(userIndices);
                includedFiles.AddRange(root);
                includedFiles.AddRange(user);
                _ = includedFiles.Count;
                _ = files.Count;
                return includedFiles;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GetCustomFiles");
            }
            Logger.Warning("Getting all files and do not ignore any files or return them in any order");
            return GetFiles(path, "*.xml");
        }

        private static void CreateDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                Logger.Info("Creating Directory: '{0}'", directoryName);
                Directory.CreateDirectory(directoryName);
            }
        }

        private static async Task<XmlDocument> CreateXmlDocument(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                string xml = await reader.ReadToEndAsync();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                return xmlDocument;
            }
        }

        public static int GetPercentage(double count, double totalCount)
        {
            return (int)Math.Round(100.0 * count / totalCount);
        }

        private void AppendElements(IEnumerable<XmlNode> appendNodes, ElementBaseCollection coreElements, ElementParser elementParser, ElementParser defaultParser, List<ElementParser> elementParserCollection)
        {
            foreach (XmlNode appendNode in appendNodes)
            {
                if (!appendNode.ContainsAttribute("id"))
                {
                    continue;
                }
                string appendId = appendNode.GetAttributeValue("id");
                string appendType = (appendNode.ContainsAttribute("type") ? appendNode.GetAttributeValue("type") : "");
                if (elementParser.ParserType != appendType)
                {
                    elementParser = elementParserCollection.FirstOrDefault((ElementParser p) => p.ParserType == appendType) ?? defaultParser;
                }
                ElementBase elementBase = coreElements.FirstOrDefault((ElementBase x) => x.Id == appendId);
                if (elementBase == null)
                {
                    Logger.Warning("unable to extend non existing element: " + appendId);
                    continue;
                }
                try
                {
                    ElementBase elementBase2 = elementParser.ParseElement(appendNode, elementBase.ElementHeader);
                    bool flag = false;
                    if (elementBase2.HasSupports)
                    {
                        foreach (string support in elementBase2.Supports)
                        {
                            if (!elementBase.Supports.Contains(support))
                            {
                                elementBase.Supports.Add(support);
                            }
                        }
                        flag = true;
                    }
                    if (elementBase2.ElementSetters.Any())
                    {
                        foreach (ElementSetters.Setter elementSetter in elementBase2.ElementSetters)
                        {
                            if (!elementBase.ElementSetters.ContainsSetter(elementSetter.Name))
                            {
                                elementBase.ElementSetters.Add(elementSetter);
                            }
                        }
                        flag = true;
                    }
                    if (elementBase2.HasRules)
                    {
                        elementBase.Rules.AddRange(elementBase2.Rules);
                        flag = true;
                    }
                    if (elementBase2.HasSpellcastingInformation)
                    {
                        if (!elementBase.HasSpellcastingInformation)
                        {
                            elementBase.SpellcastingInformation = elementBase2.SpellcastingInformation;
                            flag = true;
                        }
                        else
                        {
                            Logger.Warning("unable to extend spellcasting on existing spellcasting at: " + appendId);
                        }
                    }
                    if (flag)
                    {
                        elementBase.IsExtended = true;
                    }
                }
                catch (Exception value)
                {
                    Console.WriteLine(value);
                    throw;
                }
            }
        }
    }
}
