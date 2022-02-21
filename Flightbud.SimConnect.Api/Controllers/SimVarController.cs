using Flightbud.SimConnect.Api.Data;
using Flightbud.SimConnect.Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FlightSimulator.SimConnect;
using SimConnectHelper;
using SimConnectHelper.Common;
using System.Runtime.InteropServices;

namespace Flightbud.SimConnect.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimVarController : ControllerBase
    {
        static object _requestLock = new object();

        private readonly ILogger<SimVarController> _logger;
        private readonly Dictionary<string, SimVarDefinition> simVarVariables = SimVarUnits.DefaultUnits;
        static Location location;

        public SimVarController(ILogger<SimVarController> logger)
        {
            _logger = logger;
            SimConnectHelper.SimConnectHelper.SimData += SimConnect_DataReceived;
        }

        //[HttpGet(Name = "GetGpsPosition")]
        //public Location GetGpsPosition()
        //{
        //    Location location = default;

        //    lock (_requestLock)
        //    {
        //        if (!SimConnectHelper.SimConnectHelper.IsConnected)
        //        {
        //            try
        //            {
        //                SimConnectHelper.SimConnectHelper.Connect();

        //                // get altitude
        //                UpdateSimVarResult("GPS POSITION ALT");
        //                location.Altitude = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

        //                // get latitude
        //                UpdateSimVarResult("GPS POSITION LAT");
        //                location.Latitude = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

        //                // get longitude
        //                UpdateSimVarResult("GPS POSITION LON");
        //                location.Longitude = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

        //                UpdateSimVarResult("GPS GROUND TRUE HEADING");
        //                location.Course = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

        //                UpdateSimVarResult("GPS GROUND SPEED");
        //                location.Speed = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

        //            }
        //            catch (InvalidOperationException iox)
        //            {
        //                _logger.LogError(iox, iox.Message);
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, ex.Message);
        //            }
        //            finally
        //            {
        //                SimConnectHelper.SimConnectHelper.Disconnect();
        //            }
        //        }
        //    }

        //    Thread.Sleep(500);

        //    return location;
        //}

        [HttpGet(Name = "GetGpsPosition")]
        public Location GetGpsPosition()
        {
            lock (_requestLock)
            {
                try
                {
                    if (!SimConnectHelper.SimConnectHelper.IsConnected)
                    {
                        location = default;

                        SimConnectHelper.SimConnectHelper.Connect();

                        InvokeUpdateSimVar("GPS POSITION ALT");
                        InvokeUpdateSimVar("GPS POSITION LAT");
                        InvokeUpdateSimVar("GPS POSITION LON");
                        InvokeUpdateSimVar("GPS GROUND TRUE HEADING");
                        InvokeUpdateSimVar("GPS GROUND SPEED");

                        const int resultDelayCheckMilliseconds = 100;
                        const int maxWaitForResultMilliseconds = Constants.LOCATION_TIMEOUT;

                        var endWaitTime = DateTime.Now.AddMilliseconds(maxWaitForResultMilliseconds);
                        while (location.Latitude == default && location.Longitude == default && endWaitTime > DateTime.Now)
                        {
                            Thread.Sleep(resultDelayCheckMilliseconds); // Wait to receive the value
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    SimConnectHelper.SimConnectHelper.Disconnect();
                }
            }
            return location;
        }

        private void InvokeUpdateSimVar(string simVarFieldName)
        {
            List<SimConnectVariable> failures = new List<SimConnectVariable>();

            var simVarAltitude = simVarVariables.FirstOrDefault(v => v.Key == simVarFieldName);
            SimConnectVariable request = new SimConnectVariable { Name = simVarAltitude.Value.Name, Unit = simVarAltitude.Value.DefaultUnit };
            int requestId = SimConnectHelper.SimConnectHelper.GetSimVar(request, (int)SimConnectUpdateFrequency.SIM_Frame);

            if (requestId == -1)
            {
                throw new InvalidOperationException($"cannot get SimVar field {simVarFieldName}");
            }

            SimConnectHelper.SimConnectHelper.GetSimVar(requestId);
        }

        private void SimConnect_DataReceived(object sender, SimConnectVariableValue e)
        {
            switch (e.Request.Name)
            {
                case "GPS POSITION ALT":
                    location.Altitude = Convert.ToDouble((e.Value as object[]).FirstOrDefault());
                    break;
                case "GPS POSITION LAT":
                    location.Latitude = Convert.ToDouble((e.Value as object[]).FirstOrDefault());
                    break;
                case "GPS POSITION LON":
                    location.Longitude = Convert.ToDouble((e.Value as object[]).FirstOrDefault());
                    break;
                case "GPS GROUND TRUE HEADING":
                    location.Course = Convert.ToDouble((e.Value as object[]).FirstOrDefault());
                    break;
                case "GPS GROUND SPEED":
                    location.Speed = Convert.ToDouble((e.Value as object[]).FirstOrDefault());
                    break;
            }
        }
    }
}