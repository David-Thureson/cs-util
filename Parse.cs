using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Util
{
    public class Parse
    {
        // public const string RegexStringEmail = @"[^@\s]+@[^@\s]+\.[^@\s]+";
        // Must have ".com" at the end. This avoids matching to things like "@dataset().file" (with the quotes).
        // public const string RegexStringEmail = @"[^@\s]+@[^@\s]+\.com+";
        public const string RegexStringEmail = @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)";

        // https://stackoverflow.com/questions/5717312/regular-expression-for-url
        public const string RegexStringUrl = @"(http|https|ftp|)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?([a-zA-Z0-9\-\?\,\'\/\+&%\$#_]+)";

        public static string FileName(string path)
        {
            return AfterLastOrValue(path, @"\").Trim();
        }

        public static string FileExtension(string path)
        {
            // Note that this includes the "." at the front, like FileInfo does.
            var fileName = FileName(path);
            var pos = fileName.LastIndexOf('.');
            if (pos == -1)
            {
                // No "." found, so there is no extension.
                return "";
            }
            // The substring we want starts with the period.
            var length = fileName.Length - pos;
            return fileName.Substring(pos, length).Trim();
        }

        public static string AfterLastOrDefault(string value, string delim, string def)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(delim)} is blank."));
            }
            var pos = value.ToLower().LastIndexOf(delim.ToLower());
            if (pos == -1)
            {
                return def;
            }
            var substrPos = pos + delim.Length;
            var length = value.Length - substrPos;
            if (length == 0)
            {
                return "";
            }
            return value.Substring(substrPos, length);
        }

        public static string AfterLastOrValue(string value, string delim)
        {
            return AfterLastOrDefault(value, delim, value);
        }

        public static string AfterLastOrFail(string value, string delim, string? context = null)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(delim)} is blank."));
            }
            string tag = "AfterLastOrFail%$%^&$%&$%";
            string result = AfterLastOrDefault(value, delim, tag);
            if (result == tag)
            {
                throw new ArgumentException(String.Format($"{Program.ContextPrefix(context)}Failed to find a substring: value = \"{value}\"; delim = \"{delim}\""));
            }
            else
            {
                return result;
            }
        }

        public static string AfterOrDefault(string value, string delim, string def)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(delim)} is blank."));
            }
            var pos = value.ToLower().IndexOf(delim.ToLower());
            if (pos == -1)
            {
                return def;
            }
            var substrPos = pos + delim.Length;
            var length = value.Length - substrPos;
            if (length == 0)
            {
                return "";
            }
            return value.Substring(substrPos, length);
        }

        public static string AfterOrValue(string value, string delim)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(delim)} is blank."));
            }
            return AfterOrDefault(value, delim, value);
        }

        public static string AfterOrFail(string value, string delim, string? context = null)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{Program.ContextPrefix(context)}{nameof(delim)} is blank."));
            }
            string tag = "AfterOrFail%$%^&$%&$%";
            string result = AfterOrDefault(value, delim, tag);
            if (result == tag)
            {
                throw new ArgumentException(String.Format($"{Program.ContextPrefix(context)}Failed to find a substring: value = \"{value}\"; delim = \"{delim}\""));
            }
            else
            {
                return result;
            }
        }

        public static string BeforeOrDefault(string value, string delim, string def)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(delim)} is blank."));
            }
            var pos = value.ToLower().IndexOf(delim.ToLower());
            if (pos == -1)
            {
                return def;
            }
            if (pos == 0)
            {
                return "";
            }
            return value.Substring(0, pos);
        }

        public static string BeforeLastOrDefault(string value, string delim, string def)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(delim)} is blank."));
            }
            var pos = value.ToLower().LastIndexOf(delim.ToLower());
            if (pos == -1)
            {
                return def;
            }
            if (pos == 0)
            {
                return "";
            }
            return value.Substring(0, pos);
        }

        public static string BeforeOrFail(string value, string delim, string? context = null)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{Program.ContextPrefix(context)}{nameof(delim)} is blank."));
            }
            string tag = "BeforeOrFail%$%^&$%&$%";
            string result = BeforeOrDefault(value, delim, tag);
            if (result == tag)
            {
                throw new ArgumentException(String.Format($"{Program.ContextPrefix(context)}Failed to find a substring: value = \"{value}\"; delim = \"{delim}\""));
            }
            else
            {
                return result;
            }
        }

        public static string BeforeLastOrFail(string value, string delim, string? context = null)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(delim)} is blank."));
            }
            string tag = "BeforeLastOrFail%$%^&$%&$%";
            string result = BeforeLastOrDefault(value, delim, tag);
            if (result == tag)
            {
                throw new ArgumentException(String.Format($"Failed to find a substring: value = \"{value}\"; delim = \"{delim}\""));
            }
            else
            {
                return result;
            }
        }

        public static string BeforeOrValue(string value, string delim, string? context = null)
        {
            if (delim.Length == 0)
            {
                throw new ArgumentException(String.Format($"{Program.ContextPrefix(context)}{nameof(delim)} is blank."));
            }
            return BeforeOrDefault(value, delim, value);
        }

        private static string BetweenOrDefault(string value, string prefix, string suffix, string deflt, bool lastSuffix)
        {
            string valueLower = value.ToLower();
            if (prefix.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(prefix)} is blank."));
            }
            if (suffix.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(suffix)} is blank."));
            }
            var posPrefix = valueLower.IndexOf(prefix.ToLower());
            if (posPrefix == -1)
            {
                return deflt;
            }
            var posAfterPrefix = posPrefix + prefix.Length;
            if (posAfterPrefix >= value.Length)
            {
                return deflt;
            }
            var posSuffix = lastSuffix ? valueLower.LastIndexOf(suffix.ToLower()) : valueLower.IndexOf(suffix.ToLower(), posAfterPrefix);
            if (posSuffix == -1 || posSuffix <= posPrefix)
            {
                // The suffix was not found anywhere after the prefix.
                return deflt;
            }
            if (posSuffix == posAfterPrefix)
            {
                // The suffix is right after the prefix.
                return "";
            }
            var betweenLength = posSuffix - posAfterPrefix;
            return value.Substring(posAfterPrefix, betweenLength);
        }

        public static string BetweenOrDefault(string value, string prefix, string suffix, string deflt)
        {
            return BetweenOrDefault(value, prefix, suffix, deflt, false);
        }

        public static string BetweenLastOrDefault(string value, string prefix, string suffix, string deflt)
        {
            return BetweenOrDefault(value, prefix, suffix, deflt, true);
        }

        private static string BetweenOrFail(string value, string prefix, string suffix, bool lastSuffix, string? context = null)
        {
            string tag = "BetweenOrFail%$%^&$%&$%";
            string result = BetweenOrDefault(value, prefix, suffix, tag, lastSuffix);
            if (result == tag)
            {
                throw new ArgumentException(String.Format($"{Program.ContextPrefix(context)}Failed to find a substring: value = \"{value}\"; prefix = \"{prefix}\"; suffix = \"{suffix}\""));
            }
            else
            {
                return result;
            }
        }

        public static string BetweenOrFail(string value, string prefix, string suffix, string? context = null)
        {
            return BetweenOrFail(value, prefix, suffix, false, context);
        }

        public static string BetweenLastOrFail(string value, string prefix, string suffix, string? context = null)
        {
            return BetweenOrFail(value, prefix, suffix, true, context);
        }

        public static ulong? NumberBetweenULong(string value, string prefix, string suffix)
        {
            if (prefix.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(prefix)} is blank."));
            }
            if (suffix.Length == 0)
            {
                throw new ArgumentException(String.Format($"{nameof(suffix)} is blank."));
            }
            string between = BetweenOrDefault(value, prefix, suffix, "").Trim();
            if (ulong.TryParse(between, out ulong result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static string RemoveRepeated(string value, string substring)
        {
            string substring_doubled = substring + substring;
            while (true)
            {
                int length = value.Length;
                value = value.Replace(substring_doubled, substring);
                if (value.Length == length)
                {
                    return value;
                }
            }
            throw new Exception("Should never be reached.");
        }

        public static string RemoveStart(string value, string substring)
        {
            while (value.StartsWith(substring))
            {
                value = value.Substring(substring.Length, value.Length - substring.Length);
            }
            return value;
        }

        public static string RemoveEnd(string value, string substring)
        {
            while (value.EndsWith(substring))
            {
                value = value.Substring(0, value.Length - substring.Length);
            }
            return value;
        }

        public static string RemoveStartAndEnd(string value, string startSubstring, string? endSubstring = null)
        {
            string effectiveEndSubstring = endSubstring ?? startSubstring;
            return RemoveEnd(RemoveStart(value, startSubstring), effectiveEndSubstring);
        }

        public static string RemoveSquareBrackets(string value)
        {
            return RemoveStartAndEnd(value, "[", "]");
        }

        public static string RelativePath(string fullFileName, string basePath, string? context = null)
        {
            return AfterOrFail(fullFileName, basePath, context).TrimStart('\\').TrimStart('/');
        }

        public static (string, string?) SplitOneOrTwo(string value, string delimiter)
        {
            if (value.Contains(delimiter, StringComparison.InvariantCultureIgnoreCase))
            {
                string[] splits = value.Split(delimiter, 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length == 1)
                {
                    return (splits[0], null);
                }
                else
                {
                    string? secondValue = splits[1].Length == 0 ? null : splits[1];
                    return (splits[0], secondValue);
                }
            }
            else
            {
                return (value.Trim(), null);
            }
        }

        public static List<(int, string, bool)> SplitByRegex(string text, Regex regex)
        {
            MatchCollection matches = regex.Matches(text);
            WriteLine($"  matches.Count = {matches.Count:n}");
            List<(int, string, bool)> list = new();
            if (matches.Count == 0)
            {
                list.Add((0, text, false));
                return list;
            }
            int index = 0;
            foreach (Match oneMatch in matches.Cast<Match>())
            {
                int matchIndex = oneMatch.Index;
                int nonMatchLength = matchIndex - index;
                if (nonMatchLength > 0)
                {
                    list.Add((index, text.Substring(index, nonMatchLength), false));
                }
                list.Add((matchIndex, oneMatch.Value, true));
                index = matchIndex + oneMatch.Value.Length;
            }
            int remainingLength = text.Length - index;
            if (remainingLength > 0)
            {
                list.Add((index, text.Substring(index, remainingLength), false));
            }
            return list;
        }

        public static Regex RegexEmail(bool ignoreCase = true)
        {
            RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            return new Regex(RegexStringEmail, options);
        }

        public static Regex RegexUrl(bool ignoreCase = true)
        {
            RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            return new Regex(RegexStringUrl, options);
        }

        public static Regex RegexEmailOrUrl(bool ignoreCase = true, bool email = true, bool url = true)
        {
            Debug.Assert(email || url);
            string regexString;
            if (email && url)
            {
                regexString = "(" + RegexStringEmail + ")|(" + RegexStringUrl + ")";
            }
            else if (email)
            {
                regexString = RegexStringEmail;
            }
            else
            {
                regexString = RegexStringUrl;
            }
            RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            return new Regex(regexString, options);
        }

        public static int? ParseSqlMetadataInt(string value)
        {
            value = value.Trim().ToLower();
            if (value.Length == 0 || value.Equals("null"))
            {
                return null;
            }
            else
            {
                return int.Parse(value);
            }
        }

        public static int? ParseCellInt(string value)
        {
            value = value.Trim().ToLower();
            if (value.Length == 0 || value == "null")
            {
                return null;
            }
            return int.Parse(value);
        }

        public static bool? ParseCellBool(string value)
        {
            value = value.Trim().ToLower();
            if (value.StartsWith("t") || value.StartsWith("y") || value.StartsWith("1"))
            {
                return true;
            }
            if (value.StartsWith("f") || value.StartsWith("n") || value.StartsWith("0"))
            {
                return false;
            }
            return null;
        }

        public static DateTime? ParseCellDateTime(string value)
        {
            value = value.Trim().ToLower();
            if (value.Length == 0 || value == "null")
            {
                return null;
            }
            return DateTime.Parse(value);
        }

        public static string UnDelimit(string value, char startDelimiter, char endDelimiter)
        {
            string v = value.Trim();
            v = v.TrimStart(startDelimiter);
            v = v.TrimEnd(endDelimiter);
            return v.Trim();
        }

        public static string UnQuote(string value)
        {
            return UnDelimit(UnDelimit(value, '"', '"'), '\'', '\'');
        }

        public static string UnBracket(string value)
        {
            return UnDelimit(value, '[', ']');
        }

        public static string? UnDelimitNullable(string? value, char startDelimiter, char endDelimiter)
        {
            if (value == null)
            {
                return null;
            }
            string v = value.Trim();
            v = v.TrimStart(startDelimiter);
            v = v.TrimEnd(endDelimiter);
            return v.Trim();
        }

        public static string? UnQuoteNullable(string? value)
        {
            return UnDelimitNullable(UnDelimitNullable(value, '"', '"'), '\'', '\'');
        }

        public static string? UnBracketNullable(string? value)
        {
            return UnDelimitNullable(value, '[', ']');
        }

        public static List<string> ToLines(string text)
        {
            List<string> lines = new();
            using (StringReader sr = new StringReader(text))
            {
                string line;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            return lines;
        }

        public static string? BlankToNullTrim(string value)
        {
            if (value == null) { return null; }
            string trimValue = value.Trim();
            if (trimValue.Length == 0) { return null; }
            return trimValue;
        }

        public static List<string> ToDelimitedBlocks(string text, List<(string delimStart, string delimEnd)> delimiters)
        {
            Debug.Assert(delimiters.Count > 0);
            string delimStart = delimiters[0].delimStart;
            string delimEnd = delimiters[0].delimEnd;
            if (delimiters.Count == 1)
            {
                return ToDelimitedBlocks(text, delimStart, delimEnd);
            }
            else
            {
                List<string> blocks = new();
                foreach (string block in ToDelimitedBlocks(text, delimStart, delimEnd))
                {
                    if (block.StartsWith(delimStart))
                    {
                        blocks.Add(block);
                    }
                    else
                    {
                        foreach (string nextBlock in ToDelimitedBlocks(block, delimiters.Skip(1).ToList()))
                        {
                            blocks.Add(nextBlock);
                        }
                    }
                }
                AssertTextBlocks(text, blocks);
                return blocks;
            }
        }


        public static List<string> ToDelimitedBlocks(string text, string delimStart, string delimEnd)
        {
            List<string> blocks = new();
            int posText = 0;
            while (posText < text.Length)
            {
                int posDelimStart = text.IndexOf(delimStart, posText);

                if (posDelimStart == -1)
                {
                    blocks.Add(text.Substring(posText, text.Length - posText));
                    break;
                }
                else
                {
                    int posDelimEnd = text.IndexOf(delimEnd, posDelimStart + 1);
                    Debug.Assert(posDelimEnd > posDelimStart);
                    if (posDelimStart > 0)
                    {
                        blocks.Add(text.Substring(posText, posDelimStart - posText));
                    }
                    blocks.Add(text.Substring(posDelimStart, (posDelimEnd - posDelimStart) + delimEnd.Length));
                    posText = posDelimEnd + delimEnd.Length;
                }
            }
            AssertTextBlocks(text, blocks);
            return blocks;
        }

        private static void AssertTextBlocks(string text, List<string> blocks)
        {
            // Reconstruct the original text as a test.
            StringBuilder sb = new();
            foreach (string block in blocks)
            {
                sb.Append(block);
            }
            string rebuiltText = sb.ToString();
            Debug.Assert(rebuiltText == text);
        }

        public static bool IsLetters(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsDigits(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsLettersAndDigits(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsUpperAndDigits(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsUpper(c) && !char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsAscii(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsAscii(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsFirstCharUpper(string value)
        {
            if (value.Length == 0)
            {
                return false;
            }
            return char.IsUpper(value[0]);
        }

        public static bool IsLastCharDigit(string value)
        {
            if (value.Length == 0)
            {
                return false;
            }
            return char.IsDigit(value[value.Length - 1]);
        }

        public static List<string> SplitOnCapsAndNumbers(string value)
        {
            List<(int, int)> pos = new();
            int i = 0;
            string prevCharClass = "";
            string thisCharClass = "";
            int currentStart = 0;
            int currentLength = 0;
            foreach (char c in value)
            {
                bool newSplit = false;
                thisCharClass = char.IsUpper(c)
                    ? "u"
                    : char.IsNumber(c)
                        ? "n"
                        : "l";
                if (thisCharClass == prevCharClass)
                {
                    if (thisCharClass == "u" && i < value.Length - 1)
                    {
                        // Preview the next character.
                        char nextChar = value[i + 1];
                        if (char.IsLower(nextChar))
                        {
                            // We're starting a new split of an uppercase character
                            // followed by a lowercase character.
                            newSplit = true;
                        }
                    }
                }
                else
                {
                    // The character class has changed. This is a new split except when the
                    // previous char was uppercase and started a split and the new char is lowercase.
                    if (!(currentLength == 1 && prevCharClass == "u" && thisCharClass == "l"))
                    {
                        newSplit = true;
                    }
                }
                if (newSplit)
                {
                    if (currentLength > 0)
                    {
                        pos.Add((currentStart, currentLength));
                    }
                    currentStart = i;
                    currentLength = 1;
                }
                else
                {
                    currentLength++;
                }
                prevCharClass = thisCharClass;
                i++;
            }
            pos.Add((currentStart, currentLength));

            List<string> splits = new();
            foreach (var entry in pos)
            {
                splits.Add(value.Substring(entry.Item1, entry.Item2));
            }
            return splits;
        }

        public static List<List<string>> GroupByBlankLines(string text)
        {
            List<List<string>> groups = new();
            string workingText = text.Replace("\r\n", "\n");
            string[] splits = workingText.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (string split in splits)
            {
                groups.Add(ToLines(split));
            }
            return groups;
        }


        internal static void Test()
        {
            TestSplitOnCapsAndNumbers();
            // TestRemoveStartAndEnd();
        }


        private static void TestSplitOnCapsAndNumbers()
        {
            // TestOneSplitOnCapsAndNumbers("CustomerName", 2);
            // TestOneSplitOnCapsAndNumbers("Customer123Name45", 4);
            TestOneSplitOnCapsAndNumbers("USDOTNumberAIntID", 5);
        }

        private static void TestOneSplitOnCapsAndNumbers(string value, int expCount)
        {
            List<string> splits = SplitOnCapsAndNumbers(value);
            WriteLine($"\nTestOneSplitOnCapsAndNumbers(\"{value}\", {expCount}):");
            foreach (string split in splits)
            {
                WriteLine($"\t\"{split}\"");
            }
            Debug.Assert(splits.Count == expCount);
            string reassembled = String.Join("", splits);
            Debug.Assert(reassembled == value);
        }

        private static void TestRemoveStartAndEnd()
        {
            string result = RemoveStartAndEnd("| Accelerator Framework | [[tools:profisee_software_development_kit_sdk|topic]] | | pr |", "|");
            Debug.Assert(result == " Accelerator Framework | [[tools:profisee_software_development_kit_sdk|topic]] | | pr ");
            //                                 " Accelerator Framework | [[tools:profisee_software_development_kit_sdk|topic]] | | pr "
        }

    }
}


/*
List<string> colLines = new();
foreach (CubeColumn col in tbl.columns)
{
    List<string> refs = new();
    foreach (CubeRelationship rel in cubeDef.relationships.Where(rel => rel.toTable.Equals(tbl.name) && rel.toColumn.Equals(col.name)))
    {
        refs.Add(String.Format($"[[#{rel.fromTable}]].{rel.fromColumn}"));
    }
    string refString = "";
    if (refs.Count > 0)
    {
        refString = String.Format($" (referenced by {String.Join(", ", refs)})");
    }
    string sourceProviderType = col.sourceProviderType.Length == 0 ? "" : " [" + col.sourceProviderType + "]";
    string displayFolder = col.displayFolder.Length == 0 ? "" : "; display folder: " + col.displayFolder;
    string colLine = String.Format($"{col.name} [{col.dataType}]; source: {col.sourceColumn}{sourceProviderType}{displayFolder}{refString}");
    colLines.Add(colLine);
}
page.AddList("Columns:", colLines, true);
*/
/*
List<string> relLines = new();
// Inbound.
foreach (CubeRelationship rel in cubeDef.relationships.Where(rel => rel.toTable.Equals(tbl.name)))
{
    string relLine = String.Format($"[[#{rel.fromTable}]].{rel.fromColumn} -> {rel.toTable}.{rel.toColumn}");
    relLines.Add(relLine);
}
// Outbound.
foreach (CubeRelationship rel in cubeDef.relationships.Where(rel => rel.fromTable.Equals(tbl.name)))
{
    string relLine = String.Format($"{rel.fromTable}.{rel.fromColumn} -> [[#{rel.toTable}]].{rel.toColumn}");
    relLines.Add(relLine);
}
page.AddList("Relationships:", relLines, true);
*/
