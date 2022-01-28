using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class RunwayData
    {
        public List<Runway> Get(double airportId)
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
                            while (csvReader.Read())
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
