using Builder.Core.Logging;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Builder.Presentation.Services.QuickBar.Commands.Base
{
    public class QuickBarDiceCommand : QuickBarCommand
    {
        private DiceService _dice;

        public QuickBarDiceCommand()
            : base("roll")
        {
            _dice = new DiceService();
        }

        public override async void Execute(string parameter)
        {
            MainWindowStatusUpdateEvent args = new MainWindowStatusUpdateEvent("rolling " + parameter);
            try
            {
                ApplicationManager.Current.EventAggregator.Send(args);
                List<string> list = (from x in parameter.Split('d')
                                     where !string.IsNullOrWhiteSpace(x)
                                     select x).ToList();
                int amount = int.Parse(list[0]);
                string size = list[1];
                int bonus = 0;
                if (list[1].Contains("+"))
                {
                    size = list[1].Split('+')[0];
                    bonus = int.Parse(list[1].Split('+')[1]);
                }
                else if (list[1].Contains("-"))
                {
                    size = list[1].Split('-')[0];
                    bonus = -int.Parse(list[1].Split('-')[1]);
                }
                int total = 0;
                List<int> results = new List<int>();
                for (int i = 0; i < amount; i++)
                {
                    if (size != null)
                    {
                        switch (size)
                        {
                            case "4":
                                {
                                    List<int> list2 = results;
                                    list2.Add(await _dice.D4());
                                    break;
                                }
                            case "6":
                                {
                                    List<int> list2 = results;
                                    list2.Add(await _dice.D6());
                                    break;
                                }
                            case "8":
                                {
                                    List<int> list2 = results;
                                    list2.Add(await _dice.D8());
                                    break;
                                }
                            case "10":
                                {
                                    List<int> list2 = results;
                                    list2.Add(await _dice.D10());
                                    break;
                                }
                            case "12":
                                {
                                    List<int> list2 = results;
                                    list2.Add(await _dice.D12());
                                    break;
                                }
                            case "20":
                                {
                                    List<int> list2 = results;
                                    list2.Add(await _dice.D20());
                                    break;
                                }
                        }
                    }
                    args.ProgressPercentage = (i + 1).IsPercetageOf(amount);
                    ApplicationManager.Current.EventAggregator.Send(args);
                    await Task.Delay(100);
                }
                total += results.Sum();
                total += bonus;
                args.StatusMessage = string.Format("You rolled a total of {0} on {1}. (individual rolls: {2})", total, parameter, string.Join(",", results));
                args.ProgressPercentage = 100;
                ApplicationManager.Current.EventAggregator.Send(args);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Execute");
                args.StatusMessage = ex.Message;
                args.IsDanger = true;
            }
            ApplicationManager.Current.EventAggregator.Send(args);
        }
    }
}
