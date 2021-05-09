using BenchmarkDotNet.Attributes;
using System;
using System.Text;

namespace GeoFinder.Benchmarks
{
    /* Intel(R) Core(TM) i5-7300HQ CPU @ 2.50GHz
    |                      Method |           Mean |        Error |       StdDev |
    |---------------------------- |---------------:|-------------:|-------------:|
    |     GetLocationsByCityNaive | 2,372,652.1 ns | 20,512.64 ns | 17,128.98 ns |
    | GetLocationsByCityPerfomant |     4,195.5 ns |     56.51 ns |     50.09 ns |
    |                  GetIPRange |       177.9 ns |      1.89 ns |      1.67 ns |
    */
    public class SearchService
    {
        private readonly DataAccess.GeoBaseConnector _geoBaseConnector;
        private readonly Memory<byte> _cityByteArr = new(new byte[24]);

        public SearchService()
        {
            _geoBaseConnector = new DataAccess.GeoBaseConnector();

            Encoding.ASCII.GetBytes("cit_Uj Dohefykuvevano".AsSpan(), _cityByteArr.Span);
        }

        [Benchmark]
        public void GetLocationsByCityNaive() => new Services.SearchService(_geoBaseConnector)
            .GetLocationsByCityNaive(_cityByteArr);

        [Benchmark]
        public void GetLocationsByCityPerfomant() => new Services.SearchService(_geoBaseConnector)
            .GetLocationsByCityPerfomant(_cityByteArr);

        [Benchmark]
        public void GetIPRange() => new Services.SearchService(_geoBaseConnector)
            .GetIPRange(System.Net.IPAddress.Parse("192.168.10.67"));
    }
}
