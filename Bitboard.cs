using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static Primary_Puzzle_Solver.Util;

// Current problems. We converted everything from working with 8x8 to nxm since
// this allows us to use less storage space. But the cost is significately more
// complicated code. To fix this, I'm revering back to doing things within an
// 8x8 space and then as a last step, we can convert everything to an nxm.
// We already have to method to convert which is Convert8x8toNxM()
// This should make things a lot easier.
[assembly: InternalsVisibleTo("PrimaryUnitTests")]
namespace Primary_Puzzle_Solver
{
    /// <summary>
    /// Represents a bitboard of size x, y.
    /// </summary>
    internal class Bitboard
    {
        private ulong wallData;
        public ulong WallData { get { return wallData; } }


        private readonly int width;
        public int Width { get { return width; } }


        private readonly int height;
        public int Height { get { return height; } }


        // These represent the state
        // They represent where a colored tile is
        // They are 0-indexed and start at the top left and go left to right, top to bottom ending at the bottom right corner
        // They're eventually packed into 18-bit State variable where the first 6 bits are the red tile, the next 6 are yellow and so on 
        private int red = 0;
        private int yellow = 0;
        private int blue = 0;


        // A merge of the colored tiles.
        // Defined by: state = red | (yellow << 6) | (blue << 12)
        public int State { get { return GetState(); } }


        // Cache boundaries for each width/height combination
        private static readonly Dictionary<(int width, int height), List<ulong>> BoundariesCache = new();

        // The boundaries of the edges. Used to determine if crossing the x axis
        // Since a lot of indices goes from 0 to n -1 within an x, y plane we need a way to know when we have moved to another row
        private readonly List<ulong> boundaries = new();




        /// <summary>
        /// Used when the bitboard is square
        /// </summary>
        /// <param name="bitboard"></param>
        /// <param name="width"></param>
        public Bitboard(ulong bitboard, int width)
            : this(bitboard, width, width)
        { }




        /// <summary>
        /// Used when the bitboard is not square
        /// </summary>
        /// <param name="bitboard">The wall data</param>
        /// <param name="width">The width of the bitboard</param>
        /// <param name="height">The height of the bitboard</param>
        /// <exception cref="ArgumentOutOfRangeException">width and height must be greater than 0 and less than 9</exception>
        public Bitboard(ulong bitboard, int width, int height)
        {
            // Ensures the bitboard isn't too small or too large
            CheckBounds(width, height, false);

            this.wallData = bitboard;
            this.width = width;
            this.height = height;

            // The inital state is always in the upper left corner
            GetInitialState();

            // Generates the boundaries since it varies based on the dimensions of the bitboard
            boundaries = GetCachedBoundaries(width, height);
        }

        private static List<ulong> GetCachedBoundaries(int width, int height)
        {
            (int width, int height) key = (width, height);

            if(BoundariesCache.TryGetValue(key, out var cachedBoundaries))
            {
                return cachedBoundaries;
            }

            var newBoundaries = new List<ulong>();


            // Generate top row
            ulong temp = 0UL;
            for (int i = 0; i < width; i++)
            {
                temp |= 1UL << i;
            }
            newBoundaries.Add(temp);

            // Generate right column
            temp = 0UL;
            for (int i = 0; i < height; i++)
            {
                temp |= 1UL << width * (i + 1) - 1;
            }
            newBoundaries.Add(temp);

            // Generate bottom row
            temp = 0UL;
            for (int i = 0; i < width; i++)
            {
                temp |= 1UL << (width * (height - 1)) + i;
            }
            newBoundaries.Add(temp);

            // Generate left column
            temp = 0UL;
            for (int i = 0; i < height; i++)
            {
                temp |= 1UL << i * width;
            }
            newBoundaries.Add(temp);

            // Cache and return the new boundaries
            BoundariesCache[key] = newBoundaries;
            return newBoundaries;
        }






