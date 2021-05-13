using GeoFinder.JsonConverters;
using System;
using System.Text.Json.Serialization;

namespace GeoFinder.Records
{
    public record LocationDto
    {
        public LocationDto(
            ReadOnlySpan<byte> Country,
            ReadOnlySpan<byte> Region,
            ReadOnlySpan<byte> Postal,
            ReadOnlySpan<byte> City,
            ReadOnlySpan<byte> Organization,
            float Latitude,
            float Longitude
        )
        {
            this.Country = Country.ToArray();
            this.Region = Region.ToArray();
            this.Postal = Postal.ToArray();
            this.City = City.ToArray();
            this.Organization = Organization.ToArray();
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        [JsonConverter(typeof(ByteArrayStringConverter))]
        public byte[] Country { get; }

        [JsonConverter(typeof(ByteArrayStringConverter))]
        public byte[] Region { get; }
        
        [JsonConverter(typeof(ByteArrayStringConverter))]
        public byte[] Postal { get; }
        
        [JsonConverter(typeof(ByteArrayStringConverter))]
        public byte[] City { get; }
        
        [JsonConverter(typeof(ByteArrayStringConverter))]
        public byte[] Organization { get; }
        
        public float Latitude { get; }
        
        public float Longitude { get; }
    }
}
