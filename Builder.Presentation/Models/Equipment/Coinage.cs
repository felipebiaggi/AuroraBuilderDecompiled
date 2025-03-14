using Builder.Core;
using Builder.Core.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Builder.Presentation.Models.Equipment
{
    public class Coinage : ObservableObject
    {
        public enum CurrencyCoin
        {
            Copper,
            Silver,
            Electrum,
            Gold,
            Platinum
        }

        private long _calculationBase;

        private long _copper;

        private long _silver;

        private long _electrum;

        private long _gold;

        private long _platinum;

        public long CalculationBase
        {
            get
            {
                return _calculationBase;
            }
            private set
            {
                SetProperty(ref _calculationBase, value, "CalculationBase");
                if (!ValidateBase())
                {
                    Logger.Warning($"CalculationBase does not equal the calculated [base:{_calculationBase}] [calc:{CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver) + CalculateBase(CurrencyCoin.Electrum, Electrum) + CalculateBase(CurrencyCoin.Gold, Gold) + CalculateBase(CurrencyCoin.Platinum, Platinum)}]");
                }
                OnPropertyChanged("DisplayCoinage");
            }
        }

        public long Copper
        {
            get
            {
                return _copper;
            }
            set
            {
                SetProperty(ref _copper, value, "Copper");
            }
        }

        public long Silver
        {
            get
            {
                return _silver;
            }
            set
            {
                _ = _silver;
                SetProperty(ref _silver, value, "Silver");
            }
        }

        public long Electrum
        {
            get
            {
                return _electrum;
            }
            set
            {
                _ = _electrum;
                SetProperty(ref _electrum, value, "Electrum");
            }
        }

        public long Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                _ = _gold;
                SetProperty(ref _gold, value, "Gold");
            }
        }

        public long Platinum
        {
            get
            {
                return _platinum;
            }
            set
            {
                _ = _platinum;
                SetProperty(ref _platinum, value, "Platinum");
            }
        }

        public decimal DisplayCoinage => (decimal)_calculationBase / 100m;

        public Coinage()
        {
            _calculationBase = 0L;
            _copper = 0L;
            _silver = 0L;
            _electrum = 0L;
            _gold = 0L;
            _platinum = 0L;
        }

        public void Clear()
        {
            CalculationBase = 0L;
            Copper = 0L;
            Silver = 0L;
            Electrum = 0L;
            Gold = 0L;
            Platinum = 0L;
        }

        public void Set(long copper, long silver, long electrum, long gold, long platinum)
        {
            Clear();
            Deposit(CurrencyCoin.Copper, copper);
            Deposit(CurrencyCoin.Silver, silver);
            Deposit(CurrencyCoin.Electrum, electrum);
            Deposit(CurrencyCoin.Gold, gold);
            Deposit(CurrencyCoin.Platinum, platinum);
        }

        public long UpdateCalculationBaseFromOutside([CallerMemberName] string callerName = null)
        {
            if (callerName == null || callerName.Equals("Coinage"))
            {
                return CalculationBase;
            }
            long num = CalculateBase(CurrencyCoin.Copper, Copper);
            long num2 = CalculateBase(CurrencyCoin.Silver, Silver);
            long num3 = CalculateBase(CurrencyCoin.Electrum, Electrum);
            long num4 = CalculateBase(CurrencyCoin.Gold, Gold);
            long num5 = CalculateBase(CurrencyCoin.Platinum, Platinum);
            long calculationBase = num + num2 + num3 + num4 + num5;
            CalculationBase = calculationBase;
            OnPropertyChanged("DisplayCoinage");
            return CalculationBase;
        }

        public void Deposit(CurrencyCoin coin, long amount = 1L)
        {
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    Copper += amount;
                    CalculationBase += amount;
                    break;
                case CurrencyCoin.Silver:
                    Silver += amount;
                    CalculationBase += amount * 10;
                    break;
                case CurrencyCoin.Electrum:
                    Electrum += amount;
                    CalculationBase += amount * 50;
                    break;
                case CurrencyCoin.Gold:
                    Gold += amount;
                    CalculationBase += amount * 100;
                    break;
                case CurrencyCoin.Platinum:
                    Platinum += amount;
                    CalculationBase += amount * 1000;
                    break;
            }
        }

        public bool Withdraw(CurrencyCoin coin, long amount, bool withdrawLowerDenomination = true)
        {
            if (!withdrawLowerDenomination && !CanWithdrawCoin(coin, amount))
            {
                return false;
            }
            if (!CanWithdraw(coin, amount))
            {
                return false;
            }
            long num = CalculateBase(coin, amount);
            if (CanWithdrawCoin(coin, amount))
            {
                switch (coin)
                {
                    case CurrencyCoin.Copper:
                        Copper -= amount;
                        break;
                    case CurrencyCoin.Silver:
                        Silver -= amount;
                        break;
                    case CurrencyCoin.Electrum:
                        Electrum -= amount;
                        break;
                    case CurrencyCoin.Gold:
                        Gold -= amount;
                        break;
                    case CurrencyCoin.Platinum:
                        Platinum -= amount;
                        break;
                }
                CalculationBase -= num;
            }
            else
            {
                CurrencyCoin currencyCoin = coin;
                while (!CanWithdrawUnderCoin(currencyCoin, num))
                {
                    if (currencyCoin == CurrencyCoin.Platinum)
                    {
                        ConvertDown(currencyCoin, 1L);
                    }
                    currencyCoin = Up(currencyCoin);
                }
                WithdrawUnderCoin(currencyCoin, num);
            }
            return true;
        }

        public bool HasSufficienctFunds(CurrencyCoin coin, long amount)
        {
            return CanWithdraw(coin, amount);
        }

        private bool CanWithdraw(CurrencyCoin coin, long amount)
        {
            return CalculateBase(coin, amount) <= _calculationBase;
        }

        private bool CanWithdrawCoin(CurrencyCoin coin, long amount)
        {
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    return Copper >= amount;
                case CurrencyCoin.Silver:
                    return Silver >= amount;
                case CurrencyCoin.Electrum:
                    return Electrum >= amount;
                case CurrencyCoin.Gold:
                    return Gold >= amount;
                case CurrencyCoin.Platinum:
                    return Platinum >= amount;
                default:
                    return false;
            }
        }

        private bool CanWithdrawUnderCoin(CurrencyCoin coin, long withdrawAmount)
        {
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    return false;
                case CurrencyCoin.Silver:
                    return withdrawAmount <= CalculateBase(CurrencyCoin.Copper, Copper);
                case CurrencyCoin.Electrum:
                    return withdrawAmount <= CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver);
                case CurrencyCoin.Gold:
                    return withdrawAmount <= CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver) + CalculateBase(CurrencyCoin.Electrum, Electrum);
                case CurrencyCoin.Platinum:
                    return withdrawAmount <= CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver) + CalculateBase(CurrencyCoin.Electrum, Electrum) + CalculateBase(CurrencyCoin.Gold, Gold);
                default:
                    return false;
            }
        }

        private long HowMuchUnderCoin(CurrencyCoin coin)
        {
            switch (coin)
            {
                case CurrencyCoin.Silver:
                    return CalculateBase(CurrencyCoin.Copper, Copper);
                case CurrencyCoin.Electrum:
                    return CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver);
                case CurrencyCoin.Gold:
                    return CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver) + CalculateBase(CurrencyCoin.Electrum, Electrum);
                case CurrencyCoin.Platinum:
                    return CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver) + CalculateBase(CurrencyCoin.Electrum, Electrum) + CalculateBase(CurrencyCoin.Gold, Gold);
                default:
                    return 0L;
            }
        }

        private void WithdrawUnderCoin(CurrencyCoin coin, long withdrawAmount)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (coin == CurrencyCoin.Platinum || coin == CurrencyCoin.Gold || coin == CurrencyCoin.Electrum)
            {
                while (coin != CurrencyCoin.Silver)
                {
                    coin = Down(coin);
                    while (!CanWithdrawUnderCoin(coin, withdrawAmount))
                    {
                        long required = withdrawAmount - HowMuchUnderCoin(coin);
                        long val = CalculateBasedOnRequiredBase(coin, required);
                        ConvertDown(coin, Math.Max(1L, val));
                    }
                }
            }
            while (Copper < withdrawAmount)
            {
                CalculateBasedOnRequiredBase(CurrencyCoin.Silver, withdrawAmount - Copper);
                long val2 = (withdrawAmount - Copper) / 10;
                ConvertDown(CurrencyCoin.Silver, Math.Max(1L, val2));
            }
            stopwatch.Stop();
            Logger.Warning($"WithdrawUnderCoin {stopwatch.ElapsedMilliseconds}ms");
            Copper -= withdrawAmount;
            CalculationBase -= withdrawAmount;
        }

        private bool ConvertDown(CurrencyCoin coin, long amount)
        {
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    ConvertDown(CurrencyCoin.Silver, 1L);
                    break;
                case CurrencyCoin.Silver:
                    if (Silver > 0 && Withdraw(CurrencyCoin.Silver, amount, withdrawLowerDenomination: false))
                    {
                        Deposit(CurrencyCoin.Copper, amount * 10);
                    }
                    else if (Silver > 0)
                    {
                        long silver = Silver;
                        Withdraw(CurrencyCoin.Silver, silver, withdrawLowerDenomination: false);
                        Deposit(CurrencyCoin.Copper, silver * 10);
                    }
                    else
                    {
                        ConvertDown(CurrencyCoin.Electrum, 1L);
                    }
                    break;
                case CurrencyCoin.Electrum:
                    if (Electrum > 0 && Withdraw(CurrencyCoin.Electrum, amount, withdrawLowerDenomination: false))
                    {
                        Deposit(CurrencyCoin.Silver, amount * 5);
                    }
                    else if (Electrum > 0)
                    {
                        long electrum = Electrum;
                        Withdraw(CurrencyCoin.Electrum, electrum, withdrawLowerDenomination: false);
                        Deposit(CurrencyCoin.Silver, electrum * 5);
                    }
                    else
                    {
                        ConvertDown(CurrencyCoin.Gold, 1L);
                    }
                    break;
                case CurrencyCoin.Gold:
                    if (Gold > 0 && Withdraw(CurrencyCoin.Gold, amount, withdrawLowerDenomination: false))
                    {
                        Deposit(CurrencyCoin.Electrum, amount * 2);
                    }
                    else if (Gold > 0)
                    {
                        long gold = Gold;
                        Withdraw(CurrencyCoin.Gold, gold, withdrawLowerDenomination: false);
                        Deposit(CurrencyCoin.Electrum, gold * 2);
                    }
                    else
                    {
                        ConvertDown(CurrencyCoin.Platinum, 1L);
                    }
                    break;
                case CurrencyCoin.Platinum:
                    if (Platinum > 0 && Withdraw(CurrencyCoin.Platinum, amount, withdrawLowerDenomination: false))
                    {
                        Deposit(CurrencyCoin.Gold, amount * 10);
                        break;
                    }
                    if (Platinum > 0)
                    {
                        long platinum = Platinum;
                        Withdraw(CurrencyCoin.Platinum, platinum, withdrawLowerDenomination: false);
                        Deposit(CurrencyCoin.Gold, platinum * 10);
                        break;
                    }
                    return false;
            }
            return true;
        }

        private void ConvertTo(CurrencyCoin from, CurrencyCoin to)
        {
            if (from == CurrencyCoin.Copper && to == CurrencyCoin.Silver)
            {
                Silver += Copper / 10;
                Copper %= 10L;
            }
            if (from == CurrencyCoin.Silver && to == CurrencyCoin.Electrum)
            {
                ConvertTo(Down(from), from);
                Electrum += Silver / 5;
                Silver %= 5L;
            }
            if (from == CurrencyCoin.Electrum && to == CurrencyCoin.Gold)
            {
                ConvertTo(Down(from), from);
                Gold += Electrum / 2;
                Electrum %= 2L;
            }
            if (from == CurrencyCoin.Gold && to == CurrencyCoin.Platinum)
            {
                ConvertTo(Down(from), from);
                Platinum += Gold / 10;
                Gold %= 10L;
            }
        }

        public void ConvertTo(CurrencyCoin coin)
        {
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    Copper += Silver * 10;
                    Copper += Electrum * 50;
                    Copper += Gold * 100;
                    Copper += Platinum * 1000;
                    Silver = 0L;
                    Electrum = 0L;
                    Gold = 0L;
                    Platinum = 0L;
                    break;
                case CurrencyCoin.Silver:
                    ConvertTo(CurrencyCoin.Copper, CurrencyCoin.Silver);
                    Silver += Electrum * 5;
                    Silver += Gold * 10;
                    Silver += Platinum * 100;
                    Electrum = 0L;
                    Gold = 0L;
                    Platinum = 0L;
                    break;
                case CurrencyCoin.Electrum:
                    ConvertTo(CurrencyCoin.Silver, CurrencyCoin.Electrum);
                    Electrum += Gold * 2;
                    Electrum += Platinum * 20;
                    Gold = 0L;
                    Platinum = 0L;
                    break;
                case CurrencyCoin.Gold:
                    ConvertTo(CurrencyCoin.Electrum, CurrencyCoin.Gold);
                    Gold += Platinum * 10;
                    Platinum = 0L;
                    break;
                case CurrencyCoin.Platinum:
                    ConvertTo(CurrencyCoin.Gold, CurrencyCoin.Platinum);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("coin", coin, null);
            }
        }

        private CurrencyCoin Down(CurrencyCoin coin)
        {
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    return coin;
                case CurrencyCoin.Silver:
                    return CurrencyCoin.Copper;
                case CurrencyCoin.Electrum:
                    return CurrencyCoin.Silver;
                case CurrencyCoin.Gold:
                    return CurrencyCoin.Electrum;
                case CurrencyCoin.Platinum:
                    return CurrencyCoin.Gold;
                default:
                    throw new ArgumentOutOfRangeException("coin", coin, null);
            }
        }

        private CurrencyCoin Up(CurrencyCoin coin)
        {
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    return CurrencyCoin.Silver;
                case CurrencyCoin.Silver:
                    return CurrencyCoin.Electrum;
                case CurrencyCoin.Electrum:
                    return CurrencyCoin.Gold;
                case CurrencyCoin.Gold:
                    return CurrencyCoin.Platinum;
                case CurrencyCoin.Platinum:
                    return CurrencyCoin.Platinum;
                default:
                    throw new ArgumentOutOfRangeException("coin", coin, null);
            }
        }

        private long CalculateBase(CurrencyCoin coin, long amount)
        {
            long result = 0L;
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    result = amount;
                    break;
                case CurrencyCoin.Silver:
                    result = amount * 10;
                    break;
                case CurrencyCoin.Electrum:
                    result = amount * 50;
                    break;
                case CurrencyCoin.Gold:
                    result = amount * 100;
                    break;
                case CurrencyCoin.Platinum:
                    result = amount * 1000;
                    break;
            }
            return result;
        }

        private long CalculateBasedOnRequiredBase(CurrencyCoin coin, long required)
        {
            long result = 0L;
            switch (coin)
            {
                case CurrencyCoin.Copper:
                    result = required / 1;
                    break;
                case CurrencyCoin.Silver:
                    result = required / 10;
                    break;
                case CurrencyCoin.Electrum:
                    result = required / 50;
                    break;
                case CurrencyCoin.Gold:
                    result = required / 100;
                    break;
                case CurrencyCoin.Platinum:
                    result = required / 1000;
                    break;
            }
            return result;
        }

        private bool ValidateBase()
        {
            return _calculationBase == CalculateBase(CurrencyCoin.Copper, Copper) + CalculateBase(CurrencyCoin.Silver, Silver) + CalculateBase(CurrencyCoin.Electrum, Electrum) + CalculateBase(CurrencyCoin.Gold, Gold) + CalculateBase(CurrencyCoin.Platinum, Platinum);
        }

        public override string ToString()
        {
            return $"{Copper}CP {Silver}SP {Electrum}EP {Gold}GP {Platinum}PP ({CalculationBase})";
        }

        public void ConvertToGold()
        {
            ConvertTo(CurrencyCoin.Gold);
        }

        public static CurrencyCoin GetCurrencyCoinFromAbbreviation(string abbreviation)
        {
            if (string.IsNullOrWhiteSpace(abbreviation))
            {
                throw new ArgumentException("currency abbreviation is empty");
            }
            switch (abbreviation.ToLowerInvariant())
            {
                case "cp":
                    return CurrencyCoin.Copper;
                case "sp":
                    return CurrencyCoin.Silver;
                case "ep":
                    return CurrencyCoin.Electrum;
                case "gp":
                    return CurrencyCoin.Gold;
                case "pp":
                    return CurrencyCoin.Platinum;
                default:
                    throw new ArgumentException("currency abbreviation '" + abbreviation + "' doesn't exist");
            }
        }
    }
}
