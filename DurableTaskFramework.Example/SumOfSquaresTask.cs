using DurableTask.Core;

namespace DurableTaskFramework.Example
{
    public sealed class SumOfSquaresTask : TaskActivity<int, int>
    {
        public SumOfSquaresTask()
        {

        }

        protected override int Execute(TaskContext context, int chunk)
        {
            Console.WriteLine($"Square::{chunk}::{chunk * chunk}");
            return chunk * chunk;
        }
    }
}
