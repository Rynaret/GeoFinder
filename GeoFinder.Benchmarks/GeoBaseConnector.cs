using BenchmarkDotNet.Attributes;

namespace GeoFinder.Benchmark
{
    /* Intel(R) Core(TM) i5-7300HQ CPU @ 2.50GHz
    |      Method |     Mean |    Error |   StdDev |
    |------------ |---------:|---------:|---------:|
    |  Construct1 | 43.28 ms | 0.735 ms | 0.651 ms |
    |  Construct2 | 26.22 ms | 0.148 ms | 0.138 ms |
    |  Construct4 | 20.05 ms | 0.335 ms | 0.372 ms |
    |  Construct8 | 21.44 ms | 0.299 ms | 0.280 ms |
    | Construct16 | 22.21 ms | 0.197 ms | 0.184 ms |
    | Construct32 | 23.54 ms | 0.102 ms | 0.091 ms |
    */
    public class GeoBaseConnector
    {
        public void Construct1() => new DataAccess.GeoBaseConnector(autoInit: false).Init(1);

        public void Construct2() => new DataAccess.GeoBaseConnector(autoInit: false).Init(2);

        [Benchmark]
        public void Construct4() => new DataAccess.GeoBaseConnector(autoInit: false).Init(4);

        public void Construct8() => new DataAccess.GeoBaseConnector(autoInit: false).Init(8);

        public void Construct16() => new DataAccess.GeoBaseConnector(autoInit: false).Init(16);

        public void Construct32() => new DataAccess.GeoBaseConnector(autoInit: false).Init(32);
    }
}
