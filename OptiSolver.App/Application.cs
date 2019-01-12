using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using OptiSolver.Data;
using OptiSolver.Algorithms;

namespace OptiSolver
{
    public class OptiSolverApp
    {
        static void Main(string[] args)
        {
            int[][] transportMatrix;
            using (var reader = new StreamReader("Input/Cities.txt", Encoding.UTF8, false))
            {
                var matrix = new List<IEnumerable<int>>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine()?.Split(' ').Select(int.Parse);
                    matrix.Add(new List<int>(row ?? throw new InvalidOperationException()));
                }

                transportMatrix = matrix.Select(row => row.ToArray()).ToArray();
            }
            
            var result = TSPSolver.BranchAndBound(transportMatrix);
        }
    }
}