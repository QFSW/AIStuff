using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIStuff
{
    public class TicTacToe : IGame<int, int>
    {
        private readonly char defaultChar = '-';
        private readonly char[] playerChars = new char[] { 'x', 'o' };
        private readonly int boardLength;
        private readonly int boardSize;

        private char[] board;

        public override string ToString()
        {
            string newStr = "";
            for (int i = 0; i < boardLength; i++)
            {
                for (int j = 0; j < boardLength; j++)
                {
                    newStr = $"{newStr}{board[i * boardLength + j]} ";
                }
                newStr = $"{newStr}\n";
            }

            return newStr;
        }

        public TicTacToe(int boardLength)
        {
            this.boardLength = boardLength;
            boardSize = boardLength * boardLength;
            board = new char[boardSize];

            for (int i = 0; i < boardSize; i++) { board[i] = defaultChar; }
        }

        public object Clone()
        {
            TicTacToe newGame = (TicTacToe)MemberwiseClone();
            newGame.board = new char[boardSize];
            Array.Copy(board, newGame.board, boardSize);
            return newGame;
        }

        public int[] GetAllMoves(int player)
        {
            List<int> moves = new List<int>(boardSize);
            for (int i = 0; i < boardSize; i++) { if (board[i] == defaultChar) { moves.Add(i); } }

            return moves.ToArray();
        }

        public float GetHeuristic(int player)
        {
            float heuristic = 0;
            for (int i = 0; i < boardLength; i++)
            {
                heuristic += EvaluateLine(player, i * boardLength, (i + 1) * boardLength - 1);
                heuristic += EvaluateLine(player, i, i * (boardLength + 1) - 1);
                heuristic -= EvaluateLine(1 - player, i * boardLength, (i + 1) * boardLength - 1);
                heuristic -= EvaluateLine(1 - player, i, i * (boardLength + 1) - 1);
            }

            return heuristic;
        }

        private float EvaluateLine(int player, int start, int end)
        {
            int x1 = start % boardLength;
            int x2 = end % boardLength;
            int y1 = start / boardLength;
            int y2 = end / boardLength;

            float highest = 0;
            float value = 0.1f;
            bool blocked = false;
            for (int i = y1; i <= y2; i++)
            {
                for (int j = x1; j <= x2; j++)
                {
                    if (board[i * boardLength + j] == playerChars[player]) { value *= 10; }
                    else
                    {
                        if (board[i * boardLength + j] == defaultChar)
                        {
                            blocked = false;
                            highest = Math.Max(highest, value);
                        }
                        else { blocked = true; }
                        value = 0.1f;
                    }
                }
            }

            if (!blocked) { highest = Math.Max(highest, value); }
            return highest;
        }

        public bool HasWon(int player)
        {
            bool hasWon;
            for (int i = 0; i < boardLength; ++i)
            {
                hasWon = true;
                for (int j = 0; j < boardLength; ++j)
                {
                    hasWon &= (board[i * boardLength + j] == playerChars[player]);
                }

                if (hasWon) { return true; }
            }

            for (int i = 0; i < boardLength; ++i)
            {
                hasWon = true;
                for (int j = 0; j < boardLength; ++j)
                {
                    hasWon &= (board[j * boardLength + i] == playerChars[player]);
                }

                if (hasWon) { return true; }
            }

            hasWon = true;
            for (int i = 0; i < boardLength; ++i)
            {
                hasWon &= (board[i * boardLength + i] == playerChars[player]);
            }
            if (hasWon) { return true; }

            hasWon = true;
            for (int i = 0; i < boardLength; ++i)
            {
                hasWon &= (board[(boardLength - i - 1) * boardLength + i] == playerChars[player]);
            }
            if (hasWon) { return true; }

            return false;
        }

        public bool MakeMove(int move, int player)
        {
            if (!(ValidMove(move, player))) { return false; }
            if (player < 0 || player >= 2) { return false; }

            board[move] = playerChars[player];
            return true;
        }

        public int NextPlayer(int currentPlayer)
        {
            return 1 - currentPlayer;
        }

        public TerminalGameResult TerminalStatus(int player)
        {
            if (HasWon(player)) { return TerminalGameResult.Win; }
            if (HasWon(1 - player)) { return TerminalGameResult.Lose; }

            for (int i = 0; i < boardSize; i++) { if (board[i] == defaultChar) { return TerminalGameResult.None; } }
            return TerminalGameResult.Tie;
        }

        public bool ValidMove(int move, int player)
        {
            if (move < 0 || move >= boardSize) { return false; }
            if (board[move] != defaultChar) { return false; }

            return true;
        }
    }
}
