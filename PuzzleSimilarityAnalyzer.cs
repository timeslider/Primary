using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Primary_Puzzle_Solver.Bitboard;

namespace Primary_Puzzle_Solver
{
    internal class PuzzleSimilarityAnalyzer
    {
        public class SolutionMetrics
        {
            public double AverageMovesLength { get; set; }
            public double DirectionalDiversity { get; set; }
            public double PatternComplexity { get; set; }
            public List<string> CommonPatterns { get; set; }
        }

        public double CalculatePuzzleDifference(Bitboard puzzle1, Bitboard puzzle2, Dictionary<int, List<Direction>> solutions1, Dictionary<int, List<Direction>> solutions2)
        {
            // Calculate structural similarity (how many cells differ)
            double structuralDifference = CalculateStructuralDifference(puzzle1, puzzle2);

            // Calculate solution space characteristics
            var metrics1 = AnalyzeSolutions(solutions1);
            var metrics2 = AnalyzeSolutions(solutions2);

            // Weighted combination of different factors
            double totalDifference =
                0.3 * structuralDifference +                              // How different the boards look
                0.2 * Math.Abs(metrics1.AverageMovesLength - metrics2.AverageMovesLength) +  // Difference in solution length
                0.3 * Math.Abs(metrics1.DirectionalDiversity - metrics2.DirectionalDiversity) + // How different the move patterns are
                0.2 * Math.Abs(metrics1.PatternComplexity - metrics2.PatternComplexity);      // Difference in solution complexity

            return totalDifference; // 0 = identical feel, 1 = completely different feel
        }

        private double CalculateStructuralDifference(Bitboard board1, Bitboard board2)
        {
            // XOR the boards to find differing cells
            ulong differences = board1.WallData ^ board2.WallData;
            int diffCount = CountSetBits(differences);

            // Normalize by total board size
            return (double)diffCount / (board1.Width * board1.Height);
        }

        private SolutionMetrics AnalyzeSolutions(Dictionary<int, List<Direction>> solutions)
        {
            var metrics = new SolutionMetrics
            {
                CommonPatterns = new List<string>()
            };

            // Calculate average solution length
            metrics.AverageMovesLength = solutions.Values
                .SelectMany(x => x)
                .Count() / (double)solutions.Count;

            // Calculate directional diversity (entropy of move directions)
            var directions = solutions.Values
                .SelectMany(x => x)
                .GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());

            metrics.DirectionalDiversity = CalculateEntropy(directions.Values.Select(v => (double)v));

            // Find common subsequences and patterns
            metrics.PatternComplexity = AnalyzePatternComplexity(solutions);
            metrics.CommonPatterns = FindCommonPatterns(solutions);

            return metrics;
        }

        private double AnalyzePatternComplexity(Dictionary<int, List<Direction>> solutions)
        {
            // Analyze how "predictable" the solutions are
            // Higher values mean more complex/unpredictable patterns
            double complexity = 0;

            foreach (var solution in solutions.Values.SelectMany(x => x))
            {
                // Look for repeating patterns
                // Look for alternating patterns
                // Look for directional changes
                // Could be expanded based on what makes puzzles feel unique
            }

            return complexity;
        }

        private List<string> FindCommonPatterns(Dictionary<int, List<Direction>> solutions)
        {
            // Find repeated subsequences of moves that appear frequently
            // Could be used to identify characteristic patterns
            var patterns = new List<string>();

            // Analyze solutions for common subsequences
            // Could use algorithms like longest common subsequence
            // or sliding window pattern detection

            return patterns;
        }

        private double CalculateEntropy(IEnumerable<double> probabilities)
        {
            double sum = probabilities.Sum();
            return -probabilities
                .Select(p => p / sum)
                .Where(p => p > 0)
                .Sum(p => p * Math.Log2(p));
        }

        private int CountSetBits(ulong n)
        {
            int count = 0;
            while (n != 0)
            {
                count += (int)(n & 1);
                n >>= 1;
            }
            return count;
        }
    }
}
