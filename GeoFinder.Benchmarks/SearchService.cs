using BenchmarkDotNet.Attributes;
using System;
using System.Text;

namespace GeoFinder.Benchmarks
{
    /* Intel(R) Core(TM) i5-7300HQ CPU @ 2.50GHz
    |                      Method |           Mean |        Error |       StdDev |
    |---------------------------- |---------------:|-------------:|-------------:|
    |     GetLocationsByCityNaive | 5,167,152.0 ns | 51,778.21 ns | 48,433.37 ns |
    | GetLocationsByCityPerfomant |     7,809.5 ns |    119.10 ns |     99.46 ns |
    |                  GetIPRange |       199.9 ns |      1.88 ns |      1.75 ns |
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
        public void GetLocationsByCityPerfomant() => new Services.SearchService(_geoBaseConnector)
            .GetLocationsByCityPerfomant(_cityByteArr);

        [Benchmark]
        public void GetIPRange() => new Services.SearchService(_geoBaseConnector)
            .GetIPRange(System.Net.IPAddress.Parse("192.168.10.67"));
    }
}
