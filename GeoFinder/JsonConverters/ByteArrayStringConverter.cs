using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoFinder.JsonConverters
{
    public class ByteArrayStringConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            Span<byte> span = value.AsSpan();
            int emptyBytePosition = span.Length;
            for (; emptyBytePosition > 0; emptyBytePosition--)
            {
                if (span[emptyBytePosition - 1] != 0)
                {
                    break;
                }
            }

            writer.WriteStringValue(span.Slice(0, emptyBytePosition));
        }
    }
}
