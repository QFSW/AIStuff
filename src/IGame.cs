using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIStuff
{
    public enum TerminalGameResult
    {
        None = 0,
        Lose = 1,
        Tie = 2,
        Win = 3
    }

    public interface IGame<TMove, TPlayer> : ICloneable
    {
        bool HasWon(TPlayer player);
        bool ValidMove(TMove move, TPlayer player);
        bool MakeMove(TMove move, TPlayer player);
        float GetHeuristic(TPlayer player);
        TerminalGameResult TerminalStatus(TPlayer player);
        TMove[] GetAllMoves(TPlayer player);
        TPlayer NextPlayer(TPlayer currentPlayer);
    }
}
