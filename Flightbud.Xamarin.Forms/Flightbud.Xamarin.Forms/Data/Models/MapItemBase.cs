using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Models
{
    /// <summary>
    /// A single map item data.
    /// </summary>
    public abstract class MapItemBase
    {
        public abstract double Latitude { get; set; }
        public abstract double Longitude { get; set; }

        public abstract string Name { get; set; }

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
    }
}
