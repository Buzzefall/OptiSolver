using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OptiSolver.Data;

namespace OptiSolver.Algorithms
{
    public static class TSPSolver
    {
        private static Random generator;
        private static StringBuilder solution;

        private static bool AnyCycles(List<int> starts, List<int> ends, int start, int destination)
        {
            int current = destination;

            while (starts.Exists(elem => elem == current))
            {
                current = ends[starts.IndexOf(current)];
                if (current == start)
                    return true;
            }

            return false;
        }

        private static int getLowerBound(int[][] matrix, List<int[]> excluded, List<int> starts, List<int> ends)
        {
            // Sum for rows
            var rowMins = new int[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                if (starts.Exists(elem => elem == i)) continue;

                var min = int.MaxValue;
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (i == j ||
                        ends.Exists(elem => elem == j) ||
                        excluded.Exists(elem => elem[0] == i && elem[1] == j))
                        continue;

                    if (min > matrix[i][j])
                    {
                        min = matrix[i][j];
                        rowMins[i] = min;
                    }
                }
            }

            // Sum for columns 
            var colMins = new int[matrix.Length]; // default values are zeroes (for C# 5.0 at least)
            for (int j = 0; j < matrix.Length; j++)
            {
                if (ends.Exists(elem => elem == j)) continue;

                var min = int.MaxValue;
                for (int i = 0; i < matrix.Length; i++)
                {
                    if (i == j ||
                        starts.Exists(elem => elem == i) ||
                        excluded.Exists(elem => elem[0] == i && elem[1] == j))
                        continue;

                    if (min > matrix[i][j] - rowMins[i])
                    {
                        min = matrix[i][j] - rowMins[i];
                        colMins[j] = min;
                    }
                }
            }

            int result = 0;

            // Sums for rows and columns without specifically included edges
            for (int k = 0; k < matrix.Length; k++)
            {
                result += rowMins[k] + colMins[k];
            }

            // Sum for specifically included edges (starts[i] -> ends[i])
            for (int k = 0; k < starts.Count; k++)
            {
                result += matrix[starts[k]][ends[k]];
            }

            return result;
        }

        private static Path SolveGreedy(int[][] matrix, List<int[]> excluded, List<int> starts, List<int> ends)
        {
            // Choose next vertice the greedy way
            // matrix.Length >= 2
            //if (starts.Count >= matrix.Length) return new Path(matrix, starts, ends);

            Path result = null;

            var greedyStarts = starts.Select(st => st).ToList();
            var greedyEnds = ends.Select(en => en).ToList();
            var greedyExcluded = excluded.Select(pare => pare).ToList();

            if (greedyStarts.Count == matrix.Length - 1)
            {
                greedyEnds.Add(greedyStarts.Find(st => !greedyEnds.Exists(elem => elem == st)));
                greedyStarts.Add(greedyEnds.Find(en => !greedyStarts.Exists(elem => elem == en)));
                return new Path(matrix, greedyStarts, greedyEnds);
            }

            int greedyStart = -1;
            int greedyEnd = -1;

            while (!greedyStarts.Any() && !greedyEnds.Any())
            {
                var min = int.MaxValue;
                for (int i = 0; i < matrix.Length; i++)
                {
                    for (int j = 0; j < matrix[i].Length; j++)
                    {
                        if (i != j && !greedyExcluded.Exists(elem => elem[0] == i && elem[1] == j) &&
                            matrix[i][j] < min)
                        {
                            min = matrix[i][j];
                            greedyStart = i;
                            greedyEnd = j;
                        }
                    }
                }

                if (greedyStart == -1 || greedyEnd == -1) return null;

                greedyStarts.Add(greedyStart);
                greedyEnds.Add(greedyEnd);

                result = SolveGreedy(matrix, greedyExcluded, greedyStarts, greedyEnds);

                if (result == null)
                {
                    greedyExcluded.Add(new[] {greedyStart, greedyEnd});
                    greedyStarts.RemoveAt(greedyStarts.Count - 1);
                    greedyEnds.RemoveAt(greedyEnds.Count - 1);
                    greedyEnd = -1;
                    greedyStart = -1;
                }
                else
                    return result;
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                if (!greedyStarts.Exists(elem => elem == i) && greedyEnds.Exists(elem => elem == i))
                {
                    List<int> allowedColumns = new List<int>();
                    for (int j = 0; j < matrix.Length; j++)
                    {
                        if (i != j &&
                            !greedyEnds.Exists(elem => elem == j) &&
                            !AnyCycles(greedyStarts, greedyEnds, i, j) &&
                            !excluded.Exists(elem => elem[0] == i && elem[1] == j))
                        {
                            allowedColumns.Add(j);
                        }
                    }

                    while (allowedColumns.Any())
                    {
                        var closest = allowedColumns[generator.Next(0, allowedColumns.Count)];
                        foreach (var column in allowedColumns)
                        {
                            if (matrix[i][column] < matrix[i][closest])
                            {
                                closest = column;
                            }
                        }

                        greedyStart = i;
                        greedyEnd = closest;

                        greedyStarts.Add(greedyStart);
                        greedyEnds.Add(greedyEnd);

                        result = SolveGreedy(matrix, greedyExcluded, greedyStarts, greedyEnds);

                        if (result == null)
                        {
                            greedyStarts.RemoveAt(greedyStarts.Count - 1);
                            greedyEnds.RemoveAt(greedyEnds.Count - 1);
                            allowedColumns.Remove(greedyEnd);
                        }
                        else
                            return result;
                    }
                }
            }

            return null;
        }

