using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;


[assembly: InternalsVisibleTo("UtilTests")]

namespace Tile_Slayer
{
    /// <summary>
    /// Represents a bitboard of size x, y.
    /// </summary>
    internal class Bitboard
    {

        private ulong bitboardValue;
        public ulong BitboardValue { get { return bitboardValue; } }

        private readonly int sizeX;
        public int SizeX { get { return sizeX; } }

        private readonly int sizeY;
        public int SizeY { get { return sizeY; } }

        private bool isSquare = false;

        private int red = 0;
        private int yellow = 0;
        private int blue = 0;
        private int state = 0;
        public int State { get { return state; } set { SetState(); } }
        private List<ulong> boundaries = new();


        public Bitboard(ulong bitboard, int sizeX)
            : this(bitboard, sizeX, sizeX)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitboard"></param>
        /// <param name="size"></param>
        /// <exception cref="ArgumentOutOfRangeException">size must be greater than zero and less than 9</exception>
        public Bitboard(ulong bitboard, int sizeX, int sizeY)
        {
            CheckBounds(bitboard, sizeX, sizeY);

            this.bitboardValue = bitboard;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            if (this.SizeX == this.SizeY)
            {
                this.isSquare = true;
            }
            GetInitialState();
            GetBoundaries();
        }


        public ulong SetBitboardCell(int x, int y, bool value = true)
        {
            CheckBounds(x, y);
            int bitPosition = y * SizeX + x;

            if (value == true)
            {
                return bitboardValue |= (1UL << bitPosition);
            }
            else
            {
                return bitboardValue &= ~(1UL << bitPosition);
            }
        }

        public bool GetBitboardCell(int x, int y)
        {
            CheckBounds(x, y);
            int bitPosition = y * SizeX + x;
            return (bitboardValue & (1UL << bitPosition)) != 0;
        }

        // Needs testing
        public bool GetBitboardCell(int index)
        {
            return (bitboardValue & (1UL << index)) != 0;
        }

        public void PrintBitboard(bool invert = false)
        {
            StringBuilder sb = new StringBuilder();

            // Prints the puzzle ID so we always know which puzzle we are displaying
            sb.Append(bitboardValue + "\n");

            for (int row = 0; row < SizeY; row++)
            {
                for (int col = 0; col < SizeX; col++)
                {
                    if (row * sizeX + col == red)
                    {
                        sb.Append("R ");
                        continue;
                    }
                    if (row * sizeX + col == yellow)
                    {
                        sb.Append("Y ");
                        continue;
                    }

                    if (row * sizeX + col == blue)
                    {
                        sb.Append("B ");
                        continue;
                    }

                    if (invert == true)
                    {
                        sb.Append(GetBitboardCell(col, row) ? "- " : "1 ");

                    }
                    else
                    {
                        sb.Append(GetBitboardCell(col, row) ? "1 " : "- ");
                    }
                }
                sb.Append('\n');
            }
            Console.WriteLine(sb.ToString());
        }


        public void Rotate90CCSquare()
        {
            ulong result = 0;
            for (int i = 0; i < SizeX; i++)
            {
                ulong mask = GenerateMask(SizeX, i);
                result |= Bmi2.X64.ParallelBitExtract(bitboardValue, mask) << (SizeX - 1 - i) * SizeX;
            }
            bitboardValue = result;
        }

        private ulong GenerateMask(int size, int row)
        {
            ulong mask = 0;
            for (int i = 0; i < size; i++)
            {
                mask |= 1UL << (i * size + row);
            }
            return mask;
        }

        public void Rotate180()
        {
            ulong result = 0;
            for (int i = 0; i < SizeX * SizeY; i++)
            {
                if ((bitboardValue & (1UL << i)) != 0)
                {
                    result |= 1UL << (SizeX * SizeY - 1 - i);
                }
            }

            bitboardValue = result;
        }


