//using System.Text;

//namespace Primary_Puzzle_Solver
//{
//    internal static class HelperFunctions
//    {
//        public static void PrintPuzzle(Puzzle puzzle)
//        {
//            Console.WriteLine($"Printing {puzzle.puzzleName}");
//            StringBuilder output = new StringBuilder();
//            StringBuilder outputTemp = new StringBuilder();
            
//            for(int i = 0; i < puzzle.puzzleData.GetLength(0); i++)
//            {
//                for(int j = 0; j < puzzle.puzzleData.GetLength(1); j++)
//                {
//                    outputTemp.Clear();

//                    // For testing
//                    //outputTemp.Append((int)puzzle.puzzleData[i, j] + " ");

//                    outputTemp.Append("-- ");
//                    if (puzzle.puzzleData[i, j].HasFlag(TileType.HeroRed))
//                    {
//                        outputTemp[0] = 'R';
//                    }
//                    if (puzzle.puzzleData[i, j].HasFlag(TileType.HeroYellow))
//                    {
//                        outputTemp[0] = 'Y';
//                    }
//                    if (puzzle.puzzleData[i, j].HasFlag(TileType.HeroBlue))
//                    {
//                        outputTemp[0] = 'B';
//                    }
//                    if (puzzle.puzzleData[i, j].HasFlag(TileType.Wall))
//                    {
//                        outputTemp[0] = 'W';
//                    }
//                    output.Append(outputTemp);
//                }
//                output.Append('\n');
//            }
//            Console.WriteLine(output);
//        } 
//        public static Puzzle MovePuzzle(Puzzle puzzle, MoveDirection move)
//        {
//            Puzzle temp = puzzle.DeepCopy();
//            //TileType heros = TileType.HeroRed | TileType.HeroYellow | TileType.HeroBlue;

//            if (move == MoveDirection.U)
//            {

//                // i starts at one because we don't need to check the first row
//                for (int i = 1; i < temp.puzzleData.GetLength(0); i++)
//                {
//                    for (int j = 0; j < temp.puzzleData.GetLength(1); j++)
//                    {
//                        // Does the current cell contain a hero?
//                        if (temp.puzzleData[i, j].HasFlag(TileType.HeroRed) | temp.puzzleData[i, j].HasFlag(TileType.HeroYellow) | temp.puzzleData[i, j].HasFlag(TileType.HeroBlue))
//                        {
//                            // Get it then : Bit trick magic. Don't worry it :D
//                            // https://stackoverflow.com/questions/2344431/c-getting-lowest-valued-key-in-a-bitwise-enum
//                            TileType theHeroInTheCell = (TileType)((int)temp.puzzleData[i, j] & -(int)temp.puzzleData[i, j]);

//                            // Is the cell above it empty?
//                            if (temp.puzzleData[i - 1, j].Equals(TileType.None))
//                            {
//                                temp.puzzleData[i, j] &= ~theHeroInTheCell;
//                                temp.puzzleData[i - 1, j] |= theHeroInTheCell;
//                            }
//                        }
//                    }
//                }
//            }

//            if (move == MoveDirection.L)
//            {
//                // i starts at one because we don't need to check the first row
//                for (int i = 0; i < temp.puzzleData.GetLength(0); i++)
//                {
//                    for (int j = 1; j < temp.puzzleData.GetLength(1); j++)
//                    {
//                        //Does the current cell contain a hero ?
//                        if (temp.puzzleData[i, j].HasFlag(TileType.HeroRed) | temp.puzzleData[i, j].HasFlag(TileType.HeroYellow) | temp.puzzleData[i, j].HasFlag(TileType.HeroBlue))
//                        {
//                            // Get it then : Bit trick magic. Don't worry it :D
//                            // https://stackoverflow.com/questions/2344431/c-getting-lowest-valued-key-in-a-bitwise-enum
//                            TileType theHeroInTheCell = (TileType)((int)temp.puzzleData[i, j] & -(int)temp.puzzleData[i, j]);

//                            // Is the cell to the left of it empty?
//                            if (temp.puzzleData[i, j - 1].Equals(TileType.None))
//                            {
//                                temp.puzzleData[i, j] &= ~theHeroInTheCell;
//                                temp.puzzleData[i, j - 1] |= theHeroInTheCell;
//                            }
//                        }
//                    }
//                }
//            }

