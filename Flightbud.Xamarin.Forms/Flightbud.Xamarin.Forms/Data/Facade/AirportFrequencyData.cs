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
    public class AirportFrequencySylvanDataSource : IAirportDetailsData<AirportFrequency>
    {
        public async Task<List<AirportFrequency>> Get(double airportId, CancellationToken ct = default)
        {
            List<AirportFrequency> frequencies = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.AIRPORT_FREQUENCY_DATA_RESOURCE))
            {
                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    var reader = await Sylvan.Data.Csv.CsvDataReader.CreateAsync(streamReader);
                    frequencies = new List<AirportFrequency>();

                    while (await reader.ReadAsync(ct))
                    {
                        double airportIdValue = reader.GetDouble(1);
                        
                        if (airportId == airportIdValue)
                        {
                            AirportFrequency frequency = new AirportFrequency
                            {
                                AirportId = airportIdValue,
                                Type = reader.GetString(3),
                                Description = reader.GetString(4),
                            };

                            string frequencyValue = reader.GetString(5);
                            if (!string.IsNullOrEmpty(frequencyValue))
                            {
                                frequency.Frequency = double.Parse(frequencyValue);
                            }

                            frequencies.Add(frequency);
                        }
                    }
                }
            }
            return frequencies;
        }
    }

    public class AirportFrequencyCsvReaderDataSource : IAirportDetailsData<AirportFrequency>
    {
        public async Task<List<AirportFrequency>> Get(double airportId, CancellationToken ct = default)
        {
            List<AirportFrequency> frequencies = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.AIRPORT_FREQUENCY_DATA_RESOURCE))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    if (reader != null)
                    {
                        using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "," }))
                        {
                            csvReader.Read();
                            csvReader.ReadHeader();
                            frequencies = new List<AirportFrequency>();
                            while (await csvReader.ReadAsync())
                            {
                                // this is a hard-coded field. yes it is. fite me!!!
                                var airportRef = csvReader.GetField<double>(csvReader.GetFieldIndex("airport_ref"));
                                if (airportId == airportRef)
                                {
                                    AirportFrequency frequency = csvReader.GetRecord<AirportFrequency>();
                                    frequencies.Add(frequency);
                                }
                            }
                        }
                    }
                }
            }
            return frequencies;
        }
    }
}
