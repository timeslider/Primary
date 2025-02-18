using System.Collections.Concurrent;
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
    /// Represents a bitboard of size solutionsList, y.
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
        // They're eventually packed into 18-bit State variable where the first 6 bits are the red tile, the colors 6 are yellow and so on 
        private int red = 0;
        private int yellow = 0;
        private int blue = 0;


        // A merge of the colored tiles.
        // Defined by: state = red | (yellow << 6) | (blue << 12)
        public int State { get { return GetState(); } }


        // Cache boundaries for each width/height combination
        private static readonly ConcurrentDictionary<(int width, int height), List<ulong>> BoundariesCache = new();

        // The boundaries of the edges. Used to determine if crossing the solutionsList axis
        // Since a lot of indices goes from 0 to n -1 within an solutionsList, y plane we need a way to know when we have moved to another row
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

            if (BoundariesCache.TryGetValue(key, out var cachedBoundaries))
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
            sb.Append($"Puzzle: {wallData}, state: {State}, width: {width}, height: {height}\n");

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
            
            if (GetEmptyCellCount() < 3)
            {
                throw new ArgumentOutOfRangeException("There needs to be at least 3 empty holes in the bitboard.");
            }


            List<int> colors = new List<int>(3);
            colors.Add(BitOperations.TrailingZeroCount(~wallData));

            // Case 1: Move right
            if(CanMove(Direction.Right, colors[0]) > 0)
            {
                colors.Add(CanMove(Direction.Right, colors[0]));
                // Case 3: Move right
                if(CanMove(Direction.Right, colors[1]) > 0)
                {
                    colors.Add(CanMove(Direction.Right, colors[1]));
                }
                else if (CanMove(Direction.Down, colors[0]) > 0)
                {
                    colors.Add(CanMove(Direction.Down, colors[0]));
                }
                // Case 4: Move down
                else if (CanMove(Direction.Down, colors[1]) > 0)
                {
                    colors.Add(CanMove(Direction.Down, colors[1]));
                }
                else
                {
                    throw new Exception("Couldn't find 3 empty cells");
                }
            }
            // Case 2: Move down
            else if (CanMove(Direction.Down, colors[0]) > 0)
            {
                colors.Add(CanMove(Direction.Down, colors[0]));
                // Case 5: Move left
                if (CanMove(Direction.Left, colors[1]) > 0)
                {
                    colors.Add(CanMove(Direction.Left, colors[1]));
                }
                // Case 6: Move right
                else if (CanMove(Direction.Right, colors[1]) > 0)
                {
                    colors.Add(CanMove(Direction.Right, colors[1]));
                }
                // Case 7: Move down
                else if (CanMove(Direction.Down, colors[1]) > 0)
                {
                    colors.Add(CanMove(Direction.Down, colors[1]));
                }
                else
                {
                    throw new Exception("Couldn't find 3 empty cells");
                }
            }

            



            red = colors[0];
            yellow = Math.Min(colors[1], colors[2]);
            blue = Math.Max(colors[1], colors[2]);
        }



        /// <summary>
        /// Takes a direction and a current position
        /// Returns a new position if it can move
        /// Otherwise, it returns 0
        /// </summary>
        /// <param name="bitboard"></param>
        /// <param name="direction"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        public int CanMove(Direction direction, int currentPosition)
        {
            int directionVector = 0; // Where you are going to land
            int edge = 0;

            switch (direction)
            {
                case Direction.Right:
                    directionVector = currentPosition + 1;
                    edge = 1;
                    break;
                case Direction.Left:
                    directionVector = currentPosition - 1;
                    break;
                case Direction.Down:
                    directionVector = currentPosition + width;
                    break;
                default:
                    break;
            }

            // The colors position would be larger than the size of the board
            if (directionVector > (width * height))
            {
                return 0;
            }
            
            // The direction is either left or right and you already on the edge
            if ((direction != Direction.Down && (currentPosition + edge) % width == 0))
            {
                return 0;
            }
            
            // A wall is there
            if (GetBitboardCell(directionVector) == true)
            {
                return 0;
            }

            // Red or Yellow already here
            if (directionVector == red)
            {
                return 0;
            }

            return directionVector;
        }


        /// <summary>
        /// Counts the number of 0s in the bitboard
        /// </summary>
        /// <returns></returns>
        public int GetEmptyCellCount()
        {
            // Calculate total number of cells
            int totalCells = width * height;
            if (totalCells == 64)
            {
                // When using all 64 bits, no masking needed
                return totalCells - BitOperations.PopCount(wallData);
            }
            else
            {
                // Create a mask that has 1s for all valid positions
                // If totalCells is 12, we want 0000_0000_0000_1111_1111_1111
                ulong mask = (1UL << totalCells) - 1;

                // Apply mask to wallData to clear any bits beyond our range
                ulong maskedWallData = wallData & mask;

                // Calculate empty spaces: total cells minus the count of set bits in masked data
                return totalCells - BitOperations.PopCount(maskedWallData);
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
            int lowIndex;
            int highIndex;
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
        /// Counts number of empty cells in a bitboard
        /// We need to remove anything under a certain count
        /// </summary>
        public int CountHoles()
        {
            return (width * height) - BitOperations.PopCount(wallData);
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
            // Stores the states we have already visited
            var visited = new HashSet<int>();

            // Stores the states we need to visit
            var queue = new Queue<(int state, List<Direction> path)>();

            // The output. The int is a new state and the List is a list of directions
            // If you start at the initial state and follow the directions, you'll end up at the new state
            Dictionary<int, List<Direction>> solutions = new Dictionary<int, List<Direction>>();

            // Get initial state and add to data structures
            int initialState = GetState();
            queue.Enqueue((initialState, new List<Direction>()));
            visited.Add(initialState);

            // I think I might not have to do this since yourself isn't a solution
            // The List is empty and never added to
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

                    // Reset to current state before trying colors direction
                    SetState(currentState);
                }
            }
            return solutions;
        }




        /// <summary>
        /// Prints an animation of all the solutions<br></br>
        /// Runs a long time on larger solutions<br></br>
        /// </summary>
        /// <param name="solutions">All the solutions</param>
        public void PrintAllSolutions(Dictionary<int, List<Direction>> solutions)
        {
            // How fast the animation plays
            int sleepTime = 1000;
            var solutionsList = solutions.ToList();
            SetState(solutionsList[0].Key);
            PrintBitboard();
            Thread.Sleep(sleepTime);

            int i = 0;
            foreach (var solution in solutionsList.Skip(1))
            {
                SetState(solutionsList[0].Key);
                Console.Clear();
                Console.WriteLine($"Solution {i}");
                PrintBitboard();
                Thread.Sleep(sleepTime);
                foreach (var direction in solution.Value)
                {
                    MoveToNewState(direction);
                    Console.Clear();
                    //Console.WriteLine(solutionsList);
                    Console.WriteLine($"Solution {i}");
                    PrintBitboard();
                    Thread.Sleep(sleepTime);
                }
                i++;
            }
        }




        /// <summary>
        /// Prints an animation of all the solutions<br></br>
        /// Runs a long time on larger solutions<br></br>
        /// </summary>
        /// <param name="solutions">All the solutions</param>
        public void PrintSolution(Dictionary<int, List<Direction>> solutions, int index)
        {
            // How fast the animation plays
            int sleepTime = 2000;
            var solutionsList = solutions.ToList();
            
            if(index > solutionsList.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Pick a smaller index. This puzzle doesn't have that many solutions. It is advised to look at the solution dicationary.");
            }

            SetState(solutionsList[0].Key);
            PrintBitboard();
            Thread.Sleep(sleepTime);

            int i = 1;
            foreach (var direction in solutionsList[index].Value)
            {
                MoveToNewState(direction);
                Console.Clear();
                //Console.WriteLine(solutionsList);
                Console.WriteLine($"Solution {index}: Step {i}/{solutionsList[index].Value.Count}");
                Console.WriteLine(direction);
                PrintBitboard();
                Thread.Sleep(sleepTime);
                i++;
            }
        }






        // Things we need to remove
        // RYB

        // RY
        //  B

        // Stairs
        // Example
        // 1 1 1 R
        // 1 1 Y B
        // 1 0 0 0
        // 0 0 0 0

        // In the above example, the state RYB moves around a few times, but it can never be anything else. It always sticks together

        // Empty boards
        // R Y B 0 0
        // 0 0 0 0 0
        // 0 0 0 0 0
        // 0 0 0 0 0
        // 0 0 0 0 0




        
    }
}
