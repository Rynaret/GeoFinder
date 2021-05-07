using GeoFinder.DataAccess;
using GeoFinder.Records;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public ActionResult<List<GeoPointDto>> GetCity(string city)
        {
            //byte[] cityByteArr = new byte[24];
            //Encoding.ASCII.GetBytes(city, 0, city.Length, cityByteArr, 0);

            var geoPoints = _geoBaseConnector
                .GeoPoints
                .Where(x => x.CityStr == city)
                .Select(x => new GeoPointDto(
                    //Encoding.ASCII.GetString(x.Country),
                    //Encoding.ASCII.GetString(x.Region),
                    //Encoding.ASCII.GetString(x.Postal),
                    //Encoding.ASCII.GetString(x.City),
                    //Encoding.ASCII.GetString(x.Organization),
                    x.CityStr,
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

            IPRange ipRange = _geoBaseConnector
                .IPRanges
                .Where(x => x.From <= ipAddress.Address)
                .Where(x => x.To >= ipAddress.Address)
                .FirstOrDefault();
            
            uint addressInDat = _geoBaseConnector
                .AddressSortedByCity[ipRange.LocationIndex];

            uint indexInArray = GeoPoint2.AddressInDatToIndex(addressInDat);

            GeoPoint2 geoPoint = _geoBaseConnector
                .GeoPoints[indexInArray];

            var result = new GeoPointDto(
                //geoPoint.Country,
                //Encoding.ASCII.GetString(geoPoint.Region),
                //Encoding.ASCII.GetString(geoPoint.Postal),
                //Encoding.ASCII.GetString(geoPoint.City),
                //Encoding.ASCII.GetString(geoPoint.Organization),
                geoPoint.CityStr,
                geoPoint.Latitude,
                geoPoint.Longitude
            );

            return Ok(result);
        }
    }
}
