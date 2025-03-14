using Builder.Data;
using Builder.Presentation.Services;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.Telemetry
{
    public static class AnalyticsEventHelper
    {
        private static string FormatEventName(string input)
        {
            return input.Replace(" ", "_").ToLowerInvariant();
        }

        private static string GetEventNameAddition(string name)
        {
            string result = "";
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
            if (name.StartsWith("a", comparisonType) || name.StartsWith("b", comparisonType) || name.StartsWith("c", comparisonType) || name.StartsWith("d", comparisonType) || name.StartsWith("e", comparisonType) || name.StartsWith("f", comparisonType) || name.StartsWith("g", comparisonType) || name.StartsWith("h", comparisonType) || name.StartsWith("i", comparisonType) || name.StartsWith("j", comparisonType) || name.StartsWith("k", comparisonType) || name.StartsWith("l", comparisonType) || name.StartsWith("m", comparisonType))
            {
                result = "_1";
            }
            else if (name.StartsWith("n", comparisonType) || name.StartsWith("o", comparisonType) || name.StartsWith("p", comparisonType) || name.StartsWith("q", comparisonType) || name.StartsWith("r", comparisonType) || name.StartsWith("s", comparisonType) || name.StartsWith("t", comparisonType) || name.StartsWith("u", comparisonType) || name.StartsWith("v", comparisonType) || name.StartsWith("w", comparisonType) || name.StartsWith("x", comparisonType) || name.StartsWith("y", comparisonType) || name.StartsWith("z", comparisonType))
            {
                result = "_2";
            }
            return result;
        }

        private static string GetNamePropertyAddition(string name)
        {
            string text = "";
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
            if (name != null)
            {
                if (name.StartsWith("a", comparisonType) || name.StartsWith("b", comparisonType))
                {
                    text += "_ab";
                }
                else if (name.StartsWith("c", comparisonType) || name.StartsWith("d", comparisonType))
                {
                    text += "_cd";
                }
                else if (name.StartsWith("e", comparisonType) || name.StartsWith("f", comparisonType))
                {
                    text += "_ef";
                }
                else if (name.StartsWith("g", comparisonType) || name.StartsWith("h", comparisonType))
                {
                    text += "_gh";
                }
                else if (name.StartsWith("i", comparisonType) || name.StartsWith("j", comparisonType))
                {
                    text += "_ij";
                }
                else if (name.StartsWith("k", comparisonType) || name.StartsWith("l", comparisonType))
                {
                    text += "_kl";
                }
                else if (name.StartsWith("m", comparisonType) || name.StartsWith("n", comparisonType))
                {
                    text += "_mn";
                }
                else if (name.StartsWith("o", comparisonType) || name.StartsWith("p", comparisonType))
                {
                    text += "_op";
                }
                else if (name.StartsWith("q", comparisonType) || name.StartsWith("r", comparisonType))
                {
                    text += "_qr";
                }
                else if (name.StartsWith("s", comparisonType) || name.StartsWith("t", comparisonType))
                {
                    text += "_st";
                }
                else if (name.StartsWith("u", comparisonType) || name.StartsWith("v", comparisonType))
                {
                    text += "_uv";
                }
                else if (name.StartsWith("w", comparisonType) || name.StartsWith("x", comparisonType))
                {
                    text += "_wx";
                }
                else if (name.StartsWith("y", comparisonType) || name.StartsWith("z", comparisonType))
                {
                    text += "_yz";
                }
            }
            return text;
        }

        public static void ApplicationEvent(string name)
        {
            Analytics.TrackEvent(FormatEventName("application_" + name));
        }

        public static void ApplicationEvent(string name, string key, string value)
        {
            string name2 = FormatEventName("application_" + name);
            Dictionary<string, string> properties = new Dictionary<string, string> { { key, value } };
            Analytics.TrackEvent(name2, properties);
        }

        public static void ApplicationStartupEvent(string name)
        {
            Analytics.TrackEvent(FormatEventName("application_startup_" + name));
        }

        public static void EquipmentAdd(string category, string name, string source, string categorySubType = null)
        {
            string name2 = FormatEventName(string.IsNullOrWhiteSpace(categorySubType) ? ("equipment_add_" + category) : ("equipment_add_" + category + "_" + categorySubType));
            string key = "item" + GetNamePropertyAddition(name);
            Dictionary<string, string> properties = new Dictionary<string, string>
        {
            { key, name },
            { "source", source }
        };
            Analytics.TrackEvent(name2, properties);
        }

        public static void EquipmentBuy(string category, string name, string source, string categorySubType = null)
        {
            string name2 = FormatEventName(string.IsNullOrWhiteSpace(categorySubType) ? ("equipment_buy_" + category) : ("equipment_buy_" + category + "_" + categorySubType));
            string key = "item" + GetNamePropertyAddition(name);
            Dictionary<string, string> properties = new Dictionary<string, string>
        {
            { key, name },
            { "source", source }
        };
            Analytics.TrackEvent(name2, properties);
        }

        public static void CharacterCreate(string level, string abilityScoreOption)
        {
            string name = FormatEventName("character_create");
            Dictionary<string, string> properties = new Dictionary<string, string>
        {
            { "level", level },
            { "ability generation", abilityScoreOption }
        };
            Analytics.TrackEvent(name, properties);
        }

        public static void CharacterSave(string characterRace, string characterClass, string characterBackground, string characterLevel, bool multiclass, bool spellcaster, bool companion)
        {
            string name = FormatEventName("character_save");
            Dictionary<string, string> properties = new Dictionary<string, string>
        {
            { "race", characterRace },
            { "class", characterClass },
            { "background", characterBackground },
            { "level", characterLevel },
            {
                "multiclass",
                multiclass ? "true" : "false"
            },
            {
                "spellcaster",
                spellcaster ? "true" : "false"
            },
            {
                "companion",
                companion ? "true" : "false"
            }
        };
            Analytics.TrackEvent(name, properties);
            CharacterSaveRace(characterRace);
            CharacterSaveClass(characterClass);
            CharacterSaveBackground(characterBackground);
        }

        public static void CharacterLoad(bool fromFile = false, bool fromNewFile = false)
        {
            string name = FormatEventName("character_load");
            Dictionary<string, string> properties = new Dictionary<string, string>
        {
            {
                "fromFile",
                fromFile ? "true" : "false"
            },
            {
                "fromNewFile",
                fromNewFile ? "true" : "false"
            }
        };
            Analytics.TrackEvent(name, properties);
        }

        public static void CharacterSaveRace(string characterRace)
        {
            if (!string.IsNullOrWhiteSpace(characterRace))
            {
                string name = FormatEventName("character_save_race");
                string key = "race" + GetNamePropertyAddition(characterRace);
                Dictionary<string, string> properties = new Dictionary<string, string> { { key, characterRace } };
                Analytics.TrackEvent(name, properties);
            }
        }

        public static void CharacterSaveClass(string characterClass)
        {
            if (!string.IsNullOrWhiteSpace(characterClass))
            {
                string name = FormatEventName("character_save_class");
                string key = "class" + GetNamePropertyAddition(characterClass);
                Dictionary<string, string> properties = new Dictionary<string, string> { { key, characterClass } };
                Analytics.TrackEvent(name, properties);
                CharacterSaveArchetypeClass(characterClass);
            }
        }

        public static void CharacterSaveArchetypeClass(string characterClass)
        {
            if (string.IsNullOrWhiteSpace(characterClass))
            {
                return;
            }
            try
            {
                ClassProgressionManager classProgressionManager = CharacterManager.Current.ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.ClassElement.Name == characterClass);
                if (classProgressionManager.HasArchetype())
                {
                    ElementBase elementBase = classProgressionManager.GetElements().FirstOrDefault((ElementBase x) => x.Type.Equals("Archetype"));
                    if (elementBase != null)
                    {
                        string name = FormatEventName("character_save_archetype");
                        string key = "archetype" + GetNamePropertyAddition(elementBase.Name);
                        Dictionary<string, string> properties = new Dictionary<string, string> { { key, elementBase.Name } };
                        Analytics.TrackEvent(name, properties);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void CharacterSaveBackground(string background)
        {
            if (!string.IsNullOrWhiteSpace(background))
            {
                string name = FormatEventName("character_save_background");
                string key = "background" + GetNamePropertyAddition(background);
                Dictionary<string, string> properties = new Dictionary<string, string> { { key, background } };
                Analytics.TrackEvent(name, properties);
            }
        }

        public static void CharacterSheetPreview(bool fillable, bool formatted, bool itemCards, bool attackCards, bool spellCards, bool featureCards, bool legacySpellcasting)
        {
            string name = FormatEventName("character_sheet_preview");
            Dictionary<string, string> properties = new Dictionary<string, string>
        {
            {
                "fillable",
                fillable ? "true" : "false"
            },
            {
                "formatted",
                formatted ? "true" : "false"
            },
            {
                "itemCards",
                itemCards ? "true" : "false"
            },
            {
                "attackCards",
                attackCards ? "true" : "false"
            },
            {
                "spellCards",
                spellCards ? "true" : "false"
            },
            {
                "featureCards",
                featureCards ? "true" : "false"
            },
            {
                "legacySpellcasting",
                legacySpellcasting ? "true" : "false"
            }
        };
            Analytics.TrackEvent(name, properties);
        }

        public static void CharacterSheetSave(bool fillable, bool formatted, bool itemCards, bool attackCards, bool spellCards, bool featureCards, bool legacySpellcasting)
        {
            string name = FormatEventName("character_sheet_save");
            Dictionary<string, string> properties = new Dictionary<string, string>
        {
            {
                "fillable",
                fillable ? "true" : "false"
            },
            {
                "formatted",
                formatted ? "true" : "false"
            },
            {
                "itemCards",
                itemCards ? "true" : "false"
            },
            {
                "attackCards",
                attackCards ? "true" : "false"
            },
            {
                "spellCards",
                spellCards ? "true" : "false"
            },
            {
                "featureCards",
                featureCards ? "true" : "false"
            },
            {
                "legacySpellcasting",
                legacySpellcasting ? "true" : "false"
            }
        };
            Analytics.TrackEvent(name, properties);
        }

        public static void SyndicationView(string url)
        {
            string name = FormatEventName("syndication_view");
            Dictionary<string, string> properties = new Dictionary<string, string> { { "url", url } };
            Analytics.TrackEvent(name, properties);
        }

        public static void CompendiumSearch(string criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria))
            {
                string eventNameAddition = GetEventNameAddition(criteria);
                string name = FormatEventName("compendium_search" + eventNameAddition);
                string key = "criteria_" + criteria[0];
                Dictionary<string, string> properties = new Dictionary<string, string> { { key, criteria } };
                Analytics.TrackEvent(name, properties);
            }
        }

        public static void CompendiumSearchTrackDetailed(string criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria))
            {
                string text = criteria[0].ToString();
                string name = FormatEventName("compendium_search_" + text);
                Dictionary<string, string> properties = new Dictionary<string, string> { { "criteria", criteria } };
                Analytics.TrackEvent(name, properties);
            }
        }

        public static void DescriptionPanelSnap(string elementName)
        {
            string name = FormatEventName("description_panel_snap");
            Dictionary<string, string> properties = new Dictionary<string, string> { { "name", elementName } };
            Analytics.TrackEvent(name, properties);
        }

        public static void DescriptionPanelReadAloud(string elementName)
        {
            string name = FormatEventName("description_panel_read_aloud");
            Dictionary<string, string> properties = new Dictionary<string, string> { { "name", elementName } };
            Analytics.TrackEvent(name, properties);
        }

        public static void SourcesCompendiumLookup(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                string name = FormatEventName("sources_compendium_lookup");
                string key = "source" + GetNamePropertyAddition(source);
                Dictionary<string, string> properties = new Dictionary<string, string> { { key, source } };
                Analytics.TrackEvent(name, properties);
            }
        }

        public static void ContentDownloadIndex(string url, bool bundle = false)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string name = FormatEventName("content_download_index");
                Dictionary<string, string> properties = new Dictionary<string, string>
            {
                { "url", url },
                {
                    "bundle",
                    bundle ? "true" : "false"
                }
            };
                Analytics.TrackEvent(name, properties);
            }
        }

        public static void ContentClear(bool confirmed)
        {
            string name = FormatEventName("content_clear");
            Dictionary<string, string> properties = new Dictionary<string, string> {
        {
            "confirmed",
            confirmed ? "true" : "false"
        } };
            Analytics.TrackEvent(name, properties);
        }
    }
}
