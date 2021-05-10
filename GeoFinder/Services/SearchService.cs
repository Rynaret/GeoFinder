using GeoFinder.DataAccess;
using GeoFinder.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GeoFinder.Services
{
    public class SearchService
    {
        private readonly GeoBaseConnector _geoBaseConnector;
        private readonly IPRangeFromComparer _ipRangeFromComparer = new();
        private readonly GeoPointCityComparer _geoPointCityComparer = new();

        public SearchService(GeoBaseConnector geoBaseConnector)
        {
            _geoBaseConnector = geoBaseConnector;
        }

        /// <summary>
        /// Версия BinarySearch, которая умеет работать с набором одинаковых значений в массиве
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns>left - левый индекс, right - правый индекс, null - если элемент не найден</returns>
        public (int leftIndex, int rightIndex)? RangeBinarySearch<T>(T[] array, in T value, IComparer<T> comparer = null)
        {
            int middleIndex = Array.BinarySearch(
                array: array,
                value: value,
                comparer: comparer
            );

            if (middleIndex < 0)
            {
                return null;
            }

            void FindRight(ref int rightIndex, in T value)
            {
                rightIndex += 1;

                int newRightIndex = Array.BinarySearch(
                    array: array,
                    index: rightIndex,
                    length: array.Length - rightIndex,
                    value: value,
                    comparer: comparer
                );

                if (newRightIndex < 0)
                {
                    rightIndex -= 1;
                    return;
                }

                rightIndex = newRightIndex;

                FindRight(ref rightIndex, value);
            }

            void FindLeft(ref int leftIndex, in T value)
            {
                int newLeftIndex = Array.BinarySearch(
                    array: array,
                    index: 0,
                    length: leftIndex,
                    value: value,
                    comparer: comparer
                );

                if (newLeftIndex < 0 || newLeftIndex == leftIndex)
                {
                    return;
                }

                leftIndex = newLeftIndex;

                FindLeft(ref leftIndex, value);
            }

            int rightIndex = middleIndex;
            FindRight(ref rightIndex, value);

            int leftIndex = middleIndex;
            FindLeft(ref leftIndex, value);

            return (leftIndex, rightIndex);
        }

        public List<GeoPointDto> GetLocationsByCityPerfomant(Memory<byte> cityByteArr)
        {
            var value = new GeoPoint(cityByteArr.Span);

            (int left, int right)? boundaries = RangeBinarySearch(
                array: _geoBaseConnector.GeoPoints,
                value: value,
                comparer: _geoPointCityComparer
            );

            if (boundaries is null)
            {
                return new List<GeoPointDto>();
            }

            (int leftIndex, int rightIndex) = boundaries.Value;

            /*
            | ArraySegment |         5.822 ns |       0.0816 ns |       0.0637 ns |         5.810 ns |
            | [start..end] | 5,224,671.875 ns |  24,860.0372 ns |  20,759.2570 ns | 5,221,054.688 ns |
            | Skip()Take() | 5,492,759.311 ns | 172,163.9169 ns | 502,210.2264 ns | 5,669,518.359 ns |
             */
            var geoPoints = new ArraySegment<GeoPoint>(
                array: _geoBaseConnector.GeoPoints,
                offset: leftIndex,
                count: rightIndex - leftIndex + 1 // должны получить хотябы 1 элемент
            );

            return geoPoints
                .Where(x => cityByteArr.Span.SequenceEqual(x.CitySpan))
                .Select(x => new GeoPointDto(
                    x.CountrySpan.ToArray(),
                    x.RegionSpan.ToArray(),
                    x.PostalSpan.ToArray(),
                    x.CitySpan.ToArray(),
                    x.OrganizationSpan.ToArray(),
                    x.Latitude,
                    x.Longitude
                ))
                .ToList();
        }

        /// <summary>
        /// Версия BinarySearch, которая умеет находить элемент, подходящий под диапазоном
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns>
        /// int - индекс найденого элемента (WARN проверка осуществляется только по начальному значению диапазона,
        ///     value может не попасть в представленный диапазон по полученному индексу,
        ///     в таком случае нужна доп проверка в вызывающем коде)
        /// null - индекс не найден
        /// </returns>
        public int? BetweenBinarySearch<T>(T[] array, in T value, IComparer<T> comparer)
        {
            int indexForFromComparer = Array.BinarySearch(
                array: array,
                value: value,
                comparer: comparer
            );

            if (indexForFromComparer == -1)
            {
                return null;
            }

            // Из описания метода BinarySearch:
            // "the negative number returned is the bitwise
            // complement of (the index of the last element plus 1)"
            return indexForFromComparer < 0
                ? ~indexForFromComparer - 1
                : indexForFromComparer;
        }

        public IPRange? GetIPRange(IPAddress ip)
        {
            int? index = BetweenBinarySearch(
                array: _geoBaseConnector.IPRanges,
                value: new IPRange { From = (uint)ip.Address },
                comparer: _ipRangeFromComparer
            );

            if (index is null)
            {
                return null;
            }

            IPRange result = _geoBaseConnector.IPRanges[index.Value];

            if (result.To < ip.Address || result.From > ip.Address)
            {
                return null;
            }

            return result;
        }

        public GeoPointDto GetLocationByIp(IPAddress ipAddress)
        {
            IPRange? ipRange = GetIPRange(ipAddress);

            if (ipRange is null)
            {
                return null;
            }

            uint addressInDat = _geoBaseConnector.GeoPointsDatPosSortByCity[ipRange.Value.LocationIndex];
            uint indexInArray = GeoPoint.DatPositionToIndex(addressInDat);
            GeoPoint geoPoint = _geoBaseConnector.GeoPoints[indexInArray];

            return new GeoPointDto(
                geoPoint.CountrySpan.ToArray(),
                geoPoint.RegionSpan.ToArray(),
                geoPoint.PostalSpan.ToArray(),
                geoPoint.CitySpan.ToArray(),
                geoPoint.OrganizationSpan.ToArray(),
                geoPoint.Latitude,
                geoPoint.Longitude
            );
        }
    }
}
