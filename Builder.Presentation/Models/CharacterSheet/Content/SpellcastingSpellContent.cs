using Builder.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Models.CharacterSheet.Content
{
    public class SpellcastingSpellContent : ObservableObject
    {
        private string _name;

        private bool _isPrepared;

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

        public bool IsPrepared
        {
            get
            {
                return _isPrepared;
            }
            set
            {
                SetProperty(ref _isPrepared, value, "IsPrepared");
            }
        }

        public SpellcastingSpellContent(string name, bool isPrepared = false)
        {
            _name = name;
            _isPrepared = isPrepared;
        }
    }
}
