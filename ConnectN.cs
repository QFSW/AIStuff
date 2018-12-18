﻿using System;
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
        private readonly int boardLength;
        private readonly int boardSize;
        private readonly int connectLength;

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

        public ConnectN(int boardLength, int connectLength)
        {
            this.boardLength = boardLength;
            this.connectLength = connectLength;
            boardSize = boardLength * boardLength;
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
            List<int> moves = new List<int>(boardLength);
            for (int i = 0; i < boardLength; i++) { if (board[i] == defaultChar) { moves.Add(i); } }

            return moves.ToArray();
        }

        public float GetHeuristic(int player)
        {
            float LocalHeuristic(int pl)
            {
                float heuristic = 0;
                for (int i = 0; i < boardLength; i++)
                {
                    heuristic += EvaluateLine(pl, i * boardLength, (i + 1) * boardLength - 1);
                    heuristic += EvaluateLine(pl, i, i + boardLength * (boardLength - 1));
                    heuristic += EvaluateDiagonal(pl, i, 1, 1);
                    heuristic += EvaluateDiagonal(pl, i, -1, 1);
                    heuristic += EvaluateDiagonal(pl, i * boardLength, 1, 1);
                    heuristic += EvaluateDiagonal(pl, i * boardLength, 1, -1);
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
            int x1 = start % boardLength;
            int x2 = end % boardLength;
            int y1 = start / boardLength;
            int y2 = end / boardLength;

            for (int i = y1; i <= y2; i++)
            {
                for (int j = x1; j <= x2; j++)
                {
                    yield return board[i * boardLength + j];
                }
            }
        }

        private float EvaluateDiagonal(int player, int start, int dir1, int dir2)
        {
            return EvaluateSequence(player, GetDiagonal(start, dir1, dir2));
        }

        private IEnumerable<char> GetDiagonal(int start, int dir1, int dir2)
        {
            int i = start % boardLength;
            int j = start / boardLength;

            bool valid = true;
            while (valid)
            {
                int index = j * boardLength + i;
                valid = (index >= 0 && index < boardSize);
                if (valid)
                {
                    yield return board[index];

                    i += dir1;
                    j += dir2;

                    if (i >= boardLength || i < 0) { valid = false; }
                    if (j >= boardLength || j < 0) { valid = false; }
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

            for (int i = boardLength - 1;  i >= 0; i--)
            {
                if (board[i * boardLength + move] == defaultChar)
                {
                    board[i * boardLength + move] = playerChars[player];
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
            if (move < 0 || move >= boardLength) { return false; }
            if (board[move] != defaultChar) { return false; }

            return true;
        }
    }
}