        private static void Report(Path path, int lowerBound, List<int[]> excluded, List<int> starts,
            List<int> ends, string message)
        {
            solution.AppendLine(
                "---------------------------------------------------------------------------------------------------------------------");
            solution.AppendLine($"UB: {path?.Cost.ToString() ?? "None"} [{path?.ToString() ?? ""}]");
            solution.AppendLine($"LB: {lowerBound}");

            solution.Append("Excluded: ");
            foreach (var pare in excluded)
            {
                solution.Append($"[{pare[0] + 1}->{pare[1] + 1}] ");
            }

            solution.AppendLine();

            solution.Append("Included: ");
            for (int i = 0; i < starts.Count; i++)
            {
                solution.Append($"[{starts[i] + 1}->{ends[i] + 1}] ");
            }

            solution.AppendLine();

            solution.AppendLine(message);

            solution.AppendLine(
                "---------------------------------------------------------------------------------------------------------------------");

            solution.AppendLine().AppendLine().AppendLine();
        }

        public static string BranchAndBound()
        {
            var seed = (int) DateTime.Now.Ticks % int.MaxValue;
            generator = new Random(seed);

            var dimensions = generator.Next(9) + 2;
            var matrix = new List<List<int>>();

            for (int i = 0; i < dimensions; i++)
            {
                matrix.Add(new List<int>());

                for (int j = 0; j < dimensions; j++)
                    matrix[i].Add(generator.Next(1, 778));
            }

            return BranchAndBound(matrix.Select(row => row.ToArray()).ToArray());
        }

        public static string BranchAndBound(int[][] matrix)
        {
            var path = BranchAndBound(matrix, null);
            return $"Solution: [{path}]\n" + $"Cost: {path.Cost}";
        }

