using Entities;

namespace IO;

public interface IDbManager
{
    public GameState SaveState(GameState state);
    public GameState? LoadStateFromNum(int num);
    public string[] ListGames();
    public GameState MakeBrandNewState(GameOptions options);
    public List<Game>? GetAllGames();
    public Game? GetGame(int id);
    public void DeleteGame(int id);
    public GameState[]? ListStatesForGameId(int id);
}