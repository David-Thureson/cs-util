using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    internal class FileName
    {
        internal static void Go()
        {
            // TryExcelStyleLetterSequence();
            // TryMakeFileNameNumber();
            // TryMakeFileNameLetters();
        }

        public static string MakeFileNameNumber(string path, string baseFileName, int digits, bool includeDate)
        {
            if (!Directory.Exists(path)) { 
                Directory.CreateDirectory(path);
            }

            string fileNameRoot = Parse.BeforeLastOrFail(baseFileName, ".").Trim();
            string fileExtension = Parse.AfterLastOrFail(baseFileName, ".").Trim();

            string datePart = includeDate
                ? $" {DateTime.Now.ToString("yyyy_MM_dd")}"
                : "";

            int number = 1;
            int try_count = 0;
            do
            {
                try_count++;
                Debug.Assert(try_count <= 100000);

                string numberString = // number.ToString().Length > digits
                    // ? number.ToString()
                    Format.PadZero(number, digits);
                string candidateFileName = Path.Combine(path, $"{fileNameRoot}{datePart} {numberString}.{fileExtension}");
                if (!File.Exists(candidateFileName))
                {
                    return candidateFileName;
                }

                number++;

            } while (true);

        }

        public static string MakeFileNameLetters(string path, string baseFileName, bool includeDate)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileNameRoot = Parse.BeforeLastOrFail(baseFileName, ".").Trim();
            string fileExtension = Parse.AfterLastOrFail(baseFileName, ".").Trim();

            string datePart = includeDate
                ? $" {DateTime.Now.ToString("yyyy_MM_dd")}"
                : "";

            foreach (string letters in ExcelStyleLetterSequence())
            {
                string candidateFileName = Path.Combine(path, $"{fileNameRoot}{datePart} {letters}.{fileExtension}");
                if (!File.Exists(candidateFileName))
                {
                    return candidateFileName;
                }
            }

            // If we've reached this point, we've run out of letters, so use numbers instead.
            return MakeFileNameNumber(path, baseFileName, 6, includeDate);
        }

        public static IEnumerable<string> ExcelStyleLetterSequence()
        {
            // Single letter.
            for (char c = 'a'; c <= 'z'; c++)
            {
                yield return c.ToString();
            }

            // Double letter.
            for (char c1 = 'a'; c1 <= 'z'; c1++)
            {
                for (char c2 = 'a'; c2 <= 'z'; c2++)
                {
                    yield return $"{c1}{c2}";
                }
            }

        }

        internal static void TryExcelStyleLetterSequence()
        {
            foreach (string letters in ExcelStyleLetterSequence()) {
                WriteLine(letters);
            }
        }

        internal static void TryMakeFileNameNumber()
        {
            string path = @"C:\Temp\TestMakeFileName";

            /*
            for (int i = 0; i < 110; i++)
            {
                File.WriteAllText(MakeFileNameNumber(path, "abc.txt", 2, true), "aaa");
            }
            */

            for (int i = 0; i < 5; i++)
            {
                File.WriteAllText(MakeFileNameNumber(path, "abc.sql", 2, false), "aaa");
            }

            for (int i = 0; i < 5; i++)
            {
                File.WriteAllText(MakeFileNameNumber(path, "abc.sql", 3, false), "aaa");
            }
        }

        internal static void TryMakeFileNameLetters()
        {
            string path = @"C:\Temp\TestMakeFileName";

            for (int i = 0; i < 26 * 28; i++)
            {
                File.WriteAllText(MakeFileNameLetters(path, "abc.txt", true), "aaa");
            }
        }

    }
}
