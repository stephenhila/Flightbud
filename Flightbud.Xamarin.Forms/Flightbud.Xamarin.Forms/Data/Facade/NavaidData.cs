using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class NavaidDataSylvanDataSource : IMapRegionData<MapItemBase>
    {
        public async Task<List<MapItemBase>> Get(MapSpan visibleMapSpan, CancellationToken ct = default)
        {
            List<MapItemBase> navaids = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.NAVAID_DATA_RESOURCE))
            {
                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    var reader = await Sylvan.Data.Csv.CsvDataReader.CreateAsync(streamReader);
                    navaids = new List<MapItemBase>();

                    while (await reader.ReadAsync(ct))
                    {
                        double latitudeValue = reader.GetDouble(6);
                        double longitudeValue = reader.GetDouble(7);

                        if (latitudeValue < visibleMapSpan.Center.Latitude + (visibleMapSpan.LatitudeDegrees / 2)
                                && latitudeValue > visibleMapSpan.Center.Latitude - (visibleMapSpan.LatitudeDegrees / 2)
                                && longitudeValue < visibleMapSpan.Center.Longitude + (visibleMapSpan.LongitudeDegrees / 2)
                                && longitudeValue > visibleMapSpan.Center.Longitude - (visibleMapSpan.LongitudeDegrees / 2))
                        {

                            Navaid navaid = new Navaid
                            {
                                Code = reader.GetString(2),
                                Name = reader.GetString(3),
                                Type = reader.GetString(4),
                                Latitude = latitudeValue,
                                Longitude = longitudeValue,
                            };

                            string frequencyValue = reader.GetString(5);
                            if (!string.IsNullOrEmpty(frequencyValue))
                            {
                                navaid.Frequency = double.Parse(frequencyValue);
                            }

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
        public async Task<List<MapItemBase>> Get(MapSpan visibleMapSpan, CancellationToken ct = default)
        {
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
                                if (latitudeDegreeField < visibleMapSpan.Center.Latitude + (visibleMapSpan.LatitudeDegrees / 2)
                                     && latitudeDegreeField > visibleMapSpan.Center.Latitude - (visibleMapSpan.LatitudeDegrees / 2)
                                     && longitudeDegreeField < visibleMapSpan.Center.Longitude + (visibleMapSpan.LongitudeDegrees / 2)
                                     && longitudeDegreeField > visibleMapSpan.Center.Longitude - (visibleMapSpan.LongitudeDegrees / 2))
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
