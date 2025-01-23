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


            //Console.WriteLine(Util.PolyominoChecker(9));

            // Build the states and transitions tables
            int generateStates = ExpandState("00000000", (byte)0xFF);

            int j = 0;
            Parallel.For(0, 1_000_000_000, i =>
            {
                j++;
                //Count0Islands((ulong)i);
            });
            Console.WriteLine();
            Console.WriteLine(j);


            Dictionary<ulong, ulong> worstOffenders = new Dictionary<ulong, ulong>();

            Util.



            //worstOffenders.Add(258, 2);
            //worstOffenders.Add(772, 4);
            //worstOffenders.Add(1800, 8);
            //worstOffenders.Add(3856, 16);
            //worstOffenders.Add(7968, 32);
            //worstOffenders.Add(16192, 64);
            //worstOffenders.Add(32640, 128);
            //worstOffenders.Add(65280, 256);
            //worstOffenders.Add(130560, 512);
            //worstOffenders.Add(261120, 1024);
            //worstOffenders.Add(522240, 2048);
            //worstOffenders.Add(1044480, 4096);
            //worstOffenders.Add(2088960, 8192);
            worstOffenders.Add(4177920, 16384);
            worstOffenders.Add(8355840, 32768);
            //worstOffenders.Add(16711680, 64);
            //worstOffenders.Add(33423360, 64);
            //worstOffenders.Add(66846720, 64);
            //worstOffenders.Add(133693440, 64);
            //worstOffenders.Add(267386880, 64);
            //worstOffenders.Add(534773760, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);
            //worstOffenders.Add(, 64);

            Action action = () =>
            {
                for (ulong i = 0; i < 10_000_000; i++)
                {
                    if (worstOffenders.ContainsKey(i))
                    {
                        //PrintBitboard(i);
                        //Console.WriteLine($"Skipping ahead by {worstOffenders[i]}");
                        i += worstOffenders[i];
                        //PrintBitboard(i);
                    }
                    Count0Islands(i);
                }
            };

            Action action2 = () =>
            {
                for (ulong i = 0; i < 10_000_000; i++)
                {
                    Count0Islands(i);
                }
            };

            TimeAction(action, 100);
            TimeAction(action2, 100);


            //#region test data
            //List<bool> test = new();
            //test.Add(false); 
            //test.Add(true); // 1
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(false);
            //test.Add(false);
            //test.Add(false);
            //test.Add(true); // 8
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(false);
            //test.Add(true); //14
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(false);
            //test.Add(true); //20
            //test.Add(true);
            //test.Add(false);
            //test.Add(true); //23
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(true);
            //test.Add(false);
            //#endregion

            //List<ulong> indices = new List<ulong>();
            //ulong startIndex = 3_000_000_001;
            //ulong offsetIndex = 1_000_000_000; // How many to do
            //bool reset = false;
            //if (Count0Islands(0) == 1)
            //{
            //    indices.Add(0);
            //}
            //else { 
            //    reset = true;
            //}
            //// 0 gets taken care of before entering the loop
            //for (ulong i = startIndex; i < startIndex + offsetIndex; i++)
            //{
            //    if (Count0Islands(i) != 1)
            //    {
            //        reset = true;
            //    }
            //    if (Count0Islands(i) == 1 && reset == true)
            //    {
            //        reset = false;
            //        indices.Add(i);
            //    }
            //}

            //Console.WriteLine(indices.Count);

            //foreach (int i in indices)
            //{
            //    Console.WriteLine(i);
            //}







            // Basic
            //List<int> indices = new List<int>();
            //bool reset = false;
            //if (test[0] == true)
            //{
            //    indices.Add(0);
            //}
            //else
            //{
            //    reset = true;
            //}
            //for (int i = 1; i < test.Count; i++)
            //{
            //    if (test[i] == false)
            //    {
            //        reset = true;
            //    }
            //    if (test[i] == true && reset == true)
            //    {
            //        reset = false;
            //        indices.Add(i);
            //    }
            //}









            //foreach (ulong puzzle in puzzles)
            //{
            //    PrintBitboard(puzzle);
            //}


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