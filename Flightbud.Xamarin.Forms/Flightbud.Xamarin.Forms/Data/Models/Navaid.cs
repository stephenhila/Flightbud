using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flightbud.Xamarin.Forms.Data.Models
{
    public class Navaid : MapItemBase
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
                    if (Type.Contains("VOR") || Type.Contains("TACAN"))
                    {
                        _pin = new VorPin
                        {
                            Address = Country,
                            Label = Code,
                            Name = Name,
                            Position = Position,
                        };
                    }
                    else if (Type.Contains("NDB"))
                    {
                        _pin = new NdbPin
                        {
                            Address = Country,
                            Label = Code,
                            Name = Name,
                            Position = Position,
                        };
                    }

                }
                return _pin;
            }
        }

        [CsvHelper.Configuration.Attributes.Name("name")]
        public override string Name { get; set; }
        [CsvHelper.Configuration.Attributes.Name("name")]
        public override string Country { get; set; }
        [CsvHelper.Configuration.Attributes.Name("latitude_deg")]
        public override double Latitude { get; set; }
        [CsvHelper.Configuration.Attributes.Name("longitude_deg")]
        public override double Longitude { get; set; }

        [CsvHelper.Configuration.Attributes.Name("ident")]
        public string Code { get; set; }
        [CsvHelper.Configuration.Attributes.Name("type")]
        public string Type { get; set; }
        [CsvHelper.Configuration.Attributes.Name("frequency_khz")]
        public double? Frequency { get; set; }

        [CsvHelper.Configuration.Attributes.Ignore]
        public string FrequencyWithUnits
        {
            get
            {
                if (Frequency == null)
                {
                    return Constants.NO_DATA_AVAILABLE;
                }
                else if (Type.Contains("VOR"))
                {
                    return $"{String.Format("{0:0.000}", (Frequency / 1000))}MHz";
                }
                else if (Type.Contains("NDB"))
                {
                    return $"{String.Format("{0:0.0}", Frequency)}KHz";
                }
                else
                {
                    return Constants.NO_DATA_SUPPORT;
                }
            }
        }

        public override async Task LoadDetails(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
