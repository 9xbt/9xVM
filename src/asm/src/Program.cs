using System.Diagnostics;

namespace assembler;

static class Program {
    static Stopwatch Watch = new();
    public static int Warnings = 0;
    public static int Errors = 0;

    static void Main(string[] args) {
        Console.WriteLine("9xVM version 1.0");

        Watch.Start();
        
        Parser.Filename = args[0];
        string outputFile = args[0].Replace(".asm", ".bin");
        Console.WriteLine("    " + args[0] + " -> " + outputFile + "\n");

        string[] lines = File.ReadAllLines(args[0]);

        try {
            for (int i = 0; i < lines.Length; i++) {
                Parser.ParseLine(lines[i], i);
            }

            Parser.ParseTokens();
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }

        File.WriteAllBytes(outputFile, Parser.Output.ToArray());

        DisplayInfo();
    }

    public static void DisplayWarning(Token t, string msg) {
        Console.Write(t.Location + ": ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("warning: ");
        Console.ResetColor();
        Console.WriteLine(msg);

        Warnings += 1;
    }

    public static void DisplayError(Token t, string msg) {
        Console.Write(t.Location + ": ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("error: ");
        Console.ResetColor();
        Console.WriteLine(msg);

        Errors += 1;
    }

    public static void DisplayInfo() {
        Watch.Stop();

        Console.ForegroundColor = Errors == 0 ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(Errors == 0 ? "\nBuild succeeded!" : "\nBuild failed!");
        Console.ResetColor();
        if (Warnings != 0) Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("    " + Warnings + " Warning(s)");
        if (Errors != 0) Console.ForegroundColor = ConsoleColor.Red;
        else Console.ResetColor();
        Console.WriteLine("    " + Errors + " Errors(s)\n");
        Console.ResetColor();

        Console.WriteLine("Time Elapsed " + Watch.Elapsed.ToString(@"hh\:mm\:ss\.ff"));

        if (Errors != 0) Environment.Exit(1); 
    }
}