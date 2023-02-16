namespace MenuSystem;

public class MenuOption
{
    public string Name { get; }
    public string Shortcut { get; }
    public Action<Input> F { get; }

    public MenuOption(string name, string shortcut, Action<Input> f)
    {
        Name = name;
        Shortcut = shortcut;
        F = f;
    }

    public void Run(Input inp)
    {
        F.Invoke(inp);
    }

    public override string ToString()
    {
        return Name;
    }
}