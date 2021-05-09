using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GeoFinder.DataAccess
{
    public class GeoBaseConnector
    {
        private readonly string _pathToFile = "./DataAccess/geobase.dat";

        public GeoBaseHeader Header;

        public IPRange[] IPRanges;
        public GeoPoint[] GeoPoints;
        public uint[] GeoPointsDatPosSortByCity;

        /// <summary>
        /// Коллекция для более быстрого поиска нужного индекса
        /// </summary>
        public Dictionary<uint, int> GeoPointsSortByCityDict;

        public GeoBaseConnector(bool autoInit = true)
        {
            if (autoInit)
            {
                Init(parallelism: 4);
            }
        }

        public void Init(int parallelism)
        {
            using (FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Span<byte> buffer = stackalloc byte[Marshal.SizeOf<GeoBaseHeader>()];

                stream.Read(buffer);
                Header = ReadMemoryMarshal<GeoBaseHeader>(buffer);
            }

            GeoPointsDatPosSortByCity = new uint[Header.RecordsCount];
            GeoPointsSortByCityDict = new Dictionary<uint, int>(GeoPointsDatPosSortByCity.Length);
            GeoPoints = new GeoPoint[Header.RecordsCount];
            IPRanges = new IPRange[Header.RecordsCount];

            var tasks = new Task[parallelism];

            // загружаем индексы записей местоположения первыми, т к они отсортированны по названию города
            // и более того IPRange.location_index указывает на индекс записи о местоположении
            for (int i = 0; i < parallelism; i++)
            {
                int iteration = i;

                var loadIndexesTask = Task.Run(() => Load(
                    iteration,
                    parallelism,
                    Header.OffsetCities,
                    GeoPointsDatPosSortByCity,
                    GetIndex
                ));
                tasks[i] = loadIndexesTask;
            }
            Task.WaitAll(tasks);

            for (int i = 0; i < GeoPointsDatPosSortByCity.Length; i++)
            {
                GeoPointsSortByCityDict.Add(GeoPointsDatPosSortByCity[i], i);
            }

            // загружаем оставшиеся данные
            tasks = new Task[parallelism * 2];
            for (int i = 0; i < parallelism; i++)
            {
                int iteration = i;

                var loadIpRangesTask = Task.Run(() => Load(
                    iteration,
                    parallelism,
                    Header.OffsetRanges,
                    IPRanges,
                    GetIndex
                ));
                tasks[i * 2] = loadIpRangesTask;

                var loadGeoPointsTask = Task.Run(() => Load(
                    iteration,
                    parallelism,
                    Header.OffsetLocations,
                    GeoPoints,
                    GetGeoPointIndex
                ));
                tasks[i * 2 + 1] = loadGeoPointsTask;
            }

            Task.WaitAll(tasks);
        }

        private void Load<T>(
            int iteration,
            int parallelism,
            uint offsetInFile,
            T[] result,
            in Func<int, int, int> getIndex
        ) where T : struct
        {
            using FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            int offset = iteration * Header.RecordsCount / parallelism;

            int structSize = Marshal.SizeOf<T>();

            stream.Position = offsetInFile + structSize * offset;

            Span<byte> buffer = stackalloc byte[structSize];

            int modulo = CalculateModulo(parallelism, iteration, Header.RecordsCount);

            for (int i = 0; i < Header.RecordsCount / parallelism + modulo; i++)
            {
                stream.Read(buffer);
                result[getIndex(i, offset)] = ReadMemoryMarshal<T>(buffer);
            }
        }

        private T ReadMemoryMarshal<T>(ReadOnlySpan<byte> array)
            where T : struct
        {
            return MemoryMarshal.Cast<byte, T>(array)[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateModulo(int parallelism, int iteration, int recordCount)
        {
            return parallelism - 1 - iteration == 0
                ? recordCount % parallelism
                : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetIndex(int iteration, int offset)
        {
            return iteration + offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetGeoPointIndex(int iteration, int offset)
        {
            return GeoPointsSortByCityDict[(uint)((iteration + offset) * Marshal.SizeOf<GeoPoint>())];
        }
    }
}
