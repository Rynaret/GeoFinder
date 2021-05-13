using GeoFinder.Extensions;
using GeoFinder.Infrastructure.Abstractions;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace GeoFinder.Tests
{
    public class ArrayExtensionsTests
    {
        [Theory]
        [ClassData(typeof(RangeBinarySearchTestData))]
        public void RangeBinarySearch(RangeBinarySearchTestData.TestItem data)
        {
            int searchValue = data.SearchValue;
            (int leftIndex, int rightIndex)? result = data.Array.RangeBinarySearch(
                value: ref searchValue,
                comparer: new RangeBinarySearchTestData.IntComparer(),
                out _
            );

            Assert.Equal(data.Expectation, result);
        }

        [Theory]
        [ClassData(typeof(BetweenBinarySearchTestData))]
        public void BetweenBinarySearch(BetweenBinarySearchTestData.TestItem data)
        {
            (int from, int to) searchValue = (from: data.SearchValue, to: default);
            int? result = data.Array.BetweenBinarySearch(
                value: ref searchValue,
                comparer: new BetweenBinarySearchTestData.TestItemFromComparer()
            );

            Assert.Equal(data.Expectation, result);
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

        public class IntComparer : IRefComparer<int>
        {
            public int Compare(ref int x, ref int y)
            {
                return x.CompareTo(y);
            }
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
            public (int from, int to)[] Array { get; set; }
            public int SearchValue { get; set; }
        }

        public class TestItemFromComparer : IRefComparer<(int from, int to)>
        {
            public int Compare(ref (int from, int to) x, ref (int from, int to) y)
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
