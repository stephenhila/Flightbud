﻿using Android.Content;
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

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (sender is AviationMap && e.PropertyName == "VisibleRegion")
            {
                await (sender as AviationMap).OnVisibleRegionChanged(new VisibleRegionChangedEventArgs());

                if ((sender as AviationMap).AirportPins == null 
                 && (sender as AviationMap).VorPins == null 
                 && (sender as AviationMap).NdbPins == null)
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

            foreach (var pin in map.VorPins)
            {
                if (!map.Pins.Any(p => p.Position.Equals(pin.Position)))
                {
                    await Device.InvokeOnMainThreadAsync(() => map.Pins.Add(pin));
                }
            }

            foreach (var pin in map.NdbPins)
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

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            if (pin is AirportPin)
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.icon_pin_solidblue));
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