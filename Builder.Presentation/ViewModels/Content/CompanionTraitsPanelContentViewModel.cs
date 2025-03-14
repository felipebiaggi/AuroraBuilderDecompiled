using Builder.Core.Events;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.ViewModels.Content
{
    public sealed class CompanionTraitsPanelContentViewModel : ViewModelBase, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>
    {
        public ElementBaseCollection Traits { get; }

        public ElementBaseCollection Actions { get; }

        public ElementBaseCollection Reactions { get; }

        public bool HasTraits => Traits.Any();

        public bool HasActions => Actions.Any();

        public bool HasReactions => Reactions.Any();

        public CompanionTraitsPanelContentViewModel()
        {
            Traits = new ElementBaseCollection();
            Actions = new ElementBaseCollection();
            Reactions = new ElementBaseCollection();
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
            }
            else
            {
                base.EventAggregator.Subscribe(this);
            }
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            Handle();
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            Handle();
        }

        private void Handle()
        {
            Traits.Clear();
            Actions.Clear();
            Reactions.Clear();
            if (CharacterManager.Current.GetElements().FirstOrDefault((ElementBase x) => x.Type.Equals("Companion")) is CompanionElement companionElement)
            {
                if (companionElement.Traits.Any())
                {
                    List<ElementBase> source = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Companion Trait")).ToList();
                    foreach (string companionTrait in companionElement.Traits)
                    {
                        ElementBase elementBase = source.FirstOrDefault((ElementBase x) => x.Id.Equals(companionTrait));
                        if (elementBase != null)
                        {
                            Traits.Add(elementBase);
                        }
                    }
                }
                if (companionElement.Actions.Any())
                {
                    List<ElementBase> source2 = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Companion Action")).ToList();
                    foreach (string companionTrait2 in companionElement.Actions)
                    {
                        ElementBase elementBase2 = source2.FirstOrDefault((ElementBase x) => x.Id.Equals(companionTrait2));
                        if (elementBase2 != null)
                        {
                            Actions.Add(elementBase2);
                        }
                    }
                }
                if (companionElement.Reactions.Any())
                {
                    List<ElementBase> source3 = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Companion Reaction")).ToList();
                    foreach (string companionTrait3 in companionElement.Reactions)
                    {
                        ElementBase elementBase3 = source3.FirstOrDefault((ElementBase x) => x.Id.Equals(companionTrait3));
                        if (elementBase3 != null)
                        {
                            Reactions.Add(elementBase3);
                        }
                    }
                }
            }
            OnPropertyChanged("HasTraits");
            OnPropertyChanged("HasActions");
            OnPropertyChanged("HasReactions");
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            Traits.Add(new ElementBase("Keen Smell", "Companion Trait", "", ""));
            Traits.Add(new ElementBase("Keen Smells", "Companion Trait", "", ""));
            Actions.Add(new ElementBase("Ink Cloud", "Companion Trait", "", ""));
            Actions.Add(new ElementBase("Talons", "Companion Trait", "", ""));
            Reactions.Add(new ElementBase("Ink Cloud", "Companion Trait", "", ""));
            Reactions.Add(new ElementBase("Talons", "Companion Trait", "", ""));
            foreach (ElementBase trait in Traits)
            {
                trait.SheetDescription.Add(new ElementSheetDescriptions.SheetDescription("The bear has advantage on Wisdom (Perception) checks that rely on smell."));
            }
            Actions[0].SheetDescription.Usage = "Recharges after a Short or Long Rest";
            Actions[0].SheetDescription.Add(new ElementSheetDescriptions.SheetDescription("A 5-foot-radius cloud of ink extends all around the octopus if it is underwater. The area is heavily obscured for 1 minute, although a significant current can disperse the ink. After releasing the ink, the octopus can use the Dash action as a bonus action."));
            Actions[1].SheetDescription.Add(new ElementSheetDescriptions.SheetDescription("Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: 4 (1d4 + 2) slashing damage."));
            Reactions[0].SheetDescription.Usage = "Recharges after a Short or Long Rest";
            Reactions[0].SheetDescription.Add(new ElementSheetDescriptions.SheetDescription("A 5-foot-radius cloud of ink extends all around the octopus if it is underwater. The area is heavily obscured for 1 minute, although a significant current can disperse the ink. After releasing the ink, the octopus can use the Dash action as a bonus action."));
            Reactions[1].SheetDescription.Add(new ElementSheetDescriptions.SheetDescription("Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: 4 (1d4 + 2) slashing damage."));
        }
    }
}
