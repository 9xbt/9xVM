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
            if (Parser.Tokens[i] == this) {
                return Parser.Tokens[i + 1];
            }
        }
        return null!;
    }
}