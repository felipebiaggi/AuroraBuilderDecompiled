using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Builder.Presentation.Services
{
    internal class DiceService
    {
        private const int RandomizeDelay = 50;

        private readonly Random _rnd;

        public DiceService()
        {
            _rnd = new Random();
        }

        public async Task<int> D2(int amount = 1)
        {
            return await RollAsync(2, amount);
        }

        public async Task<int> D3(int amount = 1)
        {
            return await RollAsync(3, amount);
        }

        public async Task<int> D4(int amount = 1)
        {
            return await RollAsync(4, amount);
        }

        public async Task<int> D6(int amount = 1)
        {
            return await RollAsync(6, amount);
        }

        public async Task<int> D8(int amount = 1)
        {
            return await RollAsync(8, amount);
        }

        public async Task<int> D10(int amount = 1)
        {
            return await RollAsync(10, amount);
        }

        public async Task<int> D12(int amount = 1)
        {
            return await RollAsync(12, amount);
        }

        public async Task<int> D20(int amount = 1)
        {
            return await RollAsync(20, amount);
        }

        public async Task<int> D30(int amount = 1)
        {
            return await RollAsync(30, amount);
        }

        public async Task<int> D100(int amount = 1)
        {
            return await RollAsync(100, amount);
        }

        private async Task<int> RollAsync(int sides, int amount = 1)
        {
            int result = 0;
            for (int i = 0; i < amount; i++)
            {
                await Task.Delay(50);
                result += _rnd.Next(sides) + 1;
            }
            return result;
        }

        public async Task<int> RandomizeAbilityScore()
        {
            List<int> results = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                await Task.Delay(50);
                results.Add(_rnd.Next(6) + 1);
            }
            return results.Sum() - results.Min();
        }
    }
}
