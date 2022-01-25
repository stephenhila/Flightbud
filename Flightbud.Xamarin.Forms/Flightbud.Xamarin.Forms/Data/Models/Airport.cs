using Flightbud.Xamarin.Forms.Data.Facade;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Models
{
    /// <summary>
    /// A single airport item data.
    /// </summary>
    public class Airport : MapItemBase
    {
        [CsvHelper.Configuration.Attributes.Ignore]
        AirportPin _pin;
        [CsvHelper.Configuration.Attributes.Ignore]
        public AirportPin MapPin 
        {
            get 
            {
                if (_pin == null)
                {
                    _pin = new AirportPin 
                    {
                        Address = Code,
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
        public string Country { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public bool RunwaysLoading { get; set; } = false;
        
        RunwayData _runwayDataSource = new RunwayData();
        List<Runway> _runways;

        [CsvHelper.Configuration.Attributes.Ignore]
        public List<Runway> Runways 
        {
            get
            {
                if (_runways == null)
                {
                    _runways = _runwayDataSource.Get(Id);
                    RunwaysLoading = false;
                }
                return _runways;
            }
        }

        AirportFrequencyData _airportFrequencyDataSource = new AirportFrequencyData();
        List<AirportFrequency> _frequencies;

        [CsvHelper.Configuration.Attributes.Ignore]
        public List<AirportFrequency> Frequencies
        {
            get
            {
                if (_frequencies == null)
                {
                    _frequencies = _airportFrequencyDataSource.Get(Id);
                }
                return _frequencies;
            }
        }
    }
}
