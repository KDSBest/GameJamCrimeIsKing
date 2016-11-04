using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public static class RandomHelper
    {
        private static Random Randomizer = new Random();

        public static int Generate(MinMaxValue minMax)
        {
            return Randomizer.Next(minMax.Min, minMax.Max);
        }

        public static bool GenerateChance(int percentage)
        {
            return Randomizer.Next(1, 100) <= percentage;
        }

        public static int Generate(int min, int max)
        {
            return Randomizer.Next(min, max);
        }

        public static T RandomSelect<T>(List<T> values)
        {
            return values[RandomHelper.Generate(0, values.Count - 1)];
        }

        public static int RandomGewichtung(List<int> gewichtung)
        {
            int maxGeneration = gewichtung.Select(x => x < 0 ? 0 : x).Aggregate((x1, x2) => x1 + x2);
            int selected = Generate(1, maxGeneration);
            for (int i = 0; i < gewichtung.Count; i++)
            {
                selected -= gewichtung[i];
                if (selected <= 0)
                {
                    return i;
                }
            }

            return 0;
        }

        public static T RandomSelectGewichtung<T>(List<T> values, List<int> gewichtung)
        {
            int selected = RandomGewichtung(gewichtung);
            return values[selected];
        }
    }
}
