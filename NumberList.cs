using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class NumberList
    {
        public static List<int> IntProgressionMult(int minValue, int maxValue, double mult)
        {
            Debug.Assert(mult > 0.0, $"mult = {mult}; should be positive");
            Debug.Assert(minValue > 0, $"minValue = {minValue:n}; should be positive");
            Debug.Assert(maxValue > 0, $"maxValue = {maxValue:n}; should be positive");
            Debug.Assert(minValue <= maxValue, $"minValue = {minValue:n}; maxValue = {maxValue:n}; maxValue should be >= minValue");

            List<int> list = new();

            double value = (double)minValue;

            do
            {
                try
                {
                    checked
                    {
                        int intValue = (int)value;
                        if (intValue > maxValue)
                        {
                            break;
                        }
                        list.Add(intValue);
                    }
                }
                catch (OverflowException)
                {
                    break;
                }

                value *= mult;

            } while (true);

            return list;

        }

        public static List<int> IntProgressionAdd(int minValue, int maxValue, int increment)
        {
            Debug.Assert(minValue > 0, $"minValue = {minValue:n}; should be positive");
            Debug.Assert(maxValue > 0, $"maxValue = {maxValue:n}; should be positive");
            Debug.Assert(minValue <= maxValue, $"minValue = {minValue:n}; maxValue = {maxValue:n}; maxValue should be >= minValue");
            Debug.Assert(increment > 0, $"increment = {increment:n}; should be positive");

            List<int> list = new();

            int value = minValue;

            do
            {
                list.Add(value);

                try
                {
                    checked
                    {
                        value += increment;
                    }
                }
                catch (OverflowException)
                {
                    break;
                }

                if (value > maxValue)
                {
                    break;
                }

            } while (true);

            return list;
        }

        internal static void TryIntProgressionMult()
        {
            int minValue = 1;
            int maxValue = int.MaxValue;
            double mult = 2.0;
            List<int> list = IntProgressionMult(minValue, maxValue, mult);
            Print(list, $"TryIntProgressionMult(): minValue = {minValue:n}; maxValue = {maxValue:n}; mult = {mult}");
        }
        internal static void TryIntProgressionAdd()
        {
            // int minValue = 10;
            // int maxValue = 125;
            // int increment = 10;
            int maxOverTen = int.MaxValue / 10;
            int minValue = maxOverTen;
            int maxValue = int.MaxValue;
            int increment = maxOverTen * 2;
            List<int> list = IntProgressionAdd(minValue, maxValue, increment);
            Print(list, $"TryIntProgressionAdd(): minValue = {minValue:n}; maxValue = {maxValue:n}; increment = {increment}");
        }

        public static void Print(List<int> list, string? label)
        {
            if (label != null)
            {
                WriteLine("\n{label}:");
            }
            for (int i = 0; i < list.Count; i++)
            {
                WriteLine($"\t[{i}]\t{list[i]:n}");
            }
        }
    }
}