//            if (move == MoveDirection.D)
//            {
//                // i starts at one because we don't need to check the first row
//                for (int i = temp.puzzleData.GetLength(0) - 2; i > -1 ; i--)
//                {
//                    for (int j = temp.puzzleData.GetLength(1) - 1; j > -1; j--)
//                    {
//                        // Console.WriteLine((int)temp.puzzleData[i, j]);
//                        // Does the current cell contain a hero?
//                        if (temp.puzzleData[i, j].HasFlag(TileType.HeroRed) | temp.puzzleData[i, j].HasFlag(TileType.HeroYellow) | temp.puzzleData[i, j].HasFlag(TileType.HeroBlue))
//                        {
//                            // Get it then : Bit trick magic. Don't worry it :D
//                            // https://stackoverflow.com/questions/2344431/c-getting-lowest-valued-key-in-a-bitwise-enum
//                            TileType theHeroInTheCell = (TileType)((int)temp.puzzleData[i, j] & -(int)temp.puzzleData[i, j]);

//                            // Is the cell above it empty?
//                            if (temp.puzzleData[i + 1, j].Equals(TileType.None))
//                            {
//                                temp.puzzleData[i, j] &= ~theHeroInTheCell;
//                                temp.puzzleData[i + 1, j] |= theHeroInTheCell;
//                            }
//                        }
//                    }
//                }
//            }

//            if (move == MoveDirection.R)
//            {
//                // i starts at one because we don't need to check the first row
//                for (int i = temp.puzzleData.GetLength(0) - 1; i > -1; i--)
//                {
//                    for (int j = temp.puzzleData.GetLength(1) - 2; j > -1; j--)
//                    {
//                        // Console.WriteLine((int)temp.puzzleData[i, j]);
//                        // Does the current cell contain a hero?
//                        if (temp.puzzleData[i, j].HasFlag(TileType.HeroRed) | temp.puzzleData[i, j].HasFlag(TileType.HeroYellow) | temp.puzzleData[i, j].HasFlag(TileType.HeroBlue))
//                        {
//                            // Get it then : Bit trick magic. Don't worry it :D
//                            // https://stackoverflow.com/questions/2344431/c-getting-lowest-valued-key-in-a-bitwise-enum
//                            TileType theHeroInTheCell = (TileType)((int)temp.puzzleData[i, j] & -(int)temp.puzzleData[i, j]);

//                            // Is the cell to the right of it empty?
//                            if (temp.puzzleData[i, j + 1].Equals(TileType.None))
//                            {
//                                temp.puzzleData[i, j] &= ~theHeroInTheCell;
//                                temp.puzzleData[i, j + 1] |= theHeroInTheCell;
//                            }
//                        }
//                    }
//                }
//            }

//            return temp;
//        }

//        // Claude.ai wrote this lol
//        public static Dictionary<Puzzle, List<MoveDirection>> GetAllStates(Puzzle initialPuzzle)
//        {
//            var visited = new HashSet<Puzzle>();
//            var queue = new Queue<Puzzle>();

//            initialPuzzle.Moves = new List<MoveDirection>();
//            queue.Enqueue(initialPuzzle);
//            visited.Add(initialPuzzle);

//            while (queue.Count > 0)
//            {
//                var currentPuzzle = queue.Dequeue();

//                foreach (MoveDirection direction in Enum.GetValues(typeof(MoveDirection)))
//                {
//                    var movedPuzzle = MovePuzzle(currentPuzzle.DeepCopy(), direction);
//                    movedPuzzle.Moves = new List<MoveDirection>(currentPuzzle.Moves) { direction };

//                    if (!visited.Contains(movedPuzzle))
//                    {
//                        queue.Enqueue(movedPuzzle);
//                        visited.Add(movedPuzzle);
//                    }
//                }
//            }

//            // Convert to dictionary format at the end if needed
//            return visited.ToDictionary(puzzle => puzzle, puzzle => puzzle.Moves);
//        }

//        public static void PrintSolution(List<MoveDirection> solution)
//        {
//            StringBuilder output = new StringBuilder();

//            foreach(var move in solution)
//            {
//                output.Append(move + ", ");
//            }

//            if(output.Length > 0)
//            {
//                output.Length -= 2;
//            }
//            Console.WriteLine(output);
//        }
//    }
//}