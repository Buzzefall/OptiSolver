using System;
using System.Collections.Generic;

namespace OptiSolver.Data
{
    public class Path
    {
        private List<int> starts;
        private List<int> ends;

        public int Cost { get; set; }

        public Path()
        {
            starts = new List<int>();
            ends = new List<int>();
            Cost = int.MaxValue;
        }

        public Path(int[][] matrix, List<int> starts, List<int> ends)
        {
            this.starts = starts;
            this.ends = ends;
            Cost = 0;

            if (starts.Count != ends.Count || starts.Count != matrix.Length)
                throw new Exception();

            for (int i = 0; i < starts.Count; i++)
            {
                Cost += matrix[starts[i]][ends[i]];
            }
        }

        private static string PathToString(List<int> starts, List<int> ends, int start, int counter = 0)
        {
            if (++counter == starts.Count / 2 + 1) return $"{start + 1}";

            var end = ends[starts.IndexOf(start)];

            return $"{start + 1}->{end + 1}->" + PathToString(starts, ends, ends[starts.IndexOf(end)], counter);
        }

        public override string ToString()
        {
            return PathToString(starts, ends, starts[0]);
        }
    }

    public class Assignment
    {
        private int[,] matrix;

        public Assignment(int size)
        {
            matrix = new int[size, size];
        }

        public Assignment(int[,] assignment)
        {
            matrix = assignment;
        }

        public int this[int row, int col]
        {
            get => matrix[row, col];
            set => matrix[row, col] = value;
        }
    }
}