using DurableTask.Core;
using Newtonsoft.Json.Linq;

namespace DurableTaskFramework.Example
{
    public class SumOfSquaresOrchestration : TaskOrchestration<int, string>
    {
        public override async Task<int> RunTask(OrchestrationContext context, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
               throw new ArgumentException(nameof(input));
            }

            int sum = 0;
            var chunks = new List<Task<int>>();
            JArray data = JArray.Parse(input);

            if (!context.IsReplaying)
            {
                Console.WriteLine($"Orch Input: {data.ToString(Newtonsoft.Json.Formatting.None)}");
            }

            foreach (var item in data)
            {
                switch (item.Type)
                {
                    case JTokenType.Array:
                        var subOrchestration = context.CreateSubOrchestrationInstance<int>(typeof(SumOfSquaresOrchestration), 
                            item.ToString(Newtonsoft.Json.Formatting.None));
                        if (!context.IsReplaying)
                        {
                            Console.WriteLine($"Run Sub Orch: {item.ToString(Newtonsoft.Json.Formatting.None)}");
                        }

                        chunks.Add(subOrchestration);
                        break;
                    case JTokenType.Integer:
                        var activity = context.ScheduleTask<int>(typeof(SumOfSquaresTask), (int)item);
                        if (!context.IsReplaying)
                        {
                            Console.WriteLine($"Run Activity: {item.ToString(Newtonsoft.Json.Formatting.None)}");
                        }
                        break;
                    case JTokenType.Comment:
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid input: {item.Type}");
                }
            }

            var allChunks = await Task.WhenAll(chunks.ToArray());
            foreach (int results in allChunks)
            {
                sum += results;
            }

            Console.WriteLine($"Sum of Squares: {sum} ({data.ToString(Newtonsoft.Json.Formatting.None)})");

            return sum;
        }
    }
}
