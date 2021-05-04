namespace GeoFinder.DataAccess
{
    public struct IPRange
    {
        public static uint MarshalSize = 12;

        /// <summary>
        /// начало диапазона IP адресов
        /// </summary>
        public uint From { get; set; }

        /// <summary>
        /// конец диапазона IP адресов
        /// </summary>
        public uint To { get; set; }

        /// <summary>
        /// индекс записи о местоположении
        /// </summary>
        public uint LocationIndex { get; set; }
    }
}
