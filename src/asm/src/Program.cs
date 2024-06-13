namespace assembler;

static class Program
{
    static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines(args[1]);

        for (int i = 0; i < lines.Length; i++) {
            Parser.ParseLine(lines[i], i);
        }
    }
}