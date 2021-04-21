using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace MatrixTransportationProblem.Library
{
    internal static class Core
    {
        public static TransportationResult Solve(double[] producers, double[] consumers, double[,] cost)
        {
            // build basis indices and initial plan using north-west scheme
            var (basisIndices, plan) = GetBasisIndicesAndPlan(producers, consumers);

            return FindSolution(producers, consumers, cost, basisIndices, plan);
        }

        private static TransportationResult FindSolution(
            double[] producers, 
            double[] consumers, 
            double[,] cost,
            List<(int row, int column)> basisIndices, 
            Matrix<double> plan)
        {
            // find solution
            while (true)
            {
                var (u, v) = BuildUVVectors(producers, consumers, cost, basisIndices);

                // check values that does not match with conditions
                var hasSolutionAchieved = CheckUV(producers, consumers, cost, ref basisIndices, u, v);

                if (hasSolutionAchieved)
                    return TransportationResult.Create(plan);

                // delete rows with zero or one basis item
                var basisIndicesCopy = basisIndices.ToList();
                CorrectBasisIndicesCopy(producers, consumers, ref basisIndicesCopy);

                // build a graph
                var graph = BuildGraph(basisIndicesCopy);

                var basisLabels = BuildBasisLabels(basisIndicesCopy, graph);

                var theta = RemoveIndexFromBasisIndices(basisIndicesCopy, basisLabels, ref plan, ref basisIndices);

                // change the plan
                CorrectPlan(basisIndicesCopy, basisLabels, ref plan, theta);
            }
        }

        private static void CorrectBasisIndicesCopy(double[] producers, double[] consumers, ref List<(int row, int column)> basisIndicesCopy)
        {
            while (true)
            {
                var (flagRow, flagColumn) = GetColumnFlags(producers, consumers, ref basisIndicesCopy);

                if (flagColumn == 1 && flagRow == 1)
                    break;
            }
        }

        private static void CorrectPlan(
            List<(int row, int column)> basisIndicesCopy, 
            Vector<double> basisLabels, 
            ref Matrix<double> plan, 
            double theta)
        {
            for (var index = 0; index < basisIndicesCopy.Count; index++)
            {
                if (basisLabels[index] == 1)
                    plan[basisIndicesCopy[index].row, basisIndicesCopy[index].column] += theta;
                else
                    plan[basisIndicesCopy[index].row, basisIndicesCopy[index].column] -= theta;
            }

            Debug.WriteLine(plan.ToMatrixString());
        }

        private static double RemoveIndexFromBasisIndices(
            List<(int row, int column)> basisIndicesCopy, 
            Vector<double> basisLabels, 
            ref Matrix<double> plan,
            ref List<(int row, int column)> basisIndices)
        {
            var theta = double.MaxValue;
            var rowTheta = double.MaxValue;
            var thetaColumn = double.MaxValue;
            for (var index = 0; index < basisIndicesCopy.Count; index++)
            {
                if (basisLabels[index] == 0 &&
                    plan[basisIndicesCopy[index].row, basisIndicesCopy[index].column] < theta)
                {
                    theta = plan[basisIndicesCopy[index].row, basisIndicesCopy[index].column];
                    rowTheta = basisIndicesCopy[index].row;
                    thetaColumn = basisIndicesCopy[index].column;
                }
            }
            Debug.WriteLine(rowTheta);
            Debug.WriteLine(thetaColumn);

            basisIndices.Remove((Convert.ToInt32(rowTheta), Convert.ToInt32(thetaColumn)));

            return theta;
        }

        private static Vector<double> BuildBasisLabels(List<(int row, int column)> basisIndicesCopy, Matrix<double> graph)
        {
            var ox = new Queue<int>();
            var labelsCopyBasisIndices = Vector<double>.Build.Dense(basisIndicesCopy.Count, 2);
            labelsCopyBasisIndices[^1] = 1; // ^1 means last item 
            ox.Enqueue(labelsCopyBasisIndices.Count - 1);
            while (ox.Count > 0)
            {
                var x = ox.Dequeue();
                for (var index = 0; index < basisIndicesCopy.Count; index++)
                {
                    if (graph[x, index] == 1 && labelsCopyBasisIndices[index] == 2)
                    {
                        ox.Enqueue(index);
                        labelsCopyBasisIndices[index] = (labelsCopyBasisIndices[x] + 1) % 2;
                    }
                }
            }

            return labelsCopyBasisIndices;
        }

        private static Matrix<double> BuildGraph(List<(int row, int column)> basisIndicesCopy)
        {
            int row, column;
            var graph = Matrix<double>.Build.Dense(basisIndicesCopy.Count, basisIndicesCopy.Count, 0d);
            for (row = 0; row < basisIndicesCopy.Count; row++)
            for (column = 0; column < basisIndicesCopy.Count; column++)
            {
                if (row == column)
                {
                    continue;
                }

                for (var k = 0; k < basisIndicesCopy.Count; k++)
                {
                    if (row == k || k == column)
                    {
                        var l1 = basisIndicesCopy[row].column;
                        var l2 = basisIndicesCopy[column].column;
                        var l3 = basisIndicesCopy[row].row;
                        var l4 = basisIndicesCopy[column].row;
                        if (l3 == l4 && basisIndicesCopy[k].column >= Math.Min(l1, l2) &&
                            basisIndicesCopy[k].column <= Math.Max(l1, l2)) 
                            graph[row, column] = 1;

                        if (l1 == l2 && basisIndicesCopy[k].row >= Math.Min(l3, l4) &&
                            basisIndicesCopy[k].row <= Math.Max(l3, l4)) 
                            graph[row, column] = 1;
                    }
                }
            }

            Debug.WriteLine(graph.ToMatrixString());
            
            return graph;
        }

        private static (int flagRow, int flagColumn) GetColumnFlags(
            double[] producers, 
            double[] consumers,
            ref List<(int row, int column)> basisIndicesCopy)
        {
            int row;
            int column;
            var flagRow = 1;
            var flagColumn = 1;
            for (row = 0; row < producers.Length; row++)
            {
                var count = 0;
                for (var index = 0; index < basisIndicesCopy.Count; index++)
                {
                    if (basisIndicesCopy[index].row == row)
                    {
                        count += 1;
                    }
                }

                // ибо здесь удаляется угловая вершина (количество базисных в строке 0 или 1)
                if (count < 2)
                {
                    var index = 0;
                    while (index < basisIndicesCopy.Count)
                    {
                        if (basisIndicesCopy[index].row == row)
                        {
                            basisIndicesCopy.Remove(basisIndicesCopy[index]);
                            flagRow = 0;
                        }
                        else
                            index += 1;
                    }
                }

                Debug.WriteLine("count string");
                Debug.WriteLine(count);
            }

            for (column = 0; column < consumers.Length; column++)
            {
                var count = 0;
                for (var index = 0; index < basisIndicesCopy.Count; index++)
                    if (basisIndicesCopy[index].column == column)
                        count += 1;

                // ибо здесь удаляется угловая вершина (количество базисных в строке 0 или 1)
                if (count < 2)
                {
                    var index = 0;
                    while (index < basisIndicesCopy.Count)
                    {
                        if (basisIndicesCopy[index].column == column)
                        {
                            basisIndicesCopy.Remove(basisIndicesCopy[index]);
                            flagColumn = 0;
                        }
                        else
                            index += 1;
                    }
                }
            }

            return (flagRow, flagColumn);
        }

        private static bool CheckUV(
            double[] producers, 
            double[] consumers, 
            double[,] cost, 
            ref List<(int row, int column)> basisIndices, 
            Vector<double> u,
            Vector<double> v)
        {
            int row, column;
            var labelReturn = true;
            for (row = 0; row < producers.Length; row++)
            for (column = 0; column < consumers.Length; column++)
            {
                var index = (row, column);
                if (!basisIndices.Contains(index))
                {
                    if (u[row] + v[column] > cost[row, column] && labelReturn)
                    {
                        labelReturn = false;
                        basisIndices.Add(index);
                        break;
                    }
                }
            }

            return labelReturn;
        }

        private static (Vector<double> u, Vector<double> v) BuildUVVectors(
            double[] producers, 
            double[] consumers, 
            double[,] cost,
            List<(int row, int column)> basisIndices)
        {
            var u = Vector<double>.Build.Dense(producers.Length, 0);
            var v = Vector<double>.Build.Dense(consumers.Length, 0);
            var labelsU = Vector<double>.Build.DenseOfVector(u);
            var labelsV = Vector<double>.Build.DenseOfVector(v);
            labelsU[0] = 1;
            double vectorSumlabelsU = labelsU.Sum();
            double vectorSumlabelsV = labelsV.Sum();
            Debug.WriteLine("xui");
            Debug.WriteLine(labelsV.Count);
            while (vectorSumlabelsU != labelsU.Count || vectorSumlabelsV != labelsV.Count)
            {
                for (int i = 0; i < basisIndices.Count; i++)
                {
                    var row = basisIndices[i].row;
                    var column = basisIndices[i].column;
                    if (labelsU[row] == 1)
                    {
                        if (column == 1)
                        {
                            Debug.WriteLine("sadassadsadd");
                            Debug.WriteLine(cost[row, column]);
                            Debug.WriteLine(u[row]);
                        }

                        v[column] = cost[row, column] - u[row];
                        labelsV[column] = 1;
                    }
                    else if (labelsV[column] == 1)
                    {
                        u[row] = cost[row, column] - v[column];
                        labelsU[row] = 1;
                    }
                }

                vectorSumlabelsU = labelsU.Sum();
                vectorSumlabelsV = labelsV.Sum();
                Debug.WriteLine(vectorSumlabelsU);
                Debug.WriteLine(vectorSumlabelsV);
                Debug.WriteLine(Math.Abs(vectorSumlabelsV - labelsV.Count));
            }
            
            Debug.WriteLine(u.ToVectorString());
            Debug.WriteLine(v.ToVectorString());

            return (u, v);
        }

        private static (List<(int row, int column)> basisIndices, Matrix<double> plan) GetBasisIndicesAndPlan(double[] producers, double[] consumers)
        {
            var producersCopy = Vector<double>.Build.DenseOfArray(producers);
            var consumersCopy = Vector<double>.Build.DenseOfArray(consumers);
            var plan = Matrix<double>.Build.Dense(producersCopy.Count, consumersCopy.Count, 0d);
            var basisIndices = new List<(int row, int column)>(producersCopy.Count + consumersCopy.Count + 1);
            int row = 0, column = 0;
            // north-west angle procedure
            while (true)
            {
                plan[row, column] = Math.Min(producersCopy[row], consumersCopy[column]);
                producersCopy[row] -= plan[row, column];
                consumersCopy[column] -= plan[row, column];
                basisIndices.Add((row, column));
                
                if (producersCopy[row] == 0)
                    row++;

                else if (consumersCopy[column] == 0) 
                    column++;

                if (row == producersCopy.Count || column == consumersCopy.Count)
                    break;
            }
            
            Debug.WriteLine(plan.ToMatrixString());

            return (basisIndices, plan);
        }
    }
}
