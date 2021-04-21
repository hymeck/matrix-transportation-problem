using System;
using System.Text;
using MatrixTransportationProblem.Library;

namespace Demo
{
    public  class Printer
    {
        public TransportationResult TransportationResult { get; set; }
        public Printer(TransportationResult transportationResult) => 
            TransportationResult = transportationResult;

        public void PrintTransportationResult()
        {
            var rowCount = TransportationResult.Plan.RowCount;
            var columnCount = TransportationResult.Plan.ColumnCount;
            var plan = TransportationResult.Plan;
            var sb = new StringBuilder();
            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                    sb.AppendFormat($"{plan[row, column], 5}"); // format width
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }

        public static void PrintTransportationResult(TransportationResult transportationResult)
        {
            var printer = new Printer(transportationResult);
            printer.PrintTransportationResult();
        }
    }
}
