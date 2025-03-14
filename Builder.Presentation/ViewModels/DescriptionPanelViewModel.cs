using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Events.Global;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media;
using System.Xml;

namespace Builder.Presentation.ViewModels
{
    [Obsolete("old descrition panel view model - use DescriptionPanelViewModelBase")]
    public class DescriptionPanelViewModel : ViewModelBase, ISubscriber<ExpanderHandlerSelectionChanged>, ISubscriber<ResourceDocumentDisplayRequestEvent>, ISubscriber<SettingsChangedEvent>
    {
        protected readonly string _originalSheet;

        private string _styleSheet;

        private string _elementDescription;

        public string StyleSheet
        {
            get
            {
                return _styleSheet;
            }
            set
            {
                SetProperty(ref _styleSheet, value, "StyleSheet");
            }
        }

        public string ElementDescription
        {
            get
            {
                return _elementDescription;
            }
            set
            {
                SetProperty(ref _elementDescription, value, "ElementDescription");
            }
        }

        public ElementBase CurrentElement { get; set; }

        public DescriptionPanelViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            _originalSheet = DataManager.Current.GetResourceWebDocument("stylesheet.css");
            _styleSheet = DataManager.Current.GetResourceWebDocument("stylesheet.css");
            base.EventAggregator.Subscribe(this);
        }

        public virtual void OnHandleEvent(ExpanderHandlerSelectionChanged args)
        {
            CurrentElement = args.SelectedElement;
            if (args.SelectedElement == null)
            {
                return;
            }
            if (args.SelectedElement.HasGeneratedDescription)
            {
                ElementDescription = args.SelectedElement.GeneratedDescription;
                base.EventAggregator.Send(new ElementDescriptionDisplayRequestEvent(args.SelectedElement));
                return;
            }
            if (args.SelectedElement.Description.Contains("<br>"))
            {
                Logger.Warning("the description of '" + args.SelectedElement.Name + "' contains <br> tags, please use <br/> since it's loaded into a xmldocument");
                args.SelectedElement.Description = args.SelectedElement.Description.Replace("<br>", "<br/>");
            }
            InitializeStyleSheet(Builder.Presentation.Properties.Settings.Default.Accent);
            StringBuilder stringBuilder = new StringBuilder("<h2>" + args.SelectedElement.Name.ToUpper() + "</h2>");
            switch (args.SelectedElement.Type)
            {
                case "Spell":
                    {
                        Spell spell = (Spell)args.SelectedElement;
                        stringBuilder.AppendLine("<p><i>" + spell.GetShortDescription() + "</i></p>");
                        stringBuilder.AppendLine("<p>");
                        stringBuilder.AppendLine("<b>Casting Time:</b> " + spell.CastingTime + "<br/>");
                        stringBuilder.AppendLine("<b>Range:</b> " + spell.Range + "<br/>");
                        stringBuilder.AppendLine("<b>Components:</b> " + spell.GetComponentsString() + "<br/>");
                        stringBuilder.AppendLine("<b>Duration:</b> " + spell.Duration + "<br/>");
                        stringBuilder.AppendLine("</p>");
                        break;
                    }
                case "Item":
                    {
                        Item item = (Item)args.SelectedElement;
                        stringBuilder.AppendLine("<p>");
                        stringBuilder.AppendLine("<b>Category:</b> " + item.Category + "<br/>");
                        stringBuilder.AppendLine($"<b>Cost:</b> {item.Cost} {item.CurrencyAbbreviation}<br/>");
                        stringBuilder.AppendLine("<b>Weight:</b> " + item.Weight + "<br/>");
                        stringBuilder.AppendLine("</p>");
                        break;
                    }
                case "Companion":
                    {
                        MonsterElement monsterElement = (MonsterElement)args.SelectedElement;
                        stringBuilder.AppendLine("<p>");
                        stringBuilder.AppendLine("<b>Alignment:</b> " + monsterElement.Alignment + "<br/>");
                        stringBuilder.AppendLine("</p>");
                        break;
                    }
            }
            AppendDescription(stringBuilder, args.SelectedElement);
            if (string.IsNullOrWhiteSpace(args.SelectedElement.SourceUrl))
            {
                args.SelectedElement.SourceUrl = GetSourceUrl(args.SelectedElement);
                if (string.IsNullOrWhiteSpace(args.SelectedElement.SourceUrl) && Builder.Presentation.Properties.Settings.Default.SearchMissingSourceOnline)
                {
                    args.SelectedElement.SourceUrl = "https://www.google.com/search?q=" + WebUtility.UrlEncode(args.SelectedElement.Source) + "+" + WebUtility.UrlEncode(args.SelectedElement.Name);
                }
            }
            stringBuilder.Append(string.IsNullOrWhiteSpace(args.SelectedElement.SourceUrl) ? ("<h6>SOURCE</h6><p class=\"flavor\">" + args.SelectedElement.Source + "</p>") : ("<h6>SOURCE</h6><p class=\"flavor\"><a href=\"" + args.SelectedElement.SourceUrl + "\">" + args.SelectedElement.Source + "</a></p>"));
            ElementDescription = $"<body>{stringBuilder}</body>";
            args.SelectedElement.GeneratedDescription = ElementDescription;
            base.EventAggregator.Send(new ElementDescriptionDisplayRequestEvent(args.SelectedElement));
        }

