using System.Text;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace IO;

public class DbManagerSql : IDbManager
{
    private readonly DbManagerSqlContext _context;

    public DbManagerSql()
    {
        _context = new DbManagerSqlContext();
    }
    
    public GameState SaveState(GameState state)
    {
        if (state.Finished()) state.Game.Finished = true;
        _context.States?.Add(state);
        _context.SaveChanges();
        return state;
    }

    public GameState? LoadStateFromNum(int num)
    {
        return _context.States?
            .Where(state => state.Game.Id == num)
            .OrderBy(state => state.Id)
            .Include(state => state.Squares)
            .Include(state => state.Game)
            .ThenInclude(game => game.Options)
            .Last();
    }

    public string[] ListGames()
    {
        var outList = new List<string>();
        var builder = new StringBuilder();
        
        var gamesInProgress = _context.Games?
            .Where(game => !game.Finished)
            .OrderBy(game => game.Id);
        
        if (gamesInProgress == null) return Array.Empty<string>();

        foreach (var game in gamesInProgress)
        {
            var state = LoadStateFromNum(game.Id);
            if (state == null) throw new ApplicationException("Got null gamestate where shouldn't.");
            builder.Clear();
            builder.Append($"ID: {game.Id}\n");
            builder.Append(state.GetBoardRepresentation());
            outList.Add(builder.ToString());
        }

        return outList.ToArray();
    }

    public GameState MakeBrandNewState(GameOptions options)
    {
        return new GameState(options.Copy());
    }

    public List<Game>? GetAllGames()
    {
        return _context.Games?
            .OrderBy(game => game.Id)
            .Include(game => game.Options)
            .ToList();
    }

    public Game? GetGame(int id)
    {
        try
        {
            return _context.Games?.Where(game => game.Id == id)
                .Include(game => game.Options)
                .First();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public void DeleteGame(int id)
    {
        var game = GetGame(id);

        if (game == null) return;

        var squares = _context.Squares?.Where(square => square.Id == game.Id).ToArray();
        var option = _context.GameOptions?.Where(options => options.Id == game.Id).First();
        var states = _context.States?.Where(state => state.Game.Id == game.Id).ToArray();

        if (squares != null)
        {
            foreach (var square in squares)
            {
                _context.Remove(square);
            }
        }

        if (option != null)
        {
            _context.Remove(option);
        }

        if (states != null)
        {
            foreach (var state in states)
            {
                _context.Remove(state);
            }
        }

        _context.Remove(game);

        _context.SaveChanges();
    }

    public GameState[]? ListStatesForGameId(int id)
    {
        return _context.States?
            .Where(state => state.Game.Id == id)
            .OrderBy(state => state.Id)
            .Include(state => state.Squares)
            .Include(state => state.Game)
            .ThenInclude(game => game.Options)
            .ToArray();
    }
}