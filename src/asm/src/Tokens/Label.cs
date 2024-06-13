namespace assembler;

struct Label {
    public string Name;
    public ushort Address;

    public Label(string name, ushort address) {
        Name = name;
        Address = address;
    }
}