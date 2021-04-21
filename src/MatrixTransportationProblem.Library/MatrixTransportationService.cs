using System;

namespace MatrixTransportationProblem.Library
{
    public class MatrixTransportationService
    {
        private readonly double[] _producers;
        private readonly double[] _consumers;
        private readonly double[,] _cost;

        public MatrixTransportationService(double[] producers, double[] consumers, double[,] cost)
        {
            _producers = producers;
            _consumers = consumers;
            _cost = cost;
        }

        public TransportationResult Solve()
        {
            throw new NotImplementedException(nameof(Solve));
            // todo: put impementation of algo here
        }

        public static TransportationResult Solve(double[] producers, double[] consumers, double[,] cost)
        {
            var service = new MatrixTransportationService(producers, consumers, cost);
            return service.Solve();
        }
    }
}
