using MathNet.Numerics.LinearAlgebra;

namespace MatrixTransportationProblem.Library
{
    public sealed class TransportationResult
    {
        public readonly Immutable2DArray Plan;

        private TransportationResult(Immutable2DArray plan) => Plan = plan;

        private TransportationResult(double[,] plan) :
            this(new Immutable2DArray(plan))
        {
        }

        public static TransportationResult Create(double[,] plan) => new (plan);
        public static TransportationResult Create(Matrix<double> plan) => Create(plan.ToArray());
    }
}
