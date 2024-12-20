﻿using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.Utils;
using Flightbud.Xamarin.Forms.UWP;
using Flightbud.Xamarin.Forms.View.Controls;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms;
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
        MapControl _nativeMap;
        UserControl _activeOverlay;
        MapPageViewModel _mapPageViewModel;
        MapPinOverlayViewModel _mapPinOverlayViewModel;

        TaskElapsedTimeScheduler _taskScheduler;

        MapElement3D _currentLocationMarker;

        protected async override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (e.NewElement is AviationMap)
                {
                    var formsMap = (AviationMap)e.NewElement;
                    _nativeMap = Control as MapControl;
                    _nativeMap.ActualCameraChanged += NativeMap_ActualCameraChanged;

                    _taskScheduler = new TaskElapsedTimeScheduler(
                        async () =>
                        {
                            if (_mapPageViewModel.Map.VisibleRegion != null)
                            {
                                await _mapPageViewModel.Map.OnVisibleRegionChanged(new VisibleRegionChangedEventArgs { VisibleRegion = _mapPageViewModel.Map.VisibleRegion });

                                Device.BeginInvokeOnMainThread(() => UpdatePins(_mapPageViewModel.Map));
                            }
                        }
                        , Constants.ELAPSED_TIME_LOAD_LOCATIONS_MILLISECONDS
                        , TaskElapsedTimeSchedulerBehavior.TriggerOnce);

                    if (_mapPageViewModel == null)
                    {
                        _mapPageViewModel = (e.NewElement as AviationMap).BindingContext as MapPageViewModel;
                        _mapPageViewModel.Map.CurrentLocationChanged += Map_CurrentLocationChanged;

                        var snPosition = new BasicGeoposition { Latitude = _mapPageViewModel.CurrentLocationPin.Position.Latitude, Longitude = _mapPageViewModel.CurrentLocationPin.Position.Longitude };
                        var snPoint = new Geopoint(snPosition);

                        _currentLocationMarker = new MapElement3D();
                        _currentLocationMarker.Model = await MapModel3D.CreateFrom3MFAsync(RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/{_mapPageViewModel.CurrentLocationPin.Model3DPath}")));
                        _currentLocationMarker.Location = snPoint;
                        _currentLocationMarker.Scale = new System.Numerics.Vector3 { X = 0.75F, Y = 0.75F, Z = 0.75F };
                        _currentLocationMarker.ZIndex = int.MaxValue;

                        _nativeMap.MapElements.Add(_currentLocationMarker);
                    }

                    if (_mapPinOverlayViewModel == null)
                    {
                        _mapPinOverlayViewModel = new MapPinOverlayViewModel();
                    }
                }

                _nativeMap.Children.Clear();
                _nativeMap.MapElementClick += OnMapElementClick;
            }
        }

        private async Task Map_CurrentLocationChanged(object sender, CurrentLocationChangedEventArgs e)
        {
            if (_currentLocationMarker != null)
            {
                var snPosition = new BasicGeoposition { Latitude = e.NewLocation.Latitude, Longitude = e.NewLocation.Longitude };
                var snPoint = new Geopoint(snPosition);

                _currentLocationMarker.Location = snPoint;
                double heading = (e.NewLocation.Course ?? 0) + _mapPageViewModel.CurrentLocationPin.Model3DRotationOffset;
                if (heading >= 360)
                {
                    heading -= 360;
                }
                _currentLocationMarker.Heading = heading;
            }
        }

        private void NativeMap_ActualCameraChanged(MapControl sender, MapActualCameraChangedEventArgs args)
        {
            _taskScheduler.Restart();
        }

        protected void UpdatePins(AviationMap map)
        {
            lock (_mapPageViewModel.MapItems)
            {
                foreach (var mapItem in _mapPageViewModel.MapItems)
                {
                    if (!(_nativeMap.MapElements.Any(elem => 
                        elem is MapIcon
                     && (elem as MapIcon).Location != null
                     && (elem as MapIcon).Location.Position.Latitude == mapItem.Position.Latitude 
                     && (elem as MapIcon).Location.Position.Longitude == mapItem.Position.Longitude)))
                    {
                        var snPosition = new BasicGeoposition { Latitude = mapItem.Position.Latitude, Longitude = mapItem.Position.Longitude };
                        var snPoint = new Geopoint(snPosition);

                        var mapIcon = new MapIcon();
                        mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/{mapItem.MapPin.Image}"));
                        mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                        mapIcon.Location = snPoint;
                        mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);

                        _nativeMap.MapElements.Add(mapIcon);
                    }
                }
            }
        }

        private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            _nativeMap.Children.Clear();

            if (mapIcon != null)
            {
                _nativeMap.Children.Remove(_activeOverlay);
                var mapItem = GetMapItem(mapIcon.Location.Position);
                var snPosition = new BasicGeoposition { Latitude = mapItem.Position.Latitude, Longitude = mapItem.Position.Longitude };
                var snPoint = new Geopoint(snPosition);

                if (mapItem == null)
                {
                    throw new Exception("Custom pin not found");
                }
                else if (mapItem == _mapPinOverlayViewModel.SelectedMapItem)
                {
                    _mapPinOverlayViewModel.SelectedMapItem = null;
                    _activeOverlay = null;
                }
                else if (mapItem is Airport)
                {
                    _mapPinOverlayViewModel.SelectedMapItem = mapItem;
                    _activeOverlay = new AirportPinOverlay(_mapPinOverlayViewModel, _mapPageViewModel);
                    (_activeOverlay as AirportPinOverlay).ViewModel.SelectedMapItem = mapItem as Airport;
                }
                else if (mapItem is Navaid)
                {
                    _mapPinOverlayViewModel.SelectedMapItem = mapItem;
                    _activeOverlay = new NavaidPinOverlay(_mapPinOverlayViewModel, _mapPageViewModel);
                    (_activeOverlay as NavaidPinOverlay).ViewModel.SelectedMapItem = mapItem as Navaid;
                }

                if (_activeOverlay != null)
                {
                    _nativeMap.Children.Add(_activeOverlay);
                    MapControl.SetLocation(_activeOverlay, snPoint);
                    MapControl.SetNormalizedAnchorPoint(_activeOverlay, new Windows.Foundation.Point(1, 0.75));
                }
            }
        }

        MapItemBase GetMapItem(BasicGeoposition position)
        {
            var pos = new Position(position.Latitude, position.Longitude);
            foreach (var item in _mapPageViewModel.MapItems)
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
