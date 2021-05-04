using BenchmarkDotNet.Attributes;

namespace GeoFinder.Benchmark
{
    public class GeoBaseConnector
    {
        [Benchmark]
        public void Construct() => new DataAccess.GeoBaseConnector();

        //[Benchmark]
        public void Init() => new DataAccess.GeoBaseConnector().Init();
    }
}
