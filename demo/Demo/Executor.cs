using System.Collections.Generic;
using MatrixTransportationProblem.Library;

namespace Demo
{
    public class Executor
    {
        public Dictionary<int, InputDataHolder> Variants;

        public Executor(Dictionary<int, InputDataHolder> variants) => Variants = variants;

        public TransportationResult Perform(int variant)
        {
            var (producers, consumers, cost) = Variants[variant];
            var service = new MatrixTransportationService(producers, consumers, cost);
            return service.Solve();
        }

        public static TransportationResult Perform(Dictionary<int, InputDataHolder> variants, int variant)
        {
            var executor = new Executor(variants);
            return executor.Perform(variant);
        }
    }
}
