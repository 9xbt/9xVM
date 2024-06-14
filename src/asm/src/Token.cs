namespace assembler;

class Token {
    public string Name;
    public string Type;
    public string Location;
    public ushort Address;

    public Token(string name, string type, string location, ushort address) {
        Name = name;
        Type = type;
        Location = location;
        Address = address;

    }

    public Token GetNext() {
        for (int i = 0; i < Parser.Tokens.Count; i++) {
            Console.WriteLine("i:    " + Parser.Tokens[i].Location);
            Console.WriteLine("this: " + Location);
            if (Parser.Tokens[i].Location.Split(':')[1] != Location.Split(':')[1]) {
                Program.DisplayError(this, "missing operand");
                return null!;
            }

            if (Parser.Tokens[i] == this && Parser.Tokens[i].Type != "label") {    
                return Parser.Tokens[i + 1];
            }
        }
        return null!;
    }
}