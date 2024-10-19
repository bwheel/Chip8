using System.IO;

namespace Chip8.Assembler;


public class Program
{



    public static void Main(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            Console.WriteLine("Please provide a file to compile");
            Environment.Exit(1);
        }

        string filename = args[0];
        if (!File.Exists(filename))
        {
            Console.WriteLine($"argument: {filename} does not exist");
            Environment.Exit(1);
        }

        using var fileStream = File.OpenRead(filename);
        using var reader = new StreamReader(fileStream);

        List<string> commands = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            // empty line.
            if (string.IsNullOrWhiteSpace(line))
                continue;
            // commented out line.
            if (line.TrimStart().StartsWith("#"))
                continue;
        }
    }
}