namespace MatrixTransportationProblem.Library
{
    public sealed class TransportationResult
    {
        public readonly Immutable2DArray Plan;

        public TransportationResult(Immutable2DArray plan) => Plan = plan;

        public TransportationResult(double[,] plan) => Plan = new Immutable2DArray(plan);
    }
}
