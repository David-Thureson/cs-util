using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Util;
public static class Format
{
    public const string I1 = "  ";
    public const string I2 = "    ";
    public const string I3 = "      ";
    public const string I4 = "        ";

    internal static void Try()
    {
        // TryTitleCase();
        TrySnakeCaseToTitleCase();
    }

    public static string ElapsedSeconds(Stopwatch st)
    {
        st.Stop();
        // var a = st.Elapsed.TotalSeconds / 1_000.0;
        // var b = $"{a:n3} seconds";
        return String.Format($"{st.Elapsed.TotalSeconds:n6} seconds");
    }

    public static void PrintElapsedSeconds(byte depth, Stopwatch st, string label)
    {
        st.Stop();
        WriteLine($"{Indent(depth)}{label}: {ElapsedSeconds(st)}");
    }

    public static string ElapsedMsec(Stopwatch st)
    {
        st.Stop();
        return String.Format($"{st.Elapsed.TotalMilliseconds:n6} msec");
    }

    public static void PrintElapsedMsec(int depth, Stopwatch st, string label)
    {
        st.Stop();
        WriteLine($"{Indent(depth)}{label}: {ElapsedMsec(st)}");
    }

    public static string Indent(int depth) => depth switch
    {
        0 => "",
        1 => "  ",
        2 => "    ",
        3 => "      ",
        4 => "        ",
        5 => "          ",
        _ => Repeat("  ", depth)
    };

    public static string I(int depth) => Indent(depth);

    public static string PadZero(int value, int length)
    {
        return value.ToString($"D{length}");
    }

    public static void WriteLineIndent(int depth, string line)
    {
        WriteLine($"{Indent(depth)}{line}");
    }

    public static string Repeat(string value, int count)
    {
        // if (value is null)
        //
        //{
        //    throw new ArgumentNullException(nameof(value));
        //}
        StringBuilder sb = new();
        for (int i = 0; i < count; i++)
        {
            sb.Append(value);
        }
        return sb.ToString();
    }

    public static string Truncate(string text, int maxLength)
    {
        if (text.Length <= maxLength)
        {
            return text;
        }
        else
        {
            return text.Substring(0, maxLength);
        }
    }

    public static string FormatEmbeddedSql(string sql)
    {
        sql = sql.TrimStart('"').TrimEnd('"');
        //riteLine("\n" + sql);
        // sql = Util.Parse.AfterOrValue(sql, @"\u0022");
        // sql = Util.Parse.AfterOrValue(sql, @"\u0022");
        // sql = sql.Replace(@"\u0027", "\"");
        // WriteLine(sql);
        // Debug.Assert(!sql.Contains("\\u"));

        BasicSQLFormatter.SQLFormatter fmt = new(sql);
        sql = fmt.Format();
        //riteLine(sql);

        return sql;
    }

    public static string CleanFileName(string value)
    {
        StringBuilder sb = new();

        foreach (char c in value.Trim())
        {
            sb.Append(char.IsLetterOrDigit(c)
                    || c == '.'
                    || c == '-'
                    || c == ' '
                ? c : '_');
        }

        return Parse.RemoveRepeated(sb.ToString(), "_");
    }

    public static string TitleCase(string value)
    {
        List<string> acronyms = new() { "ADF", "ID", "URL" };
        string val = value.Trim();
        if (acronyms.Contains(val.ToUpper())) {
            return val.ToUpper();
        }
        else
        {
            if (val.Length < 2)
            {
                return val.ToUpper();
            }
            else
            {
                return val.Substring(0, 1).ToUpper() + val.Substring(1, val.Length - 1).ToLower();
            }
        }
    }

    public static string SnakeCaseToTitleCase(string value, bool keepUnderscores = false)
    {
        string[] splits = value.Split('_', StringSplitOptions.TrimEntries);
        string joinDelimiter = keepUnderscores ? "_" : "";
        string newValue = String.Join(joinDelimiter, splits.Select(s => TitleCase(s)));
        return newValue;
    }

