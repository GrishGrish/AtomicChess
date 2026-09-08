using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicChess.Models
{
    public enum PieceColour
    {
        white, black
    }
    //According to good programming practices using enum is better because it eliminates spelling errors

    public enum PieceType
    {
        pawn, knight, bishop, rook, queen, king
    }

    public class ChessPiece
    {
        public PieceType type { get; set; }
        public PieceColour colour { get; set; }
        public bool HasMoved { get; set; }

        public ChessPiece(PieceType type, PieceColour colour)
        {
            this.type = type;
            this.colour = colour;
            HasMoved = false;
        }

        public ChessPiece Copy()
        {
            return new ChessPiece(type, colour)
            {
                HasMoved = HasMoved
            };
        }
        //need the copy of the piece we are moving for safety and for bot to try out moves and analyze the new positions without actually changing anything
    }
}