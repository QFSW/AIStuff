using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIStuff
{
    public class ConnectN : IGame<int, int>
    {
        private readonly char defaultChar = '-';
        private readonly char[] playerChars = new char[] { 'x', 'o' };
        private readonly int boardWidth;
        private readonly int boardHeight;
        private readonly int boardSize;
        private readonly int connectLength;

        private char[] board;

        public override string ToString()
        {
            string newStr = "";
            for (int i = 0; i < boardHeight; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                {
                    newStr = $"{newStr}{board[i * boardWidth + j]} ";
                }
                newStr = $"{newStr}\n";
            }

            return newStr;
        }

        public ConnectN(int boardWidth, int boardHeight, int connectLength)
        {
            this.boardWidth = boardWidth;
            this.boardHeight = boardHeight;
            this.connectLength = connectLength;
            boardSize = boardWidth * boardHeight;
            board = new char[boardSize];

            for (int i = 0; i < boardSize; i++) { board[i] = defaultChar; }
        }

        public object Clone()
        {
            ConnectN newGame = (ConnectN)MemberwiseClone();
            newGame.board = new char[boardSize];
            Array.Copy(board, newGame.board, boardSize);
            return newGame;
        }

        public int[] GetAllMoves(int player)
        {
            List<int> moves = new List<int>(boardWidth);
            for (int i = 0; i < boardWidth; i++) { if (board[i] == defaultChar) { moves.Add(i); } }

            return moves.ToArray();
        }

        public float GetHeuristic(int player)
        {
            float LocalHeuristic(int pl)
            {
                float heuristic = 0;
                for (int i = 0; i < boardWidth; i++)
                {
                    heuristic += EvaluateLine(pl, i, i + boardWidth * (boardHeight - 1));
                    heuristic += EvaluateDiagonal(pl, i, 1, 1);
                    heuristic += EvaluateDiagonal(pl, i, -1, 1);
                    heuristic += EvaluateDiagonal(pl, i + boardWidth * (boardHeight - 1), 1, -1);
                    heuristic += EvaluateDiagonal(pl, i + boardWidth * (boardHeight - 1), -1, -1);
                }

                for (int i = 0; i < boardHeight; i++)
                {
                    heuristic += EvaluateLine(pl, i * boardWidth, (i + 1) * boardWidth - 1);
                }

                return heuristic;

            }

            return LocalHeuristic(player) - LocalHeuristic(1 - player);
        }

        private float EvaluateLine(int player, int start, int end)
        {
            return EvaluateSequence(player, GetLine(start, end));
        }

        private IEnumerable<char> GetLine(int start, int end)
        {
            int x1 = start % boardWidth;
            int x2 = end % boardWidth;
            int y1 = start / boardWidth;
            int y2 = end / boardWidth;

            for (int i = y1; i <= y2; i++)
            {
                for (int j = x1; j <= x2; j++)
                {
                    yield return board[i * boardWidth + j];
                }
            }
        }

        private float EvaluateDiagonal(int player, int start, int dir1, int dir2)
        {
            return EvaluateSequence(player, GetDiagonal(start, dir1, dir2));
        }

        private IEnumerable<char> GetDiagonal(int start, int dir1, int dir2)
        {
            int i = start % boardWidth;
            int j = start / boardWidth;

            bool valid = true;
            while (valid)
            {
                int index = j * boardWidth + i;
                valid = (index >= 0 && index < boardSize);
                if (valid)
                {
                    yield return board[index];

                    i += dir1;
                    j += dir2;

                    if (i >= boardWidth || i < 0) { valid = false; }
                    if (j >= boardHeight || j < 0) { valid = false; }
                }
            }
        }

        private float EvaluateSequence(int player, IEnumerable<char> sequence)
        {
            const float baseValue = 1;
            int highestPossibleChain = 0;
            int possibleChain = 0;
            float highest = 0;
            float value = baseValue;

            foreach (char item in sequence)
            {
                if (item == playerChars[player])
                {
                    value *= 10;
                    possibleChain++;
                }
                else
                {
                    if (item == defaultChar) { possibleChain++; }
                    else
                    {
                        highestPossibleChain = Math.Max(highestPossibleChain, possibleChain);
                        possibleChain = 0;
                    }

                    highest = Math.Max(highest, value);
                    value = baseValue;
                }
            }

            highestPossibleChain = Math.Max(highestPossibleChain, possibleChain);
            highest = Math.Max(highest, value);
            if (highestPossibleChain < connectLength) { return 0; }
            if (highest >= baseValue * Math.Pow(10, connectLength)) { return float.PositiveInfinity; }
            return highest;
        }

        public bool HasWon(int player) { return float.IsPositiveInfinity(GetHeuristic(player)); }

        public bool MakeMove(int move, int player)
        {
            if (!(ValidMove(move, player))) { return false; }
            if (player < 0 || player >= 2) { return false; }

            for (int i = boardHeight - 1;  i >= 0; i--)
            {
                if (board[i * boardWidth + move] == defaultChar)
                {
                    board[i * boardWidth + move] = playerChars[player];
                    return true;
                }
            }

            return false;
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
            if (move < 0 || move >= boardWidth) { return false; }
            if (board[move] != defaultChar) { return false; }

            return true;
        }
    }
}
