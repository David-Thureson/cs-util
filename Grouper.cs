using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Grouper<T>
    {
        public string name;
        public SortedDictionary<string, List<T>> map = new();

        public Grouper(string name)
        {
            this.name = name;
        }

        public void Add(string key, T entry)
        {
            if (!map.ContainsKey(key))
            {
                map[key] = new();
            }
            map[key].Add(entry);
        }

        public void PrintByCountDescending(string label = "", int keyLimit = int.MaxValue, bool listEntries = false)
        {
            if (label.Length == 0)
            {
                label = this.name;
            }
            WriteLine($"Util.Grouper: {label}:");
            foreach (KeyValuePair<string, List<T>> kvp in this.map
                    .Take(keyLimit)
                    .OrderByDescending(kvp => kvp.Value.Count)
                    .ThenBy(kvp => kvp.Key))
            {
                string key = kvp.Key;
                int count = kvp.Value.Count;
                WriteLine($"  {count:n}: {key}");
                if (listEntries)
                {
                    kvp.Value.Sort();
                    foreach (T entry in kvp.Value)
                    {
                        WriteLine($"    {entry!.ToString()}");
                    }
                }
            }

            WriteLine();
        }

        public void PrintByKey(string label = "", int keyLimit = int.MaxValue, bool listEntries = false)
        {
            if (label.Length == 0)
            {
                label = this.name;
            }
            WriteLine($"Util.Grouper: {label}:");
            foreach (KeyValuePair<string, List<T>> kvp in this.map
                    .Take(keyLimit)
                    .OrderBy(kvp => kvp.Key))
            {
                string key = kvp.Key;
                int count = kvp.Value.Count;
                WriteLine($"  {key}: {count:n}");
                if (listEntries)
                {
                    kvp.Value.Sort();
                    foreach (T entry in kvp.Value)
                    {
                        WriteLine($"    {entry!.ToString()}");
                    }
                }
            }

            WriteLine();
        }

    }
}
