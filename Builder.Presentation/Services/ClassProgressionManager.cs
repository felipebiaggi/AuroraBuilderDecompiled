using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Builder.Presentation.Services
{
    public class ClassProgressionManager : ProgressionManager
    {
        private DiceService _dice;

        private int[] _averageHitPointsArray = new int[20];

        private int[] _randomHitPointsArray = new int[20];

        private ElementBase _classElement;

        public bool IsObsolete { get; set; }

        public bool IsMainClass { get; }

        public bool IsMulticlass { get; }

        public int StartingLevel { get; }

        public ElementBase ClassElement
        {
            get
            {
                return _classElement;
            }
            private set
            {
                _classElement = value;
                if (_classElement != null)
                {
                    HD = _classElement.AsElement<Class>().HitDice;
                    if (_classElement.Aquisition.WasSelected)
                    {
                        SelectRule = _classElement.Aquisition.SelectRule;
                    }
                }
            }
        }

        public ElementBaseCollection LevelElements { get; } = new ElementBaseCollection();

        public string HD { get; private set; }

        public SelectRule SelectRule { get; set; }

        public ClassProgressionManager(bool isMainClass, bool isMulticlass, int startingLevel, ElementBase classElement)
        {
            _dice = new DiceService();
            IsMainClass = isMainClass;
            IsMulticlass = isMulticlass;
            StartingLevel = startingLevel;
            SetClass(classElement);
            base.ProgressionLevel = 1;
        }

        public int GetHitDieValue()
        {
            switch (HD)
            {
                case "d2":
                    return 2;
                case "d3":
                    return 3;
                case "d4":
                    return 4;
                case "d6":
                    return 6;
                case "d8":
                    return 8;
                case "d10":
                    return 10;
                case "d12":
                    return 12;
                case "d20":
                    return 20;
                case "d50":
                    return 50;
                case "d100":
                    return 100;
                default:
                    throw new ArgumentException("HD");
            }
        }

        public void SetClass(ElementBase element)
        {
            string hD = HD;
            ClassElement = element;
            if (element != null)
            {
                _averageHitPointsArray = GenerateAverageHitPointsArray(HD, IsMainClass);
                if (element.AsElement<Class>().HitDice != hD)
                {
                    GenerateRandomArray();
                }
                if (_randomHitPointsArray == null || _randomHitPointsArray.Sum() == 0)
                {
                    GenerateRandomArray();
                }
            }
        }

        private void GenerateRandomArray()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Logger.Info(string.Format("gen random array {0} {1}", stopwatch.ElapsedMilliseconds, string.Join(",", _randomHitPointsArray)));
            int[] result = null;
            Task.Run(delegate
            {
                result = GenerateRandomHitPointsArray(HD, IsMainClass).Result;
            }).Wait();
            _randomHitPointsArray = result;
            Logger.Info(string.Format("gen random array complete {0} {1}", stopwatch.ElapsedMilliseconds, string.Join(",", _randomHitPointsArray)));
        }

        public void RemoveClass()
        {
            ClassElement = null;
        }

        private async Task<int[]> GenerateRandomHitPointsArray(string hd, bool isMainClass)
        {
            List<int> array = new List<int>();
            for (int level = 1; level <= 20; level++)
            {
                if (level == 1 && isMainClass)
                {
                    if (hd != null)
                    {
                        switch (hd)
                        {
                            case "d2":
                                array.Add(2);
                                continue;
                            case "d4":
                                array.Add(4);
                                continue;
                            case "d6":
                                array.Add(6);
                                continue;
                            case "d8":
                                array.Add(8);
                                continue;
                            case "d10":
                                array.Add(10);
                                continue;
                            case "d12":
                                array.Add(12);
                                continue;
                            case "d20":
                                array.Add(20);
                                continue;
                        }
                    }
                    throw new ArgumentException("hd");
                }
                if (hd != null)
                {
                    switch (hd)
                    {
                        case "d2":
                            {
                                List<int> list = array;
                                list.Add(await _dice.D2());
                                continue;
                            }
                        case "d4":
                            {
                                List<int> list = array;
                                list.Add(await _dice.D4());
                                continue;
                            }
                        case "d6":
                            {
                                List<int> list = array;
                                list.Add(await _dice.D6());
                                continue;
                            }
                        case "d8":
                            {
                                List<int> list = array;
                                list.Add(await _dice.D8());
                                continue;
                            }
                        case "d10":
                            {
                                List<int> list = array;
                                list.Add(await _dice.D10());
                                continue;
                            }
                        case "d12":
                            {
                                List<int> list = array;
                                list.Add(await _dice.D12());
                                continue;
                            }
                        case "d20":
                            {
                                List<int> list = array;
                                list.Add(await _dice.D20());
                                continue;
                            }
                    }
                }
                throw new ArgumentException("hd");
            }
            return array.ToArray();
        }

        private int[] GenerateAverageHitPointsArray(string hd, bool isMainClass)
        {
            List<int> list = new List<int>();
            for (int i = 1; i <= 20; i++)
            {
                if (i == 1 && isMainClass)
                {
                    switch (hd)
                    {
                        case "d2":
                            list.Add(2);
                            break;
                        case "d4":
                            list.Add(4);
                            break;
                        case "d6":
                            list.Add(6);
                            break;
                        case "d8":
                            list.Add(8);
                            break;
                        case "d10":
                            list.Add(10);
                            break;
                        case "d12":
                            list.Add(12);
                            break;
                        case "d20":
                            list.Add(20);
                            break;
                        default:
                            throw new ArgumentException("hd");
                    }
                }
                else
                {
                    switch (hd)
                    {
                        case "d2":
                            list.Add(2);
                            break;
                        case "d4":
                            list.Add(3);
                            break;
                        case "d6":
                            list.Add(4);
                            break;
                        case "d8":
                            list.Add(5);
                            break;
                        case "d10":
                            list.Add(6);
                            break;
                        case "d12":
                            list.Add(7);
                            break;
                        case "d20":
                            list.Add(11);
                            break;
                        default:
                            throw new ArgumentException("hd");
                    }
                }
            }
            return list.ToArray();
        }

        public bool HasArchetype()
        {
            return base.Elements.ContainsType("Archetype");
        }

        public string GetClassLevelStatisticsName()
        {
            return "level:" + ClassElement.Name.ToLowerInvariant();
        }

        public int GetHitPoints()
        {
            int num = 0;
            if (CharacterManager.Current.ContainsAverageHitPointsOption())
            {
                for (int i = 0; i < base.ProgressionLevel; i++)
                {
                    num += _averageHitPointsArray[i];
                }
            }
            else
            {
                for (int j = 0; j < base.ProgressionLevel; j++)
                {
                    num += _randomHitPointsArray[j];
                }
            }
            return num;
        }

        public int[] GetRandomHitPointsArrayAsync()
        {
            return _randomHitPointsArray;
        }

        public void SetRandomHitPointsArray(int[] array)
        {
            _randomHitPointsArray = array;
        }

        public override string ToString()
        {
            return $"{ClassElement} {base.ProgressionLevel}";
        }
    }

}
