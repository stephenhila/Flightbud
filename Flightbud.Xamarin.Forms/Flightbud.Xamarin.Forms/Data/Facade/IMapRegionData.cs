using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    /// <summary>
    /// Interface used as signature for retrieving map items based off of map region data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMapRegionData<T> where T : MapItemBase
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
