using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GeoFinder.DataAccess
{
    [StructLayout(LayoutKind.Sequential, Size = 96, CharSet = CharSet.Ansi)]
    public unsafe struct GeoPoint2
    {
        public static int MarshalSize = 96;

        /// <summary>
        /// название страны (случайная строка с префиксом "cou_") byte[8]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public fixed sbyte Country[8];
        public string CountryStr
        {
            get
            {
                fixed (sbyte* pointer = Country)
                {
                    int emptyBytePosition = 8;
                    for (; emptyBytePosition >= 0; emptyBytePosition--)
                    {
                        if (pointer[emptyBytePosition] != 0)
                        {
                            break;
                        }
                    }

                    return new string(pointer, 0, emptyBytePosition + 1, Encoding.ASCII);
                }
            }
        }

        /// <summary>
        /// название области (случайная строка с префиксом "reg_") byte[12]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public fixed sbyte Region[12];
        public string RegionStr
        {
            get
            {
                fixed (sbyte* pointer = Region)
                {
                    int emptyBytePosition = 12;
                    for (; emptyBytePosition >= 0; emptyBytePosition--)
                    {
                        if (pointer[emptyBytePosition] != 0)
                        {
                            break;
                        }
                    }

                    return new string(pointer, 0, emptyBytePosition + 1, Encoding.ASCII);
                }
            }
        }

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_") byte[12]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public fixed sbyte Postal[12];
        public string PostalStr
        {
            get
            {
                fixed (sbyte* pointer = Postal)
                {
                    int emptyBytePosition = 12;
                    for (; emptyBytePosition >= 0; emptyBytePosition--)
                    {
                        if (pointer[emptyBytePosition] != 0)
                        {
                            break;
                        }
                    }

                    return new string(pointer, 0, emptyBytePosition + 1, Encoding.ASCII);
                }
            }
        }

        /// <summary>
        /// название города (случайная строка с префиксом "cit_") byte[24]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public fixed sbyte City[24];

        public Span<sbyte> CitySpan => new Span<sbyte>(Unsafe.AsPointer(ref City[0]), 24);
        public string CityStr
        {
            get
            {
                fixed (sbyte* city = City)
                {
                    int emptyBytePosition = 23;
                    for (; emptyBytePosition >= 0; emptyBytePosition--)
                    {
                        if (city[emptyBytePosition] != 0)
                        {
                            break;
                        }
                    }

                    return new string(city, 0, emptyBytePosition + 1, Encoding.ASCII);
                }
            }
        }

        public static uint AddressInDatToIndex(in uint addressInDat) => (uint)(addressInDat / MarshalSize);

        /// <summary>
        /// название организации (случайная строка с префиксом "org_") byte[32]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public fixed sbyte Organization[32];
        public string OrganizationStr
        {
            get
            {
                fixed (sbyte* pointer = Organization)
                {
                    int emptyBytePosition = 32;
                    for (; emptyBytePosition >= 0; emptyBytePosition--)
                    {
                        if (pointer[emptyBytePosition] != 0)
                        {
                            break;
                        }
                    }

                    return new string(pointer, 0, emptyBytePosition + 1, Encoding.ASCII);
                }
            }
        }

        /// <summary>
        /// широта
        /// </summary>
        [MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        public float Latitude;

        /// <summary>
        /// долгота
        /// </summary>
        [MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        public float Longitude;

        ///// <summary>
        ///// адресс записи в файле относительно Header.offset_locations
        ///// </summary>
        //[FieldOffset(92), MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        //public uint AddressInDat;
    }


    [StructLayout(LayoutKind.Sequential, Size = 96, CharSet = CharSet.Ansi)]
    public struct GeoPointCopy
    {
        /// <summary>
        /// название страны (случайная строка с префиксом "cou_") byte[8]
        /// </summary>
        public string Country;

        /// <summary>
        /// название области (случайная строка с префиксом "reg_") byte[12]
        /// </summary>
        public string Region;

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_") byte[12]
        /// </summary>
        public string Postal;

        /// <summary>
        /// название города (случайная строка с префиксом "cit_") byte[24]
        /// </summary>
        public string City;

        /// <summary>
        /// название организации (случайная строка с префиксом "org_") byte[32]
        /// </summary>
        public string Organization;

        /// <summary>
        /// широта
        /// </summary>
        public float Latitude;

        /// <summary>
        /// долгота
        /// </summary>
        public float Longitude;

        ///// <summary>
        ///// адресс записи в файле относительно Header.offset_locations
        ///// </summary>
        //[FieldOffset(92), MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        //public uint AddressInDat;
    }
}