        /// <summary>
        /// Sets a single bit in a bitboard and returns the value
        /// </summary>
        /// <param name="col">The col index of the bit that will be set</param>
        /// <param name="row">The row index of the bit that will be set</param>
        /// <param name="value">The value to be set. Defaults to true</param>
        /// <returns></returns>
        public ulong SetBitboardCell(int col, int row, bool value = true)
        {
            // Just in case
            CheckBounds(col, row);

            int bitPosition = row * width + col;
            if (value == true)
            {
                return wallData |= (1UL << bitPosition);
            }
            else
            {
                return wallData &= ~(1UL << bitPosition);
            }
        }




        /// <summary>
        /// Get the value in the bitboard cell by (col, row) pair
        /// </summary>
        /// <param name="col">The col index of the bit that will be set</param>
        /// <param name="row">The row index of the bit that will be set</param>
        /// <returns>A bool</returns>
        public bool GetBitboardCell(int col, int row)
        {
            // Just in case
            CheckBounds(col, row);

            int bitPosition = row * width + col;
            return (wallData & (1UL << bitPosition)) != 0;
        }




        /// <summary>
        /// Gets bitboard cell by index.
        /// Index starts at 0 in the top left corner and works its way left to right, top to bottom and ends at (sizeX * sizeY) - 1 in the bottom right.
        /// </summary>
        /// <param name="index">The index of the cell you're querring</param>
        /// <returns>A bool</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is less than 0 or greater than (sizeX * sizeY) - 1</exception>
        public bool GetBitboardCell(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, (width * height) - 1);
            return (wallData & (1UL << index)) != 0;
        }




