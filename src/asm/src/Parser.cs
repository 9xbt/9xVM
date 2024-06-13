using System;

namespace assembler;

static class Parser {
    const ushort StartAddress = 0x8000;
    static ushort CurrentAddress = StartAddress;
    public static List<Token> Tokens = new();
    public static List<byte> Output = new();
    
    public static void ParseLine(string line, int i) {
        if (line.Trim() == string.Empty || line.Trim().StartsWith("#")) {
            return;
        }
        else if (line.StartsWith('\t')) {
            string[] tokens = line.Trim().Split(' ');

            for (int t = 0; t < tokens.Length; t++) {
                Tokens.Add(new Token(tokens[t], t == 0 ? "instruction" : "operand", 2, CurrentAddress));
                CurrentAddress += 2;

                var token = Tokens[Tokens.Count - 1];
                Console.WriteLine("Added token " + token.Name + " of type " + token.Type);
            }
        }
        else {
            Tokens.Add(new Token(line.Split(':')[0], "label", 0, (ushort)(StartAddress + i)));

            var token = Tokens[Tokens.Count - 1];
            Console.WriteLine("Added token " + token.Name + " of type " + token.Type);
        }
    }

    public static void ParseTokens() {
        for (int i = 0; i < Tokens.Count; i++) {
            Token t = Tokens[i];

            switch (t.Type) {
                case "instruction":
                    ParseInstruction(t);
                    break;
                case "operand":
                    ParseOperand(t, out ushort _);
                    break;
                case "label":
                    ParseLabel(t);
                    break;
            }
        }
    }

    static void ParseInstruction(Token t) {
        switch (t.Name) {
            case "nop":
                Output.Add(0x00);
                Output.Add(0x00);
                break;

            case "mov":
                string firstOp = ParseOperand(t.GetNext(), out ushort firstVal);
                string secondOp = ParseOperand(t.GetNext().GetNext(), out ushort secondVal);

                if (firstOp == "number" && secondOp == "number") {
                    Output.Add(0x10);
                    Output.Add(0x00);
                }
                else if (firstOp == "number" && secondOp == "register") {
                    Output.Add(0x11);
                    Output.Add(0x00);
                }
                else if (firstOp == "register" && secondOp == "num") {
                    Output.Add(0x11);
                    Output.Add(0x00);
                }
                else if (firstOp == "register" && secondOp == "register") {
                    Output.Add(0x11);
                    Output.Add(0x00);
                }

                Output.Add((byte)(firstVal & 0xFF)); // low
                Output.Add((byte)(firstVal >> 8)); // high
                Output.Add((byte)(secondVal & 0xFF)); // low
                Output.Add((byte)(secondVal >> 8)); // high
                break;

            case "hlt":
                Output.Add(0xFF);
                Output.Add(0xFF);
                break;
        }

        Console.WriteLine("Added instruction " + t.Name);
    }

    static bool ParseNumber(string input, out ushort output) {
        bool isHex = false;

        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
            input = input.Substring(2);
            isHex = true;
        }

        try {
            if (input.StartsWith('\'') && input.EndsWith('\'')) {
                output = input.Substring(1)[0];
                return true;
            }

            output = Convert.ToUInt16(input, isHex ? 16 : 0);
            return true;
        }
        catch {
            output = 0;
            return false;
        }
    }

    static string ParseOperand(Token t, out ushort value) {
        if (ParseNumber(t.Name, out ushort num)) {
            value = num;
            return "number";
        }
        else if (t.Name == "a") {
            value = 1;
            return "register";
        }
        else if (t.Name == "x") {
            value = 2;
            return "register";
        }
        else if (t.Name == "y") {
            value = 3;
            return "register";
        }
        else {
            Console.WriteLine("Unknown operand type!");
            Console.WriteLine("Token name: " + t.Name);
            Console.WriteLine("Token type: " + t.Type);

            value = 0;
            return "unknown";
        }
    }

    static void ParseLabel(Token t) {
    }
}