using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Primary_Puzzle_Solver
{
    internal class Data
    {
        // The MovePuzzle method can be broken down into smaller methods to improve readability and maintainability.Here’s a suggestion:
        // MoveHero: This method would handle the logic for moving a hero in the specified direction.It would take the current puzzle, the hero’s current position, and the move direction as parameters, and return the updated puzzle.
        private static Puzzle MoveHero(Puzzle puzzle, (int, int) position, MoveDirection move)
        {
            return puzzle;
        }




        // CanMove: This method would check if a move is possible from a given position in a specified direction.It would take the current puzzle, the position, and the move direction as parameters, and return a boolean indicating whether the move is possible.
        private static bool CanMove(Puzzle puzzle, (int, int) position, MoveDirection move)
        {
            return false;
        }
        // FindHeroes: This method would find all heroes in the puzzle.It would take the current puzzle as a parameter and return a list of positions of all heroes.




        private static List<(int, int)> FindHeroes(Puzzle puzzle)
        {
            List<(int, int)> values = new List<(int, int)>() { (1, 2), (5, 6) };
            return values;
        }




        // Then, your MovePuzzle method would look something like this:
        public static Puzzle MovePuzzle(Puzzle puzzle, MoveDirection move)
        {
            Puzzle temp = puzzle.DeepCopy();
            var heroes = FindHeroes(temp);

            foreach (var hero in heroes)
            {
                if (CanMove(temp, hero, move))
                {
                    temp = MoveHero(temp, hero, move);
                }
            }
            return temp;
        }
    }

    public enum Color
    {
        red, yellow, blue
    }

    public enum FullScreenMode
    {
        windowed,
        borderless,
        fullscreen,
    }

    public enum Resolution
    {
        FHD,
    }

    public enum FrameCap
    {
        Thirty,
        Sixty,
        Ninty,
        OneTwenty,
    }

    #region RootobjectPuzzleData
    /// <summary>
    /// The root of the JSON file
    /// <para>worlds: A list of worlds</para>
    /// </summary>
    public class RootObjectPuzzleData
    {
        public required List<World> worlds { get; set; }
    }

    /// <summary>
    /// <para>name</para> The name of the world.
    /// </summary>
    public class World
    {
        public required string name { get; set; }
        public required List<Level> levels { get; set; }
        public required PlayerWorldData playerWorldData { get; set; }
    }

    public class Level
    {
        public required string name { get; set; }
        public required GridSize gridSize { get; set; }
        public required int fewestSteps { get; set; }
        public required List<string> solution { get; set; }
        public required PuzzleData puzzleData { get; set; }
        public required PlayerLevelData playerLevelData { get; set; }
    }

    public class GridSize
    {
        public required int x { get; set; }
        public required int y { get; set; }
    }

    public class PuzzleData
    {
        public required List<Wall> walls { get; set; }
        public required List<PlayerTile> playerTiles { get; set; }
        public required List<Goal> goals { get; set; }
    }

    public class PlayerLevelData
    {
        public required double playTime { get; set; }
        public required bool completed { get; set; }
        public required int starCount { get; set; }
        public required int fewestMoves { get; set; }

    }

    public class PlayerWorldData
    {
        public required double playTime { get; set; }
        public required bool completed { get; set; }
        public required int starCount { get; set; }
    }

    public class Wall
    {
        public required int x { get; set; }
        public required int y { get; set; }
    }

    public class PlayerTile
    {
        public required int x { get; set; }
        public required int y { get; set; }
        public required Color color { get; set; }
    }

    public class Goal
    {
        public required int x { get; set; }
        public required int y { get; set; }
        public required Color color { get; set; }
    }
    #endregion

    public class RootObjectOptionData
    {
        public required Options options { get; set; }
    }

    public class Options
    {
        public required FullScreenMode FullScreenMode { get; set; }
        public required bool vsync { get; set; }
        public required FrameCap FrameCap { get; set; }
        public required Resolution Resolution { get; set; }
        public required float audioMain {  get; set; }
        public required float audioMusic { get; set; }
        public required float audioSoundFX { get; set; }

    }



    // Sure, I can help you outline a breadth-first search(BFS) algorithm for this problem.Here’s a high-level overview:
    // Initialize: Create a queue and a dictionary to keep track of states.The queue will store puzzles and their corresponding move sequences. The dictionary will store puzzles as keys and the shortest move sequence as values.
    // First State: Add the initial state of the puzzle to the queue and dictionary with an empty move sequence.
    // BFS Loop: While there are still puzzles in the queue, dequeue a puzzle and its move sequence.
    // Next States: For each possible move direction, create a new puzzle state.If this state is not in the dictionary or if the new move sequence is shorter than the one in the dictionary, enqueue the new state and move sequence, and update the dictionary.
    // Result: Once the queue is empty, your dictionary will contain all reachable states and their shortest move sequences.
    // Here’s how you might implement this in C#:


    // This code assumes that HelperFunctions.MovePuzzle returns a new puzzle state after making a move in the given direction and that two puzzles are considered equal if their puzzleData arrays have the same values in all positions.
    //Please note that this is just an outline and might need adjustments based on your specific requirements and codebase. Also, remember to handle potential exceptions and edge cases! 😊
}
