namespace assembler;

static class Program {
    static void Main(string[] args) {
        string[] lines = File.ReadAllLines(args[0]);

        for (int i = 0; i < lines.Length; i++) {
            Parser.ParseLine(lines[i], i);
        }

        Parser.ParseTokens();

        Console.WriteLine("Compilation successful!");
        Console.WriteLine("Output length: " + Parser.Output.Count);
        foreach (byte b in Parser.Output) {
            Console.Write("0x" + b.ToString("X2") + " ");
        }
        Console.WriteLine();
    }
}