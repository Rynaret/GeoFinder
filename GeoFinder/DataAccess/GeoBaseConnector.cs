using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoFinder.DataAccess
{
    public class GeoBaseConnector
    {
        public IPRange[] IPRanges;
        public GeoPoint2[] GeoPoints;
        public GeoPointCopy[] GeoPointsCopy;
        public uint[] AddressSortedByCity;
        public GeoBaseHeader Header;

        private static string _pathToFile = "./DataAccess/geobase.dat";

        public GeoBaseConnector()
        {
            InitAsync();
        }

        public void InitAsync()
        {
            GeoBaseHeader header;

            using (FileStream fileStream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using BinaryReader reader = new(fileStream);
                header = new GeoBaseHeader
                {
                    Version = reader.ReadInt32(),
                    Name = reader.ReadBytes(32),
                    Timestamp = reader.ReadUInt64(),
                    RecordsCount = reader.ReadInt32(),
                    OffsetRanges = reader.ReadUInt32(),
                    OffsetCities = reader.ReadUInt32(),
                    OffsetLocations = reader.ReadUInt32()
                };
            }

            AddressSortedByCity = new uint[header.RecordsCount];
            GeoPoints = new GeoPoint2[header.RecordsCount];
            GeoPointsCopy = new GeoPointCopy[header.RecordsCount];
            IPRanges = new IPRange[header.RecordsCount];
            
            int parallelism = 4;
            int extraItems = header.RecordsCount % 4;
            Task[] tasks = new Task[parallelism * 3];
            for (int i = 0; i < parallelism; i++)
            {
                int index = i;
                //var loadGeoPointsTask = Task.Run(() => LoadGeoPoints(header, index, parallelism));
                //tasks[i] = (loadGeoPointsTask);

                //var loadGeoPointsTask = Task.Run(() => Load(header, index, parallelism, header.OffsetLocations, GeoPoints));
                //tasks[0] = (loadGeoPointsTask);

                ////15-16ms
                //var loadIpRangesTask = Task.Run(() => LoadIPRanges(header, index, parallelism));
                //tasks.AddLast(loadIpRangesTask);

                //var loadIpRangesTask = Task.Run(() => Load(header, index, parallelism, header.OffsetRanges, IPRanges));
                //tasks[1] = (loadIpRangesTask);

                var loadIpRangesTask = Task.Run(() => Load(header, index, parallelism, header.OffsetRanges, IPRanges, Marshal.SizeOf<IPRange>()));
                tasks[i * 3] = (loadIpRangesTask);

                // загружаем индексы записей местоположения вне цикла выше, т к они отсортированны по названию города
                // и более того IPRange.location_index указывает на индекс записи о местоположении
                var loadIndexesTask = Task.Run(() => Load(header, index, parallelism, header.OffsetCities, AddressSortedByCity, Marshal.SizeOf<uint>()));
                tasks[i * 3 + 1] = (loadIndexesTask);

                //var loadGeoPointsTask = Task.Run(() => Load(header, index, parallelism, header.OffsetLocations, GeoPoints, GeoPoint.MarshalSize));
                //tasks[i * 3 + 2] = (loadGeoPointsTask);

                var loadGeoPointsTask = Task.Run(() => LoadGeoPointsCopy(header, index, parallelism));
                tasks[i * 3 + 2] = (loadGeoPointsTask);
            }

            Task.WaitAll(tasks);

            foreach (var item in GeoPoints)
            {
                var r = item.CountryStr;
                var r2 = item.RegionStr;
                var r3 = item.PostalStr;
                var r4 = item.CityStr;
                var r5 = item.OrganizationStr;
            }
        }

        private static T ReadMemoryMarshal<T>(ReadOnlySpan<byte> array) where T : struct
        {
            return MemoryMarshal.Cast<byte, T>(array)[0];
        }

        static T ReadUsingPointer<T>(ReadOnlySpan<byte> data) where T : unmanaged
        {
            unsafe
            {
                fixed (byte* packet = &data[0])
                {
                    return *(T*)packet;
                }
            }
        }

        private void Load<T>(
            GeoBaseHeader header,
            int iteration,
            int parallelism,
            uint offsetInFile,
            T[] result,
            int structSize // вынуждены добавить, так как Marshal.SizeOf<GeoPoint>() плюётся ошибкой
        ) where T : unmanaged
        {
            using FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            int offset = iteration * header.RecordsCount / parallelism;

            stream.Position = offsetInFile + structSize * offset;

            var buffer = new Span<byte>(new byte[structSize]);

            for (int i = 0; i < header.RecordsCount / parallelism; i++)
            {
                stream.Read(buffer);
                result[i + offset] = (ReadMemoryMarshal<T>(buffer));
            }
        }

        private void LoadGeoPointsCopy(GeoBaseHeader header, int iteration, int parallelism)
        {
            using FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            int offset = iteration * header.RecordsCount / parallelism;
            stream.Position = header.OffsetLocations + GeoPoint.MarshalSize * offset;

            var buffer = new Span<byte>(new byte[GeoPoint.MarshalSize]);

            for (int i = 0; i < header.RecordsCount / parallelism; i++)
            {
                stream.Read(buffer);

                GeoPoints[i + offset] = (ReadMemoryMarshal<GeoPoint2>(buffer));
            }
        }

        private void LoadGeoPoints(GeoBaseHeader header, int iteration, int parallelism)
        {
            using FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            int offset = iteration * header.RecordsCount / parallelism;
            stream.Position = header.OffsetLocations + GeoPoint.MarshalSize * offset;

            using BinaryReader reader = new(stream, Encoding.ASCII);
            for (int i = 0; i < header.RecordsCount / parallelism; i++)
            {
                var geoPoint = new GeoPoint
                {
                    AddressInDat = (uint)(stream.Position - header.OffsetLocations),
                    Country = reader.ReadChars(8),
                    Region = reader.ReadChars(12),
                    Postal = reader.ReadChars(12),
                    City = reader.ReadChars(24),
                    Organization = reader.ReadChars(32),

                    Latitude = reader.ReadSingle(),
                    Longitude = reader.ReadSingle()
                };
                //GeoPointsOld[i + offset] = (geoPoint);
            }
        }

        public void Init()
        {
            var ipAddresses = new LinkedList<IPRange>();
            var geoPoints = new LinkedList<GeoPoint>();
            var addressSortedByCity = new LinkedList<uint>();
            GeoBaseHeader header;

            using (FileStream fileStream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using BinaryReader reader = new(fileStream);
                header = new GeoBaseHeader
                {
                    Version = reader.ReadInt32(),
                    Name = reader.ReadBytes(32),
                    Timestamp = reader.ReadUInt64(),
                    RecordsCount = reader.ReadInt32(),
                    OffsetRanges = reader.ReadUInt32(),
                    OffsetCities = reader.ReadUInt32(),
                    OffsetLocations = reader.ReadUInt32()
                };

                for (int i = 0; i < header.RecordsCount; i++)
                {
                    var ipAddress = new IPRange
                    {
                        From = reader.ReadUInt32(),
                        To = reader.ReadUInt32(),
                        LocationIndex = reader.ReadUInt32()
                    };
                    ipAddresses.AddLast(ipAddress);
                }

                for (int i = 0; i < header.RecordsCount; i++)
                {
                    var geoPoint = new GeoPoint
                    {
                        //Country = reader.ReadBytes(8),
                        //Region = reader.ReadBytes(12),
                        //Postal = reader.ReadBytes(12),
                        //City = reader.ReadBytes(24),
                        //Organization = reader.ReadBytes(32),

                        Latitude = reader.ReadSingle(),
                        Longitude = reader.ReadSingle()
                    };
                    geoPoints.AddLast(geoPoint);
                }

                for (int i = 0; i < header.RecordsCount; i++)
                {
                    addressSortedByCity.AddLast(reader.ReadUInt32());
                }
            }
        }
    }
}
