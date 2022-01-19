using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.DataModels
{
    public class Airport
    {
        [CsvHelper.Configuration.Attributes.Ignore]
        Position _position;
        [CsvHelper.Configuration.Attributes.Ignore]
        public Position Position 
        {
            get 
            { 
                if (_position == default)
                {
                    _position = new Position(Latitude, Longitude);
                }
                return _position;
            }
        }

        [CsvHelper.Configuration.Attributes.Ignore]
        Pin _pin;
        [CsvHelper.Configuration.Attributes.Ignore]
        public Pin MapPin 
        {
            get 
            {
                if (_pin == null)
                {
                    _pin = new Pin 
                    {
                        Label = Code,
                        Position = Position,
                    };
                }
                return _pin; 
            }
        }

        [CsvHelper.Configuration.Attributes.Name("id")]
        public string Id { get; set; }
        [CsvHelper.Configuration.Attributes.Name("ident")]
        public string Code { get; set; }
        [CsvHelper.Configuration.Attributes.Name("type")]
        public string Type { get; set; }
        [CsvHelper.Configuration.Attributes.Name("name")]
        public string Name { get; set; }
        [CsvHelper.Configuration.Attributes.Name("latitude_deg")]
        public double Latitude { get; set; }
        [CsvHelper.Configuration.Attributes.Name("longitude_deg")]
        public double Longitude { get; set; }
    }
}
