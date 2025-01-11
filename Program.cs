using System.Buffers.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using static Primary_Puzzle_Solver.Util;

// Paths on home PC
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin"
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted.bin"
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8 inverted.bin"
// @"C:\Users\rober\Documents\Primary Puzzles\Originals"
// string originals = @"C:\Users\rober\Documents\Primary Puzzles\Originals";
// string goodOnes = @"C:\Users\rober\Documents\Primary Puzzles\Good ones";

// Paths on laptop
// @"C:\Users\Rober\Documents\Puzzles Master List Canonical Sorted 8 by 8 inverted.bin"
// @"C:\Users\Rober\Documents\Puzzles separated for Primary\Bitboards 6 x 6.bin"
// @"C:\Users\Rober\Documents\Primary Puzzles Good Ones"
namespace Primary_Puzzle_Solver
{
    class Program
    {
        public static void Main()
        {
            // Now let's remove based on more critiera

            // Don't include based on width and height

            //Action action = () =>
            //{
            //    string originals = @"C:\Users\rober\Documents\Primary Puzzles\Originals";
            //    string goodOnes = @"C:\Users\rober\Documents\Primary Puzzles\Good ones";

            //    var binFiles = Directory.EnumerateFiles(originals, "*.bin");

            //    foreach (var binFile in binFiles)
            //    {
            //        if (binFile.Contains("4 x 4"))
            //        {
            //            GetStatistics(binFile);
            //        }
            //    }
            //};

            //TimeAction(action, 1);

            //Bitboard bitboard = new Bitboard(18446744073709486341UL, 4);

            

            HashSet<ulong> set = new HashSet<ulong>();
            for(ulong i = 0; i < 1; i++)
            {
                set.Add(i);
            }
            Console.WriteLine();
        }

        public struct Hash()
        {
            public ulong value;
        }


    }
}