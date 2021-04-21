using System.Collections.Generic;

namespace Demo
{
    public class DataProvider
    {
        public static Dictionary<int, InputDataHolder> GetInputDataHolders()
        {
            var producers = new double[] {80, 100, 120};
            var consumers = new double[] {100, 100, 100};
            var cost = new double[,]
            {
                {1, 5, 6},
                {2, 2, 2},
                {7, 3, 4}
            };

            var input12 = new InputDataHolder(producers, consumers, cost);

            producers = new double[] {70, 80, 70};
            consumers = new double[] {100, 60, 60};
            cost = new double[,]
            {
                {5, 2, 8},
                {2, 1, 6},
                {7, 5, 4}
            };

            var input8 = new InputDataHolder(producers, consumers, cost);

            producers = new double[] {50, 50, 100};
            consumers = new double[] {40, 90, 70};
            cost = new double[,]
            {
                {2, 5, 3},
                {4, 3, 2},
                {5, 1, 2}
            };

            var input1 = new InputDataHolder(producers, consumers, cost);

            // 2
            //var producers = new Double[] {80, 80, 90};
            //var consumers = new double[] {50, 100, 100};
            //var cost = new double[,] {{5,1,6}, {10,3,4}, {4,5,8}};
            // 3
            //var producers = new Double[] {90, 90, 100};
            //var consumers = new double[] {50, 100, 130};
            //var cost = new double[,] {{2,5,4}, {7,6,5}, {9,8,10}};
            // 4
            //var producers = new Double[] {50, 60, 90};
            //var consumers = new double[] {40, 50, 110};
            //var cost = new double[,] {{2,3,5}, {8,5,8}, {6,4,8}};

            // 5
            // var producers = new double[] {50, 70, 80};
            // var consumers = new double[] {20, 90, 90};
            // var cost = new double[,] {{5,6,11}, {3,7,8}, {10,4,9}};

            return new Dictionary<int, InputDataHolder>
            {
                {12, input12},
                {8, input8},
                {1, input1},
            };
        }
    }
}
