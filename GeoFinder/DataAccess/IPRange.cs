namespace GeoFinder.DataAccess
{
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
}
