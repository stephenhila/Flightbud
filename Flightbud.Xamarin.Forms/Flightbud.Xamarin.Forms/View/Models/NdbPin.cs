namespace Flightbud.Xamarin.Forms.View.Models
{
    public class NdbPin : BaseAviationPin
    {
        public string Name { get; set; }

        public override string Image => "icon_map_ndb.png";
    }
}
