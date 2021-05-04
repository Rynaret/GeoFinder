using GeoFinder.DataAccess;
using GeoFinder.Records;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;

namespace GeoFinder.Controllers
{
    public class SearchController : Controller
    {
        private readonly GeoBaseConnector _geoBaseConnector;

        public SearchController(GeoBaseConnector geoBaseConnector)
        {
            _geoBaseConnector = geoBaseConnector;
        }

        public static string IpUintToString(uint ipUint)
        {
            var ipBytes = BitConverter.GetBytes(ipUint);
            var ipBytesRevert = new byte[4];
            ipBytesRevert[0] = ipBytes[3];
            ipBytesRevert[1] = ipBytes[2];
            ipBytesRevert[2] = ipBytes[1];
            ipBytesRevert[3] = ipBytes[0];
            return new System.Net.IPAddress(ipBytesRevert).ToString();
        }

        [HttpGet("city/locations")]
        public ActionResult<object> GetCity(string city)
        {
            byte[] cityByteArr = new byte[24];
            Encoding.ASCII.GetBytes(city, 0, city.Length, cityByteArr, 0);

            var geoPoints = _geoBaseConnector
                .GeoPoints
                .Where(x => x.City.SequenceEqual(cityByteArr))
                .Select(x => new GeoPointDto(
                    Encoding.ASCII.GetString(x.Country),
                    Encoding.ASCII.GetString(x.Region),
                    Encoding.ASCII.GetString(x.Postal),
                    Encoding.ASCII.GetString(x.City),
                    Encoding.ASCII.GetString(x.Organization),
                    x.Latitude,
                    x.Longitude
                ))
                .ToList();

            return Ok(geoPoints);
        }

        [HttpGet("ip/locations")]
        public ActionResult<GeoPointDto> GetLocation(string ip)
        {
            var ipAddress = System.Net.IPAddress.Parse(ip);

            var ipRange = _geoBaseConnector
                .IPRanges
                .Where(x => x.From <= ipAddress.Address)
                .Where(x => x.To >= ipAddress.Address)
                .FirstOrDefault();

            var addressInDat = _geoBaseConnector
                .AddressSortedByCity
                .ElementAtOrDefault((int)ipRange.LocationIndex);

            var geoPoint = _geoBaseConnector
                .GeoPoints
                .Where(x => x.AddressInDat == addressInDat)
                .FirstOrDefault();

            var result = new GeoPointDto(
                Encoding.ASCII.GetString(geoPoint.Country),
                Encoding.ASCII.GetString(geoPoint.Region),
                Encoding.ASCII.GetString(geoPoint.Postal),
                Encoding.ASCII.GetString(geoPoint.City),
                Encoding.ASCII.GetString(geoPoint.Organization),
                geoPoint.Latitude,
                geoPoint.Longitude
            );

            return Ok(result);
        }
    }
}
