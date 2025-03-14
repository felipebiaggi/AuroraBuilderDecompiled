using Builder.Data;
using Builder.Data.Rules;
using Builder.Presentation.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Builder.Presentation.Services
{
    public class SelectionRuleCollectionService
    {
        private readonly ExpressionInterpreter _expressionInterpreter;

        private readonly SelectRule _rule;

        private readonly int _acquisitionLevel;

        private readonly ElementBaseCollection _baseCollection;

        private ElementBaseCollection _baseSupportsCollection;

        public event EventHandler Evaluating;

        public SelectionRuleCollectionService(SelectRule rule)
        {
            _rule = rule;
            _acquisitionLevel = _rule.Attributes.RequiredLevel;
            _expressionInterpreter = new ExpressionInterpreter();
            _expressionInterpreter.InitializeWithSelectionRule(_rule);
            IEnumerable<ElementBase> elements = DataManager.Current.ElementsCollection.Where((ElementBase element) => element.Type.Equals(_rule.Attributes.Type));
            _baseCollection = new ElementBaseCollection(elements);
        }

        protected virtual void Initialize()
        {
            string supports = _rule.Attributes.Supports;
            bool containsElementIDs = _rule.Attributes.SupportsElementIdRange();
            IEnumerable<ElementBase> elements = _expressionInterpreter.EvaluateSupportsExpression(supports, _baseCollection, containsElementIDs);
            _baseSupportsCollection = new ElementBaseCollection(elements);
        }

        public IEnumerable<ElementBase> GetSupportedCollection()
        {
            List<ElementBase> list = new List<ElementBase>();
            list.AddRange(_baseSupportsCollection);
            return list;
        }

        private async void EvaluateChanges()
        {
            OnEvaluating();
            await Task.Delay(1000);
        }

        protected virtual void OnEvaluating()
        {
            this.Evaluating?.Invoke(this, EventArgs.Empty);
        }
    }
}
