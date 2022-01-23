using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    /// <summary>
    /// Facade to manage data from the Airports data file.
    /// </summary>
    public class AirportData : IMapRegionData<Airport>
    {
        public List<Airport> Get(Position center, double radius)
        {
            List<Airport> airports = null;
            // this is a hard-coded field. yes it is. fite me!!!
            var airportsCsvResourceId = Constants.AIRPORT_DATA_RESOURCE;
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(airportsCsvResourceId);

            MapSpan region = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(radius));

            using (var reader = new System.IO.StreamReader(stream))
            {
                if (reader != null)
                {
                    using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "," }))
                    {
                        csvReader.Read();
                        csvReader.ReadHeader();
                        airports = new List<Airport>();
                        while (csvReader.Read())
                        {
                            // this is a hard-coded field. yes it is. fite me again!!!
                            var latitudeDegreeField = csvReader.GetField<double>(csvReader.GetFieldIndex("latitude_deg"));
                            var longitudeDegreeField = csvReader.GetField<double>(csvReader.GetFieldIndex("longitude_deg"));
                            var airportType = csvReader.GetField<string>(csvReader.GetFieldIndex("type"));
                            if (latitudeDegreeField < center.Latitude + (region.LatitudeDegrees / 2)
                                 && latitudeDegreeField > center.Latitude - (region.LatitudeDegrees / 2)
                                 && longitudeDegreeField < center.Longitude + (region.LongitudeDegrees / 2)
                                 && longitudeDegreeField > center.Longitude - (region.LongitudeDegrees / 2)
                                 && (airportType == "small_airport" 
                                  || airportType == "medium_airport" 
                                  || airportType == "large_airport"))
                            {
                                Airport airport = csvReader.GetRecord<Airport>();
                                airports.Add(airport);
                            }
                        }
                    }
                }
            }
            return airports;
        }
    }
}
