using System.Buffers.Text;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Primary_Puzzle_Solver
{
    class Program
    {
        public static void Main()
        {
            // Set up initial board with fixed walls
            Bitboard bitboard = new Bitboard(1UL, 6);

            int startState = bitboard.State;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var solutions = bitboard.Solutions().ToList();


            Console.WriteLine(stopwatch.Elapsed.Milliseconds);

            bitboard.PrintSolution(solutions[3575], startState);

            //foreach(var solution in solutions)
            //{
            //    bitboard.SetState(solution.Key & 0x3f, (solution.Key >> 6) & 0x3f, (solution.Key >> 12) & 0x3f);
            //    bitboard.PrintBitboard();
            //}
        }
    }
}