using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Builder.Data;
using Builder.Data.Rules;
using Builder.Presentation;
using Builder.Presentation.Elements;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;


namespace Builder.Presentation.Elements
{
    [Obsolete("new implementation but not done yet!")]
    public class SelectionCollectionService
    {
        private readonly ExpressionInterpreter _expressionInterpreter;

        private readonly List<ElementBase> _baseCollection;

        private readonly SelectRule _selectionRule;

        private readonly ISourceRestrictionsProvider _sourceRestrictionsProvider;

        public bool PopulatingCollection { get; set; }

        public bool IgnoreRequirements { get; set; }

        public bool IgnoreSourceRestrictions { get; set; }

        public int AcquisitionLevel { get; }

        public bool ContainsSupports { get; }

        public bool IsElementsRange { get; }

        public List<ElementBase> SupportedCollection { get; protected set; }

        public event EventHandler<ElementCollectionEventArgs> BaseCollectionPopulated;

        public event EventHandler<ElementCollectionEventArgs> CollectionPopulated;

        public SelectionCollectionService(SelectRule selectionRule, ISourceRestrictionsProvider sourceRestrictionsProvider)
        {
            _selectionRule = selectionRule ?? throw new ArgumentNullException("selectionRule");
            _sourceRestrictionsProvider = sourceRestrictionsProvider;
            _baseCollection = new List<ElementBase>();
            _expressionInterpreter = new ExpressionInterpreter();
            _expressionInterpreter.InitializeWithSelectionRule(_selectionRule);
            AcquisitionLevel = _selectionRule.Attributes.RequiredLevel;
            ContainsSupports = _selectionRule.Attributes.ContainsSupports();
            IsElementsRange = _selectionRule.Attributes.SupportsElementIdRange();
            SupportedCollection = new List<ElementBase>();
        }

        public async Task InitializeAsync()
        {
            await PopulateBaseCollectionAsync();
        }

        public async Task<List<ElementBase>> PopulateAsync()
        {
            PopulatingCollection = true;
            await Task.Run(delegate
            {
                List<ElementBase> list = new List<ElementBase>(_baseCollection);
                if (!IgnoreSourceRestrictions)
                {
                    RemoveSourceRestrictedElements(list);
                }
                if (!IgnoreRequirements)
                {
                    ElementBaseCollection elements = CharacterManager.Current.GetElements();
                    list = GetAcceptedRequirementElements(list, elements.Select((ElementBase x) => x.Id).ToList());
                }
                SupportedCollection.Clear();
                SupportedCollection.AddRange(list);
            });
            PopulatingCollection = false;
            OnCollectionPopulated(SupportedCollection);
            return SupportedCollection;
        }

        protected async Task PopulateBaseCollectionAsync()
        {
            PopulatingCollection = true;
            await Task.Run(delegate
            {
                List<ElementBase> list = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals(_selectionRule.Attributes.Type)).ToList();
                List<ElementBase> list2 = new List<ElementBase>();
                if (ContainsSupports)
                {
                    string supports = _selectionRule.Attributes.Supports;
                    list2.AddRange(_expressionInterpreter.EvaluateSupportsExpression(supports, list, IsElementsRange));
                }
                else
                {
                    list2.AddRange(list);
                }
                _baseCollection.Clear();
                _baseCollection.AddRange(list2);
            });
            PopulatingCollection = false;
            OnBaseCollectionPopulated(_baseCollection);
        }

        protected List<ElementBase> RemoveSourceRestrictedElements(List<ElementBase> selectionElements)
        {
            List<string> list = _sourceRestrictionsProvider.GetRestrictedElements().ToList();
            List<string> list2 = _sourceRestrictionsProvider.GetUndefinedRestrictedSources().ToList();
            List<ElementBase> list3 = new List<ElementBase>();
            foreach (ElementBase selectionElement in selectionElements)
            {
                if (list.Contains(selectionElement.Id))
                {
                    list3.Add(selectionElement);
                }
                else if (list2.Contains(selectionElement.Source))
                {
                    list3.Add(selectionElement);
                }
            }
            foreach (ElementBase item in list3)
            {
                selectionElements.Remove(item);
            }
            return selectionElements;
        }

        protected List<ElementBase> GetAcceptedRequirementElements(List<ElementBase> elements, List<string> idRange)
        {
            List<ElementBase> list = new List<ElementBase>();
            foreach (ElementBase element in elements)
            {
                if (element.HasRequirements)
                {
                    if (_expressionInterpreter.EvaluateElementRequirementsExpression(element.Requirements, idRange))
                    {
                        list.Add(element);
                    }
                }
                else
                {
                    list.Add(element);
                }
            }
            return list;
        }

        protected virtual void OnBaseCollectionPopulated(List<ElementBase> elements)
        {
            this.BaseCollectionPopulated?.Invoke(this, new ElementCollectionEventArgs(elements));
        }

        protected virtual void OnCollectionPopulated(List<ElementBase> elements)
        {
            this.CollectionPopulated?.Invoke(this, new ElementCollectionEventArgs(elements));
        }
    }
}
