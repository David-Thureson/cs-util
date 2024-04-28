using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Secret
    {
        private const string FOLDER = "W:";
        private const string FILE_ENDING = " Secrets.txt";

        public static string Get(string projectKey, string key)
        {
            key = key.Trim().ToLower();
            string path = FOLDER + projectKey + FILE_ENDING;
            foreach (string line in System.IO.File.ReadLines(path))
            {
                string lineTrim = line.Trim();
                if (lineTrim.Length > 0 && !lineTrim.StartsWith("//"))
                {
                    string[] splits = lineTrim.Split('\t');
                    Debug.Assert(splits.Length == 2);
                    if (splits[0].Trim().ToLower().Equals(key))
                    {
                        return splits[1].Trim();
                    }
                }
            }
            throw new Exception($"Secret not found: projectKey = \"{projectKey}\", key = \"{key}\".");
        }
    }
}
