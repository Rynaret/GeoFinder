using BenchmarkDotNet.Attributes;

namespace GeoFinder.Benchmark
{
    /* Intel(R) Core(TM) i5-7300HQ CPU @ 2.50GHz
    |    Method | Parallelism |     Mean |    Error |   StdDev |
    |---------- |------------ |---------:|---------:|---------:|
    | Construct |           1 | 55.29 ms | 0.861 ms | 0.764 ms |
    | Construct |           2 | 31.29 ms | 0.591 ms | 0.553 ms |
    | Construct |           4 | 24.29 ms | 0.464 ms | 0.552 ms |
    | Construct |           8 | 25.06 ms | 0.334 ms | 0.296 ms |
    | Construct |          16 | 26.66 ms | 0.531 ms | 0.632 ms |
    | Construct |          32 | 29.09 ms | 0.234 ms | 0.183 ms |
    */
    public class GeoBaseConnector
    {
        [Params(/*1, 2,*/ 4/*, 8, 16, 32*/)]
        public int Parallelism { get; set; }

        [Benchmark]
        public void Construct() => new Infrastructure.DataAccess.GeoBaseConnector(autoInit: false).Init(Parallelism);
    }
}
