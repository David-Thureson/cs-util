using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public enum Alignment
    {
        Left,
        Right
    }

    public class ColumnFormatter
    {
        List<List<string>> values = new();
        List<Alignment> alignments = new();
        int colCount = 0;

        public void AddRow(params string[] rowValues)
        {
            values.Add(rowValues.ToList());
            this.colCount = Math.Max(colCount, rowValues.Length);
            int colsToAdd = this.colCount - alignments.Count;
            for (int i = 0; i < colsToAdd; i++)
            {
                this.alignments.Add(Alignment.Left);
            }
        }

        public void AddRow((string, string) rowValues)
        {
            AddRow(rowValues.Item1, rowValues.Item2);
        }

        public void SetAlignments(Alignment alignment, params int[] colIndexes)
        {
            foreach (int colIndex in colIndexes)
            {
                this.alignments[colIndex] = alignment;
            }
        }

        public void AddAlignedValues(StringBuilder sb, int depth = 0, string indent = "  ", bool blankLine = false, bool spaceBetweenColumns = true, string lineEndBeforeLast = "", string lineEndLast = "")
        {
            string indentPrefix = Format.Repeat(indent, depth);
            List<int> maxLengths = new();
            for (int i = 0; i < colCount; i++)
            {
                maxLengths.Add(0);
            }
            foreach (List<string> row in this.values)
            {
                int colsInRow = row.Count;
                for (int i = 0; i < colCount; i++)
                {
                    if (i < colsInRow)
                    {
                        maxLengths[i] = Math.Max(maxLengths[i], row[i].Trim().Length);
                    }
                }
            }

            int rowIndex = -1;
            foreach (List<string> row in this.values)
            {
                rowIndex++;
                sb.Append(indentPrefix);
                for (int colIndex = 0; colIndex < colCount; colIndex++)
                {
                    // Some rows may not have an entry for each column, so
                    // use a blank string as the replacement value.
                    // string value = colIndex < row.Count
                    //     ? row[colIndex]
                    //     : "";
                    if (colIndex < row.Count)
                    {
                        string value = row[colIndex];
                        int padLength = maxLengths[colIndex];
                        switch (this.alignments[colIndex])
                        {
                            case Alignment.Left:
                                // if (colIndex < colCount - 1)
                                if (colIndex < row.Count - 1)
                                {
                                        // We're not yet on the last column, so pad it.
                                        value = value.PadRight(padLength, ' ');
                                }
                                break;
                            case Alignment.Right:
                                value = value.PadLeft(padLength, ' ');
                                break;
                            default:
                                throw new ArgumentException($"Unexpected alignment = \"{this.alignments[colIndex]}\".");
                        }
                        sb.Append($"{value}");
                        if (spaceBetweenColumns && colIndex < row.Count - 1)
                        {
                            sb.Append(' ');
                        }
                    }
                }
                // We may need to add something like a comma to the end of the line.
                sb.Append(rowIndex < this.values.Count - 1 ? lineEndBeforeLast : lineEndLast);
                sb.AppendLine();
            }
            if (blankLine)
            {
                sb.AppendLine();
            }
        }

        public void AddAlignedValuesSql(StringBuilder sb, int depth = 0, bool blankLine = false, bool spaceBetweenColumns = true, bool addCommas = false)
        {
            string lineEndBeforeLast = addCommas ? "," : "";
            string lineEndLast = "";
            AddAlignedValues(sb, depth, "\t", blankLine, spaceBetweenColumns, lineEndBeforeLast, lineEndLast);
        }

        public void AddAlignedValuesSqlComma(StringBuilder sb, int depth = 0, bool blankLine = false, bool spaceBetweenColumns = true, string lineEndLast = "")
        {
            AddAlignedValues(sb, depth, "\t", blankLine, spaceBetweenColumns, ",", lineEndLast);
        }

        public string GetAlignedValues()
        {
            StringBuilder sb = new();
            AddAlignedValues(sb);
            return sb.ToString();
        }
    }
}
