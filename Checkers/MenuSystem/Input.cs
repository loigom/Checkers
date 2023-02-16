namespace MenuSystem;

public class Input
{
    public string? Raw { get; }
    public string? First { get; }
    public List<string>? Split { get; }

    public Input(string? inp)
    {
        if (inp == null) return;

        Raw = inp;
        Split = new List<string>();
        
        foreach (var s in inp.Split(" "))
        {
            var parsed = s.Trim().ToLower();
            if (!parsed.Equals("")) Split.Add(parsed);
        }

        if (Split.Count > 0) First = Split[0];
    }

    public bool HasArgument(string arg)
    {
        if (Split == null) return false;
        arg = arg.Trim().ToLower();
        return Split.Any(s => arg.Equals(s));
    }

    public string? Get(int i)
    {
        if (Split == null || Split.Count < i + 1) return null;
        return Split[i];
    }
}