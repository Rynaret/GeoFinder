using GeoFinder.Infrastructure.Abstractions;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GeoFinder.Infrastructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Location
    {
        public const int CityLength = 24;

        public Location(string city)
        {
            Latitude = 0;
            Longitude = 0;

            var citySpan = new Span<byte>(Unsafe.AsPointer(ref _city[0]), 24);
            Encoding.ASCII.GetBytes(city.AsSpan(), citySpan);
        }

        /// <summary>
        /// The country (string with prefix "cou_" byte[8]) 
        /// </summary>
        internal fixed byte _country[8];
        public ReadOnlySpan<byte> Country => new(Unsafe.AsPointer(ref _country[0]), 8);

        /// <summary>
        /// The region (string with prefix "reg_" byte[12]) 
        /// </summary>
        internal fixed byte _region[12];
        public ReadOnlySpan<byte> Region => new(Unsafe.AsPointer(ref _region[0]), 12);

        /// <summary>
        /// The postal (string with prefix "pos_" byte[12])
        /// </summary>
        internal fixed byte _postal[12];
        public ReadOnlySpan<byte> Postal => new(Unsafe.AsPointer(ref _postal[0]), 12);

        /// <summary>
        /// The city (string with prefix "cit_" byte[24])
        /// </summary>
        internal fixed byte _city[CityLength];
        public ReadOnlySpan<byte> City => new(Unsafe.AsPointer(ref _city[0]), 24);

        /// <summary>
        /// The organization (string with prefix "org_" byte[32])
        /// </summary>
        internal fixed byte _organization[32];
        public ReadOnlySpan<byte> Organization => new(Unsafe.AsPointer(ref _organization[0]), 32);

        /// <summary>
        /// The latitude
        /// </summary>
        public float Latitude;

        /// <summary>
        /// The longitude
        /// </summary>
        public float Longitude;

        public static uint DatPositionToIndex(in uint addressInDat) => (uint)(addressInDat / Marshal.SizeOf<Location>());

#if DEBUG // for testing purposes
        [Obsolete("WARN! Available only in Debug for testing purposes")]
        public string CityStr
        {
            get
            {
                // a bit faster (10ns) than => fixed (byte* city = _city) { return new string((sbyte*)city); }
                fixed (byte* city = _city)
                {
                    int emptyBytePosition = 23;
                    for (; emptyBytePosition >= 0; emptyBytePosition--)
                    {
                        if (city[emptyBytePosition] != 0)
                        {
                            break;
                        }
                    }

                    emptyBytePosition++;
                    return new string((sbyte*)city, 0, emptyBytePosition, Encoding.ASCII);
                }
            }
        }
#endif
    }

    public class LocationCityComparer : IRefComparer<Location>
    {
        private static readonly MethodInfo _sequenceCompareToMethod = Type.GetType("System.SpanHelpers")
            .GetMethod(
                name: "SequenceCompareTo",
                types: new[] { Type.GetType("System.Byte&"), typeof(int), Type.GetType("System.Byte&"), typeof(int) }
            );

        private delegate int SequenceCompareTo(ref byte first, int firstLength, ref byte second, int secondLength);

        private static readonly SequenceCompareTo _sequenceCompareTo = (SequenceCompareTo)Delegate.CreateDelegate(
            type: typeof(SequenceCompareTo),
            firstArgument: null,
            method: _sequenceCompareToMethod
        );

        // version with refs about 30% faster (compare to IComparer)
        public unsafe int Compare(ref Location x, ref Location y)
        {
            // about 25% faster than
            // return x.City.SequenceCompareTo(y.City);
            return _sequenceCompareTo(ref x._city[0], Location.CityLength, ref y._city[0], Location.CityLength);
        }
    }
}
