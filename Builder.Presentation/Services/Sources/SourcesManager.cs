using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation.Elements;
using Builder.Presentation.Models.Sources;
using Builder.Presentation.Properties;
using Builder.Presentation.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
namespace Builder.Presentation.Services.Sources
{
    public class SourcesManager : ISourceRestrictionsProvider
    {
        public const string WizardsGroupName = "Wizards of the Coast";

        public const string AdventurersLeagueGroupName = "Adventurers League";

        public const string UnearthedArcanaGroupName = "Unearthed Arcana";

        public const string ThirdPartyGroupName = "Third Party";

        public const string HomebrewGroupName = "Homebrew";

        public const string UndefinedSources = "Undefined Sources";

        public ObservableCollection<SourcesGroup> SourceGroups { get; } = new ObservableCollection<SourcesGroup>();

        public ObservableCollection<SourceItem> SourceItems { get; } = new ObservableCollection<SourceItem>();

        public ObservableCollection<SourceItem> RestrictedSources { get; } = new ObservableCollection<SourceItem>();

        public event EventHandler SourceRestrictionsApplied;

        public SourcesManager()
        {
            InitializeSources();
        }

        public void ApplyRestrictions(bool reprocess = false)
        {
            RestrictedSources.Clear();
            foreach (SourcesGroup sourceGroup in SourceGroups)
            {
                if (!sourceGroup.AllowUnchecking || sourceGroup.IsChecked == true)
                {
                    continue;
                }
                if (sourceGroup.IsChecked == false)
                {
                    foreach (SourceItem source in sourceGroup.Sources)
                    {
                        if (source.IsChecked != true)
                        {
                            RestrictedSources.Add(source);
                        }
                    }
                }
                if (sourceGroup.IsChecked.HasValue)
                {
                    continue;
                }
                foreach (SourceItem item in sourceGroup.Sources.Where((SourceItem x) => x.IsChecked == false))
                {
                    RestrictedSources.Add(item);
                }
            }
            ApplicationManager.Current.SendStatusMessage("Your source restrictions have been updated.");
            OnSourceRestrictionsApplied();
            if (reprocess)
            {
                CharacterManager.Current.ReprocessCharacter();
            }
        }

        public void ClearRestrictions(bool apply = true, bool reprocess = false)
        {
            foreach (SourcesGroup sourceGroup in SourceGroups)
            {
                sourceGroup.SetIsChecked(true, updateChildren: true);
            }
            if (apply)
            {
                ApplyRestrictions(reprocess);
            }
        }

        public IEnumerable<string> GetUndefinedRestrictedSourceNames()
        {
            return from x in RestrictedSources
                   where x.Parent.Name.Equals("Undefined Sources", StringComparison.OrdinalIgnoreCase)
                   select x.Source.Name;
        }

        public IEnumerable<string> GetRestrictedElementIds()
        {
            List<string> list = new List<string>();
            foreach (SourceItem restrictedSource in RestrictedSources)
            {
                list.AddRange(restrictedSource.Elements.Select((ElementHeader x) => x.Id));
            }
            return list;
        }

        private void InitializeSources()
        {
            foreach (SourceItem item in from Source x in DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Source", StringComparison.OrdinalIgnoreCase))
                                        orderby x.ReleaseDate, x.Name
                                        select new SourceItem(x.Copy()))
            {
                SourceItems.Add(item);
            }
            foreach (SourcesGroup item2 in CreateGroups())
            {
                SourceGroups.Add(item2);
            }
        }

        private IEnumerable<SourcesGroup> CreateGroups()
        {
            List<SourcesGroup> list = new List<SourcesGroup>();
            SourcesGroup sourcesGroup = new SourcesGroup("Wizards of the Coast");
            SourcesGroup sourcesGroup2 = new SourcesGroup("Adventurers League");
            SourcesGroup sourcesGroup3 = new SourcesGroup("Unearthed Arcana");
            SourcesGroup sourcesGroup4 = new SourcesGroup("Third Party");
            SourcesGroup sourcesGroup5 = new SourcesGroup("Homebrew");
            SourcesGroup sourcesGroup6 = new SourcesGroup("Undefined Sources");
            Queue<SourceItem> queue = new Queue<SourceItem>();
            foreach (SourceItem sourceItem2 in SourceItems)
            {
                if (sourceItem2.Source.IsOfficialContent)
                {
                    if (sourceItem2.Source.IsAdventureLeagueContent)
                    {
                        sourcesGroup2.Sources.Add(sourceItem2);
                    }
                    else if (sourceItem2.Source.IsPlaytestContent && !sourceItem2.Source.IsCoreContent && !sourceItem2.Source.IsSupplementContent)
                    {
                        sourcesGroup3.Sources.Add(sourceItem2);
                    }
                    else
                    {
                        queue.Enqueue(sourceItem2);
                    }
                }
                else if (sourceItem2.Source.IsThirdPartyContent)
                {
                    sourcesGroup4.Sources.Add(sourceItem2);
                }
                else if (sourceItem2.Source.IsHomebrewContent)
                {
                    sourcesGroup5.Sources.Add(sourceItem2);
                }
                else
                {
                    sourcesGroup6.Sources.Add(sourceItem2);
                }
            }
            List<SourceItem> list2 = new List<SourceItem>();
            List<SourceItem> list3 = new List<SourceItem>();
            List<SourceItem> list4 = new List<SourceItem>();
            while (queue.Any())
            {
                SourceItem sourceItem = queue.Dequeue();
                if (sourceItem.Source.IsCoreContent)
                {
                    sourceItem.AllowUnchecking = false;
                    list2.Add(sourceItem);
                }
                else if (sourceItem.Source.IsSupplementContent)
                {
                    list3.Add(sourceItem);
                }
                else
                {
                    list4.Add(sourceItem);
                }
            }
            foreach (SourceItem item in list2)
            {
                sourcesGroup.Sources.Add(item);
            }
            foreach (SourceItem item2 in list3)
            {
                sourcesGroup.Sources.Add(item2);
            }
            foreach (SourceItem item3 in list4)
            {
                sourcesGroup.Sources.Add(item3);
            }
            list.Add(sourcesGroup);
            if (sourcesGroup2.Sources.Any())
            {
                list.Add(sourcesGroup2);
            }
            if (sourcesGroup3.Sources.Any())
            {
                list.Add(sourcesGroup3);
            }
            if (sourcesGroup4.Sources.Any())
            {
                List<SourceItem> list5 = (from x in sourcesGroup4.Sources
                                          orderby x.Source.Author, x.Source.ReleaseDate, x.Source.Name
                                          select x).ToList();
                sourcesGroup4.Sources.Clear();
                foreach (SourceItem item4 in list5)
                {
                    sourcesGroup4.Sources.Add(item4);
                }
                list.Add(sourcesGroup4);
            }
            if (sourcesGroup5.Sources.Any())
            {
                List<SourceItem> list6 = (from x in sourcesGroup5.Sources
                                          orderby x.Source.Author, x.Source.ReleaseDate, x.Source.Name
                                          select x).ToList();
                sourcesGroup5.Sources.Clear();
                foreach (SourceItem item5 in list6)
                {
                    sourcesGroup5.Sources.Add(item5);
                }
                list.Add(sourcesGroup5);
            }
            GetUndefinedSourceNames(sourcesGroup6);
            if (sourcesGroup6.Sources.Any())
            {
                list.Add(sourcesGroup6);
            }
            foreach (SourcesGroup item6 in list)
            {
                foreach (SourceItem source in item6.Sources)
                {
                    source.SetParent(item6);
                }
                item6.SetIsChecked(true, updateChildren: true);
            }
            return list;
        }

        private IEnumerable<string> GetUndefinedSourceNames(SourcesGroup undefinedGroup)
        {
            string[] source = new string[2] { "internal", "core" };
            IEnumerable<ElementBase> enumerable = DataManager.Current.ElementsCollection.Where((ElementBase x) => !x.Type.Equals("Source") && !x.Type.Equals("Internal") && !x.Type.Equals("Core") && !x.Type.Equals("Ability Score Improvement") && !x.Type.Equals("Level") && !x.Type.Equals("Multiclass") && !x.Type.Equals("Skill") && !x.Type.Equals("Support"));
            List<string> source2 = SourceItems.Select((SourceItem x) => x.Source.Name).ToList();
            List<string> list = new List<string>();
            foreach (ElementBase item in enumerable)
            {
                string elementSourceName = item.Source;
                if (!source.Contains(elementSourceName, StringComparer.OrdinalIgnoreCase))
                {
                    SourceItems.FirstOrDefault((SourceItem x) => x.Source.Name.Equals(elementSourceName, StringComparison.OrdinalIgnoreCase))?.Elements.Add(item.ElementHeader);
                    if (!list.Contains(elementSourceName, StringComparer.OrdinalIgnoreCase) && !source2.Contains(elementSourceName, StringComparer.OrdinalIgnoreCase))
                    {
                        list.Add(elementSourceName);
                    }
                }
            }
            foreach (string item2 in list)
            {
                Source source3 = new Source
                {
                    ElementHeader = new ElementHeader(item2, "Source", item2, item2),
                    Author = "Missing Source Details",
                    Description = "<p>Create an element of type 'Source' so it can be classified with a proper description.</p>"
                };
                undefinedGroup.Sources.Add(new SourceItem(source3));
            }
            return list;
        }

        public void Load(IEnumerable<string> sources)
        {
            ClearRestrictions(apply: false);
            foreach (SourcesGroup sourceGroup in SourceGroups)
            {
                foreach (SourceItem source in sourceGroup.Sources)
                {
                    if (sources.Contains(source.Source.Id))
                    {
                        source.SetIsChecked(false, updateChildren: false, updateParent: true);
                    }
                }
            }
            ApplyRestrictions();
        }

        public void LoadDefaults()
        {
            try
            {
                string defaultSourceRestrictions = Settings.Default.DefaultSourceRestrictions;
                if (!string.IsNullOrWhiteSpace(defaultSourceRestrictions))
                {
                    string[] sources = defaultSourceRestrictions.Split(',');
                    Load(sources);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "LoadDefaults");
                MessageDialogService.ShowException(ex);
            }
        }

        public void StoreDefaults()
        {
            List<string> list = new List<string>();
            foreach (SourcesGroup sourceGroup in SourceGroups)
            {
                if (!sourceGroup.AllowUnchecking || sourceGroup.IsChecked == true)
                {
                    continue;
                }
                if (sourceGroup.IsChecked == false)
                {
                    foreach (SourceItem source in sourceGroup.Sources)
                    {
                        list.Add(source.Source.Id);
                    }
                }
                if (sourceGroup.IsChecked.HasValue)
                {
                    continue;
                }
                foreach (SourceItem item in sourceGroup.Sources.Where((SourceItem x) => x.IsChecked == false))
                {
                    list.Add(item.Source.Id);
                }
            }
            Settings.Default.DefaultSourceRestrictions = string.Join(",", list);
            ApplicationManager.Current.SendStatusMessage("Your default source restrictions have been saved.");
        }

        public IEnumerable<ElementBase> GetOrderedElements(IEnumerable<ElementBase> elements)
        {
            List<Source> source = SourceItems.Select((SourceItem x) => x.Source).ToList();
            List<Source> list = (from x in source
                                 where x.IsOfficialContent
                                 orderby (!x.IsCoreContent) ? 1 : 0, x.ReleaseDate, x.Name
                                 select x).ToList();
            source.Where((Source x) => !x.IsOfficialContent).ToList();
            ElementBaseCollection elementBaseCollection = new ElementBaseCollection();
            foreach (Source source2 in list)
            {
                IEnumerable<ElementBase> elements2 = elements.Where((ElementBase x) => x.Source.Equals(source2.Name));
                elementBaseCollection.AddRange(elements2);
            }
            foreach (ElementBase element in elements)
            {
                if (!elementBaseCollection.Contains(element))
                {
                    elementBaseCollection.Add(element);
                }
            }
            if (elementBaseCollection.Count != elements.Count() && Debugger.IsAttached)
            {
                Debugger.Break();
            }
            return elementBaseCollection;
        }

        public IEnumerable<string> GetRestrictedSources()
        {
            return RestrictedSources.Select((SourceItem x) => x.Source.Name);
        }

        public IEnumerable<string> GetUndefinedRestrictedSources()
        {
            return GetUndefinedRestrictedSourceNames();
        }

        public IEnumerable<string> GetRestrictedElements()
        {
            return GetRestrictedElementIds();
        }

        protected virtual void OnSourceRestrictionsApplied()
        {
            this.SourceRestrictionsApplied?.Invoke(this, EventArgs.Empty);
        }
    }
}
