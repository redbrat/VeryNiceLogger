public struct Parameter
{
    public string Name;

    public Parameter(string name)
    {
        Name = name.Trim();
    }

    public override string ToString() => Name;
}
