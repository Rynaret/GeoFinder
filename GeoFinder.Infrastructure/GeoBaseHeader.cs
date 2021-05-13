using System.Runtime.InteropServices;

namespace GeoFinder.Infrastructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct GeoBaseHeader
    {
        /// <summary>
        /// The version
        /// </summary>
        public int Version;

        /// <summary>
        /// The name
        /// </summary>
        public fixed byte Name[32];

        /// <summary>
        /// Created at
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// Total records
        /// </summary>
        public int RecordsCount;

        /// <summary>
        /// Offset in the dat file for IPRanges data
        /// </summary>
        public uint OffsetRanges;

        /// <summary>
        /// Offset in the dat file for Indexes of Locations, which Sorted by City
        /// </summary>
        public uint OffsetCities;

        /// <summary>
        /// Offset in the dat file for Locations
        /// </summary>
        public uint OffsetLocations;
    }
}
