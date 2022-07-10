
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AILZ80ASM.Benshmark
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }

    }

    //[ShortRunJob]
    public class Benchmark
    {
        [Benchmark]
        public int Benchmark1()
        {
            var result = AILZ80ASM.Program.Main(@"./Main.Z80", @"-cd", @"./TestSource/Benchmark1", "-f", "-dw", "W0001", "W0002", "W0003", "-lst", "-bin", "-sym");

            return result;
        }
    }
}