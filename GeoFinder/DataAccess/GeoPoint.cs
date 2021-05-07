namespace GeoFinder.DataAccess
{
    public struct GeoPoint
    {
        public static int MarshalSize = 96;

        /// <summary>
        /// название страны (случайная строка с префиксом "cou_") byte[8]
        /// </summary>
        public char[] Country { get; set; }

        /// <summary>
        /// название области (случайная строка с префиксом "reg_") byte[12]
        /// </summary>
        public char[] Region { get; set; }

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_") byte[12]
        /// </summary>
        public char[] Postal { get; set; }

        /// <summary>
        /// название города (случайная строка с префиксом "cit_") byte[24]
        /// </summary>
        public char[] City { get; set; }

        /// <summary>
        /// название организации (случайная строка с префиксом "org_") byte[32]
        /// </summary>
        public char[] Organization { get; set; }

        /// <summary>
        /// широта
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// долгота
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// адресс записи в файле относительно Header.offset_locations
        /// </summary>
        public uint AddressInDat { get; set; }
    }
}
