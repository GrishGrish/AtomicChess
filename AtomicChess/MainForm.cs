using AtomicChess.Game;
using AtomicChess.Models;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Text;
namespace AtomicChess
{
    public partial class MainForm : Form
    {
        private const int numSquares = 8;
        private const int squareSize = 60;
        //good programming prctice. Do i get marks for this ?

        private Button[,] buttons;

        private AtomicChessGame game;
        private BoardPosition? selectedPos;
        private MoveGenerator moveGenerator;
        private List<ChessMove> moves;

        

        private readonly Color lightSqrClr = Color.LightGreen;
        private readonly Color darkSqrClr = Color.DarkViolet;
        private readonly Color selectedSqrClr = Color.Yellow;
        private readonly Color possibleSqrCLrLight = Color.LightSeaGreen;
        private readonly Color possibleSqrClrDark = Color.DarkCyan;

        private Panel boardPanel;
        private Label turnLabel;
        private Label statusLabel;
        private Button restartButton;

        private bool gameOver;

        public MainForm()
        {
            InitializeComponent();

            buttons = new Button[numSquares, numSquares];

            game = new AtomicChessGame();
            moveGenerator = new MoveGenerator(game);
            moves = new List<ChessMove>();

            gameOver = false;

            SetUpWindow();
            CreateInterface();
            CreateVisualBoard();
            DisplayPieces();
            UpdateGameInformation();
        }

