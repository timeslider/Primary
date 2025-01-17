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
            // Check for 3-way intersections
            // This bitboard has no intersections: 18446732828894229745
            // It's one continues "hallway"

            // PC Version
            //SeparatePuzzles(@"C:\Users\Rober\Documents\Puzzles Master List Canonical Sorted 8 by 8 inverted reversed.bin", @"C:\Users\rober\Documents\Primary Puzzles\No intersections\No intersections.bin");

            // Laptop Version
            //SeparatePuzzles(@"C:\Users\Rober\Documents\Puzzles Master List Canonical Sorted 8 by 8 inverted reversed.bin", @"C:\Users\Rober\Documents\No intersections.bin");

            
            Console.WriteLine(Util.PolyominoChecker(9));
            //Action myAction = () =>
            //{
            //    List<ulong> polyominoes = new List<ulong>();
            //    Parallel.For(0, 68_719_4767, i =>
            //    {
            //        ulong u = (ulong)(i + (-(long.MinValue + 1))) + 1;
            //        if (PolyominoChecker(u) == true)
            //        {
            //            polyominoes.Add(u);
            //        }
            //    });
            //    Console.WriteLine(polyominoes.Count);
            //};

            //TimeAction(myAction, 1);

            Console.WriteLine();

            //foreach (ulong polyomino in polyominoes)
            //{
            //    PrintBitboard(polyomino);
            //}
            //PuzzlesToExclude(0UL);

            //PrintBitboardRange(@"C:\Users\rober\Documents\Primary Puzzles\No intersections\No intersections.bin", 0, 200);

            // Now let's remove based on more critiera

            // Don't include based on width and height

            // We need to invert the Originals

            // How to invert bitboards in nxm
            //string originals = @"C:\Users\rober\Documents\Primary Puzzles\Originals";
            //var binFiles = Directory.EnumerateFiles(originals, "*.bin");
            //foreach (var binFile in binFiles)
            //{
            //    (int width, int height) dimensions = FilePathToDimensions(binFile);
            //    int byteCount = GetByteCount(dimensions.width, dimensions.height);
            //    InvertFile(binFile, originals + @$"\Bitboards {dimensions.width} x {dimensions.height} inverted.bin", byteCount);
            //}



            //Action action = () =>
            //{
            //    string originals = @"C:\Users\rober\Documents\Primary Puzzles\Originals\Inverted";
            //    string goodOnes = @"C:\Users\rober\Documents\Primary Puzzles\Good ones";

            //    var binFiles = Directory.EnumerateFiles(originals, "*.bin");
            //    int i = 0;
            //    foreach (var binFile in binFiles)
            //    {
            //        Util.ProcessBinaryFileRead(binFile, reader =>
            //        {
            //            (int width, int height) dimensions = FilePathToDimensions(binFile);
            //            int byteCount = GetByteCount(dimensions.width, dimensions.height);
            //            while (reader.BaseStream.Position < reader.BaseStream.Length && i < 1000)
            //            {

            //                byte[] bytes = reader.ReadBytes(byteCount);

            //                //if(bytes.Length < 5)
            //                //{
            //                //    throw new EndOfStreamException();
            //                //}
            //                ulong value = 0;
            //                for (int j = 0; j < byteCount; j++)
            //                {
            //                    value |= (ulong)bytes[j] << (8 * j);
            //                }
            //                try
            //                {
            //                    Console.WriteLine("--------");
            //                    Bitboard bitboard = new Bitboard(value, dimensions.width, dimensions.height);
            //                    PrintBitboard(value, dimensions.width, dimensions.height);
            //                    bitboard.PrintBitboard();
            //                    Console.WriteLine($"Empty cell count: {bitboard.GetEmptyCellCount()}");
            //                    Console.WriteLine($"Solution count: {bitboard.Solutions().Count}");
            //                    Console.WriteLine("\n\n");
            //                }
            //                catch (Exception e)
            //                {
            //                    Console.WriteLine($"This is a bad board because: {e.Message}");
            //                    PrintBitboard(value, dimensions.width, dimensions.height);
            //                    Console.WriteLine("\n\n");
            //                };
            //                i++;
            //            }
            //        });

            //    }
            //};

            //TimeAction(action, 1);





            //Action action = () =>
            //{
            //    string originals = @"C:\Users\rober\Documents\Polyomino List\Original Data Don't Delete\Puzzles Master List Canonical Sorted 8 by 8 inverted reversed.bin";
            //    //string goodOnes = @"C:\Users\rober\Documents\Primary Puzzles\Good ones";

            //    int i = 0;
            //    ProcessBinaryFileRead(originals, reader =>
            //    {
            //        while (reader.BaseStream.Position < reader.BaseStream.Length && i < 250)
            //        {

            //            ulong value = reader.ReadUInt64();
            //            try
            //            {
            //                Console.WriteLine("--------");
            //                Bitboard bitboard = new Bitboard(value, 8);
            //                PrintBitboard(value, 8, 8);
            //                bitboard.PrintBitboard();
            //                Console.WriteLine($"Empty cell count: {bitboard.GetEmptyCellCount()}");
            //                Console.WriteLine($"Solution count: {bitboard.Solutions().Count}");
            //                Console.WriteLine($"Longest solution: {bitboard.Solutions().ToList()[^1].Value.Count}");
            //                Console.WriteLine($"");
            //                Console.WriteLine("\n\n");
            //            }
            //            catch (Exception e)
            //            {
            //                Console.WriteLine($"This is a bad board because: {e.Message}");
            //                PrintBitboard(value, 8, 8);
            //                Console.WriteLine("\n\n");
            //            };
            //            i++;
            //        }
            //    });
            //};

            //TimeAction(action, 1);








            //Bitboard bitboard = new Bitboard(18446744073709548004UL, 8, 8);
            //Dictionary<int, List<Bitboard.Direction>> x = bitboard.Solutions();

            //bitboard.PrintSolution(x, 10);

            //Bitboard bitboard = new Bitboard(18446744073709486341UL, 4);
            //PrintBitboard(549791UL, 5, 6);
            //PrintBitboard(0x1f4, 3, 3);
            //Bitboard bitboard = new Bitboard(0xfffffffffff3ffff, 8);
            //bitboard.PrintBitboard();

            //Dictionary<int, List<Bitboard.Direction>>.KeyCollection solutions = bitboard.Solutions().Keys;

            //Console.WriteLine(  );




            //    HashSet<ulong> set = new HashSet<ulong>();
            //    for(ulong i = 0; i < 1; i++)
            //    {
            //        set.Add(i);
            //    }
            //    Console.WriteLine();
            //}

            //public struct Hash()
            //{
            //    public ulong value;
            //}

        }
    }
}