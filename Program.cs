using System.Buffers.Text;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static Primary_Puzzle_Solver.Util;

// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin"
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted.bin"
namespace Primary_Puzzle_Solver
{
    class Program
    {
        public static void Main()
        {
            string filePath = @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin";
            //Bitboard bitboard = new Bitboard(1, 2);
            //bitboard.PrintBitboard();

            ////bitboard.SetState(4487);
            ////bitboard.PrintBitboard();

            ////bitboard.MoveToNewState(Bitboard.Direction.Down);
            ////bitboard.PrintBitboard();

            //var x = bitboard.Solutions().ToList();

            //bitboard.PrintSolution(x[^1], x[0].Key, new Bitboard(1, 3));

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    PrintBitboardFromFile(filePath, i);
                }
                catch { }
            }

        }
    }
}