using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using Tile_Slayer;

namespace Primary_Puzzle_Solver
{
    class Program
    {
        public static void Main()
        {
            Bitboard bitboard = new Bitboard(65540UL, 6, 3);
            bitboard.SetState(9, 10, 11);
            bitboard.PrintBitboard();
            bitboard.GetNewState(Bitboard.Direction.Down);
            bitboard.PrintBitboard();
        }
    }
}