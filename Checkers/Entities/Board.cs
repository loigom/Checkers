namespace Entities;

public class Board
{
    private Dictionary<int, Dictionary<int, EPieceType>> Squares { get; set; } = new();

    public Board(int dimensions)
    {
        var half = dimensions / 2;

        for (var y = 0; y < dimensions; y++)
        {
            if (y == half || y == half - 1) continue;
            var type = y < half ? EPieceType.White : EPieceType.Black;
            for (var x = y % 2; x < dimensions; x += 2) AddSquare(x, y, type);
        }
    }

    public Board(List<Square> squares)
    {
        foreach (var square in squares)
        {
            if (!Squares.ContainsKey(square.Row)) Squares.Add(square.Row, new Dictionary<int, EPieceType>());
            Squares[square.Row][square.Column] = square.PieceType;
        }
    }
    
    public List<Square> ToList(GameState state)
    {
        return (from y in Squares.Keys
                from x in Squares[y].Keys
                select new Square(x, y, state, Squares[y][x]))
            .ToList();
    }

    public List<Square> ToListOfType(GameState state, EPieceType type)
    {
        return ToList(state)
            .Where(square => square.IsWhite() && type == EPieceType.White ||
                             !square.IsWhite() && type == EPieceType.Black)
            .ToList();
    }
    
    public Square? SquareAt(int x, int y)
    {
        if (Squares.ContainsKey(y) && Squares[y].ContainsKey(x))
            return new Square(x, y, null, Squares[y][x]);
        return null;
    }

    public void AddSquare(int x, int y, EPieceType type)
    {
        if (!Squares.ContainsKey(y)) Squares[y] = new Dictionary<int, EPieceType>();
        Squares[y][x] = type;
    }

    public void RemoveSquare(int x, int y)
    {
        if (SquareAt(x, y) == null) return;
        Squares[y].Remove(x);
        if (Squares[y].Count == 0) Squares.Remove(y);
    }

    public void RemoveSquare(Square square)
    {
        RemoveSquare(square.Column, square.Row);
    }

    public void AddSquare(Square square)
    {
        AddSquare(square.Column, square.Row, square.PieceType);
    }
}