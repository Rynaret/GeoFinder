namespace GeoFinder.Records
{
    public record GeoPointDto(
        string Country,
        string Region,
        string Postal,
        string City,
        string Organization,
        float Latitude,
        float Longitude
    );
}
