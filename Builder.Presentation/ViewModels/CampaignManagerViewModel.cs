using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Builder.Core;
using Builder.Core.Events;
using Builder.Presentation;
using Builder.Presentation.Models.Campaign;
using Builder.Presentation.Models.Sources;
using Builder.Presentation.Services.Sources;
using Builder.Presentation.Telemetry;
using Builder.Presentation.ViewModels;
using Builder.Presentation.ViewModels.Base;
using Builder.Presentation.Views;
using Builder.Presentation.Views.Sliders;

namespace Builder.Presentation.ViewModels
{
    public sealed class CampaignManagerViewModel : ViewModelBase, ISubscriber<RestrictedSourcesLoadRequest>
    {
        private string _name;

        private SourcesGroup _selectedSourcesGroup;

        private SourceItem _selectedSourceItem;

        private IEnumerable<SourceItem> _selectedSourceItems;

        private bool _isCompendiumSeachAvailable;

        public CharacterManager Manager => CharacterManager.Current;

        public SourcesManager SourcesManager => CharacterManager.Current.SourcesManager;

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

        public SourcesGroup SelectedSourcesGroup
        {
            get
            {
                return _selectedSourcesGroup;
            }
            set
            {
                SetProperty(ref _selectedSourcesGroup, value, "SelectedSourcesGroup");
            }
        }

        public SourceItem SelectedSourceItem
        {
            get
            {
                return _selectedSourceItem;
            }
            set
            {
                SetProperty(ref _selectedSourceItem, value, "SelectedSourceItem");
                SendDisplayRequest();
                ViewInCompendiumCommand?.RaiseCanExecuteChanged();
                IsCompendiumSeachAvailable = CanViewInCompendium();
            }
        }

        public IEnumerable<SourceItem> SelectedSourceItems
        {
            get
            {
                return _selectedSourceItems;
            }
            set
            {
                SetProperty(ref _selectedSourceItems, value, "SelectedSourceItems");
            }
        }

        public ICommand ApplyRestrictionsCommand => new RelayCommand(ApplyRestrictions);

        public ICommand LoadRestrictionsCommand => new RelayCommand(LoadRestrictions);

        public ICommand ClearRestrictionsCommand => new RelayCommand(ClearRestrictions);

        public ICommand StoreDefaultRestrictionsCommand => new RelayCommand(StoreDefaultRestrictions);

        public ICommand RestrictAllPlaytestCommand => new RelayCommand(RestrictAllPlaytest);

        public RelayCommand ViewInCompendiumCommand { get; }

        public bool IsCompendiumSeachAvailable
        {
            get
            {
                return _isCompendiumSeachAvailable;
            }
            set
            {
                SetProperty(ref _isCompendiumSeachAvailable, value, "IsCompendiumSeachAvailable");
            }
        }

        public ICommand ToggleSelectedSourceGroupCommand => new RelayCommand<SourcesGroup>(ToggleSelectedSourceGroup);

        public ICommand ToggleSelectedSourceItemCommand => new RelayCommand<SourceItem>(ToggleSelectedSourceItem);

        public ICommand ToggleSelectedSourceItemsCommand => new RelayCommand(ToggleSelectedSourceItems);

        public ICommand ToggleSameAuthorSourcesCommand => new RelayCommand<SourceItem>(ToggleSameAuthorSources);

        public CampaignManagerViewModel()
        {
            _name = "";
            if (base.IsInDesignMode)
            {
                Name = "Design Time Campaign";
                InitializeDesignData();
                return;
            }
            ViewInCompendiumCommand = new RelayCommand(ViewInCompendium, CanViewInCompendium);
            ViewInCompendiumCommand.RaiseCanExecuteChanged();
            SubscribeWithEventAggregator();
            SelectedSourcesGroup = SourcesManager.SourceGroups.FirstOrDefault();
            SelectedSourceItem = SelectedSourcesGroup?.Sources.FirstOrDefault();
            SourcesManager.LoadDefaults();
            SendDisplayRequest();
        }

        private void SendDisplayRequest()
        {
            base.EventAggregator.Send(new SourceElementDescriptionDisplayRequestEvent(_selectedSourceItem?.Source));
        }

        private void ApplyRestrictions()
        {
            SourcesManager.ApplyRestrictions(reprocess: true);
        }

        private void LoadRestrictions()
        {
            SourcesManager.LoadDefaults();
        }

        private void ClearRestrictions()
        {
            SourcesManager.ClearRestrictions();
        }

        private void RestrictAllPlaytest()
        {
            foreach (SourcesGroup sourceGroup in SourcesManager.SourceGroups)
            {
                foreach (SourceItem source in sourceGroup.Sources)
                {
                    if (source.Source.IsPlaytestContent)
                    {
                        source.SetIsChecked(false, updateChildren: false, updateParent: true);
                    }
                }
            }
        }

        private void ViewInCompendium()
        {
            if (SelectedSourceItem != null)
            {
                string name = SelectedSourceItem.Source.Name;
                AnalyticsEventHelper.SourcesCompendiumLookup(name);
                //base.EventAggregator.Send(new CompendiumShowSourceEventArgs(name));
                base.EventAggregator.Send(new ShowSliderEvent(Slider.Compendium));
            }
        }

        private bool CanViewInCompendium()
        {
            return SelectedSourceItem != null;
        }

        private void StoreDefaultRestrictions()
        {
            SourcesManager.StoreDefaults();
        }

        private void ToggleSelectedSourceGroup(SourcesGroup group)
        {
            if (group != null && group.AllowUnchecking && group.IsChecked.HasValue)
            {
                group.SetIsChecked(!group.IsChecked.Value, updateChildren: true);
            }
        }

        private void ToggleSelectedSourceItem(SourceItem source)
        {
            if (source != null && source.AllowUnchecking && source.IsChecked.HasValue)
            {
                source.SetIsChecked(!source.IsChecked.Value, updateChildren: false, updateParent: true);
            }
        }

        private void ToggleSelectedSourceItems()
        {
            if (SelectedSourceItems == null)
            {
                return;
            }
            foreach (SourceItem selectedSourceItem in SelectedSourceItems)
            {
                ToggleSelectedSourceItem(selectedSourceItem);
            }
        }

        private void ToggleSameAuthorSources(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            SourceItem source = parameter as SourceItem;
            foreach (SourceItem item in SelectedSourcesGroup.Sources.Where((SourceItem x) => x.Source.Author == source.Source.Author))
            {
                ToggleSelectedSourceItem(item);
            }
        }

        [Obsolete]
        public void OnHandleEvent(RestrictedSourcesLoadRequest args)
        {
            SourcesManager.ClearRestrictions(apply: false);
            foreach (SourcesGroup sourceGroup in SourcesManager.SourceGroups)
            {
                foreach (SourceItem source in sourceGroup.Sources)
                {
                    if (args.SourceIds.Contains(source.Source.Id))
                    {
                        source.SetIsChecked(false, updateChildren: false, updateParent: true);
                    }
                }
            }
            SourcesManager.ApplyRestrictions();
        }
    }
}
