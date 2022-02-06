using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class NavaidDataSylvanDataSource : IMapRegionData<MapItemBase>
    {
        public async Task<List<MapItemBase>> Get(Position center, double radius, CancellationToken ct)
        {
            MapSpan region = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(radius));
            List<MapItemBase> navaids = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.NAVAID_DATA_RESOURCE))
            {
                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    var reader = await Sylvan.Data.Csv.CsvDataReader.CreateAsync(streamReader);
                    navaids = new List<MapItemBase>();

                    while (await reader.ReadAsync(ct))
                    {
                        Navaid navaid = new Navaid
                        {
                            Code = reader.GetString(2),
                            Name = reader.GetString(3),
                            Type = reader.GetString(4),
                            Latitude = reader.GetDouble(6),
                            Longitude = reader.GetDouble(7),
                        };

                        string frequencyValue = reader.GetString(5);
                        if (!string.IsNullOrEmpty(frequencyValue))
                        {
                            navaid.Frequency = double.Parse(frequencyValue);
                        }

                        if (navaid.Latitude < center.Latitude + (region.LatitudeDegrees / 2)
                                && navaid.Latitude > center.Latitude - (region.LatitudeDegrees / 2)
                                && navaid.Longitude < center.Longitude + (region.LongitudeDegrees / 2)
                                && navaid.Longitude > center.Longitude - (region.LongitudeDegrees / 2))
                        {
                            navaids.Add(navaid);
                        }
                    }
                }
            }
            return navaids;
        }
    }

    public class NavaidDataCsvReaderDataSource : IMapRegionData<MapItemBase>
    {
        public async Task<List<MapItemBase>> Get(Position center, double radius, CancellationToken ct)
        {
            MapSpan region = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(radius));
            List<MapItemBase> navaids = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.NAVAID_DATA_RESOURCE))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    if (reader != null)
                    {
                        using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "," }))
                        {
                            csvReader.Read();
                            csvReader.ReadHeader();
                            navaids = new List<MapItemBase>();
                            while (await csvReader.ReadAsync())
                            {
                                // this is a hard-coded field. yes it is. fite me!!!
                                var latitudeDegreeField = csvReader.GetField<double>(csvReader.GetFieldIndex("latitude_deg"));
                                var longitudeDegreeField = csvReader.GetField<double>(csvReader.GetFieldIndex("longitude_deg"));
                                var navaidType = csvReader.GetField<string>(csvReader.GetFieldIndex("type"));
                                if (latitudeDegreeField < center.Latitude + (region.LatitudeDegrees / 2)
                                     && latitudeDegreeField > center.Latitude - (region.LatitudeDegrees / 2)
                                     && longitudeDegreeField < center.Longitude + (region.LongitudeDegrees / 2)
                                     && longitudeDegreeField > center.Longitude - (region.LongitudeDegrees / 2))
                                {
                                    Navaid navaid = csvReader.GetRecord<Navaid>();
                                    navaids.Add(navaid);
                                }
                            }
                        }
                    }
                }
            }
            return navaids;
        }
    }
}
