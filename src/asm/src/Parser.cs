//#define DEBUG_INFO

namespace assembler;

static class Parser {
    const ushort StartAddress = 0x8000;
    static ushort CurrentAddress = StartAddress;
    public static string Filename = string.Empty;
    public static List<Token> Tokens = new();
    public static List<byte> Output = new();
    
    public static void ParseLine(string line, int i) {
        if (line.Trim() == string.Empty || line.Trim().StartsWith("#")) {
            return;
        }
        else if (line.StartsWith('\t')) {
            string[] tokens = line.Trim().Replace("\' \'", "0x20").Split(' ');

            for (int t = 0; t < tokens.Length; t++) {
                var token = new Token(tokens[t], t == 0 ? "instruction" : "operand", Filename + ":" + (i + 1) + ":" + (line.IndexOf(tokens[t]) + 1), StartAddress);
                Tokens.Add(token);

#if DEBUG_INFO
                Console.WriteLine("Added token " + token.Name + " of type " + token.Type);
#endif
            }
        }
        else {
            var token = new Token(line.Split(':')[0], "label", Filename + ":" + (i + 1) + ":0", StartAddress);
            Tokens.Add(token);

            if (!line.EndsWith(':')) {
                Program.DisplayError(token, "missing operand");
            }

#if DEBUG_INFO
            Console.WriteLine("Added token " + token.Name + " of type " + token.Type);
#endif
        }
    }

    public static void ParseTokens() {
        for (int i = 0; i < Tokens.Count; i++) {
            Token t = Tokens[i];

            switch (t.Type) {
                case "instruction":
                    CurrentAddress += 2;
                    break;
                case "operand":
                    CurrentAddress += 2;
                    break;
                case "label":
                    t.Address = CurrentAddress;
                    break;
            }
        }

        CurrentAddress = StartAddress;

        for (int i = 0; i < Tokens.Count; i++) {
            Token t = Tokens[i];

            switch (t.Type) {
                case "instruction":
                    ParseInstruction(t);
                    break;
                case "operand":
                    ParseOperand(t, out ushort _);
                    break;
            }
        }
    }

    static void ParseInstruction(Token t) {
        string[] operands = new string[2];
        ushort[] opValues = new ushort[2];

        switch (t.Name) {
            case "nop":
                Output.Add(0x00);
                Output.Add(0x00);

                CurrentAddress += 2;
                break;

            case "jmp":
                operands[0] = ParseOperand(t.GetNext(), out opValues[0]);

                if (operands[0] == "value") {
                    if (opValues[0] < CurrentAddress + 4) {
                        Program.DisplayWarning(t.GetNext(), "probably accidental infinite loop");
                    }

                    Output.Add(0x01);
                    Output.Add(0x00);
                }
                else if (operands[0] == "register") {
                    Output.Add(0x02);
                    Output.Add(0x00);
                }

                Output.Add((byte)(opValues[0] & 0xFF)); // low
                Output.Add((byte)(opValues[0] >> 8)); // high

                CurrentAddress += 4;
                break;

            case "mov":
                operands[0] = ParseOperand(t.GetNext(), out opValues[0]);
                operands[1] = ParseOperand(t.GetNext().GetNext(), out opValues[1]);

                if (operands[0] == "value" && operands[1] == "value") {
                    Output.Add(0x10);
                    Output.Add(0x00);
                }
                else if (operands[0] == "value" && operands[1] == "register") {
                    Output.Add(0x11);
                    Output.Add(0x00);
                }
                else if (operands[0] == "register" && operands[1] == "value") {
                    Output.Add(0x12);
                    Output.Add(0x00);
                }
                else if (operands[0] == "register" && operands[1] == "register") {
                    Output.Add(0x13);
                    Output.Add(0x00);
                }

                Output.Add((byte)(opValues[0] & 0xFF)); // low
                Output.Add((byte)(opValues[0] >> 8)); // high
                Output.Add((byte)(opValues[1] & 0xFF)); // low
                Output.Add((byte)(opValues[1] >> 8)); // high

                CurrentAddress += 6;
                break;

            case "hlt":
                Output.Add(0xFF);
                Output.Add(0xFF);

                CurrentAddress += 2;
                break;
        }

#if DEBUG_INFO
        Console.WriteLine("Added instruction " + t.Name);
#endif
    }

    static bool Parsevalue(string input, out ushort output) {
        bool isHex = false;

        try {
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                input = input.Substring(2);
                isHex = true;
            }
            else if (input.StartsWith('\'') && input.EndsWith('\'')) {
                output = input.Substring(1)[0];
                return true;
            }

            output = Convert.ToUInt16(input, isHex ? 16 : 10);
            return true;
        }
        catch {
            output = 0;
            return false;
        }
    }

    static string ParseOperand(Token t, out ushort value) {
        if (t.Name == "a") {
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
        else if (t.Name == "$") {
            value = CurrentAddress;
            return "value";
        }
        else if (Parsevalue(t.Name, out ushort num)) {
            value = num;
            return "value";
        }
        else {
            value = GetLabelByName(t, t.Name).Address;
            return "value";
        }
    }

    static Token GetLabelByName(Token parent, string name) {
        foreach (Token t in Tokens) {
            if (t.Type == "label" && t.Name == name) {
                return t;
            }
        }

        Program.DisplayError(parent, "unknown label '" + parent.Name + "'");

        return null!;
    }
}