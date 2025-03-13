using Builder.Data.Rules.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Builder.Data
{
    public static class ElementParserFactory
    {
        public static IEnumerable<ElementParser> GetParsers()
        {
            return (from t in Assembly.GetAssembly(typeof(ElementParser)).GetTypes()
                    where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ElementParser))
                    select t into parser
                    select (ElementParser)Activator.CreateInstance(parser)).ToList();
        }

        public static IEnumerable<RuleParser> GetRuleParsers()
        {
            return (from t in Assembly.GetAssembly(typeof(RuleParser)).GetTypes()
                    where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(RuleParser))
                    select t into parser
                    select (RuleParser)Activator.CreateInstance(parser)).ToList();
        }

        public static IEnumerable<RuleParser> GetImplementations()
        {
            return (from type in Assembly.GetAssembly(typeof(RuleParser)).GetTypes()
                    where type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(RuleParser))
                    select type into parser
                    select (RuleParser)Activator.CreateInstance(parser)).ToList();
        }
    }
}
