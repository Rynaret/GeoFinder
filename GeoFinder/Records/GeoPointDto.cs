using GeoFinder.JsonConverters;
using System.Text.Json.Serialization;

namespace GeoFinder.Records
{
    public record GeoPointDto
    {
        public GeoPointDto(
            byte[] Country,
            byte[] Region,
            byte[] Postal,
            byte[] City,
            byte[] Organization,
            float Latitude,
            float Longitude
        )
        {
            this.Country = Country;
            this.Region = Region;
            this.Postal = Postal;
            this.City = City;
            this.Organization = Organization;
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
