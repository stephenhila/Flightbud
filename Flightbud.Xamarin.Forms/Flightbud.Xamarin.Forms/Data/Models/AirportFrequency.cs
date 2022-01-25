using System;
using System.Collections.Generic;
using System.Text;

namespace Flightbud.Xamarin.Forms.Data.Models
{
    public class AirportFrequency
    {
        [CsvHelper.Configuration.Attributes.Name("type")]
        public string Type { get; set; }

        [CsvHelper.Configuration.Attributes.Name("description")]
        public string Description { get; set; }

        [CsvHelper.Configuration.Attributes.Name("frequency_mhz")]
        public double? Frequency { get; set; }
    }
}
