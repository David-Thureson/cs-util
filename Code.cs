using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public abstract class Code
    {
        private const string projectPathWikiCsharp = @"C:\Projects\CSharp\Utility\Wiki";

        internal static void Go()
        {
            // FormatCSharpListInitializations(projectPathWikiCsharp);
            GenCSharpSqlToGenCode(@"C:\Projects\SqlServer\MP\Example Merge Procedure Stage to Raw.sql", @"M:\CSharp Gen Merge Stage to Raw 2023-11-24.txt");
        }

        public static void GenCSharpSqlToGenCode(string fullFileNameIn, string fullFileNameOut)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string line in File.ReadLines(fullFileNameIn))
            {
                if (line.Trim().Length == 0)
                {
                    sb.AppendLine("\t\t\tsb.AppendLine();");
                }
                else
                {
                    sb.AppendLine($"\t\t\tsb.AppendLine(\"{line.TrimEnd().Replace("\t", @"\t")}\");");
                }
            }

            WriteLine($"Writing to \"{fullFileNameOut}\".");
            File.WriteAllText(fullFileNameOut, sb.ToString());
        }

        public static void GenCSharpColumnIndexConstants(string prefix, string headers)
        {
            string[] splits = headers.Split('\t', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            WriteLine();
            int index = -1;
            foreach (string header in splits)
            {
                index++;
                WriteLine($"{prefix}{header} = {index};");
            }
        }

        internal static void FormatCSharpListInitializations(string projectPath)
        {
            foreach (string fileName in Directory.EnumerateFiles(projectPath, "*.cs", SearchOption.AllDirectories))
            {
                string[] lines = File.ReadAllLines(fileName);
                // The first line of the block will be something like:
                //   List<string> includeNonTreeTopics = new() { "abc", "def",
                int lineIndex = 0;
                bool blockFoundThisFile = false;
                while (lineIndex < lines.Length)
                {
                    string line = lines[lineIndex];
                    if (line.Trim().StartsWith("List<string> ") && line.Contains("new() {"))
                    {
                        if (!blockFoundThisFile)
                        {
                            string shortFileName = Parse.AfterOrFail(fileName, projectPath);
                            WriteLine($"\n******************************\n******************************\n******************************\n{shortFileName}\n");
                        }
                        string declaration = $"{Parse.BeforeOrFail(line, "{")} {{";
                        WriteLine(declaration);
                        List<string> valueLineParts = new();
                        string valuePart = Parse.AfterOrFail(line, "new() {").Trim();
                        valueLineParts.Add(valuePart);
                        lineIndex++;
                        if (lineIndex < lines.Length)
                        {
                            do
                            {
                                line = lines[lineIndex];
                                valuePart = Parse.BeforeOrValue(line, "};").Trim();
                                valueLineParts.Add(valuePart);
                                WriteLine(valuePart);
                                lineIndex++;
                            } while (lineIndex < lines.Length && !line.Trim().EndsWith("};"));
                        }
                    }
                    else
                    {
                        lineIndex++;
                    }
                }
            }


        }

    }
}