        private void SetUpWindow()
        {
            ClientSize = new Size(numSquares * squareSize + 220, numSquares * squareSize);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Atomic Chess";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }
        //just creating the board

        private void CreateInterface()
        {
            boardPanel = new Panel();
            boardPanel.Left = 0;
            boardPanel.Top = 0;
            boardPanel.Width = numSquares * squareSize;
            boardPanel.Height = numSquares * squareSize;
            boardPanel.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(boardPanel);

            turnLabel = new Label();
            turnLabel.Left = boardPanel.Right + 20;
            turnLabel.Top = 30;
            turnLabel.Width = 180;
            turnLabel.Height = 35;
            turnLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            Controls.Add(turnLabel);

            statusLabel = new Label();
            statusLabel.Left = boardPanel.Right + 20;
            statusLabel.Top = 85;
            statusLabel.Width = 180;
            statusLabel.Height = 70;
            statusLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            statusLabel.Text = "Game in progress";
            Controls.Add(statusLabel);

            restartButton = new Button();
            restartButton.Left = boardPanel.Right + 20;
            restartButton.Top = 180;
            restartButton.Width = 160;
            restartButton.Height = 45;
            restartButton.Text = "Restart Game";
            restartButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            restartButton.Click += restartButton_Click;
            Controls.Add(restartButton);
        }

        private void UpdateGameInformation()
        {
            if (gameOver)
            {
                turnLabel.Text = "Game Over";
                return;
            }
            if (game.currentTurn == PieceColour.white)
            {
                turnLabel.Text = "White's Turn";
            }
            else
            {
                turnLabel.Text = "Black's Turn";
            }
            statusLabel.Text = "Game in progress";
        }

        private void endGame(string message)
        {
            gameOver = true;

            statusLabel.Text = message;
            turnLabel.Text = "Game Over";

            MessageBox.Show(message, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            selectedPos = null;
            moves.Clear();

            for (int row = 0; row < numSquares; row++)
            {
                for (int col = 0; col < numSquares; col++)
                {
                    buttons[row, col].Enabled = false;
                }
            }
            refreshClrs();
            DisplayPieces();
        }

        private void restartButton_Click(object? sender, EventArgs e)
        {
            game = new AtomicChessGame();
            moveGenerator = new MoveGenerator(game);
            selectedPos = null;
            moves.Clear();
            gameOver = false;
            for (int row = 0; row < numSquares; row++)
            {
                for (int col = 0; col < numSquares; col++)
                {
                    buttons[row, col].Enabled = true;
                }
            }
            refreshClrs();
            DisplayPieces();
            UpdateGameInformation();
        }

        private void CreateVisualBoard()
        //actually making the board - squares are buttons
        {
            for (int row = 0; row < numSquares; row++)
            {
                for (int col = 0; col < numSquares; col++)
                {
                    Button button = new Button();
                    button.Width = squareSize;
                    button.Height = squareSize;
                    button.Left = col * squareSize;
                    button.Top = row * squareSize;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;

                    //had to add this line because the selection color was not working properly
                    button.UseVisualStyleBackColor = false;

                    //storing the info about the square that has been clicked
                    button.Tag = new BoardPosition(row, col);
                    button.Click += clickBtn;


                    //jst setting colours
                    BoardPosition position = new BoardPosition(row, col);
                    button.Tag = position;

                    Color sqrClr = GetSqrClr(position);
                    button.BackColor = sqrClr;
                    button.FlatAppearance.MouseOverBackColor = sqrClr;
                    button.FlatAppearance.MouseDownBackColor = sqrClr;

                    boardPanel.Controls.Add(button);
                    buttons[row, col] = button;
                }
            }
        }

        private void clickBtn(object? sender, EventArgs e)
        {
            if (gameOver)
            {
                return;
            }
            if (sender is not Button clickedButton)
            {
                return;
            }
            //if a button is clicked we initialize the variable clickedButton
            if (clickedButton.Tag is not BoardPosition clickedPos)
            {
                return;
            }

            ChessPiece? chosenPiece = game.board[clickedPos.row, clickedPos.column];
            if (selectedPos == null)
            {
                if ((chosenPiece == null)) { return; }
                if (chosenPiece.colour != game.currentTurn) { return; }
                selectedPos = clickedPos;
                moves = moveGenerator.GetChessMoves(clickedPos);
            }
            else
            {
                ChessMove? selectedMove = null;
                foreach (ChessMove move in moves)
                {
                    if (move.endPos.IsSameAs(clickedPos))
                    {
                        selectedMove = move;
                        break;
                    }
                }

                if ((selectedMove != null)) { Moving(selectedMove); }
                else if (selectedPos.Value.IsSameAs(clickedPos)) { selectedPos = null; moves.Clear(); }
                else if (chosenPiece != null && chosenPiece.colour == game.currentTurn)
                {
                    selectedPos = clickedPos;

                    moves = moveGenerator.GetChessMoves(clickedPos);
                }
                else
                {
                    selectedPos = null;
                    moves.Clear();
                }
            
            }
            refreshClrs();
            DisplayPieces();
        }

        private void Moving(ChessMove move)
        {
            ChessPiece? movingPc = game.board[move.startPos.row, move.startPos.column];
            if (movingPc == null)
            {
                return;
            }

            ChessPiece? targetPiece = game.board[move.endPos.row, move.endPos.column];

            bool castlingMove = isCastling(move, movingPc);
            bool enPassantMove = isEnPassantMove(move, movingPc);

            if (castlingMove)
            {
                castle(move, movingPc);

                game.enPassantTarget = null;
                game.enPassantPawnPos = null;
            }
            else if (enPassantMove)
            {
                if (kingSacrafice(move, movingPc.colour))
                {
                    MessageBox.Show(
                        "You cannot make a capture that explodes your own king.",
                        "Illegal Move",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (game.enPassantPawnPos == null)
                {
                    return;
                }

                explosion(move, game.enPassantPawnPos.Value);

                game.enPassantTarget = null;
                game.enPassantPawnPos = null;
            }
            else if (targetPiece != null)
            {
                if (kingSacrafice(move, movingPc.colour))
                {
                    MessageBox.Show("You cannot make a capture that explodes your own king", "Illegal Move", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                explosion(move);
                game.enPassantTarget = null;
                game.enPassantPawnPos = null;
            }
            else
            {
                game.board[move.endPos.row,move.endPos.column] = movingPc;
                game.board[move.startPos.row, move.startPos.column] = null;
                movingPc.HasMoved = true;
                updateEnPassant(move, movingPc);
                promotion(move.endPos);
            }

            selectedPos = null;
            moves.Clear();

            if (!isKingHere(PieceColour.white))
            {
                refreshClrs();
                DisplayPieces();

                endGame("Black wins by exploding the white king!");

                return;
            }
            if (!isKingHere(PieceColour.black))
            {
                refreshClrs();
                DisplayPieces();
                endGame("White wins by exploding the black king!");
                return;
            }
            game.ChangeTurn();
            UpdateGameInformation();
        }

        private void explosion(ChessMove move, BoardPosition? capturedPos = null)
        {
            int explosionRow = move.endPos.row;
            int explosionColumn = move.endPos.column;

            // Remove the capturing piece.
            game.board[move.startPos.row, move.startPos.column] = null;

            // For a normal capture, the captured piece is on endPos.
            // For en passant, it is on capturedPos.
            BoardPosition pieceToRemove;
            if (capturedPos != null)
            {
                pieceToRemove = capturedPos.Value;
            }
            else
            {
                pieceToRemove = move.endPos;
            }

            game.board[pieceToRemove.row, pieceToRemove.column] = null;

            for (int rowChange = -1; rowChange <= 1; rowChange++)
            {
                for (int colChange = -1; colChange <= 1; colChange++)
                {
                    BoardPosition nearbyPosition = new BoardPosition(explosionRow + rowChange,explosionColumn + colChange);
                    if (!nearbyPosition.IsInsideBoard())
                    {
                        continue;
                    }
                    ChessPiece? nearbyPiece = game.board[nearbyPosition.row, nearbyPosition.column];
                    if (nearbyPiece == null)
                    {
                        continue;
                    }
                    if (nearbyPiece.type == PieceType.pawn)
                    {
                        continue;
                    }
                    game.board[nearbyPosition.row, nearbyPosition.column] = null;
                }
            }
        }

        private Color GetSqrClr(BoardPosition pos)
        {
            if (selectedPos != null && selectedPos.Value.IsSameAs(pos)) { return selectedSqrClr; }
            foreach (ChessMove move in moves)
            {
                if (move.endPos.IsSameAs(pos) && (buttons[move.endPos.row, move.endPos.column].BackColor) == lightSqrClr) { return possibleSqrCLrLight; }
                else if (move.endPos.IsSameAs(pos) && (buttons[move.endPos.row, move.endPos.column].BackColor) == darkSqrClr) { return possibleSqrClrDark; }
            }
            if ((pos.row + pos.column) % 2 == 0) { return lightSqrClr; }
            else { return darkSqrClr; }
        }
        //so this method was created because i had a kind of repeating chunk of code in refresh colors method and the visual board method since they both set a colour to a square. I thought that there was a btter way to do this so i made another method which is then used in other 2
        

        private void refreshClrs()
        {
            for (int row = 0; row < numSquares; row++)
            {
                for (int col = 0; col < numSquares; col++)
                {
                    BoardPosition pos = new BoardPosition(row, col);
                    Color squareCLr = GetSqrClr(pos);

                    //also had to add this to fix the selected button color
                    buttons[row, col].BackColor = squareCLr;
                    buttons[row,col].FlatAppearance.MouseOverBackColor = squareCLr;
                    buttons[row,col].FlatAppearance.MouseDownBackColor = squareCLr;
                }
            }
        }

        private void DisplayPieces()
        {
            for (int row = 0; row < numSquares; row++)
            {
                for (int col = 0; col < numSquares; col++)
                {
                    ChessPiece? piece = game.board[row, col];
                    Button button = buttons[row, col];
                    button.BackgroundImage = GetPieceImg(piece);
                }
            }
        }

        private Image? GetPieceImg(ChessPiece? piece)
        {
            if (piece == null) return null;
            if (piece.colour == PieceColour.white)
            {
                switch (piece.type)
                {
                    case PieceType.pawn: return Properties.Resources.white_pawn;
                    case PieceType.rook: return Properties.Resources.white_rook;
                    case PieceType.knight: return Properties.Resources.white_knight;
                    case PieceType.bishop: return Properties.Resources.white_bishop;
                    case PieceType.queen: return Properties.Resources.white_queen;
                    case PieceType.king: return Properties.Resources.white_king;
                }
            }
            else
            {
                switch (piece.type)
                {
                    case PieceType.pawn: return Properties.Resources.black_pawn;
                    case PieceType.rook: return Properties.Resources.black_rook;
                    case PieceType.knight: return Properties.Resources.black_knight;
                    case PieceType.bishop: return Properties.Resources.black_bishop;
                    case PieceType.queen: return Properties.Resources.black_queen;
                    case PieceType.king: return Properties.Resources.black_king;
                }
            }
            return null;
        }


        private bool kingSacrafice(ChessMove move,PieceColour movingColour)
        {
            int explosionRow = move.endPos.row;
            int explosionColumn = move.endPos.column;

            for (int rowChange = -1; rowChange <= 1; rowChange++)
            {
                for (int colChange = -1; colChange <= 1; colChange++)
                {
                    BoardPosition position = new BoardPosition(
                        explosionRow + rowChange,
                        explosionColumn + colChange
                    );

                    if (!position.IsInsideBoard())
                    {
                        continue;
                    }
                    ChessPiece? piece = game.board[position.row, position.column];
                    if (piece != null && piece.type == PieceType.king && piece.colour == movingColour)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool isKingHere(PieceColour colour)
        {
            for (int row = 0; row < numSquares; row++)
            {
                for (int col = 0; col < numSquares; col++)
                {
                    ChessPiece? piece = game.board[row, col];
                    if (piece != null && piece.type == PieceType.king && piece.colour == colour)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void promotion(BoardPosition position)
        {
            ChessPiece? piece = game.board[position.row, position.column];

            if (piece == null)
            {
                return;
            }

            if (piece.type != PieceType.pawn)
            {
                return;
            }

            bool whitePromotion = piece.colour == PieceColour.white && position.row == 0;

            bool blackPromotion = piece.colour == PieceColour.black && position.row == 7;
            if (whitePromotion || blackPromotion)
            {
                game.board[position.row, position.column] = new ChessPiece(PieceType.queen, piece.colour);
            }
        }

        private bool isCastling(ChessMove move, ChessPiece movingPiece)
        {
            return movingPiece.type == PieceType.king && Math.Abs(move.endPos.column - move.startPos.column) == 2;
        }
        private void castle(ChessMove move, ChessPiece king)
        {
            int row = move.startPos.row;
            // Kingside
            if (move.endPos.column == 6)
            {
                ChessPiece? rook = game.board[row, 7];

                game.board[row, 6] = king;
                game.board[row, 4] = null;

                game.board[row, 5] = rook;
                game.board[row, 7] = null;

                if (rook != null)
                {
                    rook.HasMoved = true;
                }
            }
            // Queenside
            else if (move.endPos.column == 2)
            {
                ChessPiece? rook = game.board[row, 0];

                game.board[row, 2] = king;
                game.board[row, 4] = null;

                game.board[row, 3] = rook;
                game.board[row, 0] = null;

                if (rook != null)
                {
                    rook.HasMoved = true;
                }
            }
            king.HasMoved = true;
        }

        private bool isEnPassantMove(ChessMove move, ChessPiece movingPiece)
        {
            if (movingPiece.type != PieceType.pawn)
            {
                return false;
            }
            if (game.enPassantTarget == null)
            {
                return false;
            }
            if (!move.endPos.IsSameAs(game.enPassantTarget.Value))
            {
                return false;
            }

            ChessPiece? destinationPiece = game.board[move.endPos.row,move.endPos.column];
            return destinationPiece == null;
        }

        private void updateEnPassant(ChessMove move, ChessPiece movingPiece)
        {
            // By default, the previous opportunity expires.
            game.enPassantTarget = null;
            game.enPassantPawnPos = null;

            if (movingPiece.type != PieceType.pawn)
            {
                return;
            }
            int rowDistance = Math.Abs(move.endPos.row - move.startPos.row);
            if (rowDistance != 2)
            {
                return;
            }
            int middleRow = (move.startPos.row + move.endPos.row) / 2;
            game.enPassantTarget = new BoardPosition(middleRow, move.startPos.column);
            game.enPassantPawnPos = move.endPos;
        }
    }
}
