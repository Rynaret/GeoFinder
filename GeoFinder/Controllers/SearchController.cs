using GeoFinder.Records;
using GeoFinder.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GeoFinder.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("city/locations")]
        public ActionResult<List<GeoPointDto>> GetLocationsByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return new List<GeoPointDto>();
            }

            var cityByteArr = new Memory<byte>(new byte[24]);
            Encoding.ASCII.GetBytes(city.AsSpan(), cityByteArr.Span);

            return _searchService.GetLocationsByCityPerfomant(cityByteArr);
        }

        [HttpGet("ip/location")]
        public ActionResult<GeoPointDto> GetLocationByIp(string ip)
        {
            if (false == IPAddress.TryParse(ip, out IPAddress ipAddress))
            {
                return null;
            }

            return _searchService.GetLocationByIp(ipAddress);
        }
    }
}
