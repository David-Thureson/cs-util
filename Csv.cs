using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Csv
    {
        public static TextFieldParser MakeTextFieldParser(string fullFileName)
        {
            TextFieldParser csvParser = new TextFieldParser(fullFileName);
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            return csvParser;
        }

        public static IEnumerable<string[]> EnumCsvLines(string fullFileName, bool skipFirstLine, int skipLineCount = 0)
        {
            using (TextFieldParser csvParser = MakeTextFieldParser(fullFileName))
            {
                if (skipFirstLine)
                {
                    csvParser.ReadLine();
                }
                for (int i = 0; i < skipLineCount; i++)
                {
                    csvParser.ReadLine();
                }
                while (!csvParser.EndOfData)
                {
                    string[]? fields = null;
                    try
                    {
                        fields = csvParser.ReadFields();
                    }
                    catch (Microsoft.VisualBasic.FileIO.MalformedLineException)
                    {
                    }
                    if (fields != null)
                    {
                        yield return fields;
                    }
                }
            }
            yield break;
        }

        public static void StitchFiles(string folderIn, string searchPattern, string fullFileNameOut, bool hasHeader, string? checkHeaderStart = null)
        {
            StringBuilder sb = new();
            int fileIndex = -1;
            foreach (string fileNameIn in Directory.GetFiles(folderIn, searchPattern, System.IO.SearchOption.TopDirectoryOnly))
            {
                fileIndex++;
                int lineIndex = -1;
                foreach (string line in File.ReadLines(fileNameIn))
                {
                    lineIndex++;
                    if (hasHeader && lineIndex == 0)
                    {
                        if (checkHeaderStart != null)
                        {
                            // Check that the first line is really a header.
                            Debug.Assert(line.StartsWith(checkHeaderStart), $"\"{fileNameIn}\" line {lineIndex + 1:n0}: header starting with \"{checkHeaderStart}\" not found.");
                            if (fileIndex > 0)
                            {
                                // We want to take the header from the first file (fileIndex == 0) but not from
                                // subsequent files.
                                continue;
                            }
                        }
                    }
                    sb.AppendLine(line);
                }
            }
            WriteLine($"\nUtil.Csv.StitchFiles(): writing to \"{fullFileNameOut}\"");
            File.WriteAllText(fullFileNameOut, sb.ToString());
        }

    }
}
