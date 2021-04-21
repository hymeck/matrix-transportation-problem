using System;
using System.Collections.Immutable;

namespace MatrixTransportationProblem.Library
{
    public class Immutable2DArray
    {
        private ImmutableArray<ImmutableArray<double>> _storage;
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public Immutable2DArray(double[,] matrix)
        {
            RowCount = matrix.GetLength(0);
            ColumnCount = matrix.GetLength(1);
            _storage = matrix.ToImmutable2DArray();
        }

        public ImmutableArray<double> this[int row] => row < 0 || row >= RowCount
            ? throw new ArgumentOutOfRangeException(nameof(row), "row is out of zero-based range")
            : _storage[row];

        public double this[int row, int column] => column < 0 || column >= ColumnCount
        ? throw new ArgumentOutOfRangeException(nameof(column), "column is out of zero-based range")
        : this[row][column];
    }
}
