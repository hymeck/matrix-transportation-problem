using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MatrixTransportationProblem.Library
{
    public static class ImmutableArrayExtensions
    {
        public static ImmutableArray<ImmutableArray<double>> ToImmutable2DArray(this double[,] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            if (source.Length == 0) // there is no items
                return ImmutableArray<ImmutableArray<double>>.Empty;
            
            return GetRows(source).ToImmutableArray();
        }

        private static IEnumerable<ImmutableArray<double>> GetRows(double[,] source)
        {
            const int doubleSize = sizeof(double);
            
            var rowCount = source.GetLength(0);
            var columnCount = source.GetLength(1);
            // https://stackoverflow.com/questions/2977242/getting-a-double-row-array-of-a-double-rectangular-array
            for (var r = 0; r < rowCount; r++)
            {
                var row = new double[rowCount];
                Buffer.BlockCopy(source, doubleSize * columnCount * r, row, 0, doubleSize * columnCount);
                yield return row.ToImmutableArray();
            }
        }
    }
}
