using System.Text;
using Entities;
using IO;

namespace MenuSystem;

public class MainMenu : Menu
{
    private readonly GameOptions _gameOptions = new();
    private const string DbSelectionPath = "dbSelection.txt";
    private IDbManager _dbManager;

    public MainMenu()
    {
        if (!File.Exists(DbSelectionPath)) _dbManager = new DbManagerSql();
        else
        {
            var contents = File.ReadAllText(DbSelectionPath);
            if (contents.Equals("sql")) _dbManager = new DbManagerSql();
            else _dbManager = new DbManagerJson();
        }
    }

    private void FlipDbManager(Input inp)
    {
        var contents = _dbManager.GetType() == typeof(DbManagerJson) ? "sql" : "json";
        File.WriteAllText(DbSelectionPath, contents);
        _dbManager = contents.Equals("json") ? new DbManagerJson() : new DbManagerSql();
        Console.WriteLine($"Now using {contents} database.");
    }

    protected override void Greet()
    {
        Console.WriteLine($"Welcome to checkers!\n{this}");
    }

    protected override void BuildOptions()
    {
        Options.AddRange(new[]
        {
            new MenuOption("(N)ew game", "n", NewGame),
            new MenuOption("(L)ist games", "l", ListGames),
            new MenuOption("(P)lay [id]", "p", Play),
            new MenuOption("(O)ptions", "o", OptionsMenu),
            new MenuOption("(F)lip database", "f", FlipDbManager)
        });
    }

    private void NewGame(Input inp)
    {
        var state = _dbManager.MakeBrandNewState(_gameOptions);
        state = _dbManager.SaveState(state);
        Play(new Input($"p {state.Game.Id}"));
    }

    private void ListGames(Input inp)
    {
        var gameDescriptions = _dbManager.ListGames();
        Console.WriteLine(string.Join("\n\n", gameDescriptions));
        Console.WriteLine($"Found a total of {gameDescriptions.Length} games.");
    }

    private void Play(Input inp)
    {
        if (!int.TryParse(inp.Get(1), out var num))
        {
            Console.WriteLine("Invalid argument, please insert an ID for a saved game.");
            return;
        }

        var state = _dbManager.LoadStateFromNum(num);
        if (state == null)
        {
            Console.WriteLine($"Invalid argument, did not find game with num {num}");
            return;
        }

        var builder = new StringBuilder($"Now playing game #{state.Game.Id}\n");
        var printBoard = true;

        while (!state.Game.Finished)
        {
            if (printBoard)
            {
                builder.Append(state.GetBoardRepresentation() + "\n");
                builder.Append($"Turn of: {state.Turn.ToString()}" + "\n" +
                               "'e' to exit\n>");
                printBoard = false;
            }
            
            Console.Write(builder.ToString());
            builder.Clear();
            
            inp = new Input(Console.ReadLine());
            if (inp.First is "e") break;

            if (inp.Split is not { Count: 2 })
            {
                Console.WriteLine("Invalid input. Valid input is [(column)(row)] [(column)(row)] like 'A2 A4'.");
                continue;
            }

            var from = Square.FromInput(inp.Split[0]);
            var to = Square.FromInput(inp.Split[1]);
            if (from == null || to == null)
            {
                Console.WriteLine("Invalid input. Valid input is [(column)(row)] [(column)(row)] like 'A2 A4'.");
                continue;
            }
            
            try
            {
                var newState = state.MakeMove(from, to);
                Console.WriteLine(newState.GetBoardRepresentation());
                state = _dbManager.SaveState(newState);
                printBoard = true;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Bad input: " + e.Message);
            }
        }
    }

    private void OptionsMenu(Input inp)
    {
        new OptionsMenu(_gameOptions).Loop();
        Greet();
    }
}