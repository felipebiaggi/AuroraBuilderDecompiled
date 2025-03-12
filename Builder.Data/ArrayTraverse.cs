using System;

namespace Builder.Data
{
    internal class ArrayTraverse
    {
        public int[] Position;

        private int[] maxLenght;

        public ArrayTraverse(Array array)
        {
            maxLenght = new int[array.Rank];
            for (int i = 0; i < array.Rank; i++)
            {
                maxLenght[i] = array.GetLength(i) - 1;
            }
            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (int i = 0; i < Position.Length; i++)
            {
                if (Position[i] < maxLenght[i])
                {
                    Position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        Position[j] = 0;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
