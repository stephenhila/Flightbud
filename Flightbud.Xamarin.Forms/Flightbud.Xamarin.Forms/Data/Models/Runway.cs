namespace Flightbud.Xamarin.Forms.Data.Models
{
    public class Runway
    {
        [CsvHelper.Configuration.Attributes.Name("le_ident")]
        public string HeadingLowEnd { get; set; }
        [CsvHelper.Configuration.Attributes.Name("he_ident")]
        public string HeadingHighEnd { get; set; }
        [CsvHelper.Configuration.Attributes.Name("length_ft")]
        public int? Length { get; set; }
        [CsvHelper.Configuration.Attributes.Name("width_ft")]
        public int? Width { get; set; }
        [CsvHelper.Configuration.Attributes.Name("surface")]
        public string Surface { get; set; }
        [CsvHelper.Configuration.Attributes.Name("he_elevation_ft")]
        public double? ElevationHighEnd { get; set; }
        [CsvHelper.Configuration.Attributes.Name("le_elevation_ft")]
        public double? ElevationLowEnd { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string Headings
        {
            get 
            {
                if (HeadingLowEnd == null && HeadingHighEnd == null)
                    return Constants.NO_DATA_AVAILABLE;
                else if (HeadingLowEnd == null)
                    return $"{HeadingHighEnd}";
                else if (HeadingHighEnd == null)
                    return $"{HeadingLowEnd}";
                else
                    return $"{HeadingLowEnd} - {HeadingHighEnd}"; 
            }
        }

        public string LengthInFt
        {
            get
            {
                if (Length == null)
                {
                    return Constants.NO_DATA_AVAILABLE;
                }
                else
                {
                    return $"{Length}ft";
                }
            }
        }

        public string ElevationsInFt
        {
            get 
            {
                if (ElevationLowEnd == null || ElevationHighEnd == null)
                    return Constants.NO_DATA_AVAILABLE;
                else
                    return $"{ElevationLowEnd}ft - {ElevationHighEnd}ft"; 
            }
        }
    }
}
