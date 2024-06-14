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

    public override string ToString() {
        return "Token " + Name + " with type " + Type + " at " + Location + " at address " + Address.ToString("X4");
    }

    public Token GetNext() {
        var token = Parser.Tokens[Parser.Tokens.IndexOf(this) + 1];

        if (token.Location.Split(':')[1] != Location.Split(':')[1]) {
            Program.DisplayError(this, "missing operand");
            return null!;
        }

        return token;
    }
}