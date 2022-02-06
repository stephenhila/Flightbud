using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        /// Get data based on the geolocation and kilometer radius.
        /// </summary>
        /// <param name="mapPosition">The position of the region's center, containing geolocation points.</param>
        /// <param name="radius">The radius of the region to search, in kilometers.</param>
        /// <param name="ct">Cancellation Token for handling data retrieval cancellation scenarios.</param>
        /// <returns></returns>
        Task<List<T>> Get(Position center, double radius, CancellationToken ct);
    }
}
