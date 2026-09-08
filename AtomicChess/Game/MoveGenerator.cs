using AtomicChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicChess.Game
{
    public class MoveGenerator
    {
        private AtomicChessGame game;
        public MoveGenerator(AtomicChessGame game)
        {
            this.game = game;
        }

        public List<ChessMove> GetChessMoves(BoardPosition startPos)
        {
            List<ChessMove> possibleMoves = new List<ChessMove>();
            ChessPiece? piece = game.GetPiece(startPos);

            if (piece == null)
            {
                return possibleMoves;
            }

            switch (piece.type)
            {
                case PieceType.pawn: addPawnMoves(startPos, piece, possibleMoves); break;
                case PieceType.knight: knight(startPos, piece, possibleMoves); break;
                case PieceType.bishop: bishop(startPos, piece, possibleMoves); break;
                case PieceType.rook: rook(startPos, piece, possibleMoves); break;
                case PieceType.queen: queen(startPos, piece, possibleMoves); break;
                case PieceType.king: king(startPos, piece, possibleMoves); break;
            }
            return possibleMoves;
        }

        private void addPawnMoves(BoardPosition startPos, ChessPiece pawn, List<ChessMove> possibleMoves)
        {
            int whichWay;
            if (pawn.colour == PieceColour.white)
            {
                whichWay = -1;
            }
            else
            {
                whichWay = 1;
            }

            BoardPosition oneSqrFw = new BoardPosition(startPos.row + whichWay, startPos.column);
            if (oneSqrFw.IsInsideBoard() && game.GetPiece(oneSqrFw) == null)
            {
                possibleMoves.Add(new ChessMove(startPos, oneSqrFw));

                pawnDouble(startPos, pawn, whichWay, possibleMoves);
            }

            pawnCapture(startPos, pawn, whichWay, -1, possibleMoves);
            pawnCapture(startPos, pawn, whichWay, 1, possibleMoves);
            addEnPassantMoves(startPos, pawn, possibleMoves);
        }

        private void pawnCapture (BoardPosition startPos, ChessPiece pawn, int whichWay, int colTo, List<ChessMove> possibleMoves)
        {
            BoardPosition capture = new BoardPosition(startPos.row + whichWay, startPos.column + colTo);
            if (capture.IsInsideBoard() == false)
            {
                return;
            }
            ChessPiece? target = game.GetPiece(capture);
            if (target != null && target.colour != pawn.colour)
            {
                possibleMoves.Add(new ChessMove(startPos, capture));
            }
        }

        private void pawnDouble(BoardPosition startPos, ChessPiece pawn, int whichWay, List<ChessMove> possibleMoves)
        {
            if (pawn.HasMoved)
            {
                return;
            }
            BoardPosition twoSqrFw = new BoardPosition(startPos.row + (2 * whichWay), startPos.column);

            if (game.GetPiece(twoSqrFw) == null)
            {
                possibleMoves.Add(new ChessMove(startPos, twoSqrFw));
            }
        }

        private void addEnPassantMoves(BoardPosition startPos, ChessPiece pawn, List<ChessMove> moves)
        {
            if (game.enPassantTarget == null)
            {
                return;
            }
            BoardPosition target = game.enPassantTarget.Value;

            int direction;
            if (pawn.colour == PieceColour.white)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }

            bool oneRowForward = target.row == startPos.row + direction;
            bool oneColumnSideways = Math.Abs(target.column - startPos.column) == 1;
            if (oneRowForward && oneColumnSideways)
            {
                moves.Add(new ChessMove(startPos, target));
            }
        }

        private void knight(BoardPosition startPos, ChessPiece knight, List<ChessMove> moves)
        {
            int[,] movesPossible = { { -2, 1 }, { -2, -1 }, { -1, -2 }, { -1, 2 }, { 1, -2 }, { 1, 2 }, { 2, -1 }, { 2, 1 } };
            for (int i = 0; i < movesPossible.GetLength(0); i++)
            {
                int rowTo = movesPossible[i, 0];
                int colTo = movesPossible[i, 1];
                BoardPosition endPos = new BoardPosition(startPos.row + rowTo, startPos.column + colTo);
                allowedMovesKnightKing(startPos, endPos, knight, moves);
            }
        }

        private void king(BoardPosition startPos, ChessPiece king, List<ChessMove> moves)
        {
            int[,] movesPossible = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
            for (int i = 0; i < movesPossible.GetLength(0); i++)
            {
                int rowTo = movesPossible[i, 0];
                int colTo = movesPossible[i, 1];
                BoardPosition endPos = new BoardPosition(startPos.row + rowTo, startPos.column + colTo);
                allowedMovesKnightKing(startPos, endPos, king, moves);
            }
            castling(startPos, king, moves);
        }
        private void castling(BoardPosition startPos, ChessPiece king, List<ChessMove> moves)
        {
            if (king.HasMoved)
            {
                return;
            }

            // Kingside castling
            BoardPosition kingsideRookPos = new BoardPosition(startPos.row, 7);
            ChessPiece? kingsideRook = game.GetPiece(kingsideRookPos);
            if (kingsideRook != null && kingsideRook.type == PieceType.rook && kingsideRook.colour == king.colour && !kingsideRook.HasMoved)
            {
                BoardPosition square1 = new BoardPosition(startPos.row, 5);
                BoardPosition square2 = new BoardPosition(startPos.row, 6);
                if (game.GetPiece(square1) == null && game.GetPiece(square2) == null)
                {
                    moves.Add(new ChessMove(startPos, square2));
                }
            }

            // Queenside castling
            BoardPosition queensideRookPos = new BoardPosition(startPos.row, 0);
            ChessPiece? queensideRook = game.GetPiece(queensideRookPos);

            if (queensideRook != null && queensideRook.type == PieceType.rook && queensideRook.colour == king.colour && !queensideRook.HasMoved)
            {
                BoardPosition square1 = new BoardPosition(startPos.row, 1);
                BoardPosition square2 = new BoardPosition(startPos.row, 2);
                BoardPosition square3 = new BoardPosition(startPos.row, 3);
                if (game.GetPiece(square1) == null && game.GetPiece(square2) == null && game.GetPiece(square3) == null)
                {
                    moves.Add(new ChessMove(startPos, square2));
                }
            }
        }

        private void bishop(BoardPosition startPos, ChessPiece bishop, List<ChessMove> moves)
        {
            int[,] directions = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int rowTo = directions[i, 0];
                int colTo = directions[i, 1];
                directionalMoves(startPos, bishop, rowTo, colTo, moves);
            }
        }

        private void rook(BoardPosition startPos, ChessPiece rook, List<ChessMove> moves)
        {
            int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int rowTo = directions[i, 0];
                int colTo = directions[i, 1];
                directionalMoves(startPos, rook, rowTo, colTo, moves);
            }
        }

        private void queen(BoardPosition startPos, ChessPiece queen, List<ChessMove> moves)
        {
            int[,] directions = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int rowTo = directions[i, 0];
                int colTo = directions[i, 1];
                directionalMoves(startPos, queen, rowTo, colTo, moves);
            }
        }




        //method that finds final destination of the piece
        private void allowedMovesKnightKing(BoardPosition startPos, BoardPosition endPos, ChessPiece piece, List<ChessMove> moves)
        {
            if (endPos.IsInsideBoard() == false)
            {
                return;
            }

            ChessPiece? target = game.GetPiece(endPos);
            if (target == null || target.colour != piece.colour) { moves.Add(new ChessMove(startPos, endPos)); }
        }

        //method that finds allowed squares along a directional line
        private void directionalMoves(BoardPosition startPos, ChessPiece movPc, int rowTo, int colTo, List<ChessMove> moves)
        {
            int currentRow = startPos.row + rowTo;
            int currentCol = startPos.column + colTo;

            while (new BoardPosition(currentRow, currentCol).IsInsideBoard())
            {
                BoardPosition endPos = new BoardPosition(currentRow, currentCol);
                ChessPiece? target = game.GetPiece(endPos);

                if (target == null) { moves.Add(new ChessMove(startPos, endPos)); }

                else
                {
                    if (target.colour != movPc.colour) { moves.Add(new ChessMove(startPos, endPos)); }
                    break;   
                }
                currentRow+= rowTo;
                currentCol+= colTo;
            }
        }
    }
}
