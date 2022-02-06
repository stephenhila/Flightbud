using System;
using System.Collections.Generic;
using System.Text;

namespace Flightbud.Xamarin.Forms.Data.Models
{
    public class AirportFrequency
    {
        [CsvHelper.Configuration.Attributes.Name("airport_ref")]
        public double AirportId { get; set; }

        [CsvHelper.Configuration.Attributes.Name("type")]
        public string Type { get; set; }

        [CsvHelper.Configuration.Attributes.Name("description")]
        public string Description { get; set; }

        [CsvHelper.Configuration.Attributes.Name("frequency_mhz")]
        public double? Frequency { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string FrequencyInMhz
        {
            get
            {
                if (Frequency == null)
                {
                    return Constants.NO_DATA_AVAILABLE;
                }
                else
                {
                    return $"{String.Format("{0:0.000}",Frequency)}MHz";
                }
            }
        }
    }
}
