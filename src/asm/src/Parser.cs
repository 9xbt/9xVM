using System.Collections.Generic;

namespace assembler;

static class Parser {
    const ushort StartAddress = 0x8000;
    static List<Label> Labels = new();
    
    public static void ParseLine(string line, int i) {
        if (line.StartsWith('\t')) {
            
        }
        else {
            Labels.Add(new Label(line.Split(':')[0], (ushort)(StartAddress + i)));
            Console.WriteLine("Added label: " + Labels[Labels.Count - 1]);
        }
    }
}