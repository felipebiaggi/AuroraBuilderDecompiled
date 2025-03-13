using Builder.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Elements
{
    public class Multiclass : Class
    {
        public override bool AllowMultipleElements => true;

        public string MulticlassRequirements { get; set; }

        public string MulticlassPrerequisites { get; set; }

        public string MulticlassDescription { get; set; }

        public string MulticlassProficiencies { get; set; }

        public bool HasMulticlassRequirements => !string.IsNullOrWhiteSpace(MulticlassRequirements);

        public bool HasMulticlassPrerequisites => !string.IsNullOrWhiteSpace(MulticlassPrerequisites);
    }

}
