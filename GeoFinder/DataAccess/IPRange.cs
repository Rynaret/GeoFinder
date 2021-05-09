using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GeoFinder.DataAccess
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IPRange
    {
        /// <summary>
        /// начало диапазона IP адресов
        /// </summary>
        public uint From;

        /// <summary>
        /// конец диапазона IP адресов
        /// </summary>
        public uint To;

        /// <summary>
        /// индекс записи о местоположении
        /// </summary>
        public uint LocationIndex;
    }

    public class IPRangeFromComparer : IComparer<IPRange>
    {
        public int Compare(IPRange x, IPRange y)
        {
            return x.From.CompareTo(y.From);
        }
    }
}
