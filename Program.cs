using System.Buffers.Text;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using static Primary_Puzzle_Solver.Util;

// Paths on home PC
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin";
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted.bin";
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8 inverted.bin";
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8 inverted reversed.bin";
// @"C:\Users\rober\Documents\Primary Puzzles\No intersections\No intersections.bin"
// @"C:\Users\rober\Documents\Primary Puzzles\Originals"
// string originals = @"C:\Users\rober\Documents\Primary Puzzles\Originals";
// string goodOnes = @"C:\Users\rober\Documents\Primary Puzzles\Good ones";

// Paths on laptop
// @"C:\Users\Rober\Documents\Puzzles Master List Canonical Sorted 8 by 8 inverted.bin";
// @"C:\Users\Rober\Documents\Puzzles separated for Primary\Bitboards 6 x 6.bin";
// @"C:\Users\Rober\Documents\Primary Puzzles Good Ones";
namespace Primary_Puzzle_Solver
{
    // Current primary goal: Create a method to check if a bitboard is a hallway with no 3- or 4-way intersections
    class Program
    {
        public static void Main()
        {
            int startState = ExpandState("00000000", (byte)0xFF);


            MakeGenState(0, startState, 0);
            ulong TOTAL_POLYMINOES = genStates[0].LastOrDefault().cumulativePaths;

            //ulong startIndex = 1_000_000;
            //ulong offset = 10;
            //int maxSolutions = 0;
            //ulong maxBitboard = 0;
            //for (ulong i = startIndex; i < startIndex + offset; i++)
            //{
            //    Bitboard bitboard = new Bitboard(GetNthPolimyno(i), 8);
            //    maxSolutions = Math.Max(bitboard.Solutions().Count, maxSolutions);
            //    if( maxSolutions == bitboard.Solutions().Count)
            //    {
            //        maxBitboard = GetNthPolimyno(i);
            //    }
            //}

            Bitboard bitboard = new Bitboard(3817552, 8);

            var solutions = bitboard.Solutions();

            bitboard.PrintSolution(solutions, solutions.Count - 1);
            
        }
    }
}