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

            int emptyBytePosition = span.Length - 1;
            for (; emptyBytePosition > 0; emptyBytePosition--)
            {
                if (span[emptyBytePosition] != 0)
                {
                    break;
                }
            }
            emptyBytePosition++;

            writer.WriteStringValue(span.Slice(0, emptyBytePosition));
        }
    }
}
