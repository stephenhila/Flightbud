using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.Droid;
using Flightbud.Xamarin.Forms.View.Controls;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AviationMap), typeof(CustomMapRenderer))]
namespace Flightbud.Xamarin.Forms.Droid
{
    /// <summary>
    /// Custom map renderer for the Android platform only.
    /// </summary>
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        MapPageViewModel mapPageViewModel;

        public CustomMapRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                if (e.NewElement is AviationMap)
                {
                    var formsMap = (AviationMap)e.NewElement;

                    if (mapPageViewModel == null)
                    {
                        mapPageViewModel = (e.NewElement as AviationMap).BindingContext as MapPageViewModel;
                    }
                }
            }
        }

        protected void UpdatePins(AviationMap map)
        {
            lock (mapPageViewModel.MapItems)
            {
                lock (map.Pins)
                {
                    foreach (var mapItem in mapPageViewModel.MapItems)
                    {
                            if (!(map.Pins.Any(pin => 
                                   pin.Position.Latitude == mapItem.Position.Latitude 
                                && pin.Position.Longitude == mapItem.Position.Longitude)))
                            {
                                Device.InvokeOnMainThreadAsync(() => map.Pins.Add(mapItem.MapPin));
                            }
                    }
                }
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
            NativeMap.CameraIdle += NativeMap_CameraIdle;
            NativeMap.CameraMoveStarted += NativeMap_CameraMoveStarted;
        }

        private async void NativeMap_CameraMoveStarted(object sender, GoogleMap.CameraMoveStartedEventArgs e)
        {
            if (e.Reason == 1 || e.Reason == 2)
            {
                await mapPageViewModel.Map.OnMapPanning(new MapPanningEventArgs());
            }
        }

        private async void NativeMap_CameraIdle(object sender, EventArgs e)
        {
            await mapPageViewModel.Map.OnVisibleRegionChanged(new VisibleRegionChangedEventArgs { VisibleRegion = mapPageViewModel.Map.VisibleRegion });

            if (mapPageViewModel.Map.ItemsSource == null)
                return;

            UpdatePins(mapPageViewModel.Map);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            if (pin is AirportPin)
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.icon_map_airport));
            else if (pin is VorPin)
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.icon_map_vor));
            else if (pin is NdbPin)
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.icon_map_ndb));
            return marker;
        }

        async void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var mapItem = GetMapItem(e.Marker);
            if (mapItem == null)
            {
                throw new Exception("Map Item not found");
            }

            await mapPageViewModel.Map.OnMapItemDetailsRequested(new MapItemDetailsRequestedEventArgs { SelectedMapItem = mapItem});
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            mapPageViewModel.IsAutoFollow = false;

            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var mapItem = GetMapItem(marker);
                if (mapItem == null)
                {
                    throw new Exception("Custom pin not found");
                }

                if (mapItem is Airport)
                {
                    view = inflater.Inflate(Resource.Layout.AirportPinOverlay, null);

                    view.FindViewById<TextView>(Resource.Id.AirportCode).Text = (mapItem as Airport).Code;
                    view.FindViewById<TextView>(Resource.Id.AirportName).Text = (mapItem as Airport).Name;

                    return view;
                }
                else if (mapItem is Navaid)
                {
                    view = inflater.Inflate(Resource.Layout.NavaidPinOverlay, null);

                    view.FindViewById<TextView>(Resource.Id.Code).Text = (mapItem as Navaid).Code;
                    view.FindViewById<TextView>(Resource.Id.Name).Text = (mapItem as Navaid).Name;
                    view.FindViewById<TextView>(Resource.Id.Type).Text = (mapItem as Navaid).Type;
                    view.FindViewById<TextView>(Resource.Id.Frequency).Text = (mapItem as Navaid).FrequencyWithUnits;
                    if ((mapItem as Navaid).Type.Contains("VOR"))
                        view.FindViewById<ImageView>(Resource.Id.InfoWindowButton).SetImageResource(Resource.Drawable.icon_map_vor);
                    if ((mapItem as Navaid).Type.Contains("NDB"))
                        view.FindViewById<ImageView>(Resource.Id.InfoWindowButton).SetImageResource(Resource.Drawable.icon_map_ndb);

                    return view;
                }
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        MapItemBase GetMapItem(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var item in mapPageViewModel.MapItems)
            {
                if (item.Position == position)
                {
                    return item;
                }
            }
            return null;
        }
    }
}