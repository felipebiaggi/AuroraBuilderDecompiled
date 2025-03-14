using System.Collections.Generic;

namespace Builder.Presentation.Services
{
    public interface IExpressionInterpreter
    {
        bool EvaluateEquipped(string key, string value);

        bool EvaluateRequire(string key, string value);

        bool EvaluateContains(IEnumerable<string> list, string value);
    }
}
