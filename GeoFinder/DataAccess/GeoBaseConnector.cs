using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoFinder.DataAccess
{
    public class GeoBaseConnector
    {
        public ConcurrentBag<IPRange> IPRanges = new();
        public ConcurrentBag<GeoPoint> GeoPoints = new();
        public LinkedList<uint> AddressSortedByCity = new();

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

            int parallelism = 4;
            var tasks = new LinkedList<Task>();
            for (int i = 0; i < parallelism; i++)
            {
                int index = i;

                // 60ms
                var loadGeoPointsTask = Task.Run(() => LoadGeoPoints(header, index, parallelism));
                tasks.AddLast(loadGeoPointsTask);

                //15-16ms
                var loadIpRangesTask = Task.Run(() => LoadIPRanges(header, index, parallelism));
                tasks.AddLast(loadIpRangesTask);
            }

            // загружаем индексы записей местоположения вне цикла выше, т к они отсортированны по названию города
            // и более того IPRange.location_index указывает на индекс записи о местоположении
            var loadIndexesTask = Task.Run(() => LoadIndexes(header, 0, 1));
            tasks.AddLast(loadIndexesTask);

            Task.WaitAll(tasks.ToArray());
        }

        private void LoadIndexes(GeoBaseHeader header, int iteration, int parallelism)
        {
            using FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.Position = header.OffsetCities + iteration * Marshal.SizeOf<uint>() * header.RecordsCount / parallelism;

            using BinaryReader reader = new(stream);
            for (int i = 0; i < header.RecordsCount / parallelism; i++)
            {
                AddressSortedByCity.AddLast(reader.ReadUInt32());
            }
        }

        private void LoadIPRanges(GeoBaseHeader header, int iteration, int parallelism)
        {
            using FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.Position = header.OffsetRanges + iteration * IPRange.MarshalSize * header.RecordsCount / parallelism;

            using BinaryReader reader = new(stream);
            for (int i = 0; i < header.RecordsCount / parallelism; i++)
            {
                var ipAddress = new IPRange
                {
                    From = reader.ReadUInt32(),
                    To = reader.ReadUInt32(),
                    LocationIndex = reader.ReadUInt32()
                };
                IPRanges.Add(ipAddress);
            }
        }

        private async Task LoadGeoPoints(GeoBaseHeader header, int iteration, int parallelism)
        {
            await Task.Delay(0);
            using FileStream stream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.Position = header.OffsetLocations + iteration * GeoPoint.MarshalSize * header.RecordsCount / parallelism;
            
            using BinaryReader reader = new(stream, Encoding.ASCII);
            for (int i = 0; i < header.RecordsCount / parallelism; i++)
            {
                var geoPoint = new GeoPoint
                {
                    AddressInDat = (uint)(stream.Position - header.OffsetLocations),
                    Country = reader.ReadBytes(8),
                    Region = reader.ReadBytes(12),
                    Postal = reader.ReadBytes(12),
                    City = reader.ReadBytes(24),
                    Organization = reader.ReadBytes(32),

                    Latitude = reader.ReadSingle(),
                    Longitude = reader.ReadSingle()
                };
                GeoPoints.Add(geoPoint);
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
                        Country = reader.ReadBytes(8),
                        Region = reader.ReadBytes(12),
                        Postal = reader.ReadBytes(12),
                        City = reader.ReadBytes(24),
                        Organization = reader.ReadBytes(32),

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
