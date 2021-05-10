using GeoFinder.Services;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace GeoFinder.Tests
{
    public class SearchServiceTests
    {
        private readonly SearchService _searchService;

        public SearchServiceTests()
        {
            var dbConnector = new DataAccess.GeoBaseConnector(autoInit: false);
            _searchService = new SearchService(dbConnector);
        }

        [Theory]
        [ClassData(typeof(RangeBinarySearchTestData))]
        public void RangeBinarySearch(RangeBinarySearchTestData.TestItem data)
        {
            (int leftIndex, int rightIndex)? result = _searchService.RangeBinarySearch(data.Array, data.SearchValue);

            Assert.Equal(data.Expectation, result);
        }

        [Theory]
        [ClassData(typeof(BetweenBinarySearchTestData))]
        public void BetweenBinarySearch(BetweenBinarySearchTestData.TestItem data)
        {
            int? result = _searchService.BetweenBinarySearch(
                array: data.Array,
                value: (from: data.SearchValue, to: default),
                comparer: new BetweenBinarySearchTestData.TestItemFromComparer()
            );

            Assert.Equal(data.Expectation, result);
            
            // доп проверка, для случаев вне диапазонов
            if (result.HasValue)
            {
                bool arrayItemFound = true;
                (int from, int to) searchedValue = data.Array[result.Value];
                if (searchedValue.to < data.SearchValue || searchedValue.from > data.SearchValue)
                {
                    arrayItemFound = false;
                }

                Assert.Equal(data.ExpectationOfTest, arrayItemFound);
            }
        }
    }

    public class RangeBinarySearchTestData : IEnumerable<object[]>
    {
        public class TestItem
        {
            public (int leftIndex, int rightIndex)? Expectation { get; set; }
            public int[] Array { get; set; }
            public int SearchValue { get; set; }
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            // start
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 1, 1, 1, 2 },
                    Expectation = (0, 2),
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 1, 1, 2, 2, 3 },
                    Expectation = (0, 1),
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 1, 2, 2, 3 },
                    Expectation = (0, 0),
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 1, 2, 2, 3 },
                    Expectation = null,
                    SearchValue = -1
                }
            };

            // middle
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 1, 1, 1, 2 },
                    Expectation = (1, 3),
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 0, 1, 1, 2, 2 },
                    Expectation = (2, 3),
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 0, 1, 2, 2 },
                    Expectation = (2, 2),
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 1, 1, 2, 4, 5, 6 },
                    Expectation = null,
                    SearchValue = 3
                }
            };

            // end
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 1, 1, 1 },
                    Expectation = (1, 3),
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 1, 1, 2, 2 },
                    Expectation = (3, 4),
                    SearchValue = 2
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 1, 1, 2 },
                    Expectation = (3, 3),
                    SearchValue = 2
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { 0, 1, 1, 2 },
                    Expectation = null,
                    SearchValue = 3
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BetweenBinarySearchTestData : IEnumerable<object[]>
    {
        public class TestItem
        {
            public int? Expectation { get; set; }
            public bool ExpectationOfTest { get; set; } = true;
            public (int from, int to)[] Array { get; set; }
            public int SearchValue { get; set; }
        }

        public class TestItemFromComparer : IComparer<(int from, int to)>
        {
            public int Compare((int from, int to) x, (int from, int to) y)
            {
                return x.from.CompareTo(y.from);
            }
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            // start
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 9) },
                    Expectation = 0,
                    SearchValue = 2
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 9) },
                    Expectation = 0,
                    SearchValue = 1
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 9) },
                    Expectation = 0,
                    SearchValue = 3
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 9) },
                    Expectation = null,
                    ExpectationOfTest = false,
                    SearchValue = 0
                }
            };

            // middle
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 9) },
                    Expectation = 1,
                    SearchValue = 4
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 9) },
                    Expectation = 1,
                    SearchValue = 5
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 9) },
                    Expectation = 1,
                    SearchValue = 6
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (5, 6), (7, 9) },
                    Expectation = 0,
                    ExpectationOfTest = false,
                    SearchValue = 4
                }
            };

            // end
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 11) },
                    Expectation = 2,
                    SearchValue = 7
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 11) },
                    Expectation = 2,
                    SearchValue = 9
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 11) },
                    Expectation = 2,
                    SearchValue = 11
                }
            };
            yield return new object[]
            {
                new TestItem
                {
                    Array = new[] { (1, 3), (4, 6), (7, 11) },
                    Expectation = 2,
                    ExpectationOfTest = false,
                    SearchValue = 12
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
