using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicChess.Models
{
    public readonly struct BoardPosition
    {
        public int row { get; }
        public int column { get; }

        public BoardPosition(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public bool IsSameAs(BoardPosition other)
        {
            return row == other.row && column == other.column;
        }
        //checking if the same square is selected

        public bool IsInsideBoard()
        {
            return (row >= 0 && row < 8 && column >= 0 && column < 8);
        }

        public override string ToString()
        {
            char file = (char)('a' + column);
            int rank = 8 - row;

            return $"{file}{rank}";
        }
        //Should work for displaying chess notation
    }
}