        public void GetInitialState()
        {
            //int i = 0;
            //for (int row = 0; row < sizeY && i == 0; row++)
            //{
            //    for (int col = 0; col < sizeX && i == 0; col++)
            //    {
            //        if(GetBitboardCell(col, row) == false && i == 0)
            //        {
            //            Red = (ulong)(Math.Pow(2,row * sizeX + col));
            //            i++;
            //        }
            //    }
            //}
            //(int x, int y) RedIndicees = Util.FindXYofIndices(Red);
            //GetBitboardCell(Red << 1);


            // Find red's index
            for (int i = 0; i < 64; i++)
            {
                if (GetBitboardCell(i) == false)
                {
                    red = i;
                    break;
                }
            }

            // The following logic can probably be simplifed using a BFS/DFS

            List<int> next = new List<int>();
            // Find the next empty cell that is either to the right or below Red 
            if (GetBitboardCell(red + 1) == false) // checks right
            {
                next.Add(red + 1);
                if (GetBitboardCell(next[0] + 1) == false) // Checks right again
                {
                    next.Add(next[0] + 1);
                }
                else
                {
                    if (GetBitboardCell(red + sizeX) == false) // Try going back to red and looking under it first
                    {
                        next.Add(red + sizeX);
                    }
                    else
                    {
                        next.Add(next[0] + sizeX); // Add down
                    }
                }
            }
            else
            {
                next.Add(red + sizeX); // Adds down
                // Check Left
                if (GetBitboardCell(next[0] - 1) == false)
                {
                    next.Add(next[0] - 1);
                }
                // Check right
                else if (GetBitboardCell(next[0] + 1) == false)
                {
                    next.Add(next[0] + 1);
                }
                // Add down
                else
                {
                    next.Add(next[0] + sizeX);
                }
            }

            yellow = Math.Min(next[0], next[1]);
            blue = Math.Max(next[0], next[1]);
        }

        public void PrintStateSplit()
        {
            for (int i = 0; i < 3; i++)
            {
                int temp = (state >> (i * 6)) & 63;
                switch (i)
                {
                    case 0:
                        Console.WriteLine($"Red: {temp}");
                        break;
                    case 1:
                        Console.WriteLine($"Yellow: {temp}");
                        break;
                    case 2:
                        Console.WriteLine($"Blue: {temp}");
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// The boundaries are the rows and columns along the edge.
        /// They are used by the move function to help with edge cases.
        /// The order is up, right, down, and left.
        /// </summary>
        public void GetBoundaries()
        {
            ulong temp = 0UL;

            // Generate the top row: This works
            for (int i = 0; i < sizeX; i++)
            {
                temp += 1UL << i;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the right col: Missing one at the bottom
            for (int i = 0; i < sizeY; i++)
            {
                temp += 1UL << sizeX * (i + 1) - 1;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the bottom row
            for (int i = 0; i < sizeX; i++)
            {
                temp += 1UL << (sizeX * (sizeY - 1)) + i;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the left col
            for (int i = 0; i < sizeY; i++)
            {
                temp += 1UL << i * sizeX;
            }
            boundaries.Add(temp);
            temp = 0UL;
        }

        public void PrintState()
        {
            Console.WriteLine(state);
        }


        public void SetState()
        {
            state = red | (yellow << 6) | (blue << 12);
        }

        public void SetState(int red, int yellow, int blue)
        {
            this.red = red;
            this.yellow = yellow;
            this.blue = blue;
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        public void GetNewState(Direction direction)
        {

            // Colors
            var colors = new List<(ulong bitboard, char name)>
            {
                (1UL << red, 'r'),
                (1UL << yellow, 'y'),
                (1UL << blue, 'b')
            };



            ulong boundary = 0UL;
            int moveDistance = 0;
            // Set up initial conditions
            switch (direction)
            {
                case Direction.Up:
                    boundary = boundaries[0];
                    moveDistance = sizeX;
                    colors.Sort();
                    break;
                case Direction.Right:
                    boundary = boundaries[1];
                    moveDistance = sizeY;
                    colors.Reverse();
                    break;
                case Direction.Down:
                    boundary = boundaries[2];
                    moveDistance = -sizeX;
                    colors.Reverse();
                    break;
                case Direction.Left:
                    boundary = boundaries[3];
                    moveDistance = -sizeY;
                    colors.Sort();
                    break;
                default:
                    break;
            }

            for (int i = 0; i < colors.Count; i++)
            {
                if ((colors[i].bitboard & boundary) != 0) // Touching border already
                {
                    continue;
                }
                if (CheckOverlapColors(i, colors, direction) == true) // Would have overlapped another tile
                {
                    continue;
                }

                if (((ShiftBitboardCell(colors[i].bitboard, moveDistance)) & bitboardValue) != 0) // Would have overlapped a wall
                {
                    continue;
                }
                //Console.WriteLine($"{colors[i].name} was at {colors[i].bitboard}");
                colors[i] = (ShiftBitboardCell(colors[i].bitboard, moveDistance), colors[i].name);

                //Console.WriteLine($"{colors[i].name} is now at {colors[i].bitboard}");
            }

            // Set the new state
            for (int i = 0; i < colors.Count; i++)
            {
                switch (colors[i].name)
                {
                    case 'r':
                        red = BitboardToIndex(colors[i].bitboard);
                        //Console.WriteLine($"Red is {colors[i]}");
                        break;
                    case 'y':
                        yellow = BitboardToIndex(colors[i].bitboard);
                        //Console.WriteLine($"{colors[i].name} is at {colors[i].bitboard}");
                        break;
                    case 'b':
                        blue = BitboardToIndex(colors[i].bitboard);
                        //Console.WriteLine($"{colors[i].name} is at {colors[i].bitboard}");
                        break;
                    default:
                        break;
                }
            }
        }

        //public void GetNewState(Direction direction)
        //{
        //    //ulong tiles = (ulong)Math.Pow(2, state & 0x3f) + (ulong)Math.Pow(2, state >> 6 & 0x3f) + (ulong)Math.Pow(2, state >> 12 & 0x3f);
        //    List<(ulong color, ulong comp, string c)> tiles = new List<(ulong color, ulong comp, string c)> ();

        //    // Add color red and it's comp green
        //    tiles.Add(((ulong)Math.Pow(2, state & 0x3f), (ulong)Math.Pow(2, state >> 6 & 0x3f) + (ulong)Math.Pow(2, state >> 12 & 0x3f), "R"));
        //    // Add color yellow and it's comp purple
        //    tiles.Add(((ulong)Math.Pow(2, state >> 6 & 0x3f), (ulong)Math.Pow(2, state & 0x3f) + (ulong)Math.Pow(2, state >> 12 & 0x3f), "Y"));
        //    // Add color blue and it's comp orange
        //    tiles.Add(((ulong)Math.Pow(2, state >> 12 & 0x3f), (ulong)Math.Pow(2, state & 0x3f) + (ulong)Math.Pow(2, state >> 6 & 0x3f), "B"));
        //    // Sort: If we know the order, then it becomes very easy to check if tiles overly with other tiles or not


        //    (ulong color, ulong comp, string c) temp = tiles[0];
        //    if (tiles[0].color > tiles[1].color)
        //    {
        //        tiles[0] = tiles[1];
        //        tiles[1] = temp;
        //    }
        //    if (tiles[1].color > tiles[2].color)
        //    {
        //        temp = tiles[1];
        //        tiles[1] = tiles[2];
        //        tiles[2] = temp;
        //        if (tiles[0].color > tiles[1].color)
        //        {
        //            temp = tiles[0];
        //            tiles[0] = tiles[1];
        //            tiles[1] = temp;
        //        }
        //    }

        //    if (direction == Direction.Up)
        //    {
        //        for (int i = 0; i < tiles.Count; i++)
        //        {
        //            if ((tiles[i].color >> SizeX | bitboardValue) != bitboardValue && (tiles[i].color >> sizeX | tiles[i].comp) != tiles[i].comp)
        //            {
        //                tiles[i] = (tiles[i].color >> sizeX, tiles[i].comp, tiles[i].c);
        //            }
        //        }
        //    }

        //    if (direction == Direction.Left)
        //    {
        //        for (int i = 0; i < tiles.Count; i++)
        //        {
        //            if ((tiles[i].color >> 1 | bitboardValue) != bitboardValue && (tiles[i].color >> 1 | tiles[i].comp) != tiles[i].comp)
        //            {
        //                tiles[i] = (tiles[i].color >> 1, tiles[i].comp, tiles[i].c);
        //            }
        //        }
        //    }

        //    if (direction == Direction.Down)
        //    {
        //        for (int i = tiles.Count - 1; i >= 0 ; i--)
        //        {
        //            if ((tiles[i].color << SizeX | bitboardValue) != bitboardValue && (tiles[i].color << sizeX | tiles[i].comp) != tiles[i].comp)
        //            {
        //                tiles[i] = (tiles[i].color << sizeX, tiles[i].comp, tiles[i].c);
        //            }
        //        }
        //    }

        //    if (direction == Direction.Right)
        //    {
        //        for (int i = tiles.Count - 1; i >= 0; i--)
        //        {
        //            if ((tiles[i].color << 1 | bitboardValue) != bitboardValue && (tiles[i].color << 1 | tiles[i].comp) != tiles[i].comp)
        //            {
        //                tiles[i] = (tiles[i].color << 1, tiles[i].comp, tiles[i].c);
        //            }
        //        }
        //    }

        //    // Convert tiles back into a state
        //    int newState = 0;
        //    foreach (var tile in tiles)
        //    {
        //        switch (tile.c)
        //        {
        //            case "R":
        //                this.red = (int)Math.Log2(tile.color);
        //                break;
        //            case "Y":
        //                this.yellow = ((int)Math.Log2(tile.color) & 0x3f) << 6;
        //                break;
        //            case "B":
        //                this.blue = ((int)Math.Log2(tile.color) & 0x3f) << 6;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    // Return new state

        //}


        // Helper methods
        private void CheckBounds(ulong bitboard, int sizeX, int sizeY)
        {
            if (sizeX < 1)
            {
                throw new ArgumentOutOfRangeException("sizeX was too small.");
            }
            if (sizeY < 1)
            {
                throw new ArgumentOutOfRangeException("sizeY was too small.");
            }
            if (sizeX > 8)
            {
                throw new ArgumentOutOfRangeException("sizeX was too large");
            }
            if (sizeY > 8)
            {
                throw new ArgumentOutOfRangeException("sizeY was too large.");
            }
        }

        private void CheckBounds(int x, int y)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeX {SizeX} with value {x} because {x} is too small.");
            }
            if (y < 0)
            {
                throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeY {SizeY} with value {y} because {y} is too small.");
            }
            if (x >= SizeX)
            {
                throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeX {SizeX} with value {x} because {x} is too large");
            }
            if (y >= SizeY)
            {
                throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeY {SizeY} with value {y} because {y} is too large");
            }
        }

        /// <summary>
        /// Accepts a bitboard with a single bit and returns its index
        /// </summary>
        /// <param name="bitboard"></param>
        private int BitboardToIndex(ulong bitboard)
        {
            if (BitOperations.PopCount(bitboard) != 1)
            {
                throw new ArgumentOutOfRangeException("Bitboard must contain exact 1 bit.");
            }
            // Convert single bit back into an index
            int bitPosition = BitOperations.TrailingZeroCount(bitboard);
            int row = bitPosition / sizeX;
            int col = bitPosition % sizeX;
            return row * sizeX + col;
        }

        /// <summary>
        /// Checks if the color at colors[i] would overlap if it moved in 'direction' with another color in colors.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private bool CheckOverlapColors(int i, List<(ulong bitboard, char name)> colors, Direction direction)
        {
            if (direction == Direction.Up)
            {
                for (int j = 0; j < colors.Count; j++) // Check if the other tiles are here
                {
                    if (j == i) // Don't check yourself
                    {
                        continue;
                    }

                    if (((colors[i].bitboard >> sizeX) & (colors[j].bitboard)) != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private ulong ShiftBitboardCell(ulong bitboard, int shiftAmount)
        {
            if (shiftAmount > 0)
            {
                return bitboard >> shiftAmount;
            }
            else
            {
                return bitboard << -shiftAmount;
            }
        }
    }
}
