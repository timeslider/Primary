using System.Buffers.Text;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using static Primary_Puzzle_Solver.Util;

// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin"
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted.bin"
// @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8 inverted.bin"
namespace Primary_Puzzle_Solver
{
    class Program
    {
        public static void Main()
        {
            string filePath = @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin";
            int offset = 0;
            HashSet<(int, int)> dimensions = new HashSet<(int, int)>();
            for (int i = offset; i < offset + 1000; i++)
            {
                Console.WriteLine($"i: {i}");
                //PrintBitboardFromFile(filePath, i);
                dimensions.Add(ProcessBitboardFromFile(filePath, i));
            }

            Bitboard bitboard = new Bitboard(~6765632493369UL, 8);
            var solutions = bitboard.Solutions().ToList();
            foreach (var solution in solutions)
            {
                foreach(var directionList in solution.Value)
                {
                    Console.Write(directionList.ToString() + " ");
                }
                Console.WriteLine();
            }
            //bitboard.PrintSolution(solutions[^1], solutions[0].Key, new Bitboard(~6765632493369UL, 8));

            Console.WriteLine();

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