        /// <summary>
        /// Prints the bitboard
        /// </summary>
        /// <param name="invert">Inverts the bitboard so 1s get displayed as 0s and visa versa</param>
        public void PrintBitboard(bool invert = false)
        {
            StringBuilder sb = new StringBuilder();

            // Prints puzzle info so we always know which puzzle we are displaying
            sb.Append($"Puzzle: {wallData}, state: {State}, sizeX: {width}, sizeY: {height}\n");

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (row * width + col == red)
                    {
                        sb.Append("R ");
                        continue;
                    }
                    if (row * width + col == yellow)
                    {
                        sb.Append("Y ");
                        continue;
                    }

                    if (row * width + col == blue)
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




        /// <summary>
        /// For a given board layout, finds where the starting tiles will go.
        /// Scanning left to right, top to bottom find the first empty cell and place a red tile.
        /// The remaining 2 colors should be attached to the red tile edge wise and their placement should create a minimum.
        /// </summary>
        public void GetInitialState()
        {
            // Create a mask for the actual board size
            ulong boardMask = (1UL << (width * height)) - 1;

            // Apply mask to wallData to only consider relevant bits
            ulong maskedWallData = wallData & boardMask;

            // Now count empty spaces within the actual board area
            int emptySpaces = (width * height) - BitOperations.PopCount(maskedWallData);

            if (emptySpaces < 3)
            {
                throw new ArgumentOutOfRangeException("There needs to be at least 3 empty holes in the bitboard.");
            }

            // Find red's index
            for (int i = 0; i < 64; i++)
            {
                if (GetBitboardCell(i) == false)
                {
                    //Console.WriteLine($"Found red's index at {i}");
                    red = i;
                    break;
                }
            }




        }






        /// <summary>
        /// Prints the state into the separated colors
        /// </summary>
        public void PrintStateSplit()
        {
            Console.WriteLine($"Red: {red}");
            Console.WriteLine($"Yellow: {yellow}");
            Console.WriteLine($"Blue: {blue}");
        }




        /// <summary>
        /// The boundaries are the rows and columns along the border.
        /// They are used by the move function to help with edge cases.
        /// The order is always up, right, down, and left like a clock.
        /// The order is IMPORTANT. DO NOT REORDER.
        /// </summary>
        public void GetBoundaries()
        {
            ulong temp = 0UL;

            // Generate the top row
            for (int i = 0; i < width; i++)
            {
                temp |= 1UL << i;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the right col
            for (int i = 0; i < height; i++)
            {
                temp |= 1UL << width * (i + 1) - 1;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the bottom row
            for (int i = 0; i < width; i++)
            {
                temp |= 1UL << (width * (height - 1)) + i;
            }
            boundaries.Add(temp);
            temp = 0UL;

            // Generate the left col
            for (int i = 0; i < height; i++)
            {
                temp |= 1UL << i * width;
            }
            boundaries.Add(temp);
            temp = 0UL;
        }




        /// <summary>
        /// Returns the current state
        /// </summary>
        /// <returns></returns>
        public int GetState()
        {
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




        /// <summary>
        /// Sets the red, yellow, and blue channels from an existing state
        /// </summary>
        /// <param name="existingState">An int where the first 18 bits are split into the 3 colors</param>
        public void SetState(int existingState)
        {
            red = existingState & 0x3f;
            yellow = (existingState >> 6) & 0x3f;
            blue = (existingState >> 12) & 0x3f;
        }




        /// <summary>
        /// Represents a direction the state can try to move in
        /// </summary>
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }




        /// <summary>
        /// Given a direction, return a new state with the direction applied
        /// </summary>
        /// <param name="direction">The direction to try to move in</param>
        /// <returns>A new state</returns>
        public int MoveToNewState(Direction direction)
        {

            // Colors
            var colors = new List<(ulong bitboard, char name)>
            {
                (1UL << red, 'r'),
                (1UL << yellow, 'y'),
                (1UL << blue, 'b')
            };

            // Prevents us from reinitializing variables
            ulong boundary = 0UL;
            int moveDirection = 0;

            // Set up initial conditions
            switch (direction)
            {
                case Direction.Up:
                    boundary = boundaries[0];
                    moveDirection = width;
                    colors.Sort();
                    break;
                case Direction.Right:
                    boundary = boundaries[1];
                    moveDirection = -1;
                    colors.Sort();
                    colors.Reverse();
                    break;
                case Direction.Down:
                    boundary = boundaries[2];
                    moveDirection = -width;
                    colors.Sort();
                    colors.Reverse();
                    break;
                case Direction.Left:
                    boundary = boundaries[3];
                    moveDirection = 1;
                    colors.Sort();
                    break;
                default:
                    break;
            }
            // It might be worth it to figure out which is most common and front load that
            for (int i = 0; i < colors.Count; i++)
            {
                if ((colors[i].bitboard & boundary) != 0) // Touching border already
                {
                    continue;
                }
                if (CheckOverlapColors(i, colors, moveDirection) == true) // Would have overlapped another tile
                {
                    continue;
                }

                if (((ShiftBitboardCell(colors[i].bitboard, moveDirection)) & wallData) != 0) // Would have overlapped a wall
                {
                    continue;
                }
                colors[i] = (ShiftBitboardCell(colors[i].bitboard, moveDirection), colors[i].name);
            }

            // Set the new state
            for (int i = 0; i < colors.Count; i++)
            {
                switch (colors[i].name)
                {
                    case 'r':
                        red = BitboardToIndex(colors[i].bitboard, width, height);
                        break;
                    case 'y':
                        yellow = BitboardToIndex(colors[i].bitboard, width, height);
                        break;
                    case 'b':
                        blue = BitboardToIndex(colors[i].bitboard, width, height);
                        break;
                }
            }

            return red | (yellow << 6) | (blue << 12);
        }




        /// <summary>
        /// Not yet in use. My refactor based on my Claude.ai message
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private (ulong boundary, int moveDistance, bool ShouldReverse) GetDirectionParameters(Direction direction)
        {
            return direction switch
            {
                Direction.Up => (boundaries[0], width, false),
                Direction.Right => (boundaries[0], width, false),
                Direction.Down => (boundaries[0], width, false),
                Direction.Left => (boundaries[0], width, false),
                _ => throw new ArgumentException("Invalid direction", nameof(direction)),
            };
        }




        // Helper methods
        /// <summary>
        /// Throws an error if col or row is out of bounds
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="zeroIndexed">Defaults to true. Otherwise, assumes 1-indexed</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void CheckBounds(int col, int row, bool zeroIndexed = true)
        {
            int lowIndex = 0;
            int highIndex = 0;
            if (zeroIndexed == true)
            {
                lowIndex = 0;
                highIndex = 7;
            }
            else
            {
                lowIndex = 1;
                highIndex = 8;
            }
            if (col < lowIndex)
            {
                throw new ArgumentOutOfRangeException("width was too small.");
            }
            if (row < lowIndex)
            {
                throw new ArgumentOutOfRangeException("height was too small.");
            }
            if (col > highIndex)
            {
                throw new ArgumentOutOfRangeException("width was too large");
            }
            if (row > highIndex)
            {
                throw new ArgumentOutOfRangeException("height was too large.");
            }
        }

        //private void CheckBounds(int x, int y)
        //{
        //    if (x < 0)
        //    {
        //        throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeX {Width} with value {x} because {x} is too small.");
        //    }
        //    if (y < 0)
        //    {
        //        throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeY {Height} with value {y} because {y} is too small.");
        //    }
        //    if (x >= Width)
        //    {
        //        throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeX {Width} with value {x} because {x} is too large");
        //    }
        //    if (y >= Height)
        //    {
        //        throw new ArgumentOutOfRangeException($"Can't get bitboard of sizeY {Height} with value {y} because {y} is too large");
        //    }
        //}
        /// <summary>
        /// Accepts a bitboard with a single bit and returns its index
        /// </summary>
        /// <param name="bitboard"></param>
        private int BitboardToIndex(ulong bitboard, int width, int height)
        {
            if (BitOperations.PopCount(bitboard) != 1)
            {
                throw new ArgumentOutOfRangeException("Bitboard must contain exact 1 bit.");
            }
            // Convert single bit back into an index
            int bitPosition = BitOperations.TrailingZeroCount(bitboard);
            int row = bitPosition / width;
            int col = bitPosition % width;
            return row * width + col;
        }






        /// <summary>
        /// Checks if the color at position in colors[i] would overlap with another color in colors if it moved in 'moveDirection' .
        /// Since the colors are sorted, this might not be neccessary.
        /// </summary>
        /// <param name="i">The index of the color to check</param>
        /// <param name="colors">The rest of the colors</param>
        /// <param name="moveDirection">The direction we are trying to move</param>
        /// <returns></returns>
        private bool CheckOverlapColors(int i, List<(ulong bitboard, char name)> colors, int moveDirection)
        {
            for (int j = 0; j < colors.Count; j++) // Check if the other tiles are here
            {
                if (j == i) // Don't check yourself
                {
                    continue;
                }
                ;
                if (((ShiftBitboardCell(colors[i].bitboard, moveDirection)) & (colors[j].bitboard)) != 0)
                {
                    return true;
                }
            }

            return false;
        }




        /// <summary>
        /// This lets us shift by a negative amount since you can't shift by a negative number
        /// </summary>
        /// <param name="bitboard">It assumes that bitboard has only 1 bit in it</param>
        /// <param name="shiftAmount">The amount to shift. Typical � 1 or � width</param>
        /// <returns>A bitboard with the bit shifted</returns>
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




        /// <summary>
        /// Prints the boundaries for debugging reasons
        /// </summary>
        /// <returns>A string containing the visualization</returns>
        public string PrintBoundaries()
        {
            var result = new StringBuilder();

            for (int boundary = 0; boundary < boundaries.Count; boundary++)
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
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int index = row * width + col;
                        result.Append(((boundaries[boundary] >> index) & 1UL) == 1 ? "1 " : "- ");
                    }
                    result.AppendLine();
                }
                result.AppendLine();
            }
            Console.WriteLine(result.ToString());
            return result.ToString();
        }




        /// <summary>
        /// Performs a BFS to find all the possible states of a given bitboard and inital starting state
        /// </summary>
        /// <TODO>There might be a simplier version of this on Claude.ai</TODO>
        /// <returns>A dictionary of type int, list where int is a new state and list contains all the moves needed to go from the inital state to the new state</returns>
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




        /// <summary>
        /// Prints an animation of the solution
        /// </summary>
        /// <param name="solution">The ending state and list of directions</param>
        /// <param name="startState">The starting state</param>
        /// <param name="bitboard">I might just set the current bitboard to startState and remove this</param>
        public void PrintSolution(KeyValuePair<int, List<Direction>> solution, int startState, Bitboard bitboard)
        {
            // How fast the animation plays
            int sleepTime = 50;
            Console.Clear();
            Console.WriteLine("Initial state");
            bitboard.PrintBitboard();

            foreach (var x in solution.Value)
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
