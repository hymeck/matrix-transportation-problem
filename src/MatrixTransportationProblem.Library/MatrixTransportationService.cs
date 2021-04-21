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

        public TransportationResult Solve() => 
            Core.Solve(_producers, _consumers, _cost);

        public static TransportationResult Solve(double[] producers, double[] consumers, double[,] cost)
        {
            var service = new MatrixTransportationService(producers, consumers, cost);
            return service.Solve();
        }
    }
}
