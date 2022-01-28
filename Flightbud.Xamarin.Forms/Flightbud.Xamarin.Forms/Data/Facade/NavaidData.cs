using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class NavaidData : IMapRegionData<MapItemBase>
    {
        public List<MapItemBase> Get(Position center, double radius)
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
                            while (csvReader.Read())
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
