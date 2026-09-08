using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using AtomicChess.Models;

//logical layout
namespace AtomicChess.Game
{
    public class AtomicChessGame
    {
        public BoardPosition? enPassantTarget { get; set; }
        public BoardPosition? enPassantPawnPos { get; set; }

        public ChessPiece?[,] board { get; private set; }
        //Research + fixing: we need the question mark because the cell may be empty - without a piece
        public PieceColour currentTurn { get; private set; }

        public AtomicChessGame()
        {
            board = new ChessPiece?[8, 8];
            currentTurn = PieceColour.white;
            //Cant use new with enums. Learnt that lesson...
            SetUpInitialPosition(); 
        }

        public ChessPiece? GetPiece(BoardPosition boardPosition)
        {
            if(boardPosition.IsInsideBoard() == false) return null;
            //checkign if inside the board
            return board[boardPosition.row, boardPosition.column];
        }

        private void SetUpInitialPosition()
        {
            placePawns(1, PieceColour.black);
            placePawns(6, PieceColour.white);

            placeMeatyPieces(0, PieceColour.black);
            placeMeatyPieces(7, PieceColour.white);
        }

        private void placePawns(int row, PieceColour pieceColour)
        {
            for (int column = 0; column < 8; column++)
            {
                board[row, column] = new ChessPiece(PieceType.pawn, pieceColour);
                // PieceColour and pieceColour are too simillar. Next time use something that is more obviously different
            }
        }

        private void placeMeatyPieces(int row, PieceColour colour) //not making the same mistake twice lol
        {
            board[row, 0] = new ChessPiece(PieceType.rook, colour);
            board[row, 1] = new ChessPiece(PieceType.knight, colour);
            board[row, 2] = new ChessPiece(PieceType.bishop, colour);
            board[row, 3] = new ChessPiece(PieceType.queen, colour);
            board[row, 4] = new ChessPiece(PieceType.king, colour);
            board[row, 5] = new ChessPiece(PieceType.bishop, colour);
            board[row, 6] = new ChessPiece(PieceType.knight, colour);
            board[row, 7] = new ChessPiece(PieceType.rook, colour);
        }

        public void ChangeTurn()
        {
            if (currentTurn == PieceColour.white)
            {
                currentTurn = PieceColour.black;
            }
            else { currentTurn = PieceColour.white; }
        }
    }
}
