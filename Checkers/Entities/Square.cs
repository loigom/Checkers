namespace Entities;

public class Square
{
    public int Id { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public GameState State { get; set; } = new();
    public EPieceType PieceType { get; set; }
    
    public Square() {}

    public Square(int x, int y, GameState? state, EPieceType? type)
    {
        Row = y;
        Column = x;
        if (state != null) State = state;
        if (type != null) PieceType = (EPieceType)type;
    }

    public string Location()
    {
        return $"{((char)(Column + 'A')).ToString()}{Row}";
    }

    public static Square? FromInput(string inp)
    {
        inp = inp.ToLower();
        
        var array = inp.ToCharArray();
        if (array.Length is < 2 or > 3) return null;
        if (array[0] is < 'a' or > 'z') return null;
        if (array[1] is < '0' or > '9') return null;
        if (array.Length == 3 && array[2] is < '0' or > '9') return null;

        var x = array[0] - 'a';
        var y = int.Parse(inp[1..]);

        return new Square(x, y, null, null);
    }

    public bool ValidFor(GameOptions options)
    {
        return Row >= 0 && Row < options.Dimensions &&
               Column >= 0 && Column < options.Dimensions;
    }

    public bool IsWhite()
    {
        return PieceType is EPieceType.White or EPieceType.WhiteKing;
    }

    public bool IsKing()
    {
        return PieceType is EPieceType.BlackKing or EPieceType.WhiteKing;
    }
}