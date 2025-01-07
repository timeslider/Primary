using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;


[assembly: InternalsVisibleTo("PrimaryUnitTests")]

namespace Primary_Puzzle_Solver
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
        //private int state = 0;
        public int State { get { return GetState(); } }
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

        /// <summary>
        /// Get bitboard cell by (x, y) pair
        /// </summary>
        /// <param name="x">Col index</param>
        /// <param name="y">Row index</param>
        /// <returns>A bool</returns>
        public bool GetBitboardCell(int x, int y)
        {
            CheckBounds(x, y);
            int bitPosition = y * SizeX + x;
            return (bitboardValue & (1UL << bitPosition)) != 0;
        }

        /// <summary>
        /// Gets bitboard cell by index.
        /// Index starts at 0 in the top left corner and works its way left to right, top to bottom and ends at (sizeX * sizeY) - 1 in the bottom right.
        /// </summary>
        /// <param name="index">The index of the cell you're querring.</param>
        /// <returns>A bool</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is less than 0 or greater than (sizeX * sizeY) - 1</exception>
        public bool GetBitboardCell(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, (sizeX * sizeY) - 1);
            return (bitboardValue & (1UL << index)) != 0;
        }

        public void PrintBitboard(bool invert = false)
        {
            StringBuilder sb = new StringBuilder();

            // Prints the puzzle ID so we always know which puzzle we are displaying
            sb.Append($"Puzzle: {bitboardValue}, state: {State}, sizeX: {sizeX}, sizeY: {sizeY}\n");

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


        /// <summary>
        /// For a given board layout, find where the starting tiles will go.
        /// Scanning left to right, top to bottom find the first empty cell and place a red tile.
        /// The remaining 2 colors should be attached to the red tile edge wise and their placement should create a minimum.
        /// </summary>
        public void GetInitialState()
        {
            if((sizeX * sizeX) - BitOperations.PopCount(bitboardValue) < 3)
            {
                throw new ArgumentOutOfRangeException("There needs to be at least 3 empty holes in the bitboard.");
            }
            //// Find red's index
            //int i = 0;
            //for (int row = 0; row < sizeY;)
            //{
            //    for (int col = 0; col < sizeX;)
            //    {
            //        if(GetBitboardCell(row, col) == false)
            //        {
            //            Console.WriteLine($"Found red's index at {col * sizeY + row}");
            //            red = col * sizeY + row;
            //            i++;
            //            break;
            //        }
            //        col++;
            //    }
            //    if(i == 1)
            //    {
            //        break;
            //    }
            //    row++;
            //}

            // Find red's index
            for (int i = 0; i < 64; i++)
            {
                if (GetBitboardCell(i) == false)
                {
                    Console.WriteLine($"Found red's index at {i}");
                    red = i;
                    break;
                }
            }

            // The following logic can probably be simplifed using a BFS/DFS

            List<int> next = new List<int>();
            // check if the cell right of Red is empty AND next cell is not on the next row
            if(GetBitboardCell(red + 1) == false && (red + 1) % (sizeX) != 0)
            {
                Console.WriteLine("Case 1: Adding something to the right of red.");
                next.Add(red + 1);
                // Check right
                if (GetBitboardCell(next[0] + 1) == false && (next[0] + 1) % (sizeX) != 0)
                {
                    Console.WriteLine($"Case 2: The cell to the right of the 2nd cell was empty and this cell is not currently on the right most edge already.");
                    next.Add(next[0] + 1);
                }
                // Check under red
                else if (GetBitboardCell(red + sizeX) == false)
                {
                    Console.WriteLine($"Case 3: Adding something under red.");
                    next.Add(red + sizeX);
                }
                // Check under yellow
                else if (GetBitboardCell(next[0] + sizeX) == false)
                {
                    Console.WriteLine($"Case 4: Adding something under yellow.");
                    next.Add(next[0] + sizeX);
                }
                else
                {
                    Console.WriteLine($"Case 5: There wasn't enough empty cells to fit all the colored tiles.");
                    throw new Exception($"Case 5: There wasn't enough empty cells to fit all the colored tiles.");
                }
            }
            else
            {
                // The cell right of red wasn't empty OR red it touching the right most edge.
                Console.WriteLine($"Case 6: Adding something under red.");
                next.Add(red + sizeX);
                // The cell left to the previous is empty AND the previous cell wasn't already on the left most edge.
                // And the left cell isn't red.
                if (GetBitboardCell(next[0] - 1) == false && (next[0] - 1) % (sizeX - 1) != 0 && (next[0] - 1) != red)
                {
                    Console.WriteLine($"Case 7: The cell to the left of the 2nd cell was not a wall, this cell was not on the left most edge, and the left most cell was not red.");
                    next.Add(next[0] - 1);
                }
                // Check right
                else if (GetBitboardCell(next[0] + 1) == false && (next[0] + 1) % (sizeX) != 0)
                {
                    Console.WriteLine($"Case 8: The cell to the right of the 2nd cell was empty and this cell is not currently on the right most edge already.");
                    next.Add(next[0] + 1);
                }
                // Check down
                else if (GetBitboardCell(next[0] + sizeX) == false)
                {
                    Console.WriteLine($"Case 9: There's nothing below, so add below.");
                    next.Add(next[0] + sizeX);
                }
                else
                {
                    Console.WriteLine($"Case 10: There wasn't enough empty cells to fit all the colored tiles.");
                    throw new Exception("Case 10: There wasn't enough empty cells to fit all the colored tiles.");
                }
            }


            yellow = Math.Min(next[0], next[1]);
            blue = Math.Max(next[0], next[1]);
        }

        public void PrintStateSplit()
        {
            Console.WriteLine($"Red: {red}");
            Console.WriteLine($"Yellow: {yellow}");
            Console.WriteLine($"Blue: {blue}");
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
                temp |= 1UL << i;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the right col: Missing one at the bottom
            for (int i = 0; i < sizeY; i++)
            {
                temp |= 1UL << sizeX * (i + 1) - 1;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the bottom row
            for (int i = 0; i < sizeX; i++)
            {
                temp |= 1UL << (sizeX * (sizeY - 1)) + i;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the left col
            for (int i = 0; i < sizeY; i++)
            {
                temp |= 1UL << i * sizeX;
            }
            boundaries.Add(temp);
            temp = 0UL;
        }

        public int GetState()
        {
            var x = red;
            var y = yellow;
            var z = blue;
            return red | (yellow << 6) | (blue << 12);
        }

        /// <summary>
        /// Sets the state from red, yellow, and blue
        /// </summary>
        /// <param name="red"></param>
        /// <param name="yellow"></param>
        /// <param name="blue"></param>
        public void SetState(int red, int yellow, int blue)
        {
            this.red = red;
            this.yellow = yellow;
            this.blue = blue;
        }

        public void SetState(int existingState)
        {
            red = existingState & 0x3f;
            yellow = (existingState >> 6) & 0x3f;
            blue = (existingState >> 12) & 0x3f;
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        public int MoveToNewState(Direction direction)
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
                    moveDistance = -1;
                    colors.Sort();
                    colors.Reverse();
                    break;
                case Direction.Down:
                    boundary = boundaries[2];
                    moveDistance = -sizeX;
                    colors.Sort();
                    colors.Reverse();
                    break;
                case Direction.Left:
                    boundary = boundaries[3];
                    moveDistance = 1;
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
                if (CheckOverlapColors(i, colors, moveDistance) == true) // Would have overlapped another tile
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
                }
            }

            return red | (yellow << 6) | (blue << 12);
        }

        private (ulong boundary, int moveDistance, bool ShouldReverse) GetDirectionParameters(Direction direction)
        {
            return direction switch
            {
                Direction.Up => (boundaries[0], sizeX, false),
                Direction.Right => (boundaries[0], sizeX, false),
                Direction.Down => (boundaries[0], sizeX, false),
                Direction.Left => (boundaries[0], sizeX, false),
                _ => throw new ArgumentException("Invalid direction", nameof(direction)),
            };
        }


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
        private bool CheckOverlapColors(int i, List<(ulong bitboard, char name)> colors, int moveDistance)
        {
            for (int j = 0; j < colors.Count; j++) // Check if the other tiles are here
            {
                if (j == i) // Don't check yourself
                {
                    continue;
                }
                ;
                if (((ShiftBitboardCell(colors[i].bitboard, moveDistance)) & (colors[j].bitboard)) != 0)
                {
                    return true;
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

        public string VisualizeBoundary()
        {
            var result = new StringBuilder();
            for(int boundary = 0; boundary < boundaries.Count; boundary++)
            {
                switch (boundary)
                {
                    case 0:
                        result.AppendLine("Topmost row");
                        break;
                    case 1:
                        result.AppendLine("Rightmost column");
                        break;
                    case 2:
                        result.AppendLine("Bottommost row");
                        break;
                    case 3:
                        result.AppendLine("Leftmost column");
                        break;
                    default:
                        break;
                }
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        int index = y * sizeX + x;
                        result.Append(((boundaries[boundary] >> index) & 1UL) == 1 ? "1 " : "- ");
                    }
                    result.AppendLine();
                }
                result.AppendLine();
            }
            return result.ToString();
        }

        public Dictionary<int, List<Direction>> Solutions()
        {
            var visited = new HashSet<int>();
            var queue = new Queue<(int state, List<Direction> path)>();
            Dictionary<int, List<Direction>> solutions = new Dictionary<int, List<Direction>>();

            // Get initial state and add to structures
            int initialState = GetState();
            queue.Enqueue((initialState, new List<Direction>()));
            visited.Add(initialState);
            solutions[initialState] = new List<Direction>();

            while (queue.Count > 0)
            {
                var (currentState, currentPath) = queue.Dequeue();

                // Set the board to current state once before trying directions
                SetState(currentState);

                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    int newState = MoveToNewState(direction);

                    if (!visited.Contains(newState))
                    {
                        var newPath = new List<Direction>(currentPath) { direction };
                        visited.Add(newState);
                        solutions[newState] = newPath;
                        queue.Enqueue((newState, newPath));
                    }

                    // Reset to current state before trying next direction
                    SetState(currentState);
                }
            }
            return solutions;
        }

        public void PrintSolution(KeyValuePair<int, List<Direction>> solution, int startState, Bitboard bitboard)
        {
            int sleepTime = 750;
            Console.Clear();
            Console.WriteLine("Initial state");
            bitboard.PrintBitboard();
            Thread.Sleep(sleepTime);

            foreach(var x in solution.Value)
            {
                bitboard.MoveToNewState(x);
                Console.Clear();
                Console.WriteLine(x);
                bitboard.PrintBitboard();
                Thread.Sleep(sleepTime);
            }
        }
    }
}
