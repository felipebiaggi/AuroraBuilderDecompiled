using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Models.Sources;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Telemetry;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Input;
using System.Xml;

namespace Builder.Presentation.ViewModels
{
    public class DescriptionPanelViewModelBase : ViewModelBase, ISubscriber<ElementDescriptionDisplayRequestEvent>, ISubscriber<HtmlDisplayRequestEvent>, ISubscriber<ResourceDocumentDisplayRequestEvent>, ISubscriber<SettingsChangedEvent>, ISubscriber<CharacterLoadingCompletedEvent>
    {
        private string _styleSheet;

        private bool _isDarkStyle;

        private bool _includeSource;

        private string _description;

        private ElementBase _currentElement;

        private bool _isSpeechEnabled;

        private bool _isSpeechActive;

        private string _selectedText = "";

        public List<string> SupportedTypes { get; protected set; }

        public bool IsDarkStyle
        {
            get
            {
                return _isDarkStyle;
            }
            set
            {
                SetProperty(ref _isDarkStyle, value, "IsDarkStyle");
                StyleSheet = DataManager.Current.GetResourceWebDocument(_isDarkStyle ? "stylesheet-dark.css" : "stylesheet.css");
            }
        }

        public bool IncludeSource
        {
            get
            {
                return _includeSource;
            }
            set
            {
                SetProperty(ref _includeSource, value, "IncludeSource");
            }
        }

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

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetProperty(ref _description, value, "Description");
            }
        }

        public ElementBase CurrentElement
        {
            get
            {
                return _currentElement;
            }
            protected set
            {
                SetProperty(ref _currentElement, value, "CurrentElement");
            }
        }

        public bool IsSpeechEnabled
        {
            get
            {
                return _isSpeechEnabled;
            }
            set
            {
                SetProperty(ref _isSpeechEnabled, value, "IsSpeechEnabled");
            }
        }

        public bool IsSpeechActive
        {
            get
            {
                return _isSpeechActive;
            }
            set
            {
                SetProperty(ref _isSpeechActive, value, "IsSpeechActive");
            }
        }

        public string SelectedText
        {
            get
            {
                return _selectedText;
            }
            set
            {
                SetProperty(ref _selectedText, value, "SelectedText");
            }
        }

        public ICommand StartSpeechCommand => new RelayCommand(StartSpeech);

        public ICommand StopSpeechCommand => new RelayCommand(StopSpeech);

        public DescriptionPanelViewModelBase()
        {
            SupportedTypes = new List<string>();
            IncludeSource = true;
            IsDarkStyle = base.Settings.Settings.Theme.Contains("Dark");
            _styleSheet = DataManager.Current.GetResourceWebDocument(_isDarkStyle ? "stylesheet-dark.css" : "stylesheet.css");
            if (base.IsInDesignMode)
            {
                string resourceWebDocument = DataManager.Current.GetResourceWebDocument("description-panel-design-data.html");
                _description = "<body><h2>Design Time DWARF</h2>" + resourceWebDocument + "</body>";
                return;
            }
            Description = "";
            SubscribeWithEventAggregator();
            SpeechService.Default.SpeechStarted += _speechService_SpeechStarted;
            SpeechService.Default.SpeechStopped += _speechService_SpeechStopped;
        }

        protected virtual void HandleDisplayRequest(ElementDescriptionDisplayRequestEvent args)
        {
            CurrentElement = args.Element;
            if (CurrentElement == null || (SupportedTypes.Count > 0 && !SupportedTypes.Contains(args.Element.Type)))
            {
                return;
            }
            if (CurrentElement.HasGeneratedDescription && !args.IgnoreGeneratedDescription)
            {
                Description = CurrentElement.GeneratedDescription;
                return;
            }
            if (CurrentElement.Description.Contains("<br>"))
            {
                Logger.Warning("the description of '" + CurrentElement.Name + "' contains <br> tags, please use <br/> since it's loaded into a xml document");
                CurrentElement.Description = CurrentElement.Description.Replace("<br>", "<br/>");
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<h2>" + CurrentElement.Name.ToUpper() + "</h2>");
            stringBuilder.AppendLine(GenerateHeader(_currentElement));
            AppendDescription(stringBuilder, CurrentElement);
            AppendBeforeSource(stringBuilder, CurrentElement);
            if (string.IsNullOrWhiteSpace(CurrentElement.SourceUrl))
            {
                CurrentElement.SourceUrl = GenerateSourceUrl(_currentElement);
                if (string.IsNullOrWhiteSpace(CurrentElement.SourceUrl))
                {
                    string text = WebUtility.UrlEncode(CurrentElement.Name);
                    string text2 = WebUtility.UrlEncode(CurrentElement.Source);
                    CurrentElement.SourceUrl = "https://www.google.com/search?q=" + text2 + "+" + text;
                }
            }
            if (IncludeSource)
            {
                stringBuilder.Append("<h6>SOURCE</h6><p><i><a href=\"" + CurrentElement.SourceUrl + "\">" + CurrentElement.Source + "</a></i></p>");
            }
            AppendAfterSource(stringBuilder, CurrentElement);
            ElementBase currentElement = CurrentElement;
            string generatedDescription = (Description = $"<body>{stringBuilder}</body>");
            currentElement.GeneratedDescription = generatedDescription;
            if (args.ContainsStylesheet)
            {
                StyleSheet = args.Stylesheet;
            }
        }

        protected virtual void AppendBeforeSource(StringBuilder descriptionBuilder, ElementBase currentElement)
        {
        }

        protected virtual void AppendAfterSource(StringBuilder descriptionBuilder, ElementBase currentElement)
        {
        }

        protected virtual void HandleHtmlDisplayRequest(HtmlDisplayRequestEvent args)
        {
            if (args.ContainsStylesheet)
            {
                StyleSheet = args.Stylesheet;
            }
            Description = args.Html;
            CurrentElement = null;
        }

        public static string GenerateHeaderForCard(ElementBase element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            switch (element.Type)
            {
                case "Spell":
                    {
                        Spell spell = element.AsElement<Spell>();
                        stringBuilder.Append("<p class=\"underline\">" + spell.GetShortDescription() + "</p>");
                        stringBuilder.Append("<p>");
                        stringBuilder.Append("<b>Casting Time:</b> " + spell.CastingTime + "<br/>");
                        stringBuilder.Append("<b>Range:</b> " + spell.Range + "<br/>");
                        stringBuilder.Append("<b>Components:</b> " + spell.GetComponentsString() + "<br/>");
                        stringBuilder.Append("<b>Duration:</b> " + spell.Duration + "<br/>");
                        stringBuilder.Append("</p>");
                        break;
                    }
                case "Magic Item":
                    {
                        Item item = element.AsElement<Item>();
                        stringBuilder.Append("<p class=\"underline\"> ");
                        if (!string.IsNullOrWhiteSpace(item.ItemType))
                        {
                            string setterAdditionAttribute = item.GetSetterAdditionAttribute("type");
                            if (setterAdditionAttribute != null)
                            {
                                stringBuilder.Append(item.ItemType + " (" + setterAdditionAttribute + "), ");
                            }
                            else
                            {
                                stringBuilder.Append(item.ItemType + ", ");
                            }
                        }
                        else
                        {
                            stringBuilder.Append("Magic item, ");
                        }
                        if (!string.IsNullOrWhiteSpace(item.Rarity))
                        {
                            stringBuilder.Append(item.Rarity.ToLower() + " ");
                        }
                        if (item.RequiresAttunement)
                        {
                            string setterAdditionAttribute2 = item.GetSetterAdditionAttribute("attunement");
                            if (setterAdditionAttribute2 != null)
                            {
                                stringBuilder.Append("(requires attunement " + setterAdditionAttribute2 + ")");
                            }
                            else
                            {
                                stringBuilder.Append("(requires attunement)");
                            }
                        }
                        stringBuilder.Append("</p>");
                        break;
                    }
                case "Armor":
                    {
                        ArmorElement armorElement = element.AsElement<ArmorElement>();
                        stringBuilder.AppendLine("<p>");
                        stringBuilder.AppendLine(armorElement.ElementSetters.ContainsSetter("armorClass") ? ("<br/><b><i>Armor Class. </i></b>" + armorElement.ElementSetters.GetSetter("armorClass").Value) : "");
                        stringBuilder.AppendLine(armorElement.ElementSetters.ContainsSetter("strength") ? ("<br/><b><i>Strength. </i></b>" + armorElement.ElementSetters.GetSetter("strength").Value) : "<br/><b><i>Strength. </i></b>—");
                        stringBuilder.AppendLine(armorElement.ElementSetters.ContainsSetter("stealth") ? ("<br/><b><i>Stealth. </i></b>" + armorElement.ElementSetters.GetSetter("stealth").Value) : "<br/><b><i>Stealth. </i></b>—");
                        stringBuilder.AppendLine("</p>");
                        break;
                    }
                case "Weapon":
                    {
                        WeaponElement weaponElement = element.AsElement<WeaponElement>();
                        foreach (ElementBase item2 in (from x in DataManager.Current.ElementsCollection
                                                       where x.Type.Equals("Weapon Category")
                                                       orderby x.Name
                                                       select x).ToList())
                        {
                            if (weaponElement.Supports.Contains(item2.Id))
                            {
                                stringBuilder.AppendLine("<p class=\"underline\">" + item2.Name + " Weapon</p>");
                            }
                        }
                        List<ElementBase> source = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Weapon Property")).ToList();
                        stringBuilder.Append("<p>");
                        List<string> list = new List<string>();
                        foreach (string support in weaponElement.Supports)
                        {
                            ElementBase elementBase = source.FirstOrDefault((ElementBase x) => x.Id.Equals(support));
                            if (elementBase == null)
                            {
                                continue;
                            }
                            string text = (list.Any() ? elementBase.Name.ToLower() : elementBase.Name);
                            switch (elementBase.Id)
                            {
                                case "ID_INTERNAL_WEAPON_PROPERTY_THROWN":
                                case "ID_INTERNAL_WEAPON_PROPERTY_AMMUNITION":
                                    list.Add(text + " (" + weaponElement.Range + ")");
                                    break;
                                case "ID_INTERNAL_WEAPON_PROPERTY_VERSATILE":
                                    list.Add(text + " (" + weaponElement.Versatile + ")");
                                    break;
                                case "ID_INTERNAL_WEAPON_PROPERTY_SPECIAL":
                                    if (weaponElement.Supports.Count((string x) => x.Contains("WEAPON_PROPERTY_SPECIAL")) == 1)
                                    {
                                        list.Add(text + " (" + weaponElement.Versatile + ")");
                                    }
                                    break;
                                default:
                                    list.Add(text);
                                    break;
                            }
                        }
                        stringBuilder.AppendLine(list.Any() ? ("<b><i>Properties. </i></b>" + string.Join(", ", list.OrderBy((string x) => x))) : "<b><i>Properties. </i></b>—");
                        stringBuilder.AppendLine("<br/><b><i>Damage. </i></b>" + weaponElement.Damage + " " + weaponElement.DamageType);
                        stringBuilder.Append("</p>");
                        stringBuilder.AppendLine("Proficiency with a " + weaponElement.Name.ToLower() + " allows you to add your proficiency bonus to the attack roll for any attack you make with it.");
                        break;
                    }
                case "Alignment":
                    stringBuilder.AppendLine("<p>");
                    stringBuilder.AppendLine("<b>Abbreviation:</b> " + element.ElementSetters.GetSetter("Abbreviation").Value + "<br/>");
                    stringBuilder.AppendLine("</p>");
                    break;
                case "Companion":
                    {
                        CompanionElement companionElement = element.AsElement<CompanionElement>();
                        stringBuilder.Append("<p><em>" + companionElement.CreatureType + "</em></p>");
                        break;
                    }
            }
            return stringBuilder.ToString();
        }

        protected virtual string GenerateHeader(ElementBase element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            switch (element.Type)
            {
                case "Spell":
                    {
                        Spell spell = element.AsElement<Spell>();
                        stringBuilder.Append("<p class=\"underline\">" + spell.GetShortDescription() + "</p>");
                        stringBuilder.Append("<p>");
                        stringBuilder.Append("<b>Casting Time:</b> " + spell.CastingTime + "<br/>");
                        stringBuilder.Append("<b>Range:</b> " + spell.Range + "<br/>");
                        stringBuilder.Append("<b>Components:</b> " + spell.GetComponentsString() + "<br/>");
                        stringBuilder.Append("<b>Duration:</b> " + spell.Duration + "<br/>");
                        stringBuilder.Append("</p>");
                        break;
                    }
                case "Magic Item":
                    {
                        Item item2 = element.AsElement<Item>();
                        stringBuilder.Append("<p class=\"underline\"> ");
                        if (!string.IsNullOrWhiteSpace(item2.ItemType))
                        {
                            string setterAdditionAttribute = item2.GetSetterAdditionAttribute("type");
                            if (setterAdditionAttribute != null)
                            {
                                stringBuilder.Append(item2.ItemType + " (" + setterAdditionAttribute + ")");
                            }
                            else
                            {
                                stringBuilder.Append(item2.ItemType ?? "");
                            }
                        }
                        else
                        {
                            stringBuilder.Append("Magic item");
                        }
                        if (!string.IsNullOrWhiteSpace(item2.Rarity))
                        {
                            stringBuilder.Append(", ");
                            stringBuilder.Append(item2.Rarity.ToLower() + " ");
                        }
                        else
                        {
                            stringBuilder.Append(" ");
                        }
                        if (item2.RequiresAttunement)
                        {
                            string setterAdditionAttribute2 = item2.GetSetterAdditionAttribute("attunement");
                            if (setterAdditionAttribute2 != null)
                            {
                                stringBuilder.Append("(requires attunement " + setterAdditionAttribute2 + ")");
                            }
                            else
                            {
                                stringBuilder.Append("(requires attunement)");
                            }
                        }
                        stringBuilder.Append("</p>");
                        break;
                    }
                case "Item":
                    {
                        Item item3 = element.AsElement<Item>();
                        if (item3.ElementSetters.ContainsSetter("valuable") || item3.Category.Equals("Treasure"))
                        {
                            stringBuilder.Append("<p class=\"underline\">");
                            stringBuilder.Append(item3.Category + ", " + item3.DisplayPrice);
                            stringBuilder.Append("</p>");
                        }
                        break;
                    }
                case "Armor":
                    {
                        ArmorElement armorElement = element.AsElement<ArmorElement>();
                        stringBuilder.AppendLine("<p>");
                        stringBuilder.AppendLine(armorElement.ElementSetters.ContainsSetter("armorClass") ? ("<br/><b><i>Armor Class. </i></b>" + armorElement.ElementSetters.GetSetter("armorClass").Value) : "");
                        stringBuilder.AppendLine(armorElement.ElementSetters.ContainsSetter("strength") ? ("<br/><b><i>Strength. </i></b>" + armorElement.ElementSetters.GetSetter("strength").Value) : "<br/><b><i>Strength. </i></b>—");
                        stringBuilder.AppendLine(armorElement.ElementSetters.ContainsSetter("stealth") ? ("<br/><b><i>Stealth. </i></b>" + armorElement.ElementSetters.GetSetter("stealth").Value) : "<br/><b><i>Stealth. </i></b>—");
                        stringBuilder.AppendLine("</p>");
                        break;
                    }
                case "Weapon":
                    {
                        WeaponElement weaponElement = element.AsElement<WeaponElement>();
                        foreach (ElementBase item4 in (from x in DataManager.Current.ElementsCollection
                                                       where x.Type.Equals("Weapon Category")
                                                       orderby x.Name
                                                       select x).ToList())
                        {
                            if (weaponElement.Supports.Contains(item4.Id))
                            {
                                stringBuilder.AppendLine("<p class=\"underline\">" + item4.Name + " Weapon</p>");
                            }
                        }
                        List<ElementBase> source4 = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Weapon Property")).ToList();
                        stringBuilder.Append("<p>");
                        List<string> list = new List<string>();
                        foreach (string support in weaponElement.Supports)
                        {
                            ElementBase elementBase4 = source4.FirstOrDefault((ElementBase x) => x.Id.Equals(support));
                            if (elementBase4 == null)
                            {
                                continue;
                            }
                            string text = (list.Any() ? elementBase4.Name.ToLower() : elementBase4.Name);
                            switch (elementBase4.Id)
                            {
                                case "ID_INTERNAL_WEAPON_PROPERTY_THROWN":
                                case "ID_INTERNAL_WEAPON_PROPERTY_AMMUNITION":
                                    list.Add(text + " (" + weaponElement.Range + ")");
                                    break;
                                case "ID_INTERNAL_WEAPON_PROPERTY_VERSATILE":
                                    list.Add(text + " (" + weaponElement.Versatile + ")");
                                    break;
                                case "ID_INTERNAL_WEAPON_PROPERTY_SPECIAL":
                                    if (weaponElement.Supports.Count((string x) => x.Contains("WEAPON_PROPERTY_SPECIAL")) == 1)
                                    {
                                        list.Add(text + " (" + weaponElement.Versatile + ")");
                                    }
                                    break;
                                default:
                                    list.Add(text);
                                    break;
                            }
                        }
                        stringBuilder.AppendLine(list.Any() ? ("<b><i>Properties. </i></b>" + string.Join(", ", list.OrderBy((string x) => x))) : "<b><i>Properties. </i></b>—");
                        stringBuilder.AppendLine("<br/><b><i>Damage. </i></b>" + weaponElement.Damage + " " + weaponElement.DamageType);
                        stringBuilder.Append("</p>");
                        stringBuilder.AppendLine("Proficiency with a " + weaponElement.Name.ToLower() + " allows you to add your proficiency bonus to the attack roll for any attack you make with it.");
                        break;
                    }
                case "Alignment":
                    stringBuilder.AppendLine("<p>");
                    stringBuilder.AppendLine("<b>Abbreviation:</b> " + element.ElementSetters.GetSetter("Abbreviation").Value + "<br/>");
                    stringBuilder.AppendLine("</p>");
                    break;
                case "Companion":
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        CompanionElement companionElement = element.AsElement<CompanionElement>();
                        stringBuilder.Append("<p><em>" + companionElement.Size + " " + companionElement.CreatureType.ToLower() + ", " + companionElement.Alignment.ToLower() + "</em></p>");
                        stringBuilder.Append("<p><strong>Armor Class</strong> " + companionElement.ArmorClass + "<br />");
                        stringBuilder.Append("<strong>Hit Points</strong> " + companionElement.HitPoints + "<br />");
                        stringBuilder.Append("<strong>Speed</strong> " + companionElement.Speed + "</p>");
                        stringBuilder.Append("<table class=\"abilities\">");
                        stringBuilder.Append("<tr><td><strong>STR</strong></td><td><strong>DEX</strong></td><td><strong>CON</strong></td><td><strong>INT</strong></td><td><strong>WIS</strong></td><td><strong>CHA</strong></td></tr>");
                        AbilitiesCollection abilitiesCollection = new AbilitiesCollection();
                        abilitiesCollection.DisablePointsCalculation = true;
                        abilitiesCollection.Strength.BaseScore = companionElement.Strength;
                        abilitiesCollection.Dexterity.BaseScore = companionElement.Dexterity;
                        abilitiesCollection.Constitution.BaseScore = companionElement.Constitution;
                        abilitiesCollection.Intelligence.BaseScore = companionElement.Intelligence;
                        abilitiesCollection.Wisdom.BaseScore = companionElement.Wisdom;
                        abilitiesCollection.Charisma.BaseScore = companionElement.Charisma;
                        stringBuilder.Append("<tr><td>" + abilitiesCollection.Strength.AbilityAndModifierString + "</td><td>" + abilitiesCollection.Dexterity.AbilityAndModifierString + "</td><td>" + abilitiesCollection.Constitution.AbilityAndModifierString + "</td><td>" + abilitiesCollection.Intelligence.AbilityAndModifierString + "</td><td>" + abilitiesCollection.Wisdom.AbilityAndModifierString + "</td><td>" + abilitiesCollection.Charisma.AbilityAndModifierString + "</td></tr>");
                        stringBuilder.Append("</table>");
                        stringBuilder.Append("<p>");
                        if (companionElement.HasSavingThrows)
                        {
                            stringBuilder.Append("<strong>Saving Throws</strong> " + companionElement.SavingThrows + "<br />");
                        }
                        if (companionElement.HasSkills)
                        {
                            stringBuilder.Append("<strong>Skills</strong> " + companionElement.Skills + "<br />");
                        }
                        if (companionElement.HasDamageVulnerabilities)
                        {
                            stringBuilder.Append("<strong>Damage Vulnerabilities</strong> " + companionElement.DamageVulnerabilities + "<br />");
                        }
                        if (companionElement.HasDamageResistances)
                        {
                            stringBuilder.Append("<strong>Damage Resistances</strong> " + companionElement.DamageResistances + "<br />");
                        }
                        if (companionElement.HasDamageImmunities)
                        {
                            stringBuilder.Append("<strong>Damage Immunities</strong> " + companionElement.DamageImmunities + "<br />");
                        }
                        if (companionElement.HasConditionResistances)
                        {
                            stringBuilder.Append("<strong>Condition Resistances</strong> " + companionElement.ConditionResistances + "<br />");
                        }
                        if (companionElement.HasSenses)
                        {
                            stringBuilder.Append("<strong>Senses</strong> " + companionElement.Senses + "<br />");
                        }
                        stringBuilder.Append(companionElement.HasLanguages ? ("<strong>Languages</strong> " + companionElement.Languages + "<br />") : "<strong>Languages</strong>—<br />");
                        stringBuilder.Append("<strong>Challenge</strong> " + companionElement.Challenge);
                        if (companionElement.HasExperience)
                        {
                            stringBuilder.Append(" (" + companionElement.Experience + ")");
                        }
                        stringBuilder.Append("</p>");
                        if (companionElement.Traits.Any())
                        {
                            IEnumerable<ElementBase> source = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Companion Trait"));
                            foreach (string item in companionElement.Traits)
                            {
                                ElementBase elementBase = source.FirstOrDefault((ElementBase x) => x.Id.Equals(item));
                                if (elementBase != null)
                                {
                                    stringBuilder.Append("<p><strong><em>" + elementBase.Name + ".</em></strong> " + elementBase.Description.Replace("<p>", ""));
                                }
                            }
                        }
                        if (companionElement.Actions.Any())
                        {
                            stringBuilder.Append("<p style=\"fontsize:14\"><strong>ACTIONS</strong></p>");
                            IEnumerable<ElementBase> source2 = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Companion Action"));
                            foreach (string companionAction in companionElement.Actions)
                            {
                                ElementBase elementBase2 = source2.FirstOrDefault((ElementBase x) => x.Id.Equals(companionAction));
                                if (elementBase2 != null)
                                {
                                    stringBuilder.Append("<p><strong><em>" + elementBase2.Name + ".</em></strong> " + elementBase2.Description.Replace("<p>", ""));
                                }
                            }
                        }
                        if (companionElement.Reactions.Any())
                        {
                            stringBuilder.Append("<p style=\"fontsize:14\"><strong>REACTIONS</strong></p>");
                            IEnumerable<ElementBase> source3 = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Companion Reaction"));
                            foreach (string companionReaction in companionElement.Reactions)
                            {
                                ElementBase elementBase3 = source3.FirstOrDefault((ElementBase x) => x.Id.Equals(companionReaction));
                                if (elementBase3 != null)
                                {
                                    stringBuilder.Append("<p><strong><em>" + elementBase3.Name + ".</em></strong> " + elementBase3.Description.Replace("<p>", ""));
                                }
                            }
                        }
                        stopwatch.Stop();
                        Logger.Warning($"it took {stopwatch.ElapsedMilliseconds}ms to generate the description header of {companionElement}");
                        break;
                    }
            }
            return stringBuilder.ToString();
        }

        protected virtual void AppendDescription(StringBuilder documentBuilder, ElementBase element)
        {
            if (element.Description.Contains("<div element=") || element.Description.Contains("<p element="))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml("<div>" + element.Description + "</div>");
                if (xmlDocument.DocumentElement != null)
                {
                    ReplaceInjectedElementDescription(xmlDocument.DocumentElement, element);
                }
                documentBuilder.AppendLine(xmlDocument.InnerXml);
            }
            else
            {
                documentBuilder.AppendLine(element.Description);
            }
        }

        private void ReplaceInjectedElementDescription(XmlNode parentNode, ElementBase parentElement, bool recursive = false)
        {
            if (parentNode == null)
            {
                return;
            }
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode.ParentNode != null && childNode.ParentNode.Name == "div" && childNode.ParentNode.ContainsAttribute("class"))
                {
                    _ = childNode.ParentNode.GetAttributeValue("class") == "reference";
                }
                if (childNode.ContainsAttribute("element"))
                {
                    childNode.GetAttributeValue("element").Contains("ID_WOTC_PHB_CLASS_FEATURE_MONK_EMPTY_BODY");
                }
                if (childNode.Name == "div")
                {
                    ReplaceInjectedElementDescription(childNode, parentElement, recursive: true);
                }
                if (childNode.Name == "div" && childNode.ContainsAttribute("element"))
                {
                    string injectedElementId = childNode.GetAttributeValue("element");
                    ElementBase elementBase = DataManager.Current.ElementsCollection.FirstOrDefault((ElementBase x) => x.Id == injectedElementId);
                    if (elementBase == null)
                    {
                        Logger.Warning($"the injected description div element '{parentElement.Name}' with the id '{injectedElementId}' is not found in the description of {parentElement}.");
                        MessageDialogService.Show($"the injected description div element '{parentElement.Name}' with the id '{injectedElementId}' is not found in the description of {parentElement}.");
                        continue;
                    }
                    childNode.InnerXml = "<h5 class=\"h5-enhance\">" + elementBase.Name.ToUpperInvariant().Replace(" & ", " &amp; ") + "</h5>";
                    string text = ElementDescriptionHelper.GenerateDescriptionBase(elementBase);
                    text = text.Replace("class=\"reference\"", "class=\"\"");
                    childNode.InnerXml += text;
                }
                if (!(childNode.Name == "p") || !childNode.ContainsAttribute("element"))
                {
                    continue;
                }
                string injectedElementId2 = childNode.GetAttributeValue("element");
                ElementBase elementBase2 = DataManager.Current.ElementsCollection.FirstOrDefault((ElementBase x) => x.Id == injectedElementId2);
                if (elementBase2 == null)
                {
                    Logger.Warning($"the injected description p element '{parentElement.Name}' with the id '{injectedElementId2}' is not found in the description of {parentElement}.");
                    MessageDialogService.Show($"the injected description p element '{parentElement.Name}' with the id '{injectedElementId2}' is not found in the description of {parentElement}.");
                    continue;
                }
                string text2 = "<p class=\"indent\"><strong><em>" + elementBase2.Name + ". </em></strong> ";
                try
                {
                    if (elementBase2.Description.StartsWith("<p>"))
                    {
                        text2 += elementBase2.Description.Substring(3, elementBase2.Description.Length - 3);
                    }
                    else if (elementBase2.Description.StartsWith("<p "))
                    {
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                    }
                    else
                    {
                        text2 += "</p>";
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "ReplaceInjectedElementDescription");
                    Dictionary<string, string> additionalProperties = AnalyticsErrorHelper.CreateProperties("element", elementBase2.ToString());
                    AnalyticsErrorHelper.Exception(ex, additionalProperties, elementBase2.Description, "ReplaceInjectedElementDescription", 844);
                }
                if (elementBase2.Description.StartsWith("<p>"))
                {
                    text2 += elementBase2.Description.Substring(3, elementBase2.Description.Length - 3);
                }
                else if (elementBase2.Description.StartsWith("<p "))
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }
                else
                {
                    text2 += "</p>";
                }
                childNode.InnerXml = text2;
            }
        }

        protected virtual string GenerateSourceUrl(ElementBase element)
        {
            string result = string.Empty;
            SourceItem sourceItem = CharacterManager.Current.SourcesManager.SourceItems.FirstOrDefault((SourceItem x) => x.Source.Name.Equals(element.Source));
            if (sourceItem != null && sourceItem.Source.HasSourceUrl)
            {
                return sourceItem.Source.Url;
            }
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

        public virtual void OnHandleEvent(ElementDescriptionDisplayRequestEvent args)
        {
            HandleDisplayRequest(args);
        }

        public virtual void OnHandleEvent(HtmlDisplayRequestEvent args)
        {
            HandleHtmlDisplayRequest(args);
        }

        private void HandleLinkedDescriptionRequest(string id)
        {
        }

        private void StartSpeech()
        {
            try
            {
                if (SelectedText.Length > 0)
                {
                    SpeechService.Default.StartSpeech(SelectedText);
                    return;
                }
                string text = Description.Replace("</h1>", "</h1>__ENTER__");
                text = text.Replace("</h2>", "</h2>__ENTER__");
                text = text.Replace("</h3>", "</h3>__ENTER__");
                text = text.Replace("</h4>", "</h4>__ENTER__");
                text = text.Replace("</h5>", "</h5>__ENTER__");
                text = text.Replace("</h6>", "</h6>__ENTER__");
                text = text.Replace("<br/>", "<br/>__ENTER__");
                text = text.Replace("</p>", "</p>__ENTER__");
                text = text.Replace("d10", "d 10").Replace("d12", "d 12").Replace("d20", "d 20")
                    .Replace("—", " , ")
                    .Replace(" cp ", " copper pieces ")
                    .Replace(" cp)", " copper pieces)")
                    .Replace(" cp.", " copper pieces.")
                    .Replace(" sp ", " silver pieces ")
                    .Replace(" sp)", " silver pieces)")
                    .Replace(" sp.", " silver pieces.")
                    .Replace(" ep ", " electrum pieces ")
                    .Replace(" ep)", " electrum pieces)")
                    .Replace(" ep.", " electrum pieces.")
                    .Replace(" gp ", " gold pieces ")
                    .Replace(" gp)", " gold pieces)")
                    .Replace(" gp.", " gold pieces.")
                    .Replace(" pp ", " platinum pieces ")
                    .Replace(" pp)", " platinum pieces)")
                    .Replace(" pp.", " platinum pieces.");
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                string input = xmlDocument.InnerText.Replace("__ENTER__", Environment.NewLine);
                SpeechService.Default.StartSpeech(input);
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowException(ex);
            }
        }

        private void StopSpeech()
        {
            SpeechService.Default.StopSpeech();
        }

        private void _speechService_SpeechStarted(object sender, EventArgs e)
        {
            IsSpeechActive = true;
        }

        private void _speechService_SpeechStopped(object sender, EventArgs e)
        {
            IsSpeechActive = false;
        }

        public virtual void OnHandleEvent(ResourceDocumentDisplayRequestEvent args)
        {
            string resourceWebDocument = DataManager.Current.GetResourceWebDocument(args.ResourceFilename);
            if (!resourceWebDocument.Contains("<body>"))
            {
                Logger.Warning("the contents of '" + args.ResourceFilename + "' needs to be in a <body> tag");
            }
            Description = (resourceWebDocument.Contains("<body>") ? resourceWebDocument : ("<body>" + resourceWebDocument + "</body>"));
        }

        void ISubscriber<SettingsChangedEvent>.OnHandleEvent(SettingsChangedEvent args)
        {
            UpdateTheme();
        }

        private void UpdateTheme()
        {
            IsDarkStyle = Builder.Presentation.Properties.Settings.Default.Theme.Contains("Dark");
            OnPropertyChanged("Description");
            Description += Environment.NewLine;
        }

        public void OnHandleEvent(CharacterLoadingCompletedEvent args)
        {
            UpdateTheme();
        }
    }
}
