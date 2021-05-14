using GeoFinder.Infrastructure.Abstractions;
using System.Runtime.InteropServices;

namespace GeoFinder.Infrastructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IPRange
    {
        /// <summary>
        /// IP address from
        /// </summary>
        public uint From;

        /// <summary>
        /// IP address to
        /// </summary>
        public uint To;

        /// <summary>
        /// The location index
        /// </summary>
        public uint LocationIndex;
    }

    public class IPRangeFromComparer : IRefComparer<IPRange>
    {
        public int Compare(ref IPRange x, ref IPRange y)
        {
            return x.From.CompareTo(y.From);
        }
    }
}
