using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    internal enum CaseStyle
    {
        Original,
        TitleCase
    }

    internal class SQL
    {
        const string PathBuildDataSurvey02 = @"C:\Projects\SqlServer\Data Survey Data Factory 2023\Version 02";
        const string PathCreateTable = @"C:\Temp";
        const string FileNameInCreateTable = "SQL Create Table for Formatting.txt";
        const string FileNameBuildOrderDataSurvey02 = "Build Order.txt";
        const string FileNameOutDataSurvey02 = "Build All.sql";

        internal static void Go()
        {
            // CreateBuildScript(PathBuildDataSurvey02, FileNameBuildOrderDataSurvey02, FileNameOutDataSurvey02);
            // FormatCreateTable(CaseStyle.TitleCase, true);
            FormatCreateTable(CaseStyle.Original, true);
        }

        internal static void FormatCreateTable(CaseStyle caseStyle, bool keepUnderscores = false)
        {
            ColumnFormatter colFmt = new();
            bool foundCreateTableLine = false;
            bool foundEndOfTable = false;
            List<string> linesAfterTable = new();
            string? schemaName = null;
            string? tableName = null;
            foreach (string rawLine in File.ReadLines(Path.Combine(PathCreateTable, FileNameInCreateTable))) 
            {
                string line = rawLine.Trim().ToLower();
                if (!foundCreateTableLine)
                {
                    if (line.StartsWith("create table"))
                    {
                        foundCreateTableLine = true;
                        string s = Parse.AfterOrFail(rawLine, "CREATE TABLE ").Replace("(", "");
                        string[] splits = s.Split('.', StringSplitOptions.TrimEntries);
                        Debug.Assert(splits.Length == 2);
                        schemaName = Parse.RemoveSquareBrackets(splits[0].Trim());
                        tableName = Parse.RemoveSquareBrackets(splits[1].Trim());
                    }
                }
                else if (!foundEndOfTable)
                {
                    if (line.StartsWith(")") || line.StartsWith("primary key"))
                    {
                        foundEndOfTable = true;
                        linesAfterTable.Add(rawLine);
                    }
                    else
                    {
                        string[] colSplits = rawLine.Split(" ", 3, StringSplitOptions.TrimEntries);
                        Debug.Assert(colSplits.Length == 3);
                        string colName = $"[{Parse.RemoveSquareBrackets(colSplits[0])}]";
                        switch (caseStyle)
                        {
                            case CaseStyle.Original:
                                // Don't change the column name.
                                break;
                            case CaseStyle.TitleCase:
                                colName = Format.SnakeCaseToTitleCase(colName, keepUnderscores);
                                break;
                            default:
                                throw new ArgumentException($"Unexpected caseStyle == \"{caseStyle}\".");
                        }
                        string colType = colSplits[1].Replace("[", "").Replace("]", "");
                        string colThirdPart = colSplits[2].ToLower();
                        colFmt.AddRow(colName, colType, colThirdPart);
                    }
                }
                else
                {
                    if (!line.Equals("GO", StringComparison.InvariantCultureIgnoreCase))
                    {
                        linesAfterTable.Add(rawLine);
                    }
                }
            }

            StringBuilder sb = new();
            sb.Append(SectionSeparator(1, $"Table: {schemaName}.{tableName}"));
            sb.AppendLine($"create table [{schemaName!}].[{tableName!}] (");
            colFmt.AddAlignedValuesSql(sb, 1);
            sb.AppendLine(");\n");
            foreach (string line in linesAfterTable)
            {
                sb.AppendLine(line);
            }
            sb.AppendLine("go");

            WriteLine();
            WriteLine(sb);
        }

        public static void CreateBuildScript(string path, string fileNameBuildOrder, string fileNameOut)
        {
            Debug.Assert(fileNameBuildOrder != fileNameOut);

            StringBuilder sb = new();

            foreach (string rawLine in File.ReadAllLines(Path.Combine(path, fileNameBuildOrder)))
            {
                string buildScriptFileName = rawLine.Trim();
                if (buildScriptFileName.Length > 0)
                {
                    sb.Append(SectionSeparator(0));
                    // The "go" at the beginning of the section for this script makes sure it runs in a new batch.
                    // This is needed for creating things like schemas and views.
                    sb.Append("go\n\n");
                    sb.Append(File.ReadAllText(Path.Combine(path, buildScriptFileName)));
                }
            }

            string fullFileNameOut = FileName.MakeFileNameLetters(path, fileNameOut, true);
            WriteLine(fullFileNameOut);

            File.WriteAllText(fullFileNameOut, sb.ToString());
        }

        public static string SectionSeparator(int level, string? name = null)
        {
            string divider = $"-- {Format.Repeat("=", 117)}";
            StringBuilder sb = new();
            switch (level)
            {
                case 0:
                    sb.AppendLine($"{divider}\n{divider}\n{divider}\n");
                    break;
                case 1:
                    sb.AppendLine($"{divider}\n");
                    break;
                default:
                    sb.AppendLine($"-- {Format.Repeat("-", 117)}\n");
                    break;
            }

            if (name != null)
            {
                sb.AppendLine($"-- {name}\n");
            }

            return sb.ToString();
        }


    }
}
