using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class AirportFrequencyData
    {
        public List<AirportFrequency> Get(double airportId)
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
                            while (csvReader.Read())
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
