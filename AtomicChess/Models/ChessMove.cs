using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicChess.Models
{
    public class ChessMove
    {
        public BoardPosition startPos { get; }
        public BoardPosition endPos { get; }
        public ChessMove(BoardPosition startPos, BoardPosition endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
        }

        public override string ToString()
        {
            return $"{startPos} to {endPos}";
        }
    }
}
