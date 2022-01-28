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
        UserControl activeOverlay;
        MapPageViewModel mapPageViewModel;
        MapPinOverlayViewModel mapPinOverlayViewModel;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (e.NewElement is AviationMap)
                {
                    var formsMap = (AviationMap)e.NewElement;
                    nativeMap = Control as MapControl;

                    if (mapPageViewModel == null)
                    {
                        mapPageViewModel = (e.NewElement as AviationMap).BindingContext as MapPageViewModel;
                    }

                    if (mapPinOverlayViewModel == null)
                    {
                        mapPinOverlayViewModel = new MapPinOverlayViewModel();
                    }
                }

                nativeMap.Children.Clear();
                nativeMap.MapElementClick += OnMapElementClick;

                foreach (var mapItem in mapPageViewModel.MapItems)
                {
                    var snPosition = new BasicGeoposition { Latitude = mapItem.Position.Latitude, Longitude = mapItem.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    var mapIcon = new MapIcon();
                    
                    if (mapItem is Airport)
                        mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/icon_pin_solidblue.png"));
                    else if (mapItem is Navaid && (mapItem as Navaid).Type.Contains("VOR"))
                        mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/icon_vor.png"));
                    else if (mapItem is Navaid && (mapItem as Navaid).Type.Contains("NDB"))
                        mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/icon_ndb.png"));

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

                UpdatePins(sender as AviationMap);
            }
        }

        protected void UpdatePins(AviationMap map)
        {
            foreach (var mapItem in map.ItemsSource.OfType<MapItemBase>())
            {
                if (!nativeMap.MapElements.Any(e => (e is MapIcon) && (e as MapIcon).Location.Position.Latitude == mapItem.Latitude && (e as MapIcon).Location.Position.Longitude == mapItem.Longitude))
                {
                    var snPosition = new BasicGeoposition { Latitude = mapItem.Position.Latitude, Longitude = mapItem.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    var mapIcon = new MapIcon();
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/{mapItem.MapPin.Image}"));
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
                nativeMap.Children.Remove(activeOverlay);
                var mapItem = GetMapItem(mapIcon.Location.Position);
                var snPosition = new BasicGeoposition { Latitude = mapItem.Position.Latitude, Longitude = mapItem.Position.Longitude };
                var snPoint = new Geopoint(snPosition);

                if (mapItem == null)
                {
                    throw new Exception("Custom pin not found");
                }
                else if (mapItem == mapPinOverlayViewModel.SelectedMapItem)
                {
                    mapPinOverlayViewModel.SelectedMapItem = null;
                    activeOverlay = null;
                }
                else if (mapItem is Airport)
                {
                    mapPinOverlayViewModel.SelectedMapItem = mapItem;
                    activeOverlay = new AirportPinOverlay(mapPinOverlayViewModel, mapPageViewModel);
                    (activeOverlay as AirportPinOverlay).ViewModel.SelectedMapItem = mapItem as Airport;
                }
                else if (mapItem is Navaid)
                {
                    mapPinOverlayViewModel.SelectedMapItem = mapItem;
                    activeOverlay = new NavaidPinOverlay(mapPinOverlayViewModel, mapPageViewModel);
                    (activeOverlay as NavaidPinOverlay).ViewModel.SelectedMapItem = mapItem as Navaid;
                }

                if (activeOverlay != null)
                {
                    nativeMap.Children.Add(activeOverlay);
                    MapControl.SetLocation(activeOverlay, snPoint);
                    MapControl.SetNormalizedAnchorPoint(activeOverlay, new Windows.Foundation.Point(1, 0.75));
                }
            }
        }

        MapItemBase GetMapItem(BasicGeoposition position)
        {
            var pos = new Position(position.Latitude, position.Longitude);
            foreach (var item in mapPageViewModel.MapItems)
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
