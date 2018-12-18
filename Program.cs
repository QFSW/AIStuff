using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            IGame<int, int> game = new ConnectN(7, 6, 4);
            Console.WriteLine(game);

            int turn = 0;
            int move;
            bool moveValid;
            while (game.TerminalStatus(turn) == TerminalGameResult.None)
            {
                do
                {
                    if (turn == 0)
                    {
                        Console.WriteLine($"p{(turn + 1)} turn: ");
                        try { move = int.Parse(Console.ReadLine()); }
                        catch { move = -1; }
                    }
                    else
                    {
                        Console.WriteLine($"p{(turn + 1)} is thinking...");
                        move = MiniMax.GetBestMove(game, turn, 5);
                    }

                    moveValid = game.MakeMove(move, turn);
                } while (!moveValid);

                Console.WriteLine(game);
                turn = 1 - turn;
            }

            if (game.TerminalStatus(0) == TerminalGameResult.Tie) { Console.WriteLine("It's a tie!"); }
            else { Console.WriteLine($"p{2 - turn} has won!"); }

            Console.ReadLine();
        }
    }
}
