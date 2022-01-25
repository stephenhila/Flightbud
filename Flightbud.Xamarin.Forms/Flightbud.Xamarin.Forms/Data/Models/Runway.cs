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

        [CsvHelper.Configuration.Attributes.Ignore]
        public string Headings
        {
            get { return $"{HeadingLowEnd}/{HeadingHighEnd}"; }
        }
    }
}
