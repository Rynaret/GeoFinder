using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GeoFinder.DataAccess
{
    [StructLayout(LayoutKind.Sequential, Size = 96, CharSet = CharSet.Ansi)]
    public struct GeoPoint1
    {
        public static uint MarshalSize = 96;

        /// <summary>
        /// название страны (случайная строка с префиксом "cou_") byte[8]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public sbyte[] Country;

        /// <summary>
        /// название области (случайная строка с префиксом "reg_") byte[12]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public sbyte[] Region;

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_") byte[12]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public sbyte[] Postal;

        /// <summary>
        /// название города (случайная строка с префиксом "cit_") byte[24]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public sbyte[] City;

        /// <summary>
        /// название организации (случайная строка с префиксом "org_") byte[32]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public sbyte[] Organization;

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
}
