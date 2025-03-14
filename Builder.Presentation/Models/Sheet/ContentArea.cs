using System.Collections.Generic;
using System.Text;

namespace Builder.Presentation.Models.Sheet
{
    public class ContentArea : List<ContentLine>
    {
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (Enumerator enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ContentLine current = enumerator.Current;
                    stringBuilder.Append(current);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