        private static Path BranchAndBound(int[][] matrix,
            List<int[]> excluded = null,
            List<int> starts = null,
            List<int> ends = null,
            Path recordPath = null)
        {
            #region  Initialization

            bool isRoot = true;

            if (recordPath == null)
            {
                var seed = (int) DateTime.Now.Ticks % int.MaxValue;
                generator = new Random(seed);

                solution = new StringBuilder();

                excluded = new List<int[]>();
                starts = new List<int>();
                ends = new List<int>();
                recordPath = new Path();


                solution.AppendLine(
                    "---------------------------------------------------------------------------------------------------------------------");
                solution.AppendLine(
                    $"Transport matrix for Traveling Salesman Problem ({matrix.Length}x{matrix.Length}):");
                solution.AppendLine(
                    "---------------------------------------------------------------------------------------------------------------------");

                for (int i = 0; i < matrix.Length; i++) matrix[i][i] = 0;

                var maxNumber = matrix.Max(row => row.Max());
                foreach (var row in matrix)
                {
                    foreach (var element in row)
                    {
                        var offset = element == 0
                            ? ((int) Math.Log10(maxNumber) + 1) - 1
                            : ((int) Math.Log10(maxNumber) + 1) - ((int) Math.Log10(element) + 1);

                        solution.Append(' ', offset);
                        solution.Append(element);
                        solution.Append(' ');
                    }

                    solution.AppendLine();
                }
            }
            else
                isRoot = false;

            int lowerBound = 0;
            int upperBound = 0;

            #endregion


            #region Upper Greedy & Lower Bounds

            lowerBound = getLowerBound(matrix, excluded, starts, ends);

            var greedyPath = SolveGreedy(matrix, excluded, starts, ends);

            if (greedyPath == null)
            {
                const string msg = "[X] Discarding this branch (empty, no valid solutions)";

                Report(null, lowerBound, excluded, starts, ends, msg);

                return recordPath;
            }

            upperBound = greedyPath.Cost;

            #endregion


            #region Cropping

            if (lowerBound >= recordPath.Cost)
            {
                var msg = $"[X] Discarding this branch (same record: {recordPath.Cost})";

                Report(greedyPath, lowerBound, excluded, starts, ends, msg);

                return recordPath; //it means the same record path
            }

            if (upperBound == lowerBound)
            {
                // In particular,  
                // If single valid path in branch LB = UB always
                var msg = $"[X] Discarding this branch (LB = UB, new record: {greedyPath.Cost})";

                Report(greedyPath, lowerBound, excluded, starts, ends, msg);

                // new record cost, upperBound < recordPath.Cost 
                return greedyPath;
            }

            if (recordPath.Cost > upperBound)
                recordPath = greedyPath;

            const string message = "[..] Continuing branching (LB < record)";
            Report(greedyPath, lowerBound, excluded, starts, ends, message);

            #endregion


            #region Greedy Branching and Return

            var newExcluded = excluded.Select(pare => pare).ToList();
            var newStarts = starts.Select(st => st).ToList();
            var newEnds = ends.Select(en => en).ToList();

            int newStart = -1;
            int newEnd = -1;

            //var min = int.MaxValue;
            //for (int i = 0; i < matrix.Length; i++)
            //{
            //    if (newStarts.Exists(elem => elem == i)) continue;

            //    for (int j = 0; j < matrix[i].Count; j++)
            //    {
            //        if (i != j &&
            //            !newEnds.Exists(elem => elem == j) &&
            //            !AnyCycles(newStarts, newEnds, i, j) &&
            //            !newExcluded.Exists(elem => elem[0] == i && elem[1] == j) &&
            //            matrix[i][j] < min)
            //        {
            //            min = matrix[i][j];
            //            newStart = i;
            //            newEnd = j;
            //        }
            //    }
            //}

            if (!newStarts.Any() && !newStarts.Any())
            {
                var min = int.MaxValue;
                for (int i = 0; i < matrix.Length; i++)
                {
                    for (int j = 0; j < matrix[i].Length; j++)
                    {
                        if (i != j && !newExcluded.Exists(elem => elem[0] == i && elem[1] == j) && matrix[i][j] < min)
                        {
                            min = matrix[i][j];
                            newStart = i;
                            newEnd = j;
                        }
                    }
                }
            }

            else
            {
                for (int i = 0; i < matrix.Length; i++)
                {
                    if (!newStarts.Exists(elem => elem == i) && newEnds.Exists(elem => elem == i))
                    {
                        List<int> allowedColumns = new List<int>();
                        for (int j = 0; j < matrix.Length; j++)
                        {
                            if (i != j &&
                                !newEnds.Exists(elem => elem == j) &&
                                !AnyCycles(starts, ends, i, j) &&
                                !newExcluded.Exists(elem => elem[0] == i && elem[1] == j))
                            {
                                allowedColumns.Add(j);
                            }
                        }

                        if (allowedColumns.Any())
                        {
                            var closest = allowedColumns[generator.Next(0, allowedColumns.Count)];
                            foreach (var column in allowedColumns)
                            {
                                if (matrix[i][column] < matrix[i][closest])
                                {
                                    closest = column;
                                }
                            }

                            newStart = i;
                            newEnd = closest;
                            break;
                        }
                    }
                }
            }

            if (newStart == -1 || newEnd == -1) throw new Exception();

            newStarts.Add(newStart);
            newEnds.Add(newEnd);
            newExcluded.Add(new[] {newStart, newEnd});

            if (newStarts.Count == newEnds.Count && newStarts.Count == matrix.Length - 1)
            {
                newEnds.Add(newStarts.Find(st => !newEnds.Exists(elem => elem == st)));
                newStarts.Add(newEnds.Find(en => !newStarts.Exists(elem => elem == en)));
            }


            var newRecordPath = BranchAndBound(matrix, excluded, newStarts, newEnds, recordPath);

            if (newRecordPath.Cost < recordPath.Cost)
                recordPath = newRecordPath;

            newRecordPath = BranchAndBound(matrix, newExcluded, starts, ends, recordPath);

            if (isRoot)
            {
                solution.AppendLine(
                    "---------------------------------------------------------------------------------------------------------------------");
                solution.AppendLine($"[OK] Solution found: [{newRecordPath}] (Cost: {newRecordPath.Cost})");
                solution.AppendLine();
                solution.AppendLine("#################################################################");
                solution.AppendLine("#################################################################");
                solution.AppendLine("#################################################################");
                solution.AppendLine();

                using (var fs = new System.IO.StreamWriter("Solutions.txt", true, Encoding.Unicode))
                {
                    fs.Write(solution);
                }
            }

            return newRecordPath;

            #endregion
        }
    }
}