        private void AppendDescription(StringBuilder documentBuilder, ElementBase element)
        {
            if (element.Description.Contains("<div element="))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml("<div>" + element.Description + "</div>");
                if (xmlDocument.DocumentElement != null)
                {
                    foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
                    {
                        if (childNode.Name == "div" && childNode.ContainsAttribute("element"))
                        {
                            string injectedElementId = childNode.GetAttributeValue("element");
                            ElementBase elementBase = DataManager.Current.ElementsCollection.FirstOrDefault((ElementBase x) => x.Id == injectedElementId);
                            if (elementBase == null)
                            {
                                Logger.Warning("the injected element '" + element.Name + "' with the id '" + injectedElementId + "' is not found.");
                            }
                            else
                            {
                                childNode.InnerXml = "<h5 class=\"h5-enhance\">" + elementBase.Name.ToUpper() + "</h5>";
                                childNode.InnerXml += elementBase.Description;
                            }
                        }
                    }
                }
                documentBuilder.AppendLine(xmlDocument.InnerXml);
            }
            else
            {
                documentBuilder.AppendLine(element.Description);
            }
        }

        public virtual void OnHandleEvent(ResourceDocumentDisplayRequestEvent args)
        {
            InitializeStyleSheet(Builder.Presentation.Properties.Settings.Default.Accent);
            string resourceWebDocument = DataManager.Current.GetResourceWebDocument(args.ResourceFilename);
            if (!resourceWebDocument.Contains("<body>"))
            {
                Logger.Warning("the contents of '" + args.ResourceFilename + "' needs to be in a <body> tag");
            }
            ElementDescription = (resourceWebDocument.Contains("<body>") ? resourceWebDocument : ("<body>" + resourceWebDocument + "</body>"));
        }

        private static string GetSourceUrl(ElementBase element)
        {
            string result = string.Empty;
            switch (element.Source)
            {
                case "Player’s Handbook":
                case "Player's Handbook":
                case "Players Handbook":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/rpg_playershandbook#content";
                    break;
                case "Dungeon Master’s Guide":
                case "Dungeon Master's Guide":
                case "Dungeon Masters Guide":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/dungeon-masters-guide#content";
                    break;
                case "Sword Coast Adventurer’s Guide":
                case "Sword Coast Adventurer's Guide":
                case "Sword Coast Adventurers Guide":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/sc-adventurers-guide#content";
                    break;
                case "Princes of the Apocalypse":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/princes-apocalypse#content";
                    break;
                case "Monster Manual":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/monster-manual#content";
                    break;
                case "System Reference Document":
                    result = "http://dnd.wizards.com/articles/features/systems-reference-document-srd";
                    break;
                case "Volo’s Guide to Monsters":
                case "Volo's Guide to Monsters":
                case "Volos Guide to Monsters":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/volos-guide-to-monsters";
                    break;
                case "Storm King's Thunder":
                case "Storm King’s Thunder":
                case "Storm Kings Thunder":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/storm-kings-thunder";
                    break;
                case "Curse of Strahd":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/curse-strahd";
                    break;
                case "Out of the Abyss":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/outoftheabyss";
                    break;
                case "Tales from the Yawning Portal":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/tales-yawning-portal";
                    break;
                case "Tomb of Annihilation":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/tomb-annihilation";
                    break;
                case "Xanathar's Guide to Everything":
                case "Xanathar’s Guide to Everything":
                case "Xanathars Guide to Everything":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/xanathars-guide-everything ";
                    break;
                case "Mordenkainen's Tome of Foes":
                case "Mordenkainen’s Tome of Foes":
                case "Mordenkainens Tome of Foes":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/mordenkainens-tome-foes";
                    break;
                case "Waterdeep Dragon Heist":
                case "Dragon Heist":
                case "Waterdeep: Dragon Heist":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/dragonheist";
                    break;
                case "Waterdeep Dungeon of the Mad Mage":
                case "Dungeon of the Mad Mage":
                case "Waterdeep: Dungeon of the Mad Mage":
                    result = "http://dnd.wizards.com/products/tabletop-games/rpg-products/waterdeep-dungeon-mad-mage";
                    break;
                default:
                    Logger.Info("no source url available for '" + element.Name + "'");
                    break;
            }
            return result;
        }

        protected override void InitializeDesignData()
        {
            string resourceWebDocument = DataManager.Current.GetResourceWebDocument("description-panel-design-data.html");
            ElementDescription = resourceWebDocument;
        }

        [Obsolete]
        protected virtual void InitializeStyleSheet(string accentName)
        {
            string text = ApplicationManager.Current.GetHighlightColor().ToString();
            new SolidColorBrush(Color.FromRgb(1, 2, 3));
            StyleSheet = _originalSheet.Replace("2A363B", text.Substring(3, 6));
        }

        void ISubscriber<SettingsChangedEvent>.OnHandleEvent(SettingsChangedEvent args)
        {
            InitializeStyleSheet(args.Settings.Accent);
        }
    }
}
