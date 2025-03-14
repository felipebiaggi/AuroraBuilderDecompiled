using Builder.Core.Logging;
using Builder.Data.Rules;
using Builder.Data;
using Builder.Presentation.Services;
using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Builder.Presentation
{
    public class ElementsOrganizerRefactored
    {
        private readonly ExpressionInterpreter _interpreter;

        private readonly ElementBaseCollection _elements;

        public ElementsOrganizerRefactored(IEnumerable<ElementBase> elements = null)
        {
            _interpreter = new ExpressionInterpreter();
            if (elements != null)
            {
                _elements = new ElementBaseCollection(elements);
            }
        }

        public void Initialize(IEnumerable<ElementBase> elements)
        {
            _elements.Clear();
            _elements.AddRange(elements);
        }

        public bool ContainsElementType(string type)
        {
            return _elements.Any((ElementBase x) => x.Type.Equals(type));
        }

        public IEnumerable<ElementBase> GetElementsAs(string type, bool includeDuplicates = true)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (includeDuplicates)
            {
                foreach (ElementBase item in _elements.Where((ElementBase element) => element.Type.Equals(type)))
                {
                    list.Add(item.Copy());
                }
                return list;
            }
            foreach (ElementBase item2 in from element in _elements
                                          where element.Type.Equals(type)
                                          select element into x
                                          group x by x.Id into x
                                          select x.First())
            {
                list.Add(item2.Copy());
            }
            return list;
        }

        public IEnumerable<T> GetElementsAs<T>(bool includeDuplicates = true) where T : ElementBase
        {
            List<T> list = new List<T>();
            if (includeDuplicates)
            {
                foreach (T item in _elements.Where((ElementBase element) => element is T).Cast<T>())
                {
                    list.Add(item.Copy());
                }
                return list;
            }
            foreach (T item2 in (from element in _elements
                                 where element is T
                                 select element into x
                                 group x by x.Id into x
                                 select x.First()).Cast<T>())
            {
                list.Add(item2.Copy());
            }
            return list;
        }

        public ElementBase GetElement(string id)
        {
            return _elements.FirstOrDefault((ElementBase x) => x.Id.Equals(id)).Copy();
        }

        public IEnumerable<ElementBase> GetElements()
        {
            List<ElementBase> list = new List<ElementBase>();
            foreach (ElementBase element in _elements)
            {
                list.Add(element.Copy());
            }
            return list;
        }

        public ElementBaseCollection GetSupportedElements(SelectRule rule)
        {
            bool containsElementIDs = rule.Attributes.ContainsSupports() && !rule.Attributes.Supports.Contains("||") && rule.Attributes.Supports.Contains("|");
            return GetSupportedElements(rule, containsElementIDs);
        }

        private ElementBaseCollection GetSupportedElements(SelectRule rule, bool containsElementIDs)
        {
            IEnumerable<ElementBase> enumerable = _elements.Where((ElementBase x) => x.Type.Equals(rule.Attributes.Type));
            if (!rule.Attributes.ContainsSupports())
            {
                return new ElementBaseCollection(enumerable);
            }
            if (!containsElementIDs)
            {
                return new ElementBaseCollection(_interpreter.EvaluateSupportsExpression(rule.Attributes.Supports, enumerable));
            }
            List<string> list = new List<string>();
            foreach (Match item in Regex.Matches(rule.Attributes.Supports, "([-a-zA-Z \\w]+)").Cast<Match>())
            {
                if (!list.Contains(item.Value))
                {
                    list.Add(item.Value);
                }
            }
            string text;
            if (containsElementIDs)
            {
                text = rule.Attributes.Supports;
                foreach (string item2 in list)
                {
                    text = text.Replace(item2, "x.Id.Equals(\"" + item2 + "\")");
                }
                text = text.Replace("|", "||");
            }
            else
            {
                text = rule.Attributes.Supports;
                foreach (string item3 in list)
                {
                    text = text.Replace(item3, "x.Supports.Contains(\"" + item3 + "\")");
                }
            }
            Interpreter interpreter = new Interpreter();
            interpreter.EnableAssignment(AssignmentOperators.None);
            Logger.Debug($"interpreting the {text} with {rule}");
            Expression<Func<ElementBase, bool>> predicate = interpreter.ParseAsExpression<Func<ElementBase, bool>>(text, new string[1] { "x" });
            return new ElementBaseCollection(enumerable.AsQueryable().Where(predicate));
        }
    }
}
