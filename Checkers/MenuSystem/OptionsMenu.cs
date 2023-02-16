using Entities;

namespace MenuSystem;

public class OptionsMenu : Menu
{
    private readonly GameOptions _options;

    public OptionsMenu(GameOptions options)
    {
        _options = options;
    }
    protected override void Greet()
    {
        Console.WriteLine($"-- Options menu --\n{this}");
    }

    protected override void BuildOptions()
    {
        Options.AddRange(new []
        {
            new MenuOption("(L)ist current options", "l", _ => Console.WriteLine(_options)),
            new MenuOption("(D)imensions [8|10|12]", "d", SetDimensions),
            new MenuOption("(F)irst move [white|black]", "f", SetFirstMove),
            new MenuOption("(G)ame type [human_ai|human_human|ai_ai]", "g", SetGameType)
        });
    }

    private void SetDimensions(Input inp)
    {
        var arg = inp.Get(1);
        if (arg == null)
        {
            Console.WriteLine("Bad input. Expected integer.");
            return;
        }

        var isNumeric = int.TryParse(arg, out var n);

        if (!isNumeric || (n != 8 && n != 10 && n != 12))
        {
            Console.WriteLine("Bad input. Must be 8, 10 or 12.");
            return;
        }

        _options.Dimensions = n;
    }

    private void SetFirstMove(Input inp)
    {
        var arg = inp.Get(1);
        switch (arg)
        {
            case "white":
                _options.WhiteMovesFirst = true;
                break;
            case "black":
                _options.WhiteMovesFirst = false;
                break;
            default:
                Console.WriteLine("Bad input. Must be 'black' or 'white'.");
                break;
        }
    }

    private void SetGameType(Input inp)
    {
        var arg = inp.Get(1);
        switch (arg)
        {
            case "human_ai":
                _options.GameType = EGameType.HumanVsAi;
                break;
            case "human_human":
                _options.GameType = EGameType.HumanVsHuman;
                break;
            case "ai_ai":
                _options.GameType = EGameType.AiVsAi;
                break;
            default:
                Console.WriteLine("Bad input. Must be 'human_ai', 'human_human' or 'ai_ai'.");
                break;
        }
    }
}