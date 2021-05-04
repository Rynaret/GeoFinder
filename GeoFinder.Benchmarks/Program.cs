using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace GeoFinder.Benchmark
{
    public class Program
    {
        public static void Main(string[] args) => BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args, DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }
}
