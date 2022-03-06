
namespace AILZ80ASM.Benshmark
{
    public class Program
    {
        public static int Main(params string[] args)
        {
            var result = AILZ80ASM.Program.Main(@"./Main.Z80", @"-cd", @"./TestSource/Benchmark1");

            return 0;
        }
    }
}