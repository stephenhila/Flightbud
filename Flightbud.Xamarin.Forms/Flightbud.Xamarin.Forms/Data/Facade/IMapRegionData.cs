using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public interface IMapRegionData<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapPosition">The position of the region's center, containing geolocation points.</param>
        /// <param name="radius">The radius of the region to search, in kilometers.</param>
        /// <returns></returns>
        List<T> Get(Position center, double radius);
    }
}
