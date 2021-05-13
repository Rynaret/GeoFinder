using GeoFinder.Records;
using GeoFinder.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

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
        public ActionResult<IEnumerable<LocationDto>> GetLocationsByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Ok(Array.Empty<LocationDto>());
            }

            return Ok(_searchService.GetLocationsByCity(city));
        }

        [HttpGet("ip/location")]
        public ActionResult<LocationDto> GetLocationByIp(string ip)
        {
            if (false == IPAddress.TryParse(ip, out IPAddress ipAddress))
            {
                return Ok(null);
            }

            return Ok(_searchService.GetLocationByIp(ipAddress));
        }
    }
}