    internal static void TryTitleCase()
    {
        List<string> values = new() { "", "id", "acct ", "   url", "a", "  env  ", "URL", "A", "Env" };
        WriteLine("\nTryTitleCase():");
        foreach (string s in values)
        {
            WriteLine($"\t\"{s}\" => \"{TitleCase(s)}\"");
        }
    }

    internal static void TrySnakeCaseToTitleCase()
    {
        List<string> values = new() { "", "id", "acct ", "   url", "a", "  env  ", "URL", "A", "Env", "acct_id", " row_count_pct_fmt  ", "inner_url_id_count" };
        WriteLine("\nTryTitleCase():");
        foreach (string s in values)
        {
            WriteLine($"\t\"{s}\" => \"{SnakeCaseToTitleCase(s)}\"");
        }
    }

    public static string RemoveEnding(string value, string matchEnding, bool ignoreCase, string? context = null)
    {
        Debug.Assert(value != null, $"{Program.ContextPrefix(context)}Value is null.");
        Debug.Assert(matchEnding != null, $"{Program.ContextPrefix(context)}Match ending is null.");
        Debug.Assert(matchEnding.Length > 0, $"{Program.ContextPrefix(context)}Match ending is blank.");

        StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
        if (value.Length == 0 || !value.EndsWith(matchEnding, comparison))
        {
            return value;
        }
        return value.Substring(0, value.Length - matchEnding.Length);
    }

    public static string RemoveEndingOrFail(string value, string matchEnding, bool ignoreCase, string? context = null)
    {
        Debug.Assert(value != null, $"{Program.ContextPrefix(context)}Value is null.");
        Debug.Assert(value.Length > 0, $"{Program.ContextPrefix(context)}Value is blank.");
        Debug.Assert(matchEnding != null, $"{Program.ContextPrefix(context)}Match ending is null.");
        Debug.Assert(matchEnding.Length > 0, $"{Program.ContextPrefix(context)}Match ending is blank.");
        StringComparison comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
        Debug.Assert(value.EndsWith(matchEnding, comparison), $"{Program.ContextPrefix(context)}Value \"{value}\" does not end with \"{matchEnding} (ignoreCase = {ignoreCase}).");

        return value.Substring(0, value.Length - matchEnding.Length);
    }

    public static string ReplaceEnding(string value, string matchEnding, string newEnding, bool ignoreCase, string? context = null)
    {
        Debug.Assert(value != null, $"{Program.ContextPrefix(context)}Value is null.");
        Debug.Assert(matchEnding != null, $"{Program.ContextPrefix(context)}Match ending is null.");
        Debug.Assert(matchEnding.Length > 0, $"{Program.ContextPrefix(context)}Match ending is blank.");
        Debug.Assert(newEnding != null, $"{Program.ContextPrefix(context)}New ending is null.");

        string endingRemoved = RemoveEnding(value, matchEnding, ignoreCase, context);
        return endingRemoved.Length < value.Length
            ? $"{endingRemoved}{newEnding}"
            : value;
    }

    public static string ReplaceEndingOrFail(string value, string matchEnding, string newEnding, bool ignoreCase, string? context = null)
    {
        Debug.Assert(value != null, $"{Program.ContextPrefix(context)}Value is null.");
        Debug.Assert(value.Length > 0, $"{Program.ContextPrefix(context)}Value is blank.");
        Debug.Assert(matchEnding != null, $"{Program.ContextPrefix(context)}Match ending is null.");
        Debug.Assert(matchEnding.Length > 0, $"{Program.ContextPrefix(context)}Match ending is blank.");
        Debug.Assert(newEnding != null, $"{Program.ContextPrefix(context)}New ending is null.");

        string endingRemoved = RemoveEndingOrFail(value, matchEnding, ignoreCase, context);
        return $"{endingRemoved}{newEnding}";
    }

}

