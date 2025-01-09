using System.Buffers.Text;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using static Primary_Puzzle_Solver.Util;

// Paths on home PC
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin"
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted.bin"
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8 inverted.bin"

// Paths on laptop
// @"C:\Users\Rober\Documents\Puzzles Master List Canonical Sorted 8 by 8 inverted.bin"
// @"C:\Users\Rober\Documents\Puzzles separated for Primary\Bitboards 6 x 6.bin"
namespace Primary_Puzzle_Solver
{
    class Program
    {
        public static void Main()
        {
            //// Separates the files
            string filePath = @"C:\Users\Rober\Documents\Master List.bin";
            string directory = @"C:\Users\Rober\Documents\Puzzles separated for Primary n x m";

            int offset = 0;

            SeparatePuzzlesBySize(filePath, directory);




            // Go through each file and determine if it would be better to save it as the original or in an inverted format






            //Util.PrintBitboardRange(@"C:\Users\Rober\Documents\Puzzles separated for Primary\Bitboards 6 x 6.bin", 20000000, 100);

            // Testing GetSize()
            //Console.WriteLine(GetSize(16712796644520755199));






            //for (int i = offset; i < offset + 10; i++)
            //{
            //    // Index from the file
            //    Console.WriteLine($"i: {i}");
            //    PrintBitboardFromFile(filePath, i);

            //}




            //Bitboard bitboard = new Bitboard(~6765632493369UL, 8);
            //var solutions = bitboard.Solutions().ToList();
            //HashSet<int> states = new HashSet<int>();

            //foreach (var solution in solutions)
            //{
            //    states.Add(solution.Key);
            //    foreach(var directionList in solution.Value)
            //    {

            //        Console.Write(directionList.ToString() + " ");
            //    }
            //    Console.WriteLine();
            //}

            //bitboard.PrintSolution(solutions[^1], solutions[0].Key, new Bitboard(~6765632493369UL, 8));
            //states.ToList().Sort();
            //foreach(var state in states)
            //{
            //    Console.WriteLine(state);
            //}
            //Console.WriteLine();



            //Bitboard bitboard = new Bitboard(0UL, 1);


            //ulong value = 7479;
            //(int width, int height) dimensions = GetSize(~value);
            ////(int width, int height) dimensions = (8, 8);
            //value = Convert8x8ToNxM(~value);
            //Console.WriteLine("Original");
            //PrintBitboard(value, dimensions.width, dimensions.height);
            //Console.WriteLine("Inverted");
            //PrintBitboard(~value, dimensions.width, dimensions.height);

            //// This is a test board will the work out the GetSize() on.
            //// 18446744073709089220
            //PrintBitboard(18446744073709089220, 8, 8);
            //var x = GetSize(18446744073709089220);

            //Console.WriteLine($"Width: {x.Width}, Height: {x.Height}");

            //Console.WriteLine(Convert8x8ToNxM(18446744073709089220));

            //PrintBitboard(~29627UL, 6, 3);


            //PrintBitboard(CreateColMask(3, 4), 4);
            //Console.WriteLine();


            //PrintBitboard(~18446744073709535470UL);
            //Bitboard bitboard = new Bitboard(~18446744073709535470UL, 8);
            //bitboard.PrintBitboard();

            ////bitboard.SetState(4487);
            ////bitboard.PrintBitboard();

            ////bitboard.MoveToNewState(Bitboard.Direction.Down);
            ////bitboard.PrintBitboard();

            //var x = bitboard.Solutions().ToList();
            //Console.Clear();
            //bitboard.PrintSolution(x[^1], x[0].Key, new Bitboard(~18446744073709535470UL, 6));
            ////InvertFile(filePath);

        }
    }
}