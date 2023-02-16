namespace Entities;

public class Game
{
    public int Id { get; set; }
    public GameOptions Options { get; set; } = new();
    public bool Finished { get; set; }

    public Game() {}
    
    public Game(GameOptions options)
    {
        Options = options;
    }
}