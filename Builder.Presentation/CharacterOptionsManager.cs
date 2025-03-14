using System;
using System.Linq;
using Builder.Data;
using Builder.Data.Strings;
using Builder.Presentation;

namespace Builder.Presentation
{
    [Obsolete]
    public class CharacterOptionsManager
    {
        private readonly CharacterManager _manager;

        public CharacterOptionsManager(CharacterManager manager)
        {
            _manager = manager;
        }

        public bool ContainsOption(string id)
        {
            return (from x in _manager.GetElements()
                    select x.Id).Contains(id);
        }

        public bool ContainsAverageHitPointsOption()
        {
            return ContainsOption(InternalOptions.AllowAverageHitPoints);
        }

        public bool ContainsMulticlassOption()
        {
            return ContainsOption(InternalOptions.AllowMulticlassing);
        }

        public bool ContainsFeatsOption()
        {
            return ContainsOption(InternalOptions.AllowFeats);
        }
    }
}
