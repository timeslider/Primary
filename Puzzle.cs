//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Primary_Puzzle_Solver
//{
//    internal class Puzzle
//    {
//        public string puzzleName = string.Empty;
//        public TileType[,] puzzleData;
//        public List<MoveDirection> Moves { get; set; } = new();
//        public Puzzle(TileType[,] puzzleData, string puzzleName)
//        {
//            this.puzzleData = puzzleData;
//            this.puzzleName = puzzleName;
//        }

//        public Puzzle DeepCopy()
//        {
//            // Create a new array of the same size as puzzleData
//            TileType[,] newData = (TileType[,])puzzleData.Clone();

//            // Create a new Puzzle object with the copied data and name
//            Puzzle newPuzzle = new Puzzle(newData, puzzleName);

//            // Return the new Puzzle object
//            return newPuzzle;
//        }

//        public override bool Equals(object obj)
//        {
//            if (obj == null || GetType() != obj.GetType())
//                return false;

//            Puzzle p = (Puzzle)obj;
//            return puzzleData.Cast<TileType>().SequenceEqual(p.puzzleData.Cast<TileType>());
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                int hash = 17;
//                foreach (var item in puzzleData)
//                    hash = hash * 23 + item.GetHashCode();
//                return hash;
//            }
//        }

//        public override string ToString()
//        {
//            StringBuilder sb = new StringBuilder();
//            for(int col = 0; col < puzzleData.GetLength(0); col++)
//            {
//                for(int row = 0; row < puzzleData.GetLength(1); row++)
//                {
//                    if(puzzleData[col, row] != TileType.None)
//                    {
//                        sb.Append($"({puzzleData[col, row]}, {row}, {col}) ");
//                    }
//                }
//            }
//            return sb.ToString();
//        }

//    }
//}
