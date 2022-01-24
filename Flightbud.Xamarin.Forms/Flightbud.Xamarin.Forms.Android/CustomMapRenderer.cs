using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.Droid;
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
        MapPageViewModel viewModel;

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

                    if (viewModel == null)
                    {
                        viewModel = (e.NewElement as AviationMap).BindingContext as MapPageViewModel;
                    }
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

                await UpdatePins(sender as AviationMap);
            }
        }

        protected async Task UpdatePins(AviationMap map)
        {
            foreach (var pin in map.AirportPins)
            {
                if (!map.Pins.Any(p => p.Position.Equals(pin.Position)))
                {
                    await Device.InvokeOnMainThreadAsync(() => map.Pins.Add(pin));
                }
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.SetInfoWindowAdapter(this);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.icon_pin_solidblue));
            return marker;
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetMapItem(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var mapItem = GetMapItem(marker);
                if (mapItem == null)
                {
                    throw new Exception("Custom pin not found");
                }

                if (mapItem.Name.Equals("Xamarin"))
                {
                    view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
                }
                else
                {
                    view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                }

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
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
            foreach (var item in viewModel.MapItems)
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