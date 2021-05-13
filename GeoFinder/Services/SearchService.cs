using GeoFinder.Extensions;
using GeoFinder.Infrastructure;
using GeoFinder.Infrastructure.DataAccess;
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
        private readonly LocationCityComparer _locationCityComparer = new();

        public SearchService(GeoBaseConnector geoBaseConnector)
        {
            _geoBaseConnector = geoBaseConnector;
        }

        public IEnumerable<LocationDto> GetLocationsByCity(string city)
        {
            var value = new Location(city);

            (int left, int right)? boundaries = _geoBaseConnector
                .Locations
                .RangeBinarySearch(
                    value: ref value,
                    comparer: _locationCityComparer,
                    out int middleIndex
                );

            // perhaps there is the city, but with extra spaces
            if (boundaries is null)
            {
                Location closestItem = _geoBaseConnector.Locations[~middleIndex];

                Span<byte> cityCopy = closestItem.City.ToArray().AsSpan();
                cityCopy.TrimEndAnsciiSpace();

                bool cityFound = cityCopy.SequenceEqual(value.City);
                if (cityFound)
                {
                    boundaries = _geoBaseConnector
                        .Locations
                        .RangeBinarySearch(
                            value: ref closestItem,
                            comparer: _locationCityComparer,
                            out _
                        );

                    if (boundaries is null)
                    {
                        yield break;
                    }
                }
            }

            (int leftIndex, int rightIndex) = boundaries.Value;

            // performance comparable to .AsSpan().Slice()
            var locations = new ArraySegment<Location>(
                array: _geoBaseConnector.Locations,
                offset: leftIndex,
                count: rightIndex - leftIndex + 1 // at least 1 element should be returned
            );

            foreach (Location location in locations)
            {
                yield return new LocationDto(
                    location.Country,
                    location.Region,
                    location.Postal,
                    location.City,
                    location.Organization,
                    location.Latitude,
                    location.Longitude
                );
            }
        }

        public IPRange? GetIPRange(IPAddress ip)
        {
#pragma warning disable CS0618 // We have only number without the family type
            uint targetIpAddress = (uint)ip.Address;
#pragma warning restore CS0618 // Type or member is obsolete

            var value = new IPRange { From = targetIpAddress };
            int? index = _geoBaseConnector
                .IPRanges
                .BetweenBinarySearch(
                    value: ref value,
                    comparer: _ipRangeFromComparer
                );

            if (index is null)
            {
                return null;
            }

            IPRange result = _geoBaseConnector.IPRanges[index.Value];

            if (result.To < targetIpAddress || result.From > targetIpAddress)
            {
                return null;
            }

            return result;
        }

        public LocationDto GetLocationByIp(IPAddress ipAddress)
        {
            IPRange? ipRange = GetIPRange(ipAddress);

            if (ipRange is null)
            {
                return null;
            }

            uint addressInDat = _geoBaseConnector.LocationsDatPosSortByCity[ipRange.Value.LocationIndex];

            uint indexInArray = Location.DatPositionToIndex(addressInDat);
            Location location = _geoBaseConnector.Locations[indexInArray];

            return new LocationDto(
                location.Country,
                location.Region,
                location.Postal,
                location.City,
                location.Organization,
                location.Latitude,
                location.Longitude
            );
        }
    }
}
