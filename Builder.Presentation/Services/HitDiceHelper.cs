using System;
using System.Linq;

namespace Builder.Presentation.Services
{
    public static class HitDiceHelper
    {
        public class HitDiceObject
        {
            private readonly int _sides;

            private readonly int _count;

            public HitDiceObject(int sides, int count = 1)
            {
                _sides = sides;
                _count = count;
            }

            public int GetSides()
            {
                return _sides;
            }

            public int GetCount()
            {
                return _count;
            }

            public int GetMinimumValue()
            {
                return _count;
            }

            public int GetMaximumValue()
            {
                return _count * _sides;
            }

            public int GetAverageValue()
            {
                int num = GetMinimumValue() + GetMaximumValue();
                if (_count == 1)
                {
                    return num / 2 + 1;
                }
                return num / 2;
            }

            public int GetRandomValue()
            {
                return new Random().Next(GetMinimumValue(), GetMaximumValue() + 1);
            }

            public override string ToString()
            {
                return $"{_count}d{_sides}";
            }
        }

        public static HitDiceObject Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (!input.Contains("d"))
            {
                throw new ArgumentException("hit dice string input doesn't contain a 'd'");
            }
            string text = input.Trim();
            int count;
            int sides;
            if (text.StartsWith("d"))
            {
                count = 1;
                sides = int.Parse(text.Trim('d'));
            }
            else
            {
                string[] source = text.Split('d');
                string s = source.FirstOrDefault();
                string s2 = source.LastOrDefault();
                count = int.Parse(s);
                sides = int.Parse(s2);
            }
            return new HitDiceObject(sides, count);
        }
    }
}
