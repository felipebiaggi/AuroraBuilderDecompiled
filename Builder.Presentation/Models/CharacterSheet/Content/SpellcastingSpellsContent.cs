using Builder.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Models.CharacterSheet.Content
{
    public class SpellcastingSpellsContent : ObservableObject
    {
        private int _slotsCount;

        private int _expendedSlotsCount;

        private int _remainingSlotsCount;

        private int _maximumListableCount;

        public int SlotsCount
        {
            get
            {
                return _slotsCount;
            }
            set
            {
                SetProperty(ref _slotsCount, value, "SlotsCount");
            }
        }

        public int ExpendedSlotsCount
        {
            get
            {
                return _expendedSlotsCount;
            }
            set
            {
                SetProperty(ref _expendedSlotsCount, value, "ExpendedSlotsCount");
            }
        }

        public int RemainingSlotsCount
        {
            get
            {
                return _remainingSlotsCount;
            }
            set
            {
                SetProperty(ref _remainingSlotsCount, value, "RemainingSlotsCount");
            }
        }

        public int MaximumListableCount
        {
            get
            {
                return _maximumListableCount;
            }
            set
            {
                SetProperty(ref _maximumListableCount, value, "MaximumListableCount");
            }
        }

        public ObservableCollection<SpellcastingSpellContent> Collection { get; } = new ObservableCollection<SpellcastingSpellContent>();

        public SpellcastingSpellsContent(int maximumListableCount)
        {
            _slotsCount = 0;
            _expendedSlotsCount = 0;
            _remainingSlotsCount = 0;
            _maximumListableCount = maximumListableCount;
        }

        public SpellcastingSpellContent GetSpell(int index, bool returnEmpty = false)
        {
            if (Collection.Count <= index)
            {
                if (!returnEmpty)
                {
                    return null;
                }
                return new SpellcastingSpellContent("");
            }
            return Collection[index];
        }
    }
}
