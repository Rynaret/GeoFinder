using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GeoFinder.Infrastructure.DataAccess
{
    public class GeoBaseConnector
    {
        private readonly string _pathToFile = $"{AppContext.BaseDirectory}DataAccess\\geobase.dat";

        public GeoBaseHeader Header;

        public IPRange[] IPRanges;
        public Location[] Locations;
        public uint[] LocationsDatPosSortByCity;

        /// <summary>
        /// Collecton for indexing (gives better performance during population the Locations in proper order)
        /// Dictionary.Key - position of record in the dat file relative to Header.offset_locations
        /// Dictionary.Value - index of record in the collection LocationsDatPosSortByCity
        /// </summary>
        public Dictionary<uint, int> LocationsSortByCityDict;

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
                Header = MemoryMarshal.Cast<byte, GeoBaseHeader>(buffer)[0];
            }

            LocationsDatPosSortByCity = new uint[Header.RecordsCount];
            LocationsSortByCityDict = new Dictionary<uint, int>(LocationsDatPosSortByCity.Length);
            Locations = new Location[Header.RecordsCount];
            IPRanges = new IPRange[Header.RecordsCount];

            var tasks = new Task[parallelism];

            // load LocationsDatPosSortByCity first, because they give information about Locations collection sorting
            for (int i = 0; i < parallelism; i++)
            {
                int iteration = i;

                var loadIndexesTask = Task.Run(() => Load(
                    iteration,
                    parallelism,
                    Header.OffsetCities,
                    LocationsDatPosSortByCity,
                    GetIndex
                ));
                tasks[i] = loadIndexesTask;
            }
            Task.WaitAll(tasks);

            for (int i = 0; i < LocationsDatPosSortByCity.Length; i++)
            {
                LocationsSortByCityDict.Add(LocationsDatPosSortByCity[i], i);
            }

            // load rest data
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

                var loadLocationsTask = Task.Run(() => Load(
                    iteration,
                    parallelism,
                    Header.OffsetLocations,
                    Locations,
                    GetLocationIndex
                ));
                tasks[i * 2 + 1] = loadLocationsTask;
            }

            Task.WaitAll(tasks);
        }

        private void Load<T>(
            in int iteration,
            in int parallelism,
            in uint offsetInFile,
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
                result[getIndex(i, offset)] = MemoryMarshal.Cast<byte, T>(buffer)[0];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateModulo(int parallelism, int iteration, int recordCount)
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
        private int GetLocationIndex(int iteration, int offset)
        {
            return LocationsSortByCityDict[(uint)((iteration + offset) * Marshal.SizeOf<Location>())];
        }
    }
}
