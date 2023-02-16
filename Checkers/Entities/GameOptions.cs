using System.Text;

namespace Entities;

public class GameOptions
{
    public int Id { get; set; }
    public int Dimensions { get; set; } = 8;
    public bool WhiteMovesFirst { get; set; } = true;
    public EGameType GameType { get; set; } = EGameType.HumanVsAi;
    
    public GameOptions() {}

    public GameOptions(int dimensions, bool whiteMovesFirst, EGameType gameType)
    {
        Dimensions = dimensions;
        WhiteMovesFirst = whiteMovesFirst;
        GameType = gameType;
    }

    public GameOptions Copy()
    {
        return new GameOptions(Dimensions, WhiteMovesFirst, GameType);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append($"Dimensions: {Dimensions}\n");
        builder.Append($"WhiteMovesFirst: {WhiteMovesFirst}\n");
        builder.Append($"GameType: {GameType.ToString()}");
        return builder.ToString();
    }
}