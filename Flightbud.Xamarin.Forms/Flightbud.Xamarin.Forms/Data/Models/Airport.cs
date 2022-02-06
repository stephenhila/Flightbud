using Flightbud.Xamarin.Forms.Data.Facade;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Models
{
    /// <summary>
    /// A single airport item data.
    /// </summary>
    public class Airport : MapItemBase
    {
        [CsvHelper.Configuration.Attributes.Ignore]
        BaseAviationPin _pin;
        [CsvHelper.Configuration.Attributes.Ignore]
        public override BaseAviationPin MapPin 
        {
            get 
            {
                if (_pin == null)
                {
                    _pin = new AirportPin 
                    {
                        Address = Country,
                        Label = Code,
                        Name = Name,
                        Position = Position,
                    };
                }
                return _pin; 
            }
        }

        [CsvHelper.Configuration.Attributes.Name("id")]
        public double Id { get; set; }
        [CsvHelper.Configuration.Attributes.Name("ident")]
        public string Code { get; set; }
        [CsvHelper.Configuration.Attributes.Name("type")]
        public string Type { get; set; }
        [CsvHelper.Configuration.Attributes.Name("name")]
        public override string Name { get; set; }
        [CsvHelper.Configuration.Attributes.Name("latitude_deg")]
        public override double Latitude { get; set; }
        [CsvHelper.Configuration.Attributes.Name("longitude_deg")]
        public override double Longitude { get; set; }
        [CsvHelper.Configuration.Attributes.Name("elevation_ft")]
        public double? Elevation { get; set; }
        [CsvHelper.Configuration.Attributes.Name("iso_country")]
        public override string Country { get; set; }
        
        List<Runway> _runways;

        [CsvHelper.Configuration.Attributes.Ignore]
        public List<Runway> Runways 
        {
            get
            {
                return _runways;
            }
        }

        List<AirportFrequency> _frequencies;
        [CsvHelper.Configuration.Attributes.Ignore]
        public List<AirportFrequency> Frequencies
        {
            get
            {
                return _frequencies;
            }
        }

        public override async Task LoadDetails(CancellationToken ct = default)
        {
            _frequencies = await (new AirportFrequencySylvanDataSource()).Get(Id, ct);
            _runways = await (new RunwaySylvanDataSource()).Get(Id, ct);
        }
    }
}
