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

        public List<GeoPointDto> GetLocationsByCityNaive(Memory<byte> cityByteArr)
        {
            return _geoBaseConnector
                .GeoPoints
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

        public List<GeoPointDto> GetLocationsByCityPerfomant(Memory<byte> cityByteArr)
        {
            int middleIndex = Array.BinarySearch(
                array: _geoBaseConnector.GeoPoints,
                value: new GeoPoint(cityByteArr.Span),
                comparer: _geoPointCityComparer
            );

            if (middleIndex < 0)
            {
                return new List<GeoPointDto>();
            }

            void FindRight(ref int rightIndex)
            {
                int newRightIndex = Array.BinarySearch(
                    _geoBaseConnector.GeoPoints,
                    rightIndex,
                    _geoBaseConnector.GeoPoints.Length - rightIndex,
                    new GeoPoint(cityByteArr.Span),
                    new GeoPointCityComparer()
                );

                if (newRightIndex < 0)
                {
                    return;
                }

                if (newRightIndex == rightIndex)
                {
                    return;
                }

                rightIndex = newRightIndex;

                FindRight(ref rightIndex);
            }

            void FindLeft(ref int leftIndex)
            {
                int newLeftIndex = Array.BinarySearch(
                    _geoBaseConnector.GeoPoints,
                    0,
                    leftIndex,
                    new GeoPoint(cityByteArr.Span),
                    new GeoPointCityComparer()
                );

                if (newLeftIndex < 0 || newLeftIndex == leftIndex)
                {
                    return;
                }

                leftIndex = newLeftIndex;

                FindLeft(ref leftIndex);
            }

            int rightIndex = middleIndex;
            FindRight(ref rightIndex);

            int leftIndex = middleIndex;
            FindLeft(ref leftIndex);

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

        public IPRange? GetIPRange(IPAddress ip)
        {
            int index = Array.BinarySearch(
                array: _geoBaseConnector.IPRanges,
                value: new IPRange { From = (uint)ip.Address },
                comparer: _ipRangeFromComparer
            );

            // Из описания метода:
            // "the negative number returned is the bitwise
            // complement of (the index of the last element plus 1)"
            return _geoBaseConnector.IPRanges[~index - 1];
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
