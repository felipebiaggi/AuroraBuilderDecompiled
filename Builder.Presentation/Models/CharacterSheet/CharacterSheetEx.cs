using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Aurora.Documents.ExportContent.Equipment;
using Aurora.Documents.ExportContent.Notes;
using Aurora.Documents.Resources.Aurora;
using Aurora.Documents.Sheets;
using Aurora.Documents.Writers;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Presentation;
using Builder.Presentation.Extensions;
using Builder.Presentation.Models.CharacterSheet;
using Builder.Presentation.Models.CharacterSheet.Content;
using Builder.Presentation.Models.CharacterSheet.Pages;
using Builder.Presentation.Models.CharacterSheet.Pages.Content;
using Builder.Presentation.Models.CharacterSheet.PDF;
using Builder.Presentation.Models.Sheet;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Telemetry;
using Builder.Presentation.Utilities;
using Builder.Presentation.ViewModels;
using Builder.Presentation.ViewModels.Shell.Items;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace Builder.Presentation.Models.CharacterSheet
{
    public class CharacterSheetEx
    {
        private const string CheckBoxTrue = "Yes";

        private const float BaseFontSize = 7f;

        public CharacterSheetConfiguration Configuration { get; }

        public CharacterSheetExportContent ExportContent { get; set; }

        public EquipmentExportContent EquipmentSheetExportContent { get; set; }

        public NotesExportContent NotesExportContent { get; set; }

        public List<CharacterSheetSpellcastingPageExportContent> SpellcastingPageExportContentCollection { get; }

        public List<Spell> Spells { get; }

        public List<RefactoredEquipmentItem> Items { get; }

        public List<ElementBase> Features { get; }

        public List<string> PartialFlatteningNames { get; } = new List<string>();

        [Obsolete]
        public MainSheetContent MainSheetContent { get; set; }

        [Obsolete]
        public DetailsSheetContent DetailsSheetContent { get; set; }

        [Obsolete]
        public List<SpellcastingSheetContent> SpellcastingSheetContentItems { get; set; }

        public CharacterSheetEx(CharacterSheetConfiguration configuration = null)
        {
            Configuration = configuration ?? new CharacterSheetConfiguration();
            ExportContent = new CharacterSheetExportContent();
            SpellcastingPageExportContentCollection = new List<CharacterSheetSpellcastingPageExportContent>();
            Spells = new List<Spell>();
            Items = new List<RefactoredEquipmentItem>();
            Features = new List<ElementBase>();
        }

        public FileInfo Save(string destinationPath)
        {
            FileInfo fileInfo = InitializeCharacterSheet();
            using (PdfReader reader = new PdfReader(fileInfo.FullName))
            {
                using (FileStream os = new FileStream(destinationPath, FileMode.Create))
                {
                    using (PdfStamper pdfStamper = new PdfStamper(reader, os))
                    {
                        WriteExportContent(pdfStamper);
                        List<string> list = new List<string>();
                        if (Configuration.IncludeEquipmentPage)
                        {
                            new EquipmentPageWriter(Configuration, pdfStamper).Write(EquipmentSheetExportContent);
                        }
                        if (Configuration.IncludeNotesPage)
                        {
                            new NotesPageWriter(Configuration, pdfStamper).Write(NotesExportContent);
                        }
                        if (Configuration.UseLegacyDetailsPage)
                        {
                            StampLegacyDetailsContent(pdfStamper);
                        }
                        if (Configuration.UseLegacyBackgroundPage)
                        {
                            StampLegacyBackgroundContent(pdfStamper);
                        }
                        if (Configuration.IncludeCompanionPage)
                        {
                            WriteCompanionExportContent(pdfStamper);
                        }
                        if (Configuration.UseLegacySpellcastingPage)
                        {
                            for (int i = 0; i < SpellcastingPageExportContentCollection.Count; i++)
                            {
                                CharacterSheetSpellcastingPageExportContent characterSheetSpellcastingPageExportContent = SpellcastingPageExportContentCollection[i];
                                SpellcastingSheetContent spellcastingSheetContent = new SpellcastingSheetContent();
                                spellcastingSheetContent.SpellcastingClass = characterSheetSpellcastingPageExportContent.SpellcastingClass;
                                spellcastingSheetContent.SpellcastingAbility = characterSheetSpellcastingPageExportContent.Ability;
                                spellcastingSheetContent.SpellcastingAttackModifier = characterSheetSpellcastingPageExportContent.AttackBonus;
                                spellcastingSheetContent.SpellcastingSave = characterSheetSpellcastingPageExportContent.Save;
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell in characterSheetSpellcastingPageExportContent.Cantrips.Spells)
                                {
                                    spellcastingSheetContent.Cantrips.Collection.Add(new SpellcastingSpellContent(spell.Name));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell2 in characterSheetSpellcastingPageExportContent.Spells1.Spells)
                                {
                                    spellcastingSheetContent.Spells1.Collection.Add(new SpellcastingSpellContent(spell2.GetDisplayName(), spell2.IsPrepared));
                                }
                                spellcastingSheetContent.Spells1.SlotsCount = characterSheetSpellcastingPageExportContent.Spells1.AvailableSlots;
                                spellcastingSheetContent.Spells2.SlotsCount = characterSheetSpellcastingPageExportContent.Spells2.AvailableSlots;
                                spellcastingSheetContent.Spells3.SlotsCount = characterSheetSpellcastingPageExportContent.Spells3.AvailableSlots;
                                spellcastingSheetContent.Spells4.SlotsCount = characterSheetSpellcastingPageExportContent.Spells4.AvailableSlots;
                                spellcastingSheetContent.Spells5.SlotsCount = characterSheetSpellcastingPageExportContent.Spells5.AvailableSlots;
                                spellcastingSheetContent.Spells6.SlotsCount = characterSheetSpellcastingPageExportContent.Spells6.AvailableSlots;
                                spellcastingSheetContent.Spells7.SlotsCount = characterSheetSpellcastingPageExportContent.Spells7.AvailableSlots;
                                spellcastingSheetContent.Spells8.SlotsCount = characterSheetSpellcastingPageExportContent.Spells8.AvailableSlots;
                                spellcastingSheetContent.Spells9.SlotsCount = characterSheetSpellcastingPageExportContent.Spells9.AvailableSlots;
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell3 in characterSheetSpellcastingPageExportContent.Spells2.Spells)
                                {
                                    spellcastingSheetContent.Spells2.Collection.Add(new SpellcastingSpellContent(spell3.GetDisplayName(), spell3.IsPrepared));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell4 in characterSheetSpellcastingPageExportContent.Spells3.Spells)
                                {
                                    spellcastingSheetContent.Spells3.Collection.Add(new SpellcastingSpellContent(spell4.GetDisplayName(), spell4.IsPrepared));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell5 in characterSheetSpellcastingPageExportContent.Spells4.Spells)
                                {
                                    spellcastingSheetContent.Spells4.Collection.Add(new SpellcastingSpellContent(spell5.GetDisplayName(), spell5.IsPrepared));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell6 in characterSheetSpellcastingPageExportContent.Spells5.Spells)
                                {
                                    spellcastingSheetContent.Spells5.Collection.Add(new SpellcastingSpellContent(spell6.GetDisplayName(), spell6.IsPrepared));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell7 in characterSheetSpellcastingPageExportContent.Spells6.Spells)
                                {
                                    spellcastingSheetContent.Spells6.Collection.Add(new SpellcastingSpellContent(spell7.GetDisplayName(), spell7.IsPrepared));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell8 in characterSheetSpellcastingPageExportContent.Spells7.Spells)
                                {
                                    spellcastingSheetContent.Spells7.Collection.Add(new SpellcastingSpellContent(spell8.GetDisplayName(), spell8.IsPrepared));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell9 in characterSheetSpellcastingPageExportContent.Spells8.Spells)
                                {
                                    spellcastingSheetContent.Spells8.Collection.Add(new SpellcastingSpellContent(spell9.GetDisplayName(), spell9.IsPrepared));
                                }
                                foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell10 in characterSheetSpellcastingPageExportContent.Spells9.Spells)
                                {
                                    spellcastingSheetContent.Spells9.Collection.Add(new SpellcastingSpellContent(spell10.GetDisplayName(), spell10.IsPrepared));
                                }
                                StampSpellcastingContent(pdfStamper, spellcastingSheetContent, i + 1);
                            }
                        }
                        if (Configuration.IncludeFormatting)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append("<p><strong><em>Armor Proficiencies.</em></strong> " + ExportContent.ArmorProficiencies + "</p><p>&nbsp;</p>");
                            stringBuilder.Append("<p><strong><em>Weapon Proficiencies.</em></strong> " + ExportContent.WeaponProficiencies + "</p></p><p>&nbsp;</p>");
                            stringBuilder.Append("<p><strong><em>Tool Proficiencies.</em></strong> " + ExportContent.ToolProficiencies + "</p></p><p>&nbsp;</p>");
                            stringBuilder.Append("<p><strong><em>Languages.</em></strong> " + ExportContent.Languages + "</p>");
                            ReplaceField(pdfStamper, "details_proficiencies_languages", stringBuilder.ToString(), 8f);
                            ReplaceField(pdfStamper, "details_features", ExportContent.Features, 8f);
                            ReplaceField(pdfStamper, "details_additional_notes", ExportContent.TemporaryRacialTraits, 8f);
                            ReplaceField(pdfStamper, "background_feature", ExportContent.BackgroundContent.FeatureDescription, 7f, dynamic: false);
                            ReplaceField(pdfStamper, "background_traits", ExportContent.BackgroundContent.PersonalityTrait.Replace(Environment.NewLine, "<br>"), 7f, dynamic: false);
                            ReplaceField(pdfStamper, "background_ideals", ExportContent.BackgroundContent.Ideal, 7f, dynamic: false);
                            ReplaceField(pdfStamper, "background_bonds", ExportContent.BackgroundContent.Bond, 7f, dynamic: false);
                            ReplaceField(pdfStamper, "background_flaws", ExportContent.BackgroundContent.Flaw, 7f, dynamic: false);
                            ReplaceField(pdfStamper, "background_trinket", ExportContent.BackgroundContent.Trinket, 7f, dynamic: false);
                        }
                        pdfStamper.FormFlattening = !Configuration.IsFormFillable;
                        if (PartialFlatteningNames.Any())
                        {
                            foreach (string partialFlatteningName in PartialFlatteningNames)
                            {
                                pdfStamper.PartialFormFlattening(partialFlatteningName);
                            }
                            pdfStamper.FormFlattening = true;
                        }
                        if (list.Any())
                        {
                            foreach (string item in list)
                            {
                                pdfStamper.PartialFormFlattening(item);
                            }
                            pdfStamper.FormFlattening = true;
                        }
                    }
                }
            }
            fileInfo.Delete();
            return new FileInfo(destinationPath);
        }

        private FileInfo InitializeCharacterSheet()
        {
            string tempFileName = Path.GetTempFileName();
            using (FileStream stream = new FileStream(tempFileName, FileMode.Create))
            {
                List<PdfReader> list = new List<PdfReader>();
                if (Configuration.IncludeCharacterPage)
                {
                    if (Configuration.UseLegacyDetailsPage)
                    {
                        list.Add(CharacterSheetResources.GetLegacyDetailsPage().CreateReader());
                    }
                    else
                    {
                        list.Add(CharacterSheetResources.GetDetailsPage().CreateReader());
                    }
                }
                if (Configuration.IncludeBackgroundPage)
                {
                    if (Configuration.UseLegacyBackgroundPage)
                    {
                        list.Add(CharacterSheetResources.GetLegacyBackgroundPage().CreateReader());
                    }
                    else
                    {
                        list.Add(CharacterSheetResources.GetBackgroundPage().CreateReader());
                    }
                }
                AuroraDocumentResources auroraDocumentResources = new AuroraDocumentResources();
                if (Configuration.IncludeEquipmentPage)
                {
                    PdfReader item = new PdfReader(auroraDocumentResources.GetEquipmentPage());
                    list.Add(item);
                }
                if (Configuration.IncludeNotesPage)
                {
                    PdfReader item2 = new PdfReader(auroraDocumentResources.GetNotesPage());
                    list.Add(item2);
                }
                if (Configuration.IncludeCompanionPage)
                {
                    list.Add(CharacterSheetResources.GetCompanionPage().CreateReader());
                }
                if (Configuration.IncludeSpellcastingPage)
                {
                    if (Configuration.UseLegacySpellcastingPage)
                    {
                        CharacterSheetResourcePage legacySpellcastingPage = CharacterSheetResources.GetLegacySpellcastingPage();
                        for (int i = 1; i <= SpellcastingPageExportContentCollection.Count; i++)
                        {
                            PdfReader pdfReader = legacySpellcastingPage.CreateReader();
                            foreach (KeyValuePair<string, AcroFields.Item> field in pdfReader.AcroFields.Fields)
                            {
                                pdfReader.AcroFields.RenameField(field.Key, $"{field.Key}:{i}");
                            }
                            list.Add(pdfReader);
                        }
                    }
                    else
                    {
                        SpellcastingPageGenerator spellcastingPageGenerator = new SpellcastingPageGenerator();
                        foreach (CharacterSheetSpellcastingPageExportContent item3 in SpellcastingPageExportContentCollection)
                        {
                            spellcastingPageGenerator.Add(item3);
                        }
                        list.Add(spellcastingPageGenerator.AsReader());
                        List<string> partialFlatteningNames = spellcastingPageGenerator.PartialFlatteningNames;
                        PartialFlatteningNames.AddRange(partialFlatteningNames);
                    }
                }
                CardPageGenerator cardPageGenerator = new CardPageGenerator
                {
                    Flatten = !Configuration.IsEditable
                };
                bool flag = false;
                if (Configuration.IncludeSpellcards)
                {
                    ElementBaseCollection elements = CharacterManager.Current.GetElements();
                    foreach (Spell spell in Spells)
                    {
                        SpellCardContent spellCardContent = new SpellCardContent(spell.Name, spell.Underline);
                        spellCardContent.CastingTime = spell.CastingTime;
                        spellCardContent.Range = spell.Range;
                        spellCardContent.Duration = spell.Duration;
                        spellCardContent.Components = spell.GetComponentsString();
                        spellCardContent.Description = GeneratePlainDescription(spell.Description);
                        spellCardContent.DescriptionHtml = spell.Description;
                        ElementHeader parent = spell.Aquisition.GetParentHeader();
                        if (parent == null)
                        {
                            spellCardContent.LeftFooter = "Prepared (" + spell.Aquisition.PrepareParent + ")";
                        }
                        else
                        {
                            spellCardContent.LeftFooter = parent.Name;
                            try
                            {
                                if (parent.Type.Equals("Racial Trait"))
                                {
                                    ElementHeader elementHeader = elements.FirstOrDefault((ElementBase x) => x.Id == parent.Id)?.Aquisition.GetParentHeader();
                                    if (elementHeader != null)
                                    {
                                        spellCardContent.LeftFooter = parent.Name + " (" + elementHeader.Name + ")";
                                    }
                                }
                                if (parent.Name.StartsWith("Additional Spell,") || (parent.Name.StartsWith("Additional ") && parent.Name.Contains("Spell,")))
                                {
                                    spellCardContent.LeftFooter = parent.Name.Replace(spell.Name, "").Trim().Trim(',')
                                        .Trim();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, "InitializeCharacterSheet");
                                Dictionary<string, string> dictionary = AnalyticsErrorHelper.CreateProperties("id", parent.Id);
                                dictionary.Add("comment", "trying to set parent name on spell card");
                                AnalyticsErrorHelper.Exception(ex, dictionary, null, "InitializeCharacterSheet", 450);
                            }
                            foreach (ClassProgressionManager classProgressionManager in CharacterManager.Current.ClassProgressionManagers)
                            {
                                if ((from x in classProgressionManager.GetElements()
                                     where x.Type.Equals(parent.Type)
                                     select x.Id).Contains(parent.Id) && spellCardContent.LeftFooter != classProgressionManager.ClassElement.Name)
                                {
                                    spellCardContent.LeftFooter = spellCardContent.LeftFooter + " (" + classProgressionManager.ClassElement.Name + ")";
                                    break;
                                }
                            }
                        }
                        spellCardContent.RightFooter = spell.Source.Replace("Unearthed Arcana: ", "UA: ").Replace("Adventurers League: ", "AL: ");
                        cardPageGenerator.AddSpellCard(spellCardContent);
                        flag = true;
                    }
                }
                if (Configuration.IncludeAttackCards)
                {
                    if (Configuration.StartNewAttackCardsPage && flag)
                    {
                        cardPageGenerator.StartNewPage();
                    }
                    foreach (CharacterSheetExportContent.AttackExportContent item4 in ExportContent.AttacksContent)
                    {
                        if (item4.AsCard)
                        {
                            AttackCardContent attackCardContent = new AttackCardContent(item4.Name, item4.Underline);
                            attackCardContent.Title = item4.Name;
                            attackCardContent.Range = item4.Range;
                            attackCardContent.Attack = item4.Bonus;
                            attackCardContent.Damage = item4.Damage;
                            attackCardContent.Description = item4.Description;
                            cardPageGenerator.AddAttackCard(attackCardContent);
                            flag = true;
                        }
                    }
                }
                if (Configuration.IncludeItemcards)
                {
                    if (Configuration.StartNewItemCardsPage && flag)
                    {
                        cardPageGenerator.StartNewPage();
                    }
                    foreach (RefactoredEquipmentItem item5 in Items)
                    {
                        GenericCardContent genericCardContent = new GenericCardContent("", "");
                        genericCardContent.Title = (string.IsNullOrWhiteSpace(item5.AlternativeName) ? item5.DisplayName : item5.AlternativeName);
                        genericCardContent.Subtitle = item5.Item.Category;
                        genericCardContent.DescriptionHtml = (item5.IsAdorned ? item5.AdornerItem.Description : item5.Item.Description);
                        genericCardContent.LeftFooter = item5.Item.Weight;
                        genericCardContent.RightFooter = (item5.IsAdorned ? item5.AdornerItem.Source : item5.Item.Source);
                        cardPageGenerator.AddGenericCard(genericCardContent);
                        flag = true;
                    }
                }
                if (Configuration.IncludeFeatureCards)
                {
                    if (Configuration.StartNewFeatureCardsPage && flag)
                    {
                        cardPageGenerator.StartNewPage();
                    }
                    foreach (ElementBase feature in Features)
                    {
                        if (feature.SheetDescription.DisplayOnSheet)
                        {
                            GenericCardContent genericCardContent2 = new GenericCardContent(feature.Name, feature.Type, GeneratePlainDescription(feature.Description));
                            genericCardContent2.DescriptionHtml = feature.Description;
                            genericCardContent2.LeftFooter = feature.Aquisition.GetParentHeader()?.Name ?? "";
                            genericCardContent2.RightFooter = feature.Source;
                            cardPageGenerator.AddGenericCard(genericCardContent2);
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    list.Add(cardPageGenerator.AsReader());
                }
                ConcatenateReaders(list, stream);
            }
            return new FileInfo(tempFileName);
        }

        private void ConcatenateReaders(IEnumerable<PdfReader> readers, Stream stream)
        {
            PdfConcatenate pdfConcatenate = new PdfConcatenate(stream);
            pdfConcatenate.Writer.SetMergeFields();
            pdfConcatenate.Open();
            foreach (PdfReader reader in readers)
            {
                pdfConcatenate.Writer.AddDocument(reader);
            }
            pdfConcatenate.Close();
        }

        private void WriteExportContent(PdfStamper stamper)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "details_character_name", ExportContent.CharacterName },
            { "details_player", ExportContent.PlayerName },
            { "details_race", ExportContent.Race },
            {
                "details_background",
                ExportContent.BackgroundContent.Name
            },
            {
                "details_build",
                ExportContent.GetClassBuild()
            },
            { "details_alignment", ExportContent.Alignment },
            { "details_deity", ExportContent.Deity },
            { "details_xp", ExportContent.Experience }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value);
            }
            foreach (KeyValuePair<string, string> item2 in new Dictionary<string, string>
        {
            {
                Configuration.IsAttributeDisplayFlipped ? "details_str_modifier" : "details_str_score",
                ExportContent.AbilitiesContent.Strength
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_dex_modifier" : "details_dex_score",
                ExportContent.AbilitiesContent.Dexterity
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_con_modifier" : "details_con_score",
                ExportContent.AbilitiesContent.Constitution
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_int_modifier" : "details_int_score",
                ExportContent.AbilitiesContent.Intelligence
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_wis_modifier" : "details_wis_score",
                ExportContent.AbilitiesContent.Wisdom
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_cha_modifier" : "details_cha_score",
                ExportContent.AbilitiesContent.Charisma
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_str_score" : "details_str_modifier",
                ExportContent.AbilitiesContent.StrengthModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_dex_score" : "details_dex_modifier",
                ExportContent.AbilitiesContent.DexterityModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_con_score" : "details_con_modifier",
                ExportContent.AbilitiesContent.ConstitutionModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_int_score" : "details_int_modifier",
                ExportContent.AbilitiesContent.IntelligenceModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_wis_score" : "details_wis_modifier",
                ExportContent.AbilitiesContent.WisdomModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_cha_score" : "details_cha_modifier",
                ExportContent.AbilitiesContent.CharismaModifier
            }
        })
            {
                stamper.AcroFields.SetField(item2.Key, item2.Value);
            }
            foreach (KeyValuePair<string, string> item3 in new Dictionary<string, string>
        {
            {
                "details_acrobatics_total",
                ExportContent.SkillsContent.Acrobatics
            },
            {
                "details_animalhandling_total",
                ExportContent.SkillsContent.AnimalHandling
            },
            {
                "details_arcana_total",
                ExportContent.SkillsContent.Arcana
            },
            {
                "details_athletics_total",
                ExportContent.SkillsContent.Athletics
            },
            {
                "details_deception_total",
                ExportContent.SkillsContent.Deception
            },
            {
                "details_history_total",
                ExportContent.SkillsContent.History
            },
            {
                "details_insight_total",
                ExportContent.SkillsContent.Insight
            },
            {
                "details_intimidation_total",
                ExportContent.SkillsContent.Intimidation
            },
            {
                "details_investigation_total",
                ExportContent.SkillsContent.Investigation
            },
            {
                "details_perception_total",
                ExportContent.SkillsContent.Perception
            },
            {
                "details_nature_total",
                ExportContent.SkillsContent.Nature
            },
            {
                "details_performance_total",
                ExportContent.SkillsContent.Performance
            },
            {
                "details_medicine_total",
                ExportContent.SkillsContent.Medicine
            },
            {
                "details_religion_total",
                ExportContent.SkillsContent.Religion
            },
            {
                "details_stealth_total",
                ExportContent.SkillsContent.Stealth
            },
            {
                "details_persuasion_total",
                ExportContent.SkillsContent.Persuasion
            },
            {
                "details_sleightofhand_total",
                ExportContent.SkillsContent.SleightOfHand
            },
            {
                "details_survival_total",
                ExportContent.SkillsContent.Survival
            },
            {
                "details_acrobatics_proficiency",
                ExportContent.SkillsContent.AcrobaticsProficient ? "Yes" : "No"
            },
            {
                "details_animalhandling_proficiency",
                ExportContent.SkillsContent.AnimalHandlingProficient ? "Yes" : "No"
            },
            {
                "details_arcana_proficiency",
                ExportContent.SkillsContent.ArcanaProficient ? "Yes" : "No"
            },
            {
                "details_athletics_proficiency",
                ExportContent.SkillsContent.AthleticsProficient ? "Yes" : "No"
            },
            {
                "details_deception_proficiency",
                ExportContent.SkillsContent.DeceptionProficient ? "Yes" : "No"
            },
            {
                "details_history_proficiency",
                ExportContent.SkillsContent.HistoryProficient ? "Yes" : "No"
            },
            {
                "details_insight_proficiency",
                ExportContent.SkillsContent.InsightProficient ? "Yes" : "No"
            },
            {
                "details_intimidation_proficiency",
                ExportContent.SkillsContent.IntimidationProficient ? "Yes" : "No"
            },
            {
                "details_investigation_proficiency",
                ExportContent.SkillsContent.InvestigationProficient ? "Yes" : "No"
            },
            {
                "details_medicine_proficiency",
                ExportContent.SkillsContent.MedicineProficient ? "Yes" : "No"
            },
            {
                "details_nature_proficiency",
                ExportContent.SkillsContent.NatureProficient ? "Yes" : "No"
            },
            {
                "details_perception_proficiency",
                ExportContent.SkillsContent.PerceptionProficient ? "Yes" : "No"
            },
            {
                "details_performance_proficiency",
                ExportContent.SkillsContent.PerformanceProficient ? "Yes" : "No"
            },
            {
                "details_persuasion_proficiency",
                ExportContent.SkillsContent.PersuasionProficient ? "Yes" : "No"
            },
            {
                "details_religion_proficiency",
                ExportContent.SkillsContent.ReligionProficient ? "Yes" : "No"
            },
            {
                "details_sleightofhand_proficiency",
                ExportContent.SkillsContent.SleightOfHandProficient ? "Yes" : "No"
            },
            {
                "details_stealth_proficiency",
                ExportContent.SkillsContent.StealthProficient ? "Yes" : "No"
            },
            {
                "details_survival_proficiency",
                ExportContent.SkillsContent.SurvivalProficient ? "Yes" : "No"
            },
            {
                "details_acrobatics_expertise",
                ExportContent.SkillsContent.AcrobaticsExpertise ? "Yes" : "No"
            },
            {
                "details_animalhandling_expertise",
                ExportContent.SkillsContent.AnimalHandlingExpertise ? "Yes" : "No"
            },
            {
                "details_arcana_expertise",
                ExportContent.SkillsContent.ArcanaExpertise ? "Yes" : "No"
            },
            {
                "details_athletics_expertise",
                ExportContent.SkillsContent.AthleticsExpertise ? "Yes" : "No"
            },
            {
                "details_deception_expertise",
                ExportContent.SkillsContent.DeceptionExpertise ? "Yes" : "No"
            },
            {
                "details_history_expertise",
                ExportContent.SkillsContent.HistoryExpertise ? "Yes" : "No"
            },
            {
                "details_insight_expertise",
                ExportContent.SkillsContent.InsightExpertise ? "Yes" : "No"
            },
            {
                "details_intimidation_expertise",
                ExportContent.SkillsContent.IntimidationExpertise ? "Yes" : "No"
            },
            {
                "details_investigation_expertise",
                ExportContent.SkillsContent.InvestigationExpertise ? "Yes" : "No"
            },
            {
                "details_medicine_expertise",
                ExportContent.SkillsContent.MedicineExpertise ? "Yes" : "No"
            },
            {
                "details_nature_expertise",
                ExportContent.SkillsContent.NatureExpertise ? "Yes" : "No"
            },
            {
                "details_perception_expertise",
                ExportContent.SkillsContent.PerceptionExpertise ? "Yes" : "No"
            },
            {
                "details_performance_expertise",
                ExportContent.SkillsContent.PerformanceExpertise ? "Yes" : "No"
            },
            {
                "details_persuasion_expertise",
                ExportContent.SkillsContent.PersuasionExpertise ? "Yes" : "No"
            },
            {
                "details_religion_expertise",
                ExportContent.SkillsContent.ReligionExpertise ? "Yes" : "No"
            },
            {
                "details_sleightofhand_expertise",
                ExportContent.SkillsContent.SleightOfHandExpertise ? "Yes" : "No"
            },
            {
                "details_stealth_expertise",
                ExportContent.SkillsContent.StealthExpertise ? "Yes" : "No"
            },
            {
                "details_survival_expertise",
                ExportContent.SkillsContent.SurvivalExpertise ? "Yes" : "No"
            },
            {
                "details_passive_perception_total",
                ExportContent.SkillsContent.PerceptionPassive
            }
        })
            {
                stamper.AcroFields.SetField(item3.Key, item3.Value);
            }
            foreach (KeyValuePair<string, string> item4 in new Dictionary<string, string>
        {
            {
                "details_str_save_proficiency",
                ExportContent.AbilitiesContent.StrengthSaveProficient ? "Yes" : "No"
            },
            {
                "details_dex_save_proficiency",
                ExportContent.AbilitiesContent.DexteritySaveProficient ? "Yes" : "No"
            },
            {
                "details_con_save_proficiency",
                ExportContent.AbilitiesContent.ConstitutionSaveProficient ? "Yes" : "No"
            },
            {
                "details_int_save_proficiency",
                ExportContent.AbilitiesContent.IntelligenceSaveProficient ? "Yes" : "No"
            },
            {
                "details_wis_save_proficiency",
                ExportContent.AbilitiesContent.WisdomSaveProficient ? "Yes" : "No"
            },
            {
                "details_cha_save_proficiency",
                ExportContent.AbilitiesContent.CharismaSaveProficient ? "Yes" : "No"
            },
            {
                "details_str_save_total",
                ExportContent.AbilitiesContent.StrengthSave
            },
            {
                "details_dex_save_total",
                ExportContent.AbilitiesContent.DexteritySave
            },
            {
                "details_con_save_total",
                ExportContent.AbilitiesContent.ConstitutionSave
            },
            {
                "details_int_save_total",
                ExportContent.AbilitiesContent.IntelligenceSave
            },
            {
                "details_wis_save_total",
                ExportContent.AbilitiesContent.WisdomSave
            },
            {
                "details_cha_save_total",
                ExportContent.AbilitiesContent.CharismaSave
            },
            {
                "details_saving_throws",
                ExportContent.AbilitiesContent.ConditionalSave
            },
            {
                "details_inspiration",
                ExportContent.Inspiration ? "Yes" : ""
            },
            { "details_proficiency_bonus", ExportContent.ProficiencyBonus },
            { "details_initiative", ExportContent.Initiative },
            {
                "details_initiative_advantage",
                ExportContent.InitiativeAdvantage ? "Yes" : "No"
            },
            {
                "details_hp_max",
                ExportContent.HitPointsContent.Maximum
            },
            {
                "details_hp_current",
                ExportContent.HitPointsContent.Current
            },
            {
                "details_hp_temp",
                ExportContent.HitPointsContent.Temporary
            },
            {
                "details_hd",
                ExportContent.HitPointsContent.HitDice
            },
            {
                "details_death_save_success_1",
                ExportContent.HitPointsContent.DeathSavingThrowSuccess1 ? "Yes" : "No"
            },
            {
                "details_death_save_success_2",
                ExportContent.HitPointsContent.DeathSavingThrowSuccess2 ? "Yes" : "No"
            },
            {
                "details_death_save_success_3",
                ExportContent.HitPointsContent.DeathSavingThrowSuccess3 ? "Yes" : "No"
            },
            {
                "details_death_save_fail_1",
                ExportContent.HitPointsContent.DeathSavingThrowFailure1 ? "Yes" : "No"
            },
            {
                "details_death_save_fail_2",
                ExportContent.HitPointsContent.DeathSavingThrowFailure2 ? "Yes" : "No"
            },
            {
                "details_death_save_fail_3",
                ExportContent.HitPointsContent.DeathSavingThrowFailure3 ? "Yes" : "No"
            },
            {
                "details_speed_walking",
                ExportContent.ConditionsContent.WalkingSpeed + "ft."
            },
            {
                "details_speed_fly",
                (!string.IsNullOrWhiteSpace(ExportContent.ConditionsContent.FlySpeed)) ? (ExportContent.ConditionsContent.FlySpeed + "ft.") : ""
            },
            {
                "details_speed_climb",
                (!string.IsNullOrWhiteSpace(ExportContent.ConditionsContent.ClimbSpeed)) ? (ExportContent.ConditionsContent.ClimbSpeed + "ft.") : ""
            },
            {
                "details_speed_swim",
                (!string.IsNullOrWhiteSpace(ExportContent.ConditionsContent.SwimSpeed)) ? (ExportContent.ConditionsContent.SwimSpeed + "ft.") : ""
            },
            {
                "details_vision",
                ExportContent.ConditionsContent.Vision
            },
            {
                "details_resistances",
                ExportContent.ConditionsContent.Resistances
            },
            {
                "details_coinage_cp",
                ExportContent.EquipmentContent.Copper
            },
            {
                "details_coinage_sp",
                ExportContent.EquipmentContent.Silver
            },
            {
                "details_coinage_ep",
                ExportContent.EquipmentContent.Electrum
            },
            {
                "details_coinage_gp",
                ExportContent.EquipmentContent.Gold
            },
            {
                "details_coinage_pp",
                ExportContent.EquipmentContent.Platinum
            },
            {
                "details_equipment_weight",
                ExportContent.EquipmentContent.Weight + " lb."
            },
            { "details_attack_description", ExportContent.AttackAndSpellcastingField },
            {
                "details_encounter_box",
                (ExportContent.AttacksCount == "1") ? (ExportContent.AttacksCount + " Attack / Attack Action") : (ExportContent.AttacksCount + " Attacks / Attack Action")
            }
        })
            {
                stamper.AcroFields.SetField(item4.Key, item4.Value);
            }
            stamper.SetFontSize(6f, "details_encounter_box");
            foreach (KeyValuePair<string, string> item5 in new Dictionary<string, string>
        {
            {
                "details_armor_class",
                ExportContent.ArmorClassContent.ArmorClass
            },
            {
                "details_equipped_armor",
                ExportContent.ArmorClassContent.EquippedArmor
            },
            {
                "details_equipped_shield",
                ExportContent.ArmorClassContent.EquippedShield
            },
            {
                "details_armor_stealth_disadvantage",
                ExportContent.ArmorClassContent.StealthDisadvantage ? "Ja" : ""
            },
            {
                "details_armor_conditional",
                ExportContent.ArmorClassContent.ConditionalArmorClass
            }
        })
            {
                stamper.AcroFields.SetField(item5.Key, item5.Value);
            }
            int num = 1;
            foreach (CharacterSheetExportContent.AttackExportContent item6 in ExportContent.AttacksContent)
            {
                if (num > 4)
                {
                    break;
                }
                stamper.AcroFields.SetField($"details_attack{num}_weapon", item6.Name);
                stamper.AcroFields.SetField($"details_attack{num}_range", item6.Range);
                stamper.AcroFields.SetField($"details_attack{num}_attack", item6.Bonus);
                stamper.AcroFields.SetField($"details_attack{num}_damage", item6.Damage);
                stamper.AcroFields.SetField($"details_attack{num}_misc", item6.Misc);
                stamper.AcroFields.SetField($"details_attack{num}_description", item6.Description);
                num++;
            }
            for (int i = 0; i < Math.Min(ExportContent.EquipmentContent.Equipment.Count, 15); i++)
            {
                stamper.AcroFields.SetField($"details_equipment_amount_1.{i}", ExportContent.EquipmentContent.Equipment[i].Item1);
                stamper.AcroFields.SetField($"details_equipment_name_1.{i}", ExportContent.EquipmentContent.Equipment[i].Item2);
            }
            if (!Configuration.IncludeFormatting)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Armor Proficiencies. " + ExportContent.ArmorProficiencies);
                stringBuilder.AppendLine("Weapon Proficiencies. " + ExportContent.WeaponProficiencies);
                stringBuilder.AppendLine("Tool Proficiencies. " + ExportContent.ToolProficiencies);
                stringBuilder.AppendLine("Languages. " + ExportContent.Languages);
                foreach (KeyValuePair<string, string> item7 in new Dictionary<string, string>
            {
                {
                    "details_proficiencies_languages",
                    stringBuilder.ToString()
                },
                { "details_features", ExportContent.Features },
                { "details_additional_notes", ExportContent.TemporaryRacialTraits }
            })
                {
                    stamper.AcroFields.SetField(item7.Key, item7.Value);
                }
            }
            foreach (KeyValuePair<string, string> item8 in new Dictionary<string, string>
        {
            { "background_character_name", ExportContent.CharacterName },
            {
                "background_story",
                ExportContent.BackgroundContent.Story
            },
            { "background_allies", ExportContent.AlliesAndOrganizations },
            { "background_organization_name", ExportContent.OrganizationName },
            {
                "background_gender",
                ExportContent.AppearanceContent.Gender
            },
            {
                "background_age",
                ExportContent.AppearanceContent.Age
            },
            {
                "background_eyes",
                ExportContent.AppearanceContent.Eyes
            },
            {
                "background_hair",
                ExportContent.AppearanceContent.Hair
            },
            {
                "background_height",
                ExportContent.AppearanceContent.Height
            },
            {
                "background_skin",
                ExportContent.AppearanceContent.Skin
            },
            {
                "background_weight",
                ExportContent.AppearanceContent.Weight
            },
            {
                "background_traits",
                ExportContent.BackgroundContent.PersonalityTrait
            },
            {
                "background_ideals",
                ExportContent.BackgroundContent.Ideal
            },
            {
                "background_bonds",
                ExportContent.BackgroundContent.Bond
            },
            {
                "background_flaws",
                ExportContent.BackgroundContent.Flaw
            },
            {
                "background_feature_name",
                ExportContent.BackgroundContent.FeatureName
            },
            {
                "background_feature",
                ExportContent.BackgroundContent.FeatureDescription
            },
            {
                "background_trinket",
                ExportContent.BackgroundContent.Trinket
            },
            { "background_additional_features", ExportContent.AdditionalFeaturesAndTraits },
            {
                "background_additional_treasure",
                ExportContent.EquipmentContent.AdditionalTreasure
            }
        })
            {
                stamper.AcroFields.SetField(item8.Key, item8.Value);
            }
            stamper.SetFontSize(0f, "background_feature");
            WriteImage(stamper, "background_portrait_image", ExportContent.AppearanceContent.Portrait);
            WriteImage(stamper, "background_organization_image", ExportContent.OrganizationSymbol);
        }

        private void WriteCompanionExportContent(PdfStamper stamper)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "companion_name", ExportContent.CompanionName },
            { "companion_kind", ExportContent.CompanionKind },
            { "companion_build", ExportContent.CompanionBuild },
            { "companion_skills", ExportContent.CompanionSkills },
            { "companion_appearance", ExportContent.CompanionSkills },
            { "companion_challenge", ExportContent.CompanionChallenge },
            { "companion_owner", ExportContent.CompanionOwner }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value);
            }
            foreach (KeyValuePair<string, string> item2 in new Dictionary<string, string>
        {
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_str_modifier" : "companion_str_score",
                ExportContent.CompanionAbilitiesContent.Strength
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_dex_modifier" : "companion_dex_score",
                ExportContent.CompanionAbilitiesContent.Dexterity
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_con_modifier" : "companion_con_score",
                ExportContent.CompanionAbilitiesContent.Constitution
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_int_modifier" : "companion_int_score",
                ExportContent.CompanionAbilitiesContent.Intelligence
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_wis_modifier" : "companion_wis_score",
                ExportContent.CompanionAbilitiesContent.Wisdom
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_cha_modifier" : "companion_cha_score",
                ExportContent.CompanionAbilitiesContent.Charisma
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_str_score" : "companion_str_modifier",
                ExportContent.CompanionAbilitiesContent.StrengthModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_dex_score" : "companion_dex_modifier",
                ExportContent.CompanionAbilitiesContent.DexterityModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_con_score" : "companion_con_modifier",
                ExportContent.CompanionAbilitiesContent.ConstitutionModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_int_score" : "companion_int_modifier",
                ExportContent.CompanionAbilitiesContent.IntelligenceModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_wis_score" : "companion_wis_modifier",
                ExportContent.CompanionAbilitiesContent.WisdomModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "companion_cha_score" : "companion_cha_modifier",
                ExportContent.CompanionAbilitiesContent.CharismaModifier
            }
        })
            {
                stamper.AcroFields.SetField(item2.Key, item2.Value);
            }
            foreach (KeyValuePair<string, string> item3 in new Dictionary<string, string>
        {
            {
                "companion_acrobatics_total",
                ExportContent.CompanionSkillsContent.Acrobatics
            },
            {
                "companion_animalhandling_total",
                ExportContent.CompanionSkillsContent.AnimalHandling
            },
            {
                "companion_arcana_total",
                ExportContent.CompanionSkillsContent.Arcana
            },
            {
                "companion_athletics_total",
                ExportContent.CompanionSkillsContent.Athletics
            },
            {
                "companion_deception_total",
                ExportContent.CompanionSkillsContent.Deception
            },
            {
                "companion_history_total",
                ExportContent.CompanionSkillsContent.History
            },
            {
                "companion_insight_total",
                ExportContent.CompanionSkillsContent.Insight
            },
            {
                "companion_intimidation_total",
                ExportContent.CompanionSkillsContent.Intimidation
            },
            {
                "companion_investigation_total",
                ExportContent.CompanionSkillsContent.Investigation
            },
            {
                "companion_perception_total",
                ExportContent.CompanionSkillsContent.Perception
            },
            {
                "companion_nature_total",
                ExportContent.CompanionSkillsContent.Nature
            },
            {
                "companion_performance_total",
                ExportContent.CompanionSkillsContent.Performance
            },
            {
                "companion_medicine_total",
                ExportContent.CompanionSkillsContent.Medicine
            },
            {
                "companion_religion_total",
                ExportContent.CompanionSkillsContent.Religion
            },
            {
                "companion_stealth_total",
                ExportContent.CompanionSkillsContent.Stealth
            },
            {
                "companion_persuasion_total",
                ExportContent.CompanionSkillsContent.Persuasion
            },
            {
                "companion_sleightofhand_total",
                ExportContent.CompanionSkillsContent.SleightOfHand
            },
            {
                "companion_survival_total",
                ExportContent.CompanionSkillsContent.Survival
            },
            {
                "companion_acrobatics_proficiency",
                ExportContent.CompanionSkillsContent.AcrobaticsProficient ? "Yes" : "No"
            },
            {
                "companion_animalhandling_proficiency",
                ExportContent.CompanionSkillsContent.AnimalHandlingProficient ? "Yes" : "No"
            },
            {
                "companion_arcana_proficiency",
                ExportContent.CompanionSkillsContent.ArcanaProficient ? "Yes" : "No"
            },
            {
                "companion_athletics_proficiency",
                ExportContent.CompanionSkillsContent.AthleticsProficient ? "Yes" : "No"
            },
            {
                "companion_deception_proficiency",
                ExportContent.CompanionSkillsContent.DeceptionProficient ? "Yes" : "No"
            },
            {
                "companion_history_proficiency",
                ExportContent.CompanionSkillsContent.HistoryProficient ? "Yes" : "No"
            },
            {
                "companion_insight_proficiency",
                ExportContent.CompanionSkillsContent.InsightProficient ? "Yes" : "No"
            },
            {
                "companion_intimidation_proficiency",
                ExportContent.CompanionSkillsContent.IntimidationProficient ? "Yes" : "No"
            },
            {
                "companion_investigation_proficiency",
                ExportContent.CompanionSkillsContent.InvestigationProficient ? "Yes" : "No"
            },
            {
                "companion_medicine_proficiency",
                ExportContent.CompanionSkillsContent.MedicineProficient ? "Yes" : "No"
            },
            {
                "companion_nature_proficiency",
                ExportContent.CompanionSkillsContent.NatureProficient ? "Yes" : "No"
            },
            {
                "companion_perception_proficiency",
                ExportContent.CompanionSkillsContent.PerceptionProficient ? "Yes" : "No"
            },
            {
                "companion_performance_proficiency",
                ExportContent.CompanionSkillsContent.PerformanceProficient ? "Yes" : "No"
            },
            {
                "companion_persuasion_proficiency",
                ExportContent.CompanionSkillsContent.PersuasionProficient ? "Yes" : "No"
            },
            {
                "companion_religion_proficiency",
                ExportContent.CompanionSkillsContent.ReligionProficient ? "Yes" : "No"
            },
            {
                "companion_sleightofhand_proficiency",
                ExportContent.CompanionSkillsContent.SleightOfHandProficient ? "Yes" : "No"
            },
            {
                "companion_stealth_proficiency",
                ExportContent.CompanionSkillsContent.StealthProficient ? "Yes" : "No"
            },
            {
                "companion_survival_proficiency",
                ExportContent.CompanionSkillsContent.SurvivalProficient ? "Yes" : "No"
            },
            {
                "companion_acrobatics_expertise",
                ExportContent.CompanionSkillsContent.AcrobaticsExpertise ? "Yes" : "No"
            },
            {
                "companion_animalhandling_expertise",
                ExportContent.CompanionSkillsContent.AnimalHandlingExpertise ? "Yes" : "No"
            },
            {
                "companion_arcana_expertise",
                ExportContent.CompanionSkillsContent.ArcanaExpertise ? "Yes" : "No"
            },
            {
                "companion_athletics_expertise",
                ExportContent.CompanionSkillsContent.AthleticsExpertise ? "Yes" : "No"
            },
            {
                "companion_deception_expertise",
                ExportContent.CompanionSkillsContent.DeceptionExpertise ? "Yes" : "No"
            },
            {
                "companion_history_expertise",
                ExportContent.CompanionSkillsContent.HistoryExpertise ? "Yes" : "No"
            },
            {
                "companion_insight_expertise",
                ExportContent.CompanionSkillsContent.InsightExpertise ? "Yes" : "No"
            },
            {
                "companion_intimidation_expertise",
                ExportContent.CompanionSkillsContent.IntimidationExpertise ? "Yes" : "No"
            },
            {
                "companion_investigation_expertise",
                ExportContent.CompanionSkillsContent.InvestigationExpertise ? "Yes" : "No"
            },
            {
                "companion_medicine_expertise",
                ExportContent.CompanionSkillsContent.MedicineExpertise ? "Yes" : "No"
            },
            {
                "companion_nature_expertise",
                ExportContent.CompanionSkillsContent.NatureExpertise ? "Yes" : "No"
            },
            {
                "companion_perception_expertise",
                ExportContent.CompanionSkillsContent.PerceptionExpertise ? "Yes" : "No"
            },
            {
                "companion_performance_expertise",
                ExportContent.CompanionSkillsContent.PerformanceExpertise ? "Yes" : "No"
            },
            {
                "companion_persuasion_expertise",
                ExportContent.CompanionSkillsContent.PersuasionExpertise ? "Yes" : "No"
            },
            {
                "companion_religion_expertise",
                ExportContent.CompanionSkillsContent.ReligionExpertise ? "Yes" : "No"
            },
            {
                "companion_sleightofhand_expertise",
                ExportContent.CompanionSkillsContent.SleightOfHandExpertise ? "Yes" : "No"
            },
            {
                "companion_stealth_expertise",
                ExportContent.CompanionSkillsContent.StealthExpertise ? "Yes" : "No"
            },
            {
                "companion_survival_expertise",
                ExportContent.CompanionSkillsContent.SurvivalExpertise ? "Yes" : "No"
            },
            {
                "companion_passive_perception_total",
                ExportContent.CompanionSkillsContent.PerceptionPassive
            }
        })
            {
                stamper.AcroFields.SetField(item3.Key, item3.Value);
            }
            foreach (KeyValuePair<string, string> item4 in new Dictionary<string, string>
        {
            { "companion_proficiency", ExportContent.CompanionProficiency },
            {
                "companion_str_save_total",
                ExportContent.CompanionAbilitiesContent.StrengthSave
            },
            {
                "companion_dex_save_total",
                ExportContent.CompanionAbilitiesContent.DexteritySave
            },
            {
                "companion_con_save_total",
                ExportContent.CompanionAbilitiesContent.ConstitutionSave
            },
            {
                "companion_int_save_total",
                ExportContent.CompanionAbilitiesContent.IntelligenceSave
            },
            {
                "companion_wis_save_total",
                ExportContent.CompanionAbilitiesContent.WisdomSave
            },
            {
                "companion_cha_save_total",
                ExportContent.CompanionAbilitiesContent.CharismaSave
            },
            {
                "companion_saving_throws",
                ExportContent.CompanionAbilitiesContent.ConditionalSave
            },
            {
                "companion_str_save_proficiency",
                ExportContent.CompanionAbilitiesContent.StrengthSaveProficient ? "Yes" : "No"
            },
            {
                "companion_dex_save_proficiency",
                ExportContent.CompanionAbilitiesContent.DexteritySaveProficient ? "Yes" : "No"
            },
            {
                "companion_con_save_proficiency",
                ExportContent.CompanionAbilitiesContent.ConstitutionSaveProficient ? "Yes" : "No"
            },
            {
                "companion_int_save_proficiency",
                ExportContent.CompanionAbilitiesContent.IntelligenceSaveProficient ? "Yes" : "No"
            },
            {
                "companion_wis_save_proficiency",
                ExportContent.CompanionAbilitiesContent.WisdomSaveProficient ? "Yes" : "No"
            },
            {
                "companion_cha_save_proficiency",
                ExportContent.CompanionAbilitiesContent.CharismaSaveProficient ? "Yes" : "No"
            },
            {
                "companion_hp_max",
                ExportContent.CompanionHitPointsContent.Maximum
            },
            { "companion_speed", ExportContent.CompanionSpeedString },
            {
                "companion_speed_walking",
                ExportContent.CompanionConditionsContent.WalkingSpeed + "ft."
            },
            {
                "companion_speed_fly",
                (!string.IsNullOrWhiteSpace(ExportContent.CompanionConditionsContent.FlySpeed)) ? (ExportContent.CompanionConditionsContent.FlySpeed + "ft.") : ""
            },
            {
                "companion_speed_climb",
                (!string.IsNullOrWhiteSpace(ExportContent.CompanionConditionsContent.ClimbSpeed)) ? (ExportContent.CompanionConditionsContent.ClimbSpeed + "ft.") : ""
            },
            {
                "companion_speed_swim",
                (!string.IsNullOrWhiteSpace(ExportContent.CompanionConditionsContent.SwimSpeed)) ? (ExportContent.CompanionConditionsContent.SwimSpeed + "ft.") : ""
            },
            {
                "companion_speed_burrow",
                (!string.IsNullOrWhiteSpace(ExportContent.CompanionConditionsContent.BurrowSpeed)) ? (ExportContent.CompanionConditionsContent.BurrowSpeed + "ft.") : ""
            },
            {
                "companion_vision",
                ExportContent.CompanionConditionsContent.Vision
            },
            {
                "companion_resistances",
                ExportContent.CompanionConditionsContent.Resistances
            },
            {
                "companion_stats",
                ExportContent.CompanionConditionsContent.Resistances
            }
        })
            {
                stamper.AcroFields.SetField(item4.Key, item4.Value);
            }
            foreach (KeyValuePair<string, string> item5 in new Dictionary<string, string>
        {
            {
                "companion_armor_class",
                ExportContent.CompanionArmorClassContent.ArmorClass
            },
            { "companion_initiative", ExportContent.CompanionInitiative },
            {
                "companion_armor_conditional",
                ExportContent.CompanionArmorClassContent.ConditionalArmorClass
            }
        })
            {
                stamper.AcroFields.SetField(item5.Key, item5.Value);
            }
            WriteImage(stamper, "companion_portrait_image", ExportContent.CompanionPortrait);
            if (Configuration.IncludeFormatting)
            {
                ReplaceField(stamper, "companion_features", ExportContent.CompanionFeatures);
                ReplaceField(stamper, "companion_stats", ExportContent.CompanionStats);
            }
            else
            {
                stamper.AcroFields.SetField("companion_features", ExportContent.CompanionFeatures);
                stamper.AcroFields.SetField("companion_stats", ExportContent.CompanionStats);
            }
        }

        private void WriteImage(PdfStamper stamper, string fieldName, string imagePath)
        {
            try
            {
                AcroFields.FieldPosition fieldPosition = stamper.AcroFields.GetFieldPositions(fieldName)?.FirstOrDefault();
                if (fieldPosition != null && File.Exists(imagePath))
                {
                    PushbuttonField pushbuttonField = new PushbuttonField(stamper.Writer, fieldPosition.position, fieldName + ":replaced")
                    {
                        Layout = 2,
                        Image = Image.GetInstance(imagePath),
                        ProportionalIcon = true,
                        Options = 1
                    };
                    stamper.AddAnnotation(pushbuttonField.Field, fieldPosition.page);
                    stamper.AcroFields.RemoveField(fieldName);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "WriteImage");
            }
        }

        private void ReplaceField(PdfStamper stamper, string name, string htmlContent, float fontsize = 7f, bool dynamic = true)
        {
            if (!stamper.AcroFields.Fields.ContainsKey(name))
            {
                return;
            }
            name.Contains("features");
            Rectangle position = stamper.AcroFields.GetFieldPositions(name)[0].position;
            int page = stamper.AcroFields.GetFieldPositions(name)[0].page;
            stamper.AcroFields.RemoveField(name);
            Font regular = Builder.Presentation.Models.CharacterSheet.PDF.FontsHelper.GetRegular();
            float num = ColumnText.FitText(regular, htmlContent + Environment.NewLine + "<p></p>", position, fontsize, 1);
            if (num < fontsize)
            {
                string text = "";
                if (htmlContent.Length > 1000)
                {
                    text = text + "<p>&nbsp;</p><p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 2500)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 3200)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 4000)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 4800)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 5600)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 5800)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 6000)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 6200)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                if (htmlContent.Length > 7000)
                {
                    text = text + "<p>&nbsp;</p>" + Environment.NewLine;
                }
                num = ColumnText.FitText(regular, htmlContent + text + Environment.NewLine, position, fontsize, 1);
                if (htmlContent.Length > 6000)
                {
                    num = Math.Min(num, 5f);
                }
            }
            ColumnText columnText = new ColumnText(stamper.GetOverContent(page));
            columnText.SetSimpleColumn(position);
            ElementDescriptionGenerator.FillSheetColumn(columnText, htmlContent, num, dynamic);
            columnText.Go();
        }

        [Obsolete]
        private void StampMainContentRevamped(PdfStamper stamper, MainSheetContent content)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "details_character_name", content.CharacterName },
            { "details_player", content.PlayerName },
            { "details_race", content.Race },
            { "details_background", content.Background },
            { "details_build", content.ClassLevel },
            { "details_alignment", content.Alignment },
            { "details_xp", content.Experience }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value);
            }
            foreach (KeyValuePair<string, string> item2 in new Dictionary<string, string>
        {
            {
                Configuration.IsAttributeDisplayFlipped ? "details_str_modifier" : "details_str_score",
                content.Strength
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_dex_modifier" : "details_dex_score",
                content.Dexterity
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_con_modifier" : "details_con_score",
                content.Constitution
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_int_modifier" : "details_int_score",
                content.Intelligence
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_wis_modifier" : "details_wis_score",
                content.Wisdom
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_cha_modifier" : "details_cha_score",
                content.Charisma
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_str_score" : "details_str_modifier",
                content.StrengthModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_dex_score" : "details_dex_modifier",
                content.DexterityModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_con_score" : "details_con_modifier",
                content.ConstitutionModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_int_score" : "details_int_modifier",
                content.IntelligenceModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_wis_score" : "details_wis_modifier",
                content.WisdomModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "details_cha_score" : "details_cha_modifier",
                content.CharismaModifier
            }
        })
            {
                stamper.AcroFields.SetField(item2.Key, item2.Value);
            }
            foreach (KeyValuePair<string, string> item3 in new Dictionary<string, string>
        {
            { "details_acrobatics_total", content.Acrobatics },
            { "details_animalhandling_total", content.AnimalHandling },
            { "details_arcana_total", content.Arcana },
            { "details_athletics_total", content.Athletics },
            { "details_deception_total", content.Deception },
            { "details_history_total", content.History },
            { "details_insight_total", content.Insight },
            { "details_intimidation_total", content.Intimidation },
            { "details_investigation_total", content.Investigation },
            { "details_perception_total", content.Perception },
            { "details_nature_total", content.Nature },
            { "details_performance_total", content.Performance },
            { "details_medicine_total", content.Medicine },
            { "details_religion_total", content.Religion },
            { "details_stealth_total", content.Stealth },
            { "details_persuasion_total", content.Persuasion },
            { "details_sleightofhand_total", content.SleightOfHand },
            { "details_survival_total", content.Survival },
            {
                "details_acrobatics_proficiency",
                content.AcrobaticsProficient ? "Yes" : "No"
            },
            {
                "details_animalhandling_proficiency",
                content.AnimalHandlingProficient ? "Yes" : "No"
            },
            {
                "details_arcana_proficiency",
                content.ArcanaProficient ? "Yes" : "No"
            },
            {
                "details_athletics_proficiency",
                content.AthleticsProficient ? "Yes" : "No"
            },
            {
                "details_deception_proficiency",
                content.DeceptionProficient ? "Yes" : "No"
            },
            {
                "details_history_proficiency",
                content.HistoryProficient ? "Yes" : "No"
            },
            {
                "details_insight_proficiency",
                content.InsightProficient ? "Yes" : "No"
            },
            {
                "details_intimidation_proficiency",
                content.IntimidationProficient ? "Yes" : "No"
            },
            {
                "details_investigation_proficiency",
                content.InvestigationProficient ? "Yes" : "No"
            },
            {
                "details_medicine_proficiency",
                content.MedicineProficient ? "Yes" : "No"
            },
            {
                "details_nature_proficiency",
                content.NatureProficient ? "Yes" : "No"
            },
            {
                "details_perception_proficiency",
                content.PerceptionProficient ? "Yes" : "No"
            },
            {
                "details_performance_proficiency",
                content.PerformanceProficient ? "Yes" : "No"
            },
            {
                "details_persuasion_proficiency",
                content.PersuasionProficient ? "Yes" : "No"
            },
            {
                "details_religion_proficiency",
                content.ReligionProficient ? "Yes" : "No"
            },
            {
                "details_sleightofhand_proficiency",
                content.SleightOfHandProficient ? "Yes" : "No"
            },
            {
                "details_stealth_proficiency",
                content.StealthProficient ? "Yes" : "No"
            },
            {
                "details_survival_proficiency",
                content.SurvivalProficient ? "Yes" : "No"
            },
            { "details_passive_perception_total", content.PassiveWisdomPerception },
            {
                "details_inspiration",
                content.Inspiration ? "x" : ""
            },
            { "details_proficiency_bonus", content.ProficiencyBonus },
            { "details_str_save_total", content.StrengthSavingThrow },
            { "details_dex_save_total", content.DexteritySavingThrow },
            { "details_con_save_total", content.ConstitutionSavingThrow },
            { "details_int_save_total", content.IntelligenceSavingThrow },
            { "details_wis_save_total", content.WisdomSavingThrow },
            { "details_cha_save_total", content.CharismaSavingThrow },
            {
                "details_saving_throws",
                ExportContent.AbilitiesContent.ConditionalSave
            },
            {
                "details_str_save_proficiency",
                content.StrengthSavingThrowProficient ? "Yes" : "No"
            },
            {
                "details_dex_save_proficiency",
                content.DexteritySavingThrowProficient ? "Yes" : "No"
            },
            {
                "details_con_save_proficiency",
                content.ConstitutionSavingThrowProficient ? "Yes" : "No"
            },
            {
                "details_int_save_proficiency",
                content.IntelligenceSavingThrowProficient ? "Yes" : "No"
            },
            {
                "details_wis_save_proficiency",
                content.WisdomSavingThrowProficient ? "Yes" : "No"
            },
            {
                "details_cha_save_proficiency",
                content.CharismaSavingThrowProficient ? "Yes" : "No"
            },
            { "details_armor_class", content.ArmorClass },
            { "details_initiative", content.Initiative },
            {
                "details_initiative_advantage",
                content.InitiativeAdvantage ? "Yes" : "No"
            },
            {
                "details_speed_walking",
                content.Speed + "ft."
            },
            { "details_hp_max", content.MaximumHitPoints },
            { "details_hp_current", content.CurrentHitPoints },
            { "details_hp_temp", content.TemporaryHitPoints },
            { "details_hd", content.TotalHitDice },
            {
                "details_death_save_success_1",
                content.DeathSavingThrowSuccess1 ? "Yes" : "No"
            },
            {
                "details_death_save_success_2",
                content.DeathSavingThrowSuccess2 ? "Yes" : "No"
            },
            {
                "details_death_save_success_3",
                content.DeathSavingThrowSuccess3 ? "Yes" : "No"
            },
            {
                "details_death_save_fail_1",
                content.DeathSavingThrowFailure1 ? "Yes" : "No"
            },
            {
                "details_death_save_fail_2",
                content.DeathSavingThrowFailure2 ? "Yes" : "No"
            },
            {
                "details_death_save_fail_3",
                content.DeathSavingThrowFailure3 ? "Yes" : "No"
            },
            {
                "details_vision",
                ExportContent.ConditionsContent.Vision
            },
            { "details_attack1_weapon", content.Name1 },
            { "details_attack1_attack", content.AttackBonus1 },
            { "details_attack1_damage", content.DamageType1 },
            { "details_attack2_weapon", content.Name2 },
            { "details_attack2_attack", content.AttackBonus2 },
            { "details_attack2_damage", content.DamageType2 },
            { "details_attack3_weapon", content.Name3 },
            { "details_attack3_attack", content.AttackBonus3 },
            { "details_attack3_damage", content.DamageType3 },
            { "details_attack_description", content.AttackAndSpellcastingField },
            { "details_coinage_cp", content.Copper },
            { "details_coinage_sp", content.Silver },
            { "details_coinage_ep", content.Electrum },
            { "details_coinage_gp", content.Gold },
            { "details_coinage_pp", content.Platinum },
            {
                "details_equipment_weight",
                ExportContent.EquipmentContent.Weight
            }
        })
            {
                stamper.AcroFields.SetField(item3.Key, item3.Value);
            }
            foreach (KeyValuePair<string, string> item4 in new Dictionary<string, string>
        {
            { "details_equipped_armor", content.EquippedArmor },
            { "details_equipped_shield", content.EquippedShield },
            { "details_armor_conditional", content.ConditionalArmorClass },
            {
                "details_armor_stealth_disadvantage",
                content.StealthDisadvantage ? "Yes" : "No"
            }
        })
            {
                stamper.AcroFields.SetField(item4.Key, item4.Value);
            }
            int num = 1;
            foreach (CharacterSheetExportContent.AttackExportContent item5 in ExportContent.AttacksContent)
            {
                if (num > 4)
                {
                    break;
                }
                stamper.AcroFields.SetField($"details_attack{num}_weapon", item5.Name);
                stamper.AcroFields.SetField($"details_attack{num}_range", item5.Range);
                stamper.AcroFields.SetField($"details_attack{num}_attack", item5.Bonus);
                stamper.AcroFields.SetField($"details_attack{num}_damage", item5.Damage);
                stamper.AcroFields.SetField($"details_attack{num}_misc", item5.Misc);
                stamper.AcroFields.SetField($"details_attack{num}_description", item5.Description);
                num++;
            }
            for (int i = 0; i < Math.Min(ExportContent.EquipmentContent.Equipment.Count, 15); i++)
            {
                stamper.AcroFields.SetField($"details_equipment_amount_1.{i}", ExportContent.EquipmentContent.Equipment[i].Item1);
                stamper.AcroFields.SetField("details_equipment_name_1." + i, ExportContent.EquipmentContent.Equipment[i].Item2);
                stamper.SetFontSize(7f, $"details_equipment_amount_1.{i}", $"details_equipment_name_1.{i}");
            }
            if (Configuration.IncludeFormatting)
            {
                return;
            }
            foreach (KeyValuePair<string, string> item6 in new Dictionary<string, string>
        {
            { "details_proficiencies_languages", content.ProficienciesAndLanguages },
            { "details_features", content.FeaturesAndTraitsField }
        })
            {
                stamper.AcroFields.SetField(item6.Key, item6.Value);
            }
            if (Configuration.IncludeFeatureCards)
            {
                GenerateFeatureCards(stamper, content);
            }
        }

        [Obsolete]
        private void StampDetailsContentRevamped(PdfStamper stamper, MainSheetContent mainContent, DetailsSheetContent content)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "background_character_name", content.CharacterName },
            { "background_story", content.CharacterBackstory },
            { "background_allies", content.AlliesAndOrganizations },
            { "background_organization_name", content.OrganizationName },
            { "background_age", content.Age },
            { "background_eyes", content.Eyes },
            { "background_hair", content.Hair },
            { "background_height", content.Height },
            { "background_skin", content.Skin },
            { "background_weight", content.Weight },
            { "background_traits", mainContent.PersonalityTraits },
            { "background_ideals", mainContent.Ideals },
            { "background_bonds", mainContent.Bonds },
            { "background_flaws", mainContent.Flaws },
            { "background_feature_name", content.BackgroundFeatureName },
            { "background_feature", content.BackgroundFeature },
            { "background_trinket", content.Trinket }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value, saveAppearance: false);
            }
            try
            {
                AcroFields.FieldPosition fieldPosition = stamper.AcroFields.GetFieldPositions("background_portrait_image").FirstOrDefault();
                if (fieldPosition != null)
                {
                    PushbuttonField pushbuttonField = new PushbuttonField(stamper.Writer, fieldPosition.position, "background_portrait_image-replaced")
                    {
                        Layout = 2,
                        Image = Image.GetInstance(content.CharacterAppearance),
                        ProportionalIcon = true,
                        Options = 1
                    };
                    stamper.AddAnnotation(pushbuttonField.Field, fieldPosition.page);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "StampDetailsContentRevamped");
            }
            try
            {
                AcroFields.FieldPosition fieldPosition2 = stamper.AcroFields.GetFieldPositions("background_organization_image").FirstOrDefault();
                if (fieldPosition2 != null && !string.IsNullOrWhiteSpace(content.OrganizationSymbol))
                {
                    PushbuttonField pushbuttonField2 = new PushbuttonField(stamper.Writer, fieldPosition2.position, "background_organization_image-replaced")
                    {
                        Layout = 2,
                        Image = Image.GetInstance(content.OrganizationSymbol),
                        ProportionalIcon = true,
                        Options = 1
                    };
                    stamper.AddAnnotation(pushbuttonField2.Field, fieldPosition2.page);
                }
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "StampDetailsContentRevamped");
            }
        }

        [Obsolete]
        private void StampSpellcastingContentRevamped(PdfStamper stamper, SpellcastingSheetContent content, int page)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "spellcasting_class", content.SpellcastingClass },
            { "spellcasting_ability", content.SpellcastingAbility },
            { "spellcasting_save", content.SpellcastingSave },
            { "spellcasting_attack_bonus", content.SpellcastingAttackModifier },
            { "spellcasting_prepare_count", content.SpellcastingPrepareCount },
            { "spellcasting_notes", content.SpellcastingNotes }
        })
            {
                stamper.AcroFields.SetField($"{item.Key}:{page}", item.Value);
            }
            stamper.AcroFields.SetField($"spellcasting_slot1_count:{page}", content.Spells1.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot2_count:{page}", content.Spells2.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot3_count:{page}", content.Spells3.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot4_count:{page}", content.Spells4.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot5_count:{page}", content.Spells5.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot6_count:{page}", content.Spells6.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot7_count:{page}", content.Spells7.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot8_count:{page}", content.Spells8.SlotsCount.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot9_count:{page}", content.Spells9.SlotsCount.ToString());
            int num = 0;
            foreach (SpellcastingSpellContent item2 in content.Cantrips.Collection)
            {
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item2.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item3 in content.Spells1.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item3.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item3.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item4 in content.Spells2.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item4.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item4.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item5 in content.Spells3.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item5.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item5.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item6 in content.Spells4.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item6.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item6.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item7 in content.Spells5.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item7.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item7.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item8 in content.Spells6.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item8.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item8.Name);
                num++;
            }
            foreach (SpellcastingSpellContent item9 in content.Spells7.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item9.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item9.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item10 in content.Spells8.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item10.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item10.Name);
                num++;
            }
            num++;
            foreach (SpellcastingSpellContent item11 in content.Spells9.Collection)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", item11.IsPrepared ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", item11.Name);
                num++;
            }
        }

        private void GenerateSpellcardsRevamped(PdfStamper stamper, IEnumerable<Spell> spells, int startingPage = 1)
        {
            Logger.Info("stamping revamped spellcards");
            int num = startingPage;
            int num2 = 0;
            PageContentWriter pageContentWriter = new PageContentWriter(stamper);
            foreach (Spell spell in spells)
            {
                string text = $"_{num}:{num2}";
                foreach (IPageContentItem item in new List<IPageContentItem>
            {
                new LineContent("card_title" + text, spell.Name),
                new LineContent("card_subtitle" + text, spell.Underline),
                new LineContent("card_time" + text, spell.CastingTime),
                new LineContent("card_range" + text, spell.Range),
                new LineContent("card_duration" + text, spell.Duration),
                new LineContent("card_components" + text, spell.GetComponentsString()),
                new LineContent("card_description" + text, GeneratePlainDescription(spell.Description)),
                new LineContent("card_footer_right" + text, spell.Source)
            })
                {
                    pageContentWriter.Write(item);
                }
                num2++;
                if (num2 == 9)
                {
                    num2 = 0;
                    num++;
                }
            }
        }

        private void GenerateItemCards(PdfStamper stamper, IEnumerable<RefactoredEquipmentItem> items, int currentPage = 1)
        {
            Logger.Info("stamping revamped itemcards");
            int num = 0;
            PageContentWriter pageContentWriter = new PageContentWriter(stamper);
            foreach (RefactoredEquipmentItem item2 in items)
            {
                string text = $"_{currentPage}:{num}";
                string content = "";
                if (item2.IsAdorned || item2.Item.Type.Equals("Magic Item"))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    Item item = (item2.IsAdorned ? item2.AdornerItem : item2.Item);
                    if (!string.IsNullOrWhiteSpace(item.ItemType))
                    {
                        string setterAdditionAttribute = item.GetSetterAdditionAttribute("type");
                        stringBuilder.Append((setterAdditionAttribute != null) ? (item.ItemType + " (" + setterAdditionAttribute + "), ") : (item.ItemType + ", "));
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
                        stringBuilder.Append((setterAdditionAttribute2 != null) ? ("(requires attunement " + setterAdditionAttribute2 + ")") : "(requires attunement)");
                    }
                    content = stringBuilder.ToString();
                }
                else if (!item2.IsAdorned && item2.Item.Type.Equals("Weapon"))
                {
                    WeaponElement weaponElement = item2.Item.AsElement<WeaponElement>();
                    foreach (ElementBase item3 in (from x in DataManager.Current.ElementsCollection
                                                   where x.Type.Equals("Weapon Category")
                                                   orderby x.Name
                                                   select x).ToList())
                    {
                        if (weaponElement.Supports.Contains(item3.Id))
                        {
                            content = item3.Name + " Weapon";
                        }
                    }
                }
                else
                {
                    content = item2.Item.Category;
                }
                foreach (IPageContentItem item4 in new List<IPageContentItem>
            {
                new LineContent("card_title" + text, string.IsNullOrWhiteSpace(item2.AlternativeName) ? item2.DisplayName : item2.AlternativeName),
                new LineContent("card_subtitle" + text, content),
                new LineContent("card_description" + text, GetHtmlString(item2)),
                new LineContent("card_footer_left" + text, item2.IsEquipped ? "Equipped" : ""),
                new LineContent("card_footer_right" + text, item2.Item.Source)
            })
                {
                    pageContentWriter.Write(item4);
                }
                num++;
                if (num == 9)
                {
                    num = 0;
                    currentPage++;
                }
            }
        }

        private void GenerateFeatureCards(PdfStamper stamper, MainSheetContent content, int currentPage = 1)
        {
            int num = 0;
            PageContentWriter pageContentWriter = new PageContentWriter(stamper);
            foreach (ContentLine line in content.FeaturesFieldContent.Lines)
            {
                if (line.Content == "")
                {
                    continue;
                }
                string text = $"_{currentPage}:{num}";
                foreach (IPageContentItem item in new List<IPageContentItem>
            {
                new LineContent("card_title" + text, line.Name),
                new LineContent("card_subtitle" + text, "feature"),
                new LineContent("card_description" + text, line.Content),
                new LineContent("card_footer_left" + text, ""),
                new LineContent("card_footer_right" + text, "")
            })
                {
                    pageContentWriter.Write(item);
                }
                num++;
                if (num == 9)
                {
                    num = 0;
                    currentPage++;
                }
            }
        }

        private void GenerateCardsRevamped(PdfStamper stamper)
        {
            int num = 1;
            int num2 = 0;
            PageContentWriter pageContentWriter = new PageContentWriter(stamper);
            if (Configuration.IncludeSpellcards)
            {
                foreach (Spell spell in Spells)
                {
                    string text = $"_{num}:{num2}";
                    foreach (IPageContentItem item2 in new List<IPageContentItem>
                {
                    new LineContent("card_title" + text, spell.Name),
                    new LineContent("card_subtitle" + text, spell.Underline),
                    new LineContent("card_time" + text, spell.CastingTime),
                    new LineContent("card_range" + text, spell.Range),
                    new LineContent("card_duration" + text, spell.Duration),
                    new LineContent("card_components" + text, spell.GetComponentsString()),
                    new LineContent("card_description" + text, GeneratePlainDescription(spell.Description)),
                    new LineContent("card_footer_right" + text, spell.Source)
                })
                    {
                        pageContentWriter.Write(item2);
                    }
                    num2++;
                    if (num2 == 9)
                    {
                        num2 = 0;
                        num++;
                    }
                }
            }
            if (!Configuration.IncludeItemcards)
            {
                return;
            }
            foreach (RefactoredEquipmentItem item3 in Items)
            {
                string text2 = $"_{num}:{num2}";
                string content = "";
                if (item3.IsAdorned || item3.Item.Type.Equals("Magic Item"))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    Item item = (item3.IsAdorned ? item3.AdornerItem : item3.Item);
                    if (!string.IsNullOrWhiteSpace(item.ItemType))
                    {
                        string setterAdditionAttribute = item.GetSetterAdditionAttribute("type");
                        stringBuilder.Append((setterAdditionAttribute != null) ? (item.ItemType + " (" + setterAdditionAttribute + "), ") : (item.ItemType + ", "));
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
                        stringBuilder.Append((setterAdditionAttribute2 != null) ? ("(requires attunement " + setterAdditionAttribute2 + ")") : "(requires attunement)");
                    }
                    content = stringBuilder.ToString();
                }
                else if (!item3.IsAdorned && item3.Item.Type.Equals("Weapon"))
                {
                    WeaponElement weaponElement = item3.Item.AsElement<WeaponElement>();
                    foreach (ElementBase item4 in (from x in DataManager.Current.ElementsCollection
                                                   where x.Type.Equals("Weapon Category")
                                                   orderby x.Name
                                                   select x).ToList())
                    {
                        if (weaponElement.Supports.Contains(item4.Id))
                        {
                            content = item4.Name + " Weapon";
                        }
                    }
                }
                else
                {
                    content = item3.Item.Category;
                }
                foreach (IPageContentItem item5 in new List<IPageContentItem>
            {
                new LineContent("card_title" + text2, string.IsNullOrWhiteSpace(item3.AlternativeName) ? item3.DisplayName : item3.AlternativeName),
                new LineContent("card_subtitle" + text2, content),
                new LineContent("card_description" + text2, GetHtmlString(item3)),
                new LineContent("card_footer_left" + text2, item3.IsEquipped ? "Equipped" : ""),
                new LineContent("card_footer_right" + text2, item3.Item.Source)
            })
                {
                    pageContentWriter.Write(item5);
                }
                num2++;
                if (num2 == 9)
                {
                    num2 = 0;
                    num++;
                }
            }
        }

        private string GeneratePlainDescription(string description)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (IElement item in HTMLWorker.ParseToList(new StringReader(description), null))
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                foreach (Chunk chunk in item.Chunks)
                {
                    if (item is List)
                    {
                        Chunk symbol = (item as List).Symbol;
                        stringBuilder2.AppendLine($"{symbol} {chunk.Content}");
                    }
                    else
                    {
                        stringBuilder2.Append(chunk.Content);
                    }
                }
                stringBuilder.AppendLine(stringBuilder2.ToString());
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private string GetHtmlString(RefactoredEquipmentItem _item)
        {
            List<IElement> list = HTMLWorker.ParseToList(new StringReader(_item.IsAdorned ? _item.AdornerItem.Description : _item.Item.Description), null);
            if (!_item.IsAdorned && (_item.Item.Type.Equals("Weapon") || _item.Item.Type.Equals("Armor")))
            {
                list = HTMLWorker.ParseToList(new StringReader(DescriptionPanelViewModelBase.GenerateHeaderForCard(_item.Item)), null);
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (IElement item in list)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                foreach (Chunk chunk in item.Chunks)
                {
                    if (item.GetType() == typeof(List))
                    {
                        stringBuilder2.Append((item as List).Symbol);
                        stringBuilder2.AppendLine(chunk.Content);
                    }
                    else
                    {
                        stringBuilder2.Append(chunk.Content);
                    }
                }
                stringBuilder.AppendLine(stringBuilder2.ToString());
                stringBuilder.AppendLine();
            }
            if (!string.IsNullOrWhiteSpace(_item.Notes))
            {
                stringBuilder.AppendLine("ADDITIONAL NOTES");
                stringBuilder.AppendLine(_item.Notes);
            }
            return stringBuilder.ToString();
        }

        [Obsolete("used in legacy spellcasting page")]
        private void StampSpellcastingContent(PdfStamper stamper, SpellcastingSheetContent content, int page)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>
        {
            { "Spellcasting Class 2", content.SpellcastingClass },
            { "SpellcastingAbility 2", content.SpellcastingAbility },
            { "SpellSaveDC  2", content.SpellcastingSave },
            { "SpellAtkBonus 2", content.SpellcastingAttackModifier }
        };
            Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            {
                "Spells 1014",
                content.Cantrips.GetSpell(0, returnEmpty: true).Name
            },
            {
                "Spells 1016",
                content.Cantrips.GetSpell(1, returnEmpty: true).Name
            },
            {
                "Spells 1017",
                content.Cantrips.GetSpell(2, returnEmpty: true).Name
            },
            {
                "Spells 1018",
                content.Cantrips.GetSpell(3, returnEmpty: true).Name
            },
            {
                "Spells 1019",
                content.Cantrips.GetSpell(4, returnEmpty: true).Name
            },
            {
                "Spells 1020",
                content.Cantrips.GetSpell(5, returnEmpty: true).Name
            },
            {
                "Spells 1021",
                content.Cantrips.GetSpell(6, returnEmpty: true).Name
            },
            {
                "Spells 1022",
                content.Cantrips.GetSpell(7, returnEmpty: true).Name
            }
        };
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>
        {
            {
                "SlotsTotal 19",
                content.Spells1.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 19",
                (content.Spells1.ExpendedSlotsCount > 0) ? content.Spells1.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 20",
                content.Spells2.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 20",
                (content.Spells2.ExpendedSlotsCount > 0) ? content.Spells2.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 21",
                content.Spells3.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 21",
                (content.Spells3.ExpendedSlotsCount > 0) ? content.Spells3.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 22",
                content.Spells4.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 22",
                (content.Spells4.ExpendedSlotsCount > 0) ? content.Spells4.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 23",
                content.Spells5.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 23",
                (content.Spells5.ExpendedSlotsCount > 0) ? content.Spells5.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 24",
                content.Spells6.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 24",
                (content.Spells6.ExpendedSlotsCount > 0) ? content.Spells6.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 25",
                content.Spells7.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 25",
                (content.Spells7.ExpendedSlotsCount > 0) ? content.Spells7.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 26",
                content.Spells8.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 26",
                (content.Spells8.ExpendedSlotsCount > 0) ? content.Spells8.ExpendedSlotsCount.ToString() : ""
            },
            {
                "SlotsTotal 27",
                content.Spells9.SlotsCount.ToString()
            },
            {
                "SlotsRemaining 27",
                (content.Spells9.ExpendedSlotsCount > 0) ? content.Spells9.ExpendedSlotsCount.ToString() : ""
            }
        };
            foreach (KeyValuePair<string, string> item in obj)
            {
                stamper.AcroFields.SetField($"{item.Key}:{page}", item.Value);
            }
            foreach (KeyValuePair<string, string> item2 in dictionary)
            {
                stamper.AcroFields.SetField($"{item2.Key}:{page}", item2.Value);
            }
            foreach (KeyValuePair<string, string> item3 in dictionary2)
            {
                stamper.AcroFields.SetField($"{item3.Key}:{page}", item3.Value);
            }
            Dictionary<string, string> pairs = new Dictionary<string, string>
        {
            { "Spells 1015", "Check Box 251" },
            { "Spells 1023", "Check Box 309" },
            { "Spells 1024", "Check Box 3010" },
            { "Spells 1025", "Check Box 3011" },
            { "Spells 1026", "Check Box 3012" },
            { "Spells 1027", "Check Box 3013" },
            { "Spells 1028", "Check Box 3014" },
            { "Spells 1029", "Check Box 3015" },
            { "Spells 1030", "Check Box 3016" },
            { "Spells 1031", "Check Box 3017" },
            { "Spells 1032", "Check Box 3018" },
            { "Spells 1033", "Check Box 3019" }
        };
            Dictionary<string, string> pairs2 = new Dictionary<string, string>
        {
            { "Spells 1046", "Check Box 313" },
            { "Spells 1034", "Check Box 310" },
            { "Spells 1035", "Check Box 3020" },
            { "Spells 1036", "Check Box 3021" },
            { "Spells 1037", "Check Box 3022" },
            { "Spells 1038", "Check Box 3023" },
            { "Spells 1039", "Check Box 3024" },
            { "Spells 1040", "Check Box 3025" },
            { "Spells 1041", "Check Box 3026" },
            { "Spells 1042", "Check Box 3027" },
            { "Spells 1043", "Check Box 3028" },
            { "Spells 1044", "Check Box 3029" },
            { "Spells 1045", "Check Box 3030" }
        };
            Dictionary<string, string> pairs3 = new Dictionary<string, string>
        {
            { "Spells 1048", "Check Box 315" },
            { "Spells 1047", "Check Box 314" },
            { "Spells 1049", "Check Box 3031" },
            { "Spells 1050", "Check Box 3032" },
            { "Spells 1051", "Check Box 3033" },
            { "Spells 1052", "Check Box 3034" },
            { "Spells 1053", "Check Box 3035" },
            { "Spells 1054", "Check Box 3036" },
            { "Spells 1055", "Check Box 3037" },
            { "Spells 1056", "Check Box 3038" },
            { "Spells 1057", "Check Box 3039" },
            { "Spells 1058", "Check Box 3040" },
            { "Spells 1059", "Check Box 3041" }
        };
            Dictionary<string, string> pairs4 = new Dictionary<string, string>
        {
            { "Spells 1061", "Check Box 317" },
            { "Spells 1060", "Check Box 316" },
            { "Spells 1062", "Check Box 3042" },
            { "Spells 1063", "Check Box 3043" },
            { "Spells 1064", "Check Box 3044" },
            { "Spells 1065", "Check Box 3045" },
            { "Spells 1066", "Check Box 3046" },
            { "Spells 1067", "Check Box 3047" },
            { "Spells 1068", "Check Box 3048" },
            { "Spells 1069", "Check Box 3049" },
            { "Spells 1070", "Check Box 3050" },
            { "Spells 1071", "Check Box 3051" },
            { "Spells 1072", "Check Box 3052" }
        };
            Dictionary<string, string> pairs5 = new Dictionary<string, string>
        {
            { "Spells 1074", "Check Box 319" },
            { "Spells 1073", "Check Box 318" },
            { "Spells 1075", "Check Box 3053" },
            { "Spells 1076", "Check Box 3054" },
            { "Spells 1077", "Check Box 3055" },
            { "Spells 1078", "Check Box 3056" },
            { "Spells 1079", "Check Box 3057" },
            { "Spells 1080", "Check Box 3058" },
            { "Spells 1081", "Check Box 3059" }
        };
            Dictionary<string, string> pairs6 = new Dictionary<string, string>
        {
            { "Spells 1083", "Check Box 321" },
            { "Spells 1082", "Check Box 320" },
            { "Spells 1084", "Check Box 3060" },
            { "Spells 1085", "Check Box 3061" },
            { "Spells 1086", "Check Box 3062" },
            { "Spells 1087", "Check Box 3063" },
            { "Spells 1088", "Check Box 3064" },
            { "Spells 1089", "Check Box 3065" },
            { "Spells 1090", "Check Box 3066" }
        };
            Dictionary<string, string> pairs7 = new Dictionary<string, string>
        {
            { "Spells 1092", "Check Box 323" },
            { "Spells 1091", "Check Box 322" },
            { "Spells 1093", "Check Box 3067" },
            { "Spells 1094", "Check Box 3068" },
            { "Spells 1095", "Check Box 3069" },
            { "Spells 1096", "Check Box 3070" },
            { "Spells 1097", "Check Box 3071" },
            { "Spells 1098", "Check Box 3072" },
            { "Spells 1099", "Check Box 3073" }
        };
            Dictionary<string, string> pairs8 = new Dictionary<string, string>
        {
            { "Spells 10101", "Check Box 325" },
            { "Spells 10100", "Check Box 324" },
            { "Spells 10102", "Check Box 3074" },
            { "Spells 10103", "Check Box 3075" },
            { "Spells 10104", "Check Box 3076" },
            { "Spells 10105", "Check Box 3077" },
            { "Spells 10106", "Check Box 3078" }
        };
            Dictionary<string, string> pairs9 = new Dictionary<string, string>
        {
            { "Spells 10108", "Check Box 327" },
            { "Spells 10107", "Check Box 326" },
            { "Spells 10109", "Check Box 3079" },
            { "Spells 101010", "Check Box 3080" },
            { "Spells 101011", "Check Box 3081" },
            { "Spells 101012", "Check Box 3082" },
            { "Spells 101013", "Check Box 3083" }
        };
            StampSpellList(stamper, pairs, content.Spells1, page);
            StampSpellList(stamper, pairs2, content.Spells2, page);
            StampSpellList(stamper, pairs3, content.Spells3, page);
            StampSpellList(stamper, pairs4, content.Spells4, page);
            StampSpellList(stamper, pairs5, content.Spells5, page);
            StampSpellList(stamper, pairs6, content.Spells6, page);
            StampSpellList(stamper, pairs7, content.Spells7, page);
            StampSpellList(stamper, pairs8, content.Spells8, page);
            StampSpellList(stamper, pairs9, content.Spells9, page);
        }

        [Obsolete("used in legacy spellcasting page")]
        private void StampSpellList(PdfStamper stamper, Dictionary<string, string> pairs, SpellcastingSpellsContent content, int page)
        {
            int num = 0;
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                stamper.AcroFields.SetField($"{pair.Key}:{page}", content.GetSpell(num, returnEmpty: true).Name);
                stamper.AcroFields.SetField($"{pair.Value}:{page}", content.GetSpell(num, returnEmpty: true).IsPrepared ? "Yes" : "No");
                num++;
            }
        }

        private void StampLegacyDetailsContent(PdfStamper stamper)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "CharacterName", ExportContent.CharacterName },
            { "PlayerName", ExportContent.PlayerName },
            { "Race ", ExportContent.Race },
            {
                "Background",
                ExportContent.BackgroundContent.Name
            },
            { "ClassLevel", ExportContent.Level },
            { "Alignment", ExportContent.Alignment },
            { "XP", ExportContent.Experience }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value);
            }
            foreach (KeyValuePair<string, string> item2 in new Dictionary<string, string>
        {
            {
                Configuration.IsAttributeDisplayFlipped ? "STRmod" : "STR",
                ExportContent.AbilitiesContent.Strength
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "DEXmod " : "DEX",
                ExportContent.AbilitiesContent.Dexterity
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CONmod" : "CON",
                ExportContent.AbilitiesContent.Constitution
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "INTmod" : "INT",
                ExportContent.AbilitiesContent.Intelligence
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "WISmod" : "WIS",
                ExportContent.AbilitiesContent.Wisdom
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CHamod" : "CHA",
                ExportContent.AbilitiesContent.Charisma
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "STR" : "STRmod",
                ExportContent.AbilitiesContent.StrengthModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "DEX" : "DEXmod ",
                ExportContent.AbilitiesContent.DexterityModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CON" : "CONmod",
                ExportContent.AbilitiesContent.ConstitutionModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "INT" : "INTmod",
                ExportContent.AbilitiesContent.IntelligenceModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "WIS" : "WISmod",
                ExportContent.AbilitiesContent.WisdomModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CHA" : "CHamod",
                ExportContent.AbilitiesContent.CharismaModifier
            }
        })
            {
                stamper.AcroFields.SetField(item2.Key, item2.Value);
            }
            foreach (KeyValuePair<string, string> item3 in new Dictionary<string, string>
        {
            {
                "Acrobatics",
                ExportContent.SkillsContent.Acrobatics
            },
            {
                "Animal",
                ExportContent.SkillsContent.AnimalHandling
            },
            {
                "Athletics",
                ExportContent.SkillsContent.Athletics
            },
            {
                "Deception ",
                ExportContent.SkillsContent.Deception
            },
            {
                "History ",
                ExportContent.SkillsContent.History
            },
            {
                "Insight",
                ExportContent.SkillsContent.Insight
            },
            {
                "Intimidation",
                ExportContent.SkillsContent.Intimidation
            },
            {
                "Investigation ",
                ExportContent.SkillsContent.Investigation
            },
            {
                "Arcana",
                ExportContent.SkillsContent.Arcana
            },
            {
                "Perception ",
                ExportContent.SkillsContent.Perception
            },
            {
                "Nature",
                ExportContent.SkillsContent.Nature
            },
            {
                "Performance",
                ExportContent.SkillsContent.Performance
            },
            {
                "Medicine",
                ExportContent.SkillsContent.Medicine
            },
            {
                "Religion",
                ExportContent.SkillsContent.Religion
            },
            {
                "Stealth ",
                ExportContent.SkillsContent.Stealth
            },
            {
                "Persuasion",
                ExportContent.SkillsContent.Persuasion
            },
            {
                "SleightofHand",
                ExportContent.SkillsContent.SleightOfHand
            },
            {
                "Survival",
                ExportContent.SkillsContent.Survival
            },
            {
                "Check Box 23",
                ExportContent.SkillsContent.AcrobaticsProficient ? "Yes" : "No"
            },
            {
                "Check Box 24",
                ExportContent.SkillsContent.AnimalHandlingProficient ? "Yes" : "No"
            },
            {
                "Check Box 25",
                ExportContent.SkillsContent.ArcanaProficient ? "Yes" : "No"
            },
            {
                "Check Box 26",
                ExportContent.SkillsContent.AthleticsProficient ? "Yes" : "No"
            },
            {
                "Check Box 27",
                ExportContent.SkillsContent.DeceptionProficient ? "Yes" : "No"
            },
            {
                "Check Box 28",
                ExportContent.SkillsContent.HistoryProficient ? "Yes" : "No"
            },
            {
                "Check Box 29",
                ExportContent.SkillsContent.InsightProficient ? "Yes" : "No"
            },
            {
                "Check Box 30",
                ExportContent.SkillsContent.IntimidationProficient ? "Yes" : "No"
            },
            {
                "Check Box 31",
                ExportContent.SkillsContent.InvestigationProficient ? "Yes" : "No"
            },
            {
                "Check Box 32",
                ExportContent.SkillsContent.MedicineProficient ? "Yes" : "No"
            },
            {
                "Check Box 33",
                ExportContent.SkillsContent.NatureProficient ? "Yes" : "No"
            },
            {
                "Check Box 34",
                ExportContent.SkillsContent.PerceptionProficient ? "Yes" : "No"
            },
            {
                "Check Box 35",
                ExportContent.SkillsContent.PerformanceProficient ? "Yes" : "No"
            },
            {
                "Check Box 36",
                ExportContent.SkillsContent.PersuasionProficient ? "Yes" : "No"
            },
            {
                "Check Box 37",
                ExportContent.SkillsContent.ReligionProficient ? "Yes" : "No"
            },
            {
                "Check Box 38",
                ExportContent.SkillsContent.SleightOfHandProficient ? "Yes" : "No"
            },
            {
                "Check Box 39",
                ExportContent.SkillsContent.StealthProficient ? "Yes" : "No"
            },
            {
                "Check Box 40",
                ExportContent.SkillsContent.SurvivalProficient ? "Yes" : "No"
            },
            {
                "Passive",
                ExportContent.SkillsContent.PerceptionPassive
            },
            {
                "Inspiration",
                ExportContent.Inspiration ? "x" : ""
            },
            { "ProfBonus", ExportContent.ProficiencyBonus },
            {
                "ST Strength",
                ExportContent.AbilitiesContent.StrengthSave
            },
            {
                "ST Dexterity",
                ExportContent.AbilitiesContent.DexteritySave
            },
            {
                "ST Constitution",
                ExportContent.AbilitiesContent.ConstitutionSave
            },
            {
                "ST Intelligence",
                ExportContent.AbilitiesContent.IntelligenceSave
            },
            {
                "ST Wisdom",
                ExportContent.AbilitiesContent.WisdomSave
            },
            {
                "ST Charisma",
                ExportContent.AbilitiesContent.CharismaSave
            },
            {
                "Check Box 11",
                ExportContent.AbilitiesContent.StrengthSaveProficient ? "Yes" : "No"
            },
            {
                "Check Box 18",
                ExportContent.AbilitiesContent.DexteritySaveProficient ? "Yes" : "No"
            },
            {
                "Check Box 19",
                ExportContent.AbilitiesContent.ConstitutionSaveProficient ? "Yes" : "No"
            },
            {
                "Check Box 20",
                ExportContent.AbilitiesContent.IntelligenceSaveProficient ? "Yes" : "No"
            },
            {
                "Check Box 21",
                ExportContent.AbilitiesContent.WisdomSaveProficient ? "Yes" : "No"
            },
            {
                "Check Box 22",
                ExportContent.AbilitiesContent.CharismaSaveProficient ? "Yes" : "No"
            },
            {
                "AC",
                ExportContent.ArmorClassContent.ArmorClass
            },
            { "Initiative", ExportContent.Initiative },
            {
                "Speed",
                ExportContent.ConditionsContent.WalkingSpeed + "ft."
            },
            {
                "HPMax",
                ExportContent.HitPointsContent.Maximum
            },
            {
                "HPCurrent",
                ExportContent.HitPointsContent.Current
            },
            {
                "HPTemp",
                ExportContent.HitPointsContent.Temporary
            },
            { "HD", "" },
            {
                "HDTotal",
                ExportContent.HitPointsContent.HitDice
            },
            {
                "Check Box 12",
                ExportContent.HitPointsContent.DeathSavingThrowSuccess1 ? "Yes" : "No"
            },
            {
                "Check Box 13",
                ExportContent.HitPointsContent.DeathSavingThrowSuccess2 ? "Yes" : "No"
            },
            {
                "Check Box 14",
                ExportContent.HitPointsContent.DeathSavingThrowSuccess3 ? "Yes" : "No"
            },
            {
                "Check Box 15",
                ExportContent.HitPointsContent.DeathSavingThrowFailure1 ? "Yes" : "No"
            },
            {
                "Check Box 16",
                ExportContent.HitPointsContent.DeathSavingThrowFailure2 ? "Yes" : "No"
            },
            {
                "Check Box 17",
                ExportContent.HitPointsContent.DeathSavingThrowFailure3 ? "Yes" : "No"
            },
            {
                "Wpn Name",
                ExportContent.AttacksContent[0]?.Name
            },
            {
                "Wpn1 AtkBonus",
                ExportContent.AttacksContent[0]?.Bonus
            },
            {
                "Wpn1 Damage",
                ExportContent.AttacksContent[0]?.Damage
            },
            {
                "Wpn Name 2",
                ExportContent.AttacksContent[0]?.Name
            },
            {
                "Wpn2 AtkBonus ",
                ExportContent.AttacksContent[0]?.Bonus
            },
            {
                "Wpn2 Damage ",
                ExportContent.AttacksContent[0]?.Damage
            },
            {
                "Wpn Name 3",
                ExportContent.AttacksContent[0]?.Name
            },
            {
                "Wpn3 AtkBonus  ",
                ExportContent.AttacksContent[0]?.Bonus
            },
            {
                "Wpn3 Damage ",
                ExportContent.AttacksContent[0]?.Damage
            },
            { "AttacksSpellcasting", ExportContent.AttackAndSpellcastingField },
            {
                "CP",
                ExportContent.EquipmentContent.Copper
            },
            {
                "SP",
                ExportContent.EquipmentContent.Silver
            },
            {
                "EP",
                ExportContent.EquipmentContent.Electrum
            },
            {
                "GP",
                ExportContent.EquipmentContent.Gold
            },
            {
                "PP",
                ExportContent.EquipmentContent.Platinum
            },
            {
                "Equipment",
                string.Join(Environment.NewLine, ExportContent.EquipmentContent.Equipment.Select((Tuple<string, string> x) => x.Item2))
            },
            {
                "PersonalityTraits ",
                ExportContent.BackgroundContent.PersonalityTrait
            },
            {
                "Ideals",
                ExportContent.BackgroundContent.Ideal
            },
            {
                "Bonds",
                ExportContent.BackgroundContent.Bond
            },
            {
                "Flaws",
                ExportContent.BackgroundContent.Flaw
            }
        })
            {
                stamper.AcroFields.SetField(item3.Key, item3.Value);
            }
            if (Configuration.IncludeFormatting)
            {
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Armor Proficiencies. " + ExportContent.ArmorProficiencies);
            stringBuilder.AppendLine("Weapon Proficiencies. " + ExportContent.WeaponProficiencies);
            stringBuilder.AppendLine("Tool Proficiencies. " + ExportContent.ToolProficiencies);
            stringBuilder.AppendLine("Languages. " + ExportContent.Languages);
            foreach (KeyValuePair<string, string> item4 in new Dictionary<string, string>
        {
            {
                "ProficienciesLang",
                stringBuilder.ToString()
            },
            { "Features and Traits", ExportContent.Features }
        })
            {
                stamper.AcroFields.SetField(item4.Key, item4.Value);
            }
        }

        private void StampLegacyBackgroundContent(PdfStamper stamper)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "CharacterName 2", ExportContent.CharacterName },
            {
                "Age",
                ExportContent.AppearanceContent.Age
            },
            {
                "Height",
                ExportContent.AppearanceContent.Height
            },
            {
                "Weight",
                ExportContent.AppearanceContent.Weight
            },
            {
                "Eyes",
                ExportContent.AppearanceContent.Eyes
            },
            {
                "Skin",
                ExportContent.AppearanceContent.Skin
            },
            {
                "Hair",
                ExportContent.AppearanceContent.Hair
            },
            {
                "Backstory",
                ExportContent.BackgroundContent.Story
            },
            { "Allies", ExportContent.AlliesAndOrganizations },
            { "FactionName", ExportContent.OrganizationName },
            { "Feat+Traits", ExportContent.AdditionalFeaturesAndTraits },
            { "Treasure", ExportContent.Treasure }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value);
            }
            WriteImage(stamper, "CHARACTER IMAGE", ExportContent.AppearanceContent.Portrait);
            WriteImage(stamper, "Faction Symbol Image", ExportContent.OrganizationSymbol);
        }

        private void StampSpellcastingExportRevamped2(PdfStamper stamper, CharacterSheetSpellcastingPageExportContent export, int page)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "spellcasting_class", export.SpellcastingClass },
            { "spellcasting_ability", export.Ability },
            { "spellcasting_save", export.Save },
            { "spellcasting_attack_bonus", export.AttackBonus },
            { "spellcasting_prepare_count", export.PrepareCount },
            { "spellcasting_notes", export.Notes },
            {
                "spellcasting_multiclass",
                export.IsMulticlassSpellcaster ? "Yes" : "No"
            }
        })
            {
                stamper.AcroFields.SetField($"{item.Key}:{page}", item.Value);
            }
            stamper.AcroFields.SetField($"spellcasting_slot1_count:{page}", export.Spells1.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot2_count:{page}", export.Spells2.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot3_count:{page}", export.Spells3.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot4_count:{page}", export.Spells4.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot5_count:{page}", export.Spells5.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot6_count:{page}", export.Spells6.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot7_count:{page}", export.Spells7.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot8_count:{page}", export.Spells8.AvailableSlots.ToString());
            stamper.AcroFields.SetField($"spellcasting_slot9_count:{page}", export.Spells9.AvailableSlots.ToString());
            int num = 0;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell in export.Cantrips.Spells)
            {
                _ = $"_1.{num}:{page}";
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell.IsPrepared || spell.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell2 in export.Spells1.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell2.IsPrepared || spell2.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell2.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell2.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell2.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell2.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell2.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell2.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell2.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell3 in export.Spells2.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell3.IsPrepared || spell3.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell3.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell3.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell3.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell3.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell3.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell3.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell3.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell4 in export.Spells3.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell4.IsPrepared || spell4.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell4.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell4.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell4.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell4.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell4.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell4.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell4.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell5 in export.Spells4.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell5.IsPrepared || spell5.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell5.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell5.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell5.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell5.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell5.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell5.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell5.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell6 in export.Spells5.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell6.IsPrepared || spell6.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell6.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell6.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell6.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell6.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell6.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell6.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell6.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell7 in export.Spells6.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell7.IsPrepared || spell7.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell7.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell7.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell7.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell7.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell7.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell7.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell7.School);
                num++;
            }
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell8 in export.Spells7.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell8.IsPrepared || spell8.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell8.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell8.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell8.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell8.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell8.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell8.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell8.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell9 in export.Spells8.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell9.IsPrepared || spell9.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell9.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell9.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell9.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell9.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell9.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell9.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell9.School);
                num++;
            }
            num++;
            foreach (CharacterSheetSpellcastingPageExportContent.SpellExportContent spell10 in export.Spells9.Spells)
            {
                stamper.AcroFields.SetField($"spell_prepared_1.{num}:{page}", (spell10.IsPrepared || spell10.AlwaysPrepared) ? "Yes" : "No");
                stamper.AcroFields.SetField($"spell_name_1.{num}:{page}", spell10.Name);
                stamper.AcroFields.SetField($"spell_description_1.{num}:{page}", spell10.Description);
                stamper.AcroFields.SetField($"spell_casting_time_1.{num}:{page}", spell10.CastingTime);
                stamper.AcroFields.SetField($"spell_casting_range_1.{num}:{page}", spell10.Range);
                stamper.AcroFields.SetField($"spell_casting_duration_1.{num}:{page}", spell10.Duration);
                stamper.AcroFields.SetField($"spell_casting_components_1.{num}:{page}", spell10.Components);
                stamper.AcroFields.SetField($"spell_casting_school_1.{num}:{page}", spell10.School);
                num++;
            }
        }

        [Obsolete]
        private void StampMainContent(PdfStamper stamper, MainSheetContent content)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "CharacterName", content.CharacterName },
            { "PlayerName", content.PlayerName },
            { "Race ", content.Race },
            { "Background", content.Background },
            { "ClassLevel", content.ClassLevel },
            { "Alignment", content.Alignment },
            { "XP", content.Experience }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value);
            }
            foreach (KeyValuePair<string, string> item2 in new Dictionary<string, string>
        {
            {
                Configuration.IsAttributeDisplayFlipped ? "STRmod" : "STR",
                content.Strength
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "DEXmod " : "DEX",
                content.Dexterity
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CONmod" : "CON",
                content.Constitution
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "INTmod" : "INT",
                content.Intelligence
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "WISmod" : "WIS",
                content.Wisdom
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CHamod" : "CHA",
                content.Charisma
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "STR" : "STRmod",
                content.StrengthModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "DEX" : "DEXmod ",
                content.DexterityModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CON" : "CONmod",
                content.ConstitutionModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "INT" : "INTmod",
                content.IntelligenceModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "WIS" : "WISmod",
                content.WisdomModifier
            },
            {
                Configuration.IsAttributeDisplayFlipped ? "CHA" : "CHamod",
                content.CharismaModifier
            }
        })
            {
                stamper.AcroFields.SetField(item2.Key, item2.Value);
            }
            foreach (KeyValuePair<string, string> item3 in new Dictionary<string, string>
        {
            { "Acrobatics", content.Acrobatics },
            { "Animal", content.AnimalHandling },
            { "Athletics", content.Athletics },
            { "Deception ", content.Deception },
            { "History ", content.History },
            { "Insight", content.Insight },
            { "Intimidation", content.Intimidation },
            { "Investigation ", content.Investigation },
            { "Arcana", content.Arcana },
            { "Perception ", content.Perception },
            { "Nature", content.Nature },
            { "Performance", content.Performance },
            { "Medicine", content.Medicine },
            { "Religion", content.Religion },
            { "Stealth ", content.Stealth },
            { "Persuasion", content.Persuasion },
            { "SleightofHand", content.SleightOfHand },
            { "Survival", content.Survival },
            {
                "Check Box 23",
                content.AcrobaticsProficient ? "Yes" : "No"
            },
            {
                "Check Box 24",
                content.AnimalHandlingProficient ? "Yes" : "No"
            },
            {
                "Check Box 25",
                content.ArcanaProficient ? "Yes" : "No"
            },
            {
                "Check Box 26",
                content.AthleticsProficient ? "Yes" : "No"
            },
            {
                "Check Box 27",
                content.DeceptionProficient ? "Yes" : "No"
            },
            {
                "Check Box 28",
                content.HistoryProficient ? "Yes" : "No"
            },
            {
                "Check Box 29",
                content.InsightProficient ? "Yes" : "No"
            },
            {
                "Check Box 30",
                content.IntimidationProficient ? "Yes" : "No"
            },
            {
                "Check Box 31",
                content.InvestigationProficient ? "Yes" : "No"
            },
            {
                "Check Box 32",
                content.MedicineProficient ? "Yes" : "No"
            },
            {
                "Check Box 33",
                content.NatureProficient ? "Yes" : "No"
            },
            {
                "Check Box 34",
                content.PerceptionProficient ? "Yes" : "No"
            },
            {
                "Check Box 35",
                content.PerformanceProficient ? "Yes" : "No"
            },
            {
                "Check Box 36",
                content.PersuasionProficient ? "Yes" : "No"
            },
            {
                "Check Box 37",
                content.ReligionProficient ? "Yes" : "No"
            },
            {
                "Check Box 38",
                content.SleightOfHandProficient ? "Yes" : "No"
            },
            {
                "Check Box 39",
                content.StealthProficient ? "Yes" : "No"
            },
            {
                "Check Box 40",
                content.SurvivalProficient ? "Yes" : "No"
            },
            { "Passive", content.PassiveWisdomPerception },
            {
                "Inspiration",
                content.Inspiration ? "x" : ""
            },
            { "ProfBonus", content.ProficiencyBonus },
            { "ST Strength", content.StrengthSavingThrow },
            { "ST Dexterity", content.DexteritySavingThrow },
            { "ST Constitution", content.ConstitutionSavingThrow },
            { "ST Intelligence", content.IntelligenceSavingThrow },
            { "ST Wisdom", content.WisdomSavingThrow },
            { "ST Charisma", content.CharismaSavingThrow },
            {
                "Check Box 11",
                content.StrengthSavingThrowProficient ? "Yes" : "No"
            },
            {
                "Check Box 18",
                content.DexteritySavingThrowProficient ? "Yes" : "No"
            },
            {
                "Check Box 19",
                content.ConstitutionSavingThrowProficient ? "Yes" : "No"
            },
            {
                "Check Box 20",
                content.IntelligenceSavingThrowProficient ? "Yes" : "No"
            },
            {
                "Check Box 21",
                content.WisdomSavingThrowProficient ? "Yes" : "No"
            },
            {
                "Check Box 22",
                content.CharismaSavingThrowProficient ? "Yes" : "No"
            },
            { "AC", content.ArmorClass },
            { "Initiative", content.Initiative },
            {
                "Speed",
                content.Speed + "ft."
            },
            { "HPMax", content.MaximumHitPoints },
            { "HPCurrent", content.CurrentHitPoints },
            { "HPTemp", content.TemporaryHitPoints },
            { "HD", content.HitDice },
            { "HDTotal", content.TotalHitDice },
            {
                "Check Box 12",
                content.DeathSavingThrowSuccess1 ? "Yes" : "No"
            },
            {
                "Check Box 13",
                content.DeathSavingThrowSuccess2 ? "Yes" : "No"
            },
            {
                "Check Box 14",
                content.DeathSavingThrowSuccess3 ? "Yes" : "No"
            },
            {
                "Check Box 15",
                content.DeathSavingThrowFailure1 ? "Yes" : "No"
            },
            {
                "Check Box 16",
                content.DeathSavingThrowFailure2 ? "Yes" : "No"
            },
            {
                "Check Box 17",
                content.DeathSavingThrowFailure3 ? "Yes" : "No"
            },
            { "Wpn Name", content.Name1 },
            { "Wpn1 AtkBonus", content.AttackBonus1 },
            { "Wpn1 Damage", content.DamageType1 },
            { "Wpn Name 2", content.Name2 },
            { "Wpn2 AtkBonus ", content.AttackBonus2 },
            { "Wpn2 Damage ", content.DamageType2 },
            { "Wpn Name 3", content.Name3 },
            { "Wpn3 AtkBonus  ", content.AttackBonus3 },
            { "Wpn3 Damage ", content.DamageType3 },
            { "AttacksSpellcasting", content.AttackAndSpellcastingField },
            { "CP", content.Copper },
            { "SP", content.Silver },
            { "EP", content.Electrum },
            { "GP", content.Gold },
            { "PP", content.Platinum },
            { "Equipment", content.Equipment },
            { "PersonalityTraits ", content.PersonalityTraits },
            { "Ideals", content.Ideals },
            { "Bonds", content.Bonds },
            { "Flaws", content.Flaws }
        })
            {
                stamper.AcroFields.SetField(item3.Key, item3.Value);
            }
            if (Configuration.IncludeFormatting)
            {
                return;
            }
            foreach (KeyValuePair<string, string> item4 in new Dictionary<string, string>
        {
            { "ProficienciesLang", content.ProficienciesAndLanguages },
            { "Features and Traits", content.FeaturesAndTraitsField }
        })
            {
                stamper.AcroFields.SetField(item4.Key, item4.Value);
            }
            GenerateFeatureCards(stamper, content);
        }

        [Obsolete]
        private void StampDetailsContent(PdfStamper stamper, MainSheetContent mainContent, DetailsSheetContent content)
        {
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
        {
            { "background_character_name", content.CharacterName },
            { "background_story", content.CharacterBackstory },
            { "background_allies", content.AlliesAndOrganizations },
            { "background_organization_name", content.OrganizationName },
            { "background_age", content.Age },
            { "background_eyes", content.Eyes },
            { "background_hair", content.Hair },
            { "background_height", content.Height },
            { "background_skin", content.Skin },
            { "background_weight", content.Weight },
            { "background_traits", mainContent.PersonalityTraits },
            { "background_ideals", mainContent.Ideals },
            { "background_bonds", mainContent.Bonds },
            { "background_flaws", mainContent.Flaws },
            { "background_feature_name", content.BackgroundFeatureName },
            { "background_feature", content.BackgroundFeature },
            { "background_trinket", content.Trinket }
        })
            {
                stamper.AcroFields.SetField(item.Key, item.Value, saveAppearance: false);
            }
            try
            {
                AcroFields.FieldPosition fieldPosition = stamper.AcroFields.GetFieldPositions("background_portrait_image").FirstOrDefault();
                if (fieldPosition != null)
                {
                    PushbuttonField pushbuttonField = new PushbuttonField(stamper.Writer, fieldPosition.position, "background_portrait_image-replaced")
                    {
                        Layout = 2,
                        Image = Image.GetInstance(content.CharacterAppearance),
                        ProportionalIcon = true,
                        Options = 1
                    };
                    stamper.AddAnnotation(pushbuttonField.Field, fieldPosition.page);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "StampDetailsContent");
            }
            try
            {
                AcroFields.FieldPosition fieldPosition2 = stamper.AcroFields.GetFieldPositions("background_organization_image").FirstOrDefault();
                if (fieldPosition2 != null && !string.IsNullOrWhiteSpace(content.OrganizationSymbol))
                {
                    PushbuttonField pushbuttonField2 = new PushbuttonField(stamper.Writer, fieldPosition2.position, "background_organization_image-replaced")
                    {
                        Layout = 2,
                        Image = Image.GetInstance(content.OrganizationSymbol),
                        ProportionalIcon = true,
                        Options = 1
                    };
                    stamper.AddAnnotation(pushbuttonField2.Field, fieldPosition2.page);
                }
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "StampDetailsContent");
            }
        }

        [Obsolete]
        private void CardTest()
        {
            CharacterSheetResourcePage characterSheetResourcePage = new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.aurora_spelldemo_flat.pdf");
            CharacterSheetResourcePage characterSheetResourcePage2 = new CharacterSheetResourcePage("Builder.Presentation.Resources.Sheets.Partial.partial_100h.pdf");
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "File2.pdf");
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "File3.pdf");
            MemoryStream memoryStream = new MemoryStream();
            using (Document document = new Document())
            {
                using (PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream))
                {
                    using (PdfReader pdfReader = new PdfReader(characterSheetResourcePage.GetResourceStream()))
                    {
                        PdfReader reader = characterSheetResourcePage2.CreateReader();
                        document.SetPageSize(pdfReader.GetPageSize(1));
                        document.Open();
                        document.NewPage();
                        PdfImportedPage importedPage = pdfWriter.GetImportedPage(pdfReader, 1);
                        pdfWriter.DirectContentUnder.AddTemplate(importedPage, 0f, 100f);
                        PdfImportedPage importedPage2 = pdfWriter.GetImportedPage(reader, 1);
                        pdfWriter.DirectContentUnder.AddTemplate(importedPage2, 0f, 150f);
                        pdfWriter.GetImportedPage(pdfReader, 1);
                        FillableContentGenerator fillableContentGenerator = new FillableContentGenerator(pdfWriter);
                        int num = 43;
                        float num2 = document.PageSize.Height - 160f;
                        for (int i = 0; i < 5; i++)
                        {
                            int num3 = num;
                            int num4 = i + 1;
                            int num5 = 72;
                            int num6 = 10;
                            Rectangle rectangle = new Rectangle(num3, num2, num3 + num5, num2 - (float)num6);
                            fillableContentGenerator.AddText(rectangle, $"spell_name_{num4}", "Fireball " + num4);
                            rectangle.Left += rectangle.Width;
                            rectangle.Left += 2f;
                            fillableContentGenerator.AddText(rectangle, $"spell_description_{num4}", "Fireball Description" + num4);
                            fillableContentGenerator.SetBoldItalic();
                            num3 += num5 + 2;
                            num5 = 100;
                            pdfWriter.AddAnnotation(fillableContentGenerator.CreateText(new Rectangle(num3, num2, num3 + num5, num2 - (float)num6), $"spell_description2_{num4}", "Fireball More." + num4).GetTextField());
                            fillableContentGenerator.SetItalic();
                            num3 += num5 + 2;
                            num5 = 100;
                            pdfWriter.AddAnnotation(fillableContentGenerator.CreateText(new Rectangle(num3, num2, num3 + num5, num2 - (float)num6), $"spell_description3_{num4}", "Fireball Again" + num4).GetTextField());
                            num2 -= (float)(num6 + 2);
                        }
                        Font font = new Font(BaseFont.CreateFont("Helvetica", "Cp1252", embedded: false));
                        Font font2 = new Font(BaseFont.CreateFont("Helvetica-Oblique", "Cp1252", embedded: false));
                        Font font3 = new Font(BaseFont.CreateFont("Helvetica-Bold", "Cp1252", embedded: false));
                        Font font4 = new Font(BaseFont.CreateFont("Helvetica-BoldOblique", "Cp1252", embedded: false));
                        font.Size = 7f;
                        font2.Size = 7f;
                        font3.Size = 7f;
                        font4.Size = 7f;
                        Paragraph paragraph = new Paragraph();
                        paragraph.Add(new Chunk("Fire. ", font4));
                        paragraph.Add(new Chunk("This is a test using an ", font));
                        paragraph.Add(new Chunk("italic", font2));
                        paragraph.Add(new Chunk(" font.\r\n", font));
                        foreach (Chunk chunk in paragraph.Chunks)
                        {
                            chunk.setLineHeight(7f);
                        }
                        ColumnText columnText = new ColumnText(pdfWriter.DirectContent);
                        columnText.SetSimpleColumn(new Rectangle(num, 10f, 200f, 200f));
                        columnText.AddText(paragraph);
                        columnText.AddText(paragraph);
                        columnText.AddText(paragraph);
                        columnText.Go();
                        document.Close();
                    }
                }
            }
            File.WriteAllBytes(text, memoryStream.ToArray());
            MemoryStream memoryStream2 = new MemoryStream();
            PdfReader pdfReader2 = new PdfReader(text);
            PdfStamper pdfStamper = new PdfStamper(pdfReader2, memoryStream2);
            pdfStamper.PartialFormFlattening("spell_name_1");
            pdfStamper.PartialFormFlattening("spell_description_1");
            pdfStamper.PartialFormFlattening("spell_description2_1");
            pdfStamper.PartialFormFlattening("spell_description3_1");
            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            pdfReader2.Close();
            File.WriteAllBytes(text, memoryStream2.ToArray());
            Process.Start(text);
        }
    }
}
