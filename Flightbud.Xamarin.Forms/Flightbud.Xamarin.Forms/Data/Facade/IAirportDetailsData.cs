using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    /// <summary>
    /// Interface used as signature for retrieving details for a particular airport.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAirportDetailsData<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="airportId">The unique identifier number of the airport (id field).</param>
        /// <param name="ct">Cancellation Token for handling data retrieval cancellation scenarios.</param>
        /// <returns></returns>
        Task<List<T>> Get(double airportId, CancellationToken ct);
    }
}
