using BenchmarkDotNet.Attributes;

namespace GeoFinder.Benchmark
{
    public class GeoBaseConnector
    {
        [Benchmark]
        public void Construct() => new DataAccess.GeoBaseConnector();
    }
}
