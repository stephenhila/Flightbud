using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.UWP;
using Flightbud.Xamarin.Forms.View.Controls;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.ComponentModel;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(AviationMap), typeof(CustomMapRenderer))]
namespace Flightbud.Xamarin.Forms.UWP
{
    /// <summary>
    /// Custom map renderer for the UWP platform only.
    /// </summary>
    public class CustomMapRenderer : MapRenderer
    {
        MapControl nativeMap;
        AirportPinOverlay airportPinOverlay;
        MapPageViewModel viewModel;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                nativeMap.MapElementClick -= OnMapElementClick;
                nativeMap.Children.Clear();
                airportPinOverlay = null;
                nativeMap = null;
            }

            if (e.NewElement != null)
            {
                if (e.NewElement is AviationMap)
                {
                    var formsMap = (AviationMap)e.NewElement;
                    nativeMap = Control as MapControl;

                    if (viewModel == null)
                    {
                        viewModel = (e.NewElement as AviationMap).BindingContext as MapPageViewModel;
                    }

                    if (airportPinOverlay == null)
                    {
                        airportPinOverlay = new AirportPinOverlay(new AirportPinOverlayViewModel(), viewModel);
                    }
                }

                nativeMap.Children.Clear();
                nativeMap.MapElementClick += OnMapElementClick;

                foreach (var poi in viewModel.MapItems)
                {
                    var snPosition = new BasicGeoposition { Latitude = poi.Position.Latitude, Longitude = poi.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    var mapIcon = new MapIcon();
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/icon_pin_solidblue.png"));
                    mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                    mapIcon.Location = snPoint;
                    mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);

                    nativeMap.MapElements.Add(mapIcon);
                }
            }
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (sender is AviationMap && e.PropertyName == "VisibleRegion")
            {
                await (sender as AviationMap).OnVisibleRegionChanged(new VisibleRegionChangedEventArgs());

                if ((sender as AviationMap).AirportPins == null)
                    return;

                UpdatePins(sender as AviationMap);
            }
        }

        protected void UpdatePins(AviationMap map)
        {
            foreach (var pin in map.AirportPins)
            {
                if (!map.Pins.Any(p => p.Position.Equals(pin.Position)))
                {
                    var snPosition = new BasicGeoposition { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    var mapIcon = new MapIcon();
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/icon_pin_solidblue.png"));
                    mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                    mapIcon.Location = snPoint;
                    mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);

                    nativeMap.MapElements.Add(mapIcon);
                }
            }
        }

        private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            nativeMap.Children.Clear();

            if (mapIcon != null)
            {
                nativeMap.Children.Remove(airportPinOverlay);
                var mapItem = GetMapItem(mapIcon.Location.Position);

                if (mapItem == null)
                {
                    throw new Exception("Custom pin not found");
                }
                else if (mapItem == airportPinOverlay.ViewModel.SelectedAirport)
                {
                    airportPinOverlay.ViewModel.SelectedAirport = null;
                    return;
                }

                if (mapItem is Airport)
                {
                    airportPinOverlay.ViewModel.SelectedAirport = mapItem as Airport;
                }

                var snPosition = new BasicGeoposition { Latitude = mapItem.Position.Latitude, Longitude = mapItem.Position.Longitude };
                var snPoint = new Geopoint(snPosition);
                nativeMap.Children.Add(airportPinOverlay);
                MapControl.SetLocation(airportPinOverlay, snPoint);
                MapControl.SetNormalizedAnchorPoint(airportPinOverlay, new Windows.Foundation.Point(1, 0.75));
            }
        }

        MapItemBase GetMapItem(BasicGeoposition position)
        {
            var pos = new Position(position.Latitude, position.Longitude);
            foreach (var item in viewModel.MapItems)
            {
                if (item.Position == pos)
                {
                    return item;
                }
            }
            return default;
        }
    }
}
