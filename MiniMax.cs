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
            float[] results = new float[moves.Length];

            heuristic = float.NegativeInfinity;
            bestMove = default(TMove);

            for (int i = 0; i < moves.Length; i++)
            {
                if (heuristic < float.PositiveInfinity)
                {
                    IGame<TMove, TPlayer> gameCopy = (IGame<TMove, TPlayer>)game.Clone();
                    gameCopy.MakeMove(moves[i], player);

                    TerminalGameResult state = gameCopy.TerminalStatus(player);
                    if (state == TerminalGameResult.Win) { results[i] = float.PositiveInfinity; }
                    else if (state == TerminalGameResult.Lose) { results[i] = float.NegativeInfinity; }
                    else if (state == TerminalGameResult.Tie) { results[i] = 0; }
                    else
                    {
                        if (depth > 0)
                        {
                            GetBestMove(gameCopy, gameCopy.NextPlayer(player), depth - 1, out float newHeuristic, out TMove newMove);
                            results[i] = -newHeuristic;
                        }
                        else { results[i] = -gameCopy.GetHeuristic(gameCopy.NextPlayer(player)); }
                    }

                    if (results[i] >= heuristic)
                    {
                        heuristic = results[i];
                        bestMove = moves[i];
                    }
                }
            }

            if (moves.Length == 0)
            {
                TerminalGameResult state = game.TerminalStatus(player);
                if (state == TerminalGameResult.Win) { heuristic = 1; }
                else if (state == TerminalGameResult.Lose) { heuristic = -1; }
                else if (state == TerminalGameResult.Tie) { heuristic = 0; }
            }
        }
    }
}
