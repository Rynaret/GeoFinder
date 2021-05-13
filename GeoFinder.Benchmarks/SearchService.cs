using BenchmarkDotNet.Attributes;
using GeoFinder.Infrastructure.DataAccess;
using System.Linq;

namespace GeoFinder.Benchmarks
{
    /* Intel(R) Core(TM) i5-7300HQ CPU @ 2.50GHz
    |                    Method |        Mean |     Error |    StdDev |
    |-------------------------- |------------:|----------:|----------:|
    |        GetLocationsByCity |    41.06 ns |  0.665 ns |  0.622 ns |
    | GetLocationsByCityToArray | 2,249.88 ns | 28.797 ns | 26.936 ns |
    |                GetIPRange |   186.80 ns |  2.845 ns |  2.794 ns |
    */
    public class SearchService
    {
        private readonly GeoBaseConnector _geoBaseConnector;
        private readonly string _city;

        public SearchService()
        {
            _geoBaseConnector = new GeoBaseConnector();

            _city = "cit_Uj Dohefykuvevano";
        }

        [Benchmark]
        public void GetLocationsByCity() => new Services.SearchService(_geoBaseConnector)
            .GetLocationsByCity(_city);

        [Benchmark]
        public void GetLocationsByCityToArray() => new Services.SearchService(_geoBaseConnector)
            .GetLocationsByCity(_city)
            .ToArray();

        [Benchmark]
        public void GetIPRange() => new Services.SearchService(_geoBaseConnector)
            .GetIPRange(System.Net.IPAddress.Parse("192.168.10.67"));
    }
}
