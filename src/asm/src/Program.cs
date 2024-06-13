namespace assembler;

static class Program {
    static void Main(string[] args) {
        string[] lines = File.ReadAllLines(args[0]);

        for (int i = 0; i < lines.Length; i++) {
            Parser.ParseLine(lines[i], i);
        }

        Parser.ParseTokens();

        Console.WriteLine("Writing output...");
        File.WriteAllBytes(args[0].Replace(".asm", ".bin"), Parser.Output.ToArray());

        Console.WriteLine("Compilation successful!");
    }
}