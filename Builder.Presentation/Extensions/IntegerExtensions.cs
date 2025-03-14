using System;

namespace Builder.Presentation.Extensions
{
    public static class IntegerExtensions
    {
        public static int IsPercetageOf(this int curent, int total)
        {
            return (int)Math.Round((double)(100 * curent) / (double)total);
        }
    }
}
