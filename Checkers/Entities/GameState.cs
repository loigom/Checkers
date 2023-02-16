using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities;

public class GameState
{
    public int Id { get; set; }
    public Game Game { get; set; } = new();
    public List<Square> Squares { get; set; } = new();
    public EPieceType Turn { get; set; }
    [NotMapped] private bool _jumped;
    
    
    public GameState() {}

    public GameState(GameOptions options)
    {
        Game = new Game(options);
        Turn = options.WhiteMovesFirst ? EPieceType.White : EPieceType.Black;
        Squares = new Board(options.Dimensions).ToList(this);
    }

    public GameState(Game game, Board board, EPieceType turn)
    {
        Game = game;
        Squares = board.ToList(this);
        Turn = turn;
    }

    public GameState MakeMove(Square from, Square to)
    {
        var board = new Board(Squares);
        
        var fromPiece = board.SquareAt(from.Column, from.Row);
        
        // Check if 'from' is a piece at all.
        if (fromPiece == null) throw new ArgumentException($"Cannot move from {from.Location()} as there " +
                                                           "is no piece there.");

        // Check if it is actually the turn of 'from'.
        if (fromPiece.IsWhite() && Turn != EPieceType.White || !fromPiece.IsWhite() && Turn != EPieceType.Black)
            throw new ArgumentException($"Cannot move from there, current turn is {Turn.ToString()}");

        // Check from->to bounds.
        if (!to.ValidFor(Game.Options)) throw new ArgumentException($"{to.Location()} is out of bounds.");

        var toPiece = board.SquareAt(to.Column, to.Row);
        
        var yDistance = from.Row - to.Row;
        var xDistance = Math.Abs(from.Column - to.Column);
        var directionDown = yDistance < 0;
        yDistance = Math.Abs(yDistance);

        // Check y-direction. Cannot move to the side. White can't go up. Black can't go down.
        if ((directionDown && fromPiece.PieceType == EPieceType.Black) ||
            (!directionDown && fromPiece.PieceType == EPieceType.White))
            throw new ArgumentException("Cannot " +
                                        "move that piece in that direction.");
        
        // Cannot jump on a piece.
        if (toPiece != null) throw new ArgumentException("Can only jump over pieces, not on them.");

        if (yDistance != xDistance) throw new ArgumentException("y must == x");

        if (yDistance == 2)
        {
            var betweenPiece = board.SquareAt((from.Column + to.Column) / 2,
                (from.Row + to.Row) / 2);
            if (betweenPiece == null || fromPiece.IsWhite() == betweenPiece.IsWhite())
                throw new ArgumentException("Cannot jump there. Must be an opposite piece in between.");
            board.RemoveSquare(betweenPiece);
        }
        else if (yDistance != 1) throw new ArgumentException("Distance must be 1 or 2.");

        // Upgrade to king if possible.
        fromPiece.PieceType = fromPiece.PieceType switch
        {
            EPieceType.White when to.Row == Game.Options.Dimensions - 1 => EPieceType.WhiteKing,
            EPieceType.Black when to.Row == 0 => EPieceType.BlackKing,
            _ => fromPiece.PieceType
        };

        board.RemoveSquare(fromPiece.Column, fromPiece.Row);
        board.AddSquare(to.Column, to.Row, fromPiece.PieceType);
        
        var newState = new GameState(Game, board, Turn)
        {
            _jumped = Math.Abs(from.Column - to.Column) == 2
        };
        if (newState._jumped)
        {
            var movesToMake = newState.SeekAvailableMovesFrom(to, Turn == EPieceType.White ? 1 : -1)
                .Where(state => state._jumped)
                .ToList();
            if (movesToMake.Count > 0) newState = movesToMake[0];
        }

        newState.Turn = fromPiece.IsWhite() ? EPieceType.Black : EPieceType.White;

        return newState;
    }

    public bool Finished()
    {
        var board = new Board(Squares);
        return board.ToListOfType(this, EPieceType.Black).Count == 0 ||
               board.ToListOfType(this, EPieceType.White).Count == 0;
    }

    private List<GameState> SeekAvailableMoves()
    {
        var board = new Board(Squares);
        var playableStates = new List<GameState>();
        var yNudge = Turn == EPieceType.White ? 1 : -1;

        foreach (var from in board.ToListOfType(this, Turn))
        {
            playableStates.AddRange(SeekAvailableMovesFrom(from, yNudge));
        } 

        return playableStates;
    }

    private IEnumerable<GameState> SeekAvailableMovesFrom(Square from, int yNudge)
    {
        var playableStates = new List<GameState>();
        var possibleGoTos = new List<Square>
        {
            new(from.Column - 1, from.Row + yNudge, null, null),
            new(from.Column + 1, from.Row + yNudge, null, null),
            new(from.Column - 2, from.Row + yNudge * 2, null, null),
            new(from.Column + 2, from.Row + yNudge * 2, null, null)
        };

        if (from.IsKing())
        {
            yNudge *= -1;
            possibleGoTos.AddRange(new List<Square>
            {
                new(from.Column - 1, from.Row + yNudge, null, null),
                new(from.Column + 1, from.Row + yNudge, null, null),
                new(from.Column - 2, from.Row + yNudge * 2, null, null),
                new(from.Column + 2, from.Row + yNudge * 2, null, null)
            });
        }

        foreach (var to in possibleGoTos)
        {
            try
            {
                var playableState = MakeMove(from, to);
                playableState._jumped = Math.Abs(from.Column - to.Column) == 2;
                playableStates.Add(playableState);
            } catch (ArgumentException) {}
        }

        return playableStates;
    }

    public GameState? MakeRandomMove()
    {
        var moves = SeekAvailableMoves();
        if (moves.Count == 0) return null;
        var rnd = new Random();
        return moves[rnd.Next(moves.Count)];
    }

    public string GetBoardRepresentation()
    {
        var builder = new StringBuilder("  |");
        var c = 'A';
        int x, y;
        var board = new Board(Squares);
        
        for (x = 0; x < Game.Options.Dimensions; x++)
        {
            builder.Append($" {c++} |");
        }
        builder.Append('\n');

        for (y = 0; y < Game.Options.Dimensions; y++)
        {
            builder.Append(y.ToString().PadRight(2));
            builder.Append('|');

            for (x = 0; x < Game.Options.Dimensions; x++)
            {
                c = board.SquareAt(x, y)?.PieceType switch
                {
                    EPieceType.Black => 'b',
                    EPieceType.White => 'w',
                    EPieceType.BlackKing => 'B',
                    EPieceType.WhiteKing => 'W',
                    _ => ' '
                };
                builder.Append($" {c} |");
            }
            
            builder.Append('\n');
        }

        return builder.ToString();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append($"Game ID: {Game.Id}\n");
        builder.Append($"Turn: {Turn}\n\n");
        builder.Append(GetBoardRepresentation());
        return builder.ToString();
    }
}