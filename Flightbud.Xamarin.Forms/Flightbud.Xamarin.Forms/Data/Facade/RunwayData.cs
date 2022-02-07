using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class RunwaySylvanDataSource : IAirportDetailsData<Runway>
    {
        public async Task<List<Runway>> Get(double airportId, CancellationToken ct = default)
        {
            List<Runway> runways = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.RUNWAY_DATA_RESOURCE))
            {
                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    var reader = await Sylvan.Data.Csv.CsvDataReader.CreateAsync(streamReader);
                    runways = new List<Runway>();

                    while (await reader.ReadAsync(ct))
                    {
                        double airportIdValue = reader.GetDouble(1);

                        if (airportId == airportIdValue)
                        {
                            Runway runway = new Runway
                            {
                                AirportId = airportIdValue,
                                Surface = reader.GetString(5),
                                HeadingLowEnd = reader.GetString(8),
                                HeadingHighEnd = reader.GetString(14),
                            };

                            string lengthValue = reader.GetString(3);
                            if (!string.IsNullOrEmpty(lengthValue))
                            {
                                runway.Length = int.Parse(lengthValue);
                            }
                            string widthValue = reader.GetString(4);
                            if (!string.IsNullOrEmpty(widthValue))
                            {
                                runway.Width = int.Parse(widthValue);
                            }
                            string elevationLowEndValue = reader.GetString(11);
                            if (!string.IsNullOrEmpty(elevationLowEndValue))
                            {
                                runway.ElevationLowEnd = double.Parse(elevationLowEndValue);
                            }
                            string elevationHighEndValue = reader.GetString(17);
                            if (!string.IsNullOrEmpty(elevationHighEndValue))
                            {
                                runway.ElevationHighEnd = double.Parse(elevationHighEndValue);
                            }

                            runways.Add(runway);
                        }
                    }
                }
            }
            return runways;
        }
    }

    public class RunwayCsvReaderDataSource : IAirportDetailsData<Runway>
    {
        public async Task<List<Runway>> Get(double airportId, CancellationToken ct = default)
        {
            List<Runway> runways = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.RUNWAY_DATA_RESOURCE))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    if (reader != null)
                    {
                        using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "," }))
                        {
                            csvReader.Read();
                            csvReader.ReadHeader();
                            runways = new List<Runway>();
                            while (await csvReader.ReadAsync())
                            {
                                // this is a hard-coded field. yes it is. fite me!!!
                                var airportRef = csvReader.GetField<double>(csvReader.GetFieldIndex("airport_ref"));
                                if (airportId == airportRef)
                                {
                                    Runway runway = csvReader.GetRecord<Runway>();
                                    runways.Add(runway);
                                }
                            }
                        }
                    }
                }
            }
            return runways;
        }
    }
}
