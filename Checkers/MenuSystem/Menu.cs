using System.Text;
using Entities;

namespace MenuSystem;

public abstract class Menu
{
    protected readonly List<MenuOption> Options = new();
    private readonly Dictionary<string, MenuOption> _inpToOption = new();
    
    private bool _opened = true;

    protected Menu()
    {
        BuildOptions();
        Options.Add(new MenuOption("(E)xit", "e", Exit));
        MapOptions();
        Greet();
    }

    protected abstract void Greet();

    protected abstract void BuildOptions();

    private void MapOptions()
    {
        foreach (var o in Options)
        {
            if (_inpToOption.ContainsKey(o.Shortcut))
                throw new ArgumentException("Attempted to map options " +
                                            "to menu, but shortcut string (" + o.Shortcut + ") is already in use.");

            _inpToOption[o.Shortcut] = o;
        }
    }

    private bool HasOption(Input inp)
    {
        return inp.First != null && _inpToOption.ContainsKey(inp.First);
    }

    private void RunOption(Input inp)
    {
        if (!HasOption(inp))
        {
            Console.WriteLine($"There exists no option {inp.First}, available options are:");
            Console.WriteLine(this);
        }
        else
        {
            _inpToOption[inp.First!].Run(inp);  // First! - ignore nullable warning, nullable is checked in HasOption.
        }
    }

    public void Loop()
    {
        while (_opened)
        {
            Console.Write('>');
            RunOption(new Input(Console.ReadLine()));
        }
    }

    private void Exit(Input inp)
    {
        _opened = false;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var o in Options) builder.Append(o + "\n");
        return builder.ToString().TrimEnd();
    }
}