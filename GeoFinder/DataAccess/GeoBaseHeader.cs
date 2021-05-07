namespace GeoFinder.DataAccess
{
    public struct GeoBaseHeader
    {
        /// <summary>
        /// версия база данных
        /// </summary>
        public int Version;

        /// <summary>
        /// название/префикс для базы данных
        /// </summary>
        public byte[] Name;

        /// <summary>
        /// время создания базы данных
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// общее количество записей
        /// </summary>
        public int RecordsCount;

        /// <summary>
        /// смещение относительно начала файла до начала списка записей с геоинформацией
        /// </summary>
        public uint OffsetRanges;

        /// <summary>
        /// смещение относительно начала файла до начала индекса с сортировкой по названию городов
        /// </summary>
        public uint OffsetCities;

        /// <summary>
        /// смещение относительно начала файла до начала списка записей о местоположении
        /// </summary>
        public uint OffsetLocations;
    }
}
