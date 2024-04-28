using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public abstract class Collection
    {
        public static void PrintCompareLists<T>(List<T> list_a, List<T> list_b, string label_a, string label_b, string label_entries = "entries")
        {
            PrintListExceptList(list_a, list_b, label_a, label_b, label_entries);
            PrintListExceptList(list_b, list_a, label_b, label_a, label_entries);
        }

        static void PrintListExceptList<T>(List<T> list_a, List<T> list_b, string label_a, string label_b, string label_entries)
        {
            Debug.Assert(list_a.Count > 0);
            List<T> inANotB = list_a.Except(list_b).ToList();
            if (inANotB.Count > 0)
            {
                WriteLine($"\n{Format.TitleCase(label_entries)} in {label_a} but not {label_b}:");
                foreach (T entry in inANotB)
                {
                    WriteLine($"\t{entry}");
                }
            }
            else
            {
                WriteLine($"\nAll {label_entries} in {label_a} are in {label_b}.");
            }
        }
    }
}
