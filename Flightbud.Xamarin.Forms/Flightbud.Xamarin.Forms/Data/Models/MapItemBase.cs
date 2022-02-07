using Flightbud.Xamarin.Forms.View.Models;
using System.Threading;
using System.Threading.Tasks;
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
        public abstract string Country { get; set; }
        public abstract BaseAviationPin MapPin { get; }

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

        public abstract Task LoadDetails(CancellationToken ct = default);
    }
}
