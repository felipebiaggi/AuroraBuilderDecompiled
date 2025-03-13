﻿using Builder.Data.ElementParsers;
using System.Collections.Generic;

namespace Builder.Data.Elements
{
    public class Race : ElementBase
    {
        public List<string> MaleNames { get; set; } = new List<string>();

        public List<string> FemaleNames { get; set; } = new List<string>();

        public RacialNames Names { get; set; } = new RacialNames();

        public string BaseHeight { get; set; }

        public string BaseWeight { get; set; }
    }
}
