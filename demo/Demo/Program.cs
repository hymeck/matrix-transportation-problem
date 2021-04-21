﻿namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var variant = args.Length == 0 || !int.TryParse(args[0], out var v)
                ? 12
                : v;
            
            Printer.PrintTransportationResult(Executor.Perform(DataProvider.GetInputDataHolders(), variant));
        }
    }
}
