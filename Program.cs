global using static System.Console;

namespace Util
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Util.Program.Main() - start");

            // Code.Go();
            // Parse.Test();
            // NumberList.TryIntProgressionAdd();
            // FileName.Go();
            // SQL.Go();
            // Format.Try();

            Console.WriteLine("Util.Program.Main() - done");
        }

        public static string ContextPrefix(string? context)
        {
            return context is not null ? $"Context = '{context}'; " : "";
        }

    }
}