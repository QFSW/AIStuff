using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIStuff
{
    static class MiniMax
    {
        static public TMove GetBestMove<TMove, TPlayer>(IGame<TMove, TPlayer> game, TPlayer player, int maxDepth)
        {
            GetBestMove(game, player, maxDepth, out float heuristic, out TMove bestMove);
            return bestMove;
        }

        static private void GetBestMove<TMove, TPlayer>(IGame<TMove, TPlayer> game, TPlayer player, int depth, out float heuristic, out TMove bestMove)
        {
            TMove[] moves = game.GetAllMoves(player);
            List<IGame<TMove, TPlayer>> queuedMoves = new List<IGame<TMove, TPlayer>>();

            heuristic = float.NegativeInfinity;
            bestMove = default(TMove);

            for (int i = 0; i < moves.Length; i++)
            {
                if (!float.IsPositiveInfinity(heuristic))
                {
                    IGame<TMove, TPlayer> gameCopy = (IGame<TMove, TPlayer>)game.Clone();
                    gameCopy.MakeMove(moves[i], player);

                    TerminalGameResult state = gameCopy.TerminalStatus(player);
                    float newHeuristic = float.NegativeInfinity;

                    if (state == TerminalGameResult.Win) { newHeuristic = float.PositiveInfinity; }
                    else if (state == TerminalGameResult.Lose) { newHeuristic = float.NegativeInfinity; }
                    else if (state == TerminalGameResult.Tie) { newHeuristic = 0; }
                    else { queuedMoves.Add(gameCopy); }

                    if (newHeuristic >= heuristic)
                    {
                        heuristic = newHeuristic;
                        bestMove = moves[i];
                    }
                }
            }

            for (int i = 0; i < queuedMoves.Count; i++)
            {
                if (!float.IsPositiveInfinity(heuristic))
                {
                    float newHeuristic;
                    if (depth > 0)
                    {
                        GetBestMove(queuedMoves[i], queuedMoves[i].NextPlayer(player), depth - 1, out newHeuristic, out TMove newMove);
                        newHeuristic = -newHeuristic;
                    }
                    else { newHeuristic = -queuedMoves[i].GetHeuristic(queuedMoves[i].NextPlayer(player)); }

                    if (newHeuristic >= heuristic)
                    {
                        heuristic = newHeuristic;
                        bestMove = moves[i];
                    }
                }
            }
        }
    }
}
