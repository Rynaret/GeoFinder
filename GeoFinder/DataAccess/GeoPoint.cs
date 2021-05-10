using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GeoFinder.DataAccess
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct GeoPoint
    {
        public GeoPoint(Span<byte> city)
        {
            Latitude = 0;
            Longitude = 0;
            city.CopyTo(CitySpan);
        }

        /// <summary>
        /// название страны (случайная строка с префиксом "cou_") byte[8]
        /// </summary>
        private fixed byte _country[8];
        public Span<byte> CountrySpan => new(Unsafe.AsPointer(ref _country[0]), 8);

        /// <summary>
        /// название области (случайная строка с префиксом "reg_") byte[12]
        /// </summary>
        private fixed byte _region[12];
        public Span<byte> RegionSpan => new(Unsafe.AsPointer(ref _region[0]), 12);

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_") byte[12]
        /// </summary>
        private fixed byte _postal[12];
        public Span<byte> PostalSpan => new(Unsafe.AsPointer(ref _postal[0]), 12);

        /// <summary>
        /// название города (случайная строка с префиксом "cit_") byte[24]
        /// </summary>
        private fixed byte _city[24];
        public Span<byte> CitySpan => Trim(new(Unsafe.AsPointer(ref _city[0]), 24));

#if DEBUG // для нужд тестирования
        public string CityStr
        {
            get
            {
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

                    return new string((sbyte*)city, 0, emptyBytePosition + 1, Encoding.ASCII);
                }
            }
        }
#endif

        /// <summary>
        /// название организации (случайная строка с префиксом "org_") byte[32]
        /// </summary>
        private fixed byte _organization[32];
        public Span<byte> OrganizationSpan => new(Unsafe.AsPointer(ref _organization[0]), 32);

        /// <summary>
        /// широта
        /// </summary>
        public float Latitude;

        /// <summary>
        /// долгота
        /// </summary>
        public float Longitude;

        public static uint DatPositionToIndex(in uint addressInDat) => (uint)(addressInDat / Marshal.SizeOf<GeoPoint>());

        private static Span<byte> Trim(in Span<byte> result)
        {
            int iteration = result.Length - 2;
            for (; iteration >= 0; iteration--)
            {
                if (result[iteration + 1] != 0)
                {
                    break;
                }

                // trim пробелов в конце строки
                if ((result[iteration] == 32 || result[iteration] == 160) && result[iteration + 1] == 0)
                {
                    result[iteration] = 0;
                }
            }

            return result;
        }
    }

    public class GeoPointCityComparer : IComparer<GeoPoint>
    {
        public unsafe int Compare(GeoPoint x, GeoPoint y)
        {
            return x.CitySpan.SequenceCompareTo(y.CitySpan);
        }
    }
}
