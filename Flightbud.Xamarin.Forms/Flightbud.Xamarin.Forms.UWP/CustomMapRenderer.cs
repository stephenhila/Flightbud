﻿using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.UWP;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
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

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Console.WriteLine(e.PropertyName);
            if (sender is AviationMap && e.PropertyName == "VisibleRegion")
            {
                (sender as AviationMap).OnVisibleRegionChanged(new VisibleRegionChangedEventArgs());

                if (viewModel.MapItemsUpdating)
                {
                    if ((sender as AviationMap).AirportPins == null)
                        return;

                    UpdatePins(sender as AviationMap);
                    viewModel.MapItemsUpdating = false;
                }
            }
        }

        protected void UpdatePins(AviationMap map)
        {
            foreach (var pin in map.AirportPins)
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

        private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            if (mapIcon != null)
            {
                nativeMap.Children.Remove(airportPinOverlay);
                var mapItem = GetMapItem(mapIcon.Location.Position);
                if (mapItem == null)
                {
                    throw new Exception("Custom pin not found");
                }

                if (airportPinOverlay == null)
                {
                    MapItemOverlayViewModel overlayViewModel = new MapItemOverlayViewModel();
                    overlayViewModel.SelectedMapItem = mapItem;
                    airportPinOverlay = new AirportPinOverlay(overlayViewModel);
                }
                else
                {
                    airportPinOverlay.ViewModel.SelectedMapItem = mapItem;
                }

                var snPosition = new BasicGeoposition { Latitude = mapItem.Position.Latitude, Longitude = mapItem.Position.Longitude };
                var snPoint = new Geopoint(snPosition);

                nativeMap.Children.Add(airportPinOverlay);
                MapControl.SetLocation(airportPinOverlay, snPoint);
                MapControl.SetNormalizedAnchorPoint(airportPinOverlay, new Windows.Foundation.Point(0.5, 0.75));
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
