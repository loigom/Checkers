using System.Text.Json;
using Entities;

namespace IO;

public class DbManagerJson : IDbManager
{
    /*
    private static readonly string DirPath = Directory.GetCurrentDirectory() + "\\saves\\json\\";
    private const string PkFileName = "pk.txt";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
    };

    private static int GetNewPk()
    {
        var path = DirPath + PkFileName;
        Directory.CreateDirectory(DirPath);
        
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "1");
            return 1;
        }

        var pk = int.Parse(File.ReadAllText(path)) + 1;
        File.WriteAllText(path, pk.ToString());
        return pk;
    }

    private static IEnumerable<FileInfo> GetFiles(string pattern)
    {
        Directory.CreateDirectory(DirPath);
        var d = new DirectoryInfo(DirPath);
        return d.GetFiles(pattern);
    }
    
    public GameState SaveState(GameState state)
    {
        Directory.CreateDirectory(DirPath);
        var json = JsonSerializer.Serialize(state, JsonOptions);
        var path = $"{DirPath}{state.Id}.json";
        File.WriteAllText(path, json);
        return state;
    }

    public GameState? LoadStateFromNum(int num)
    {
        GameState? state = null;
        foreach (var game in ListGames())
        {
            var deserialized = JsonSerializer.Deserialize<GameState>(game);
            if (deserialized == null || deserialized.GameNum != num) continue;
            if (state == null || deserialized.Id > state.Id) state = deserialized;
        }

        return state;
    }

    public string[] ListGames()
    {
        var mostRecentStates = new Dictionary<int, GameState>();

        foreach (var f in GetFiles("*.json"))
        {
            var contents = File.ReadAllText(f.FullName);
            var state = JsonSerializer.Deserialize<GameState>(contents);
            if (state == null) continue;
            if (!mostRecentStates.ContainsKey(state.GameNum) ||
                mostRecentStates[state.GameNum].Id < state.Id) mostRecentStates[state.GameNum] = state;
        }

        return (
            from state in mostRecentStates.Values
            select state.ToString()
        ).ToArray();
    }

    public GameState MakeBrandNewState(GameOptions options)
    {
        var newNum = 0;
        foreach (var game in ListGames())
        {
            var deserialized = JsonSerializer.Deserialize<GameState>(game);
            if (deserialized != null && deserialized.GameNum >= newNum) newNum = deserialized.GameNum + 1;
        }

        return new GameState(GetNewPk(), newNum, options, new Board(options.Height),
            options.WhiteMovesFirst ? EPieceType.White : EPieceType.Black);
    }*/
    public GameState SaveState(GameState state)
    {
        throw new NotImplementedException();
    }

    public GameState? LoadStateFromNum(int num)
    {
        throw new NotImplementedException();
    }

    public string[] ListGames()
    {
        throw new NotImplementedException();
    }

    public GameState MakeBrandNewState(GameOptions options)
    {
        throw new NotImplementedException();
    }

    public List<Game>? GetAllGames()
    {
        throw new NotImplementedException();
    }

    public Game? GetGame(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteGame(int id)
    {
        throw new NotImplementedException();
    }

    public GameState[]? ListStatesForGameId(int id)
    {
        throw new NotImplementedException();
    }
}