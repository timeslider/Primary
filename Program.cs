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
            //string filePath = @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8.bin";
            //int offset = 1000;
            //for (int i = offset; i < offset + 100; i++)
            //{
            //    try
            //    {
            //        PrintBitboardFromFile(filePath, i);
            //    }
            //    catch { }
            //}

            // This is a test board will the work out the GetSize() on.
            // 18446744073709089220
            PrintBitboard(18446744073709089220, 8, 8);
            var x = GetSize(18446744073709089220);

            Console.WriteLine($"Width: {x.Width}, Height: {x.Height}");

            Console.WriteLine(Convert8x8ToNxM(18446744073709089220));

            PrintBitboard(~29627UL, 6, 3);


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