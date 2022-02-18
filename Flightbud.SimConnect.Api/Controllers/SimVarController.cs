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

        public SimVarController(ILogger<SimVarController> logger)
        {
            _logger = logger;
            SimConnectHelper.SimConnectHelper.SimData += SimConnect_DataReceived;
        }

        SimConnectVariableValue result;

        [HttpGet(Name = "GetGpsPosition")]
        public Location GetGpsPosition()
        {
            Location location = default;

            lock (_requestLock)
            {
                if (!SimConnectHelper.SimConnectHelper.IsConnected)
                {
                    try
                    {
                        SimConnectHelper.SimConnectHelper.Connect();

                        // get altitude
                        UpdateSimVarResult("GPS POSITION ALT");
                        location.Altitude = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

                        // get latitude
                        UpdateSimVarResult("GPS POSITION LAT");
                        location.Latitude = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

                        // get longitude
                        UpdateSimVarResult("GPS POSITION LON");
                        location.Longitude = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

                        UpdateSimVarResult("GPS GROUND TRUE TRACK");
                        location.Course = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

                        UpdateSimVarResult("GPS GROUND SPEED");
                        location.Speed = Convert.ToDouble((result.Value as object[]).FirstOrDefault());

                    }
                    catch (InvalidOperationException iox)
                    {
                        _logger.LogError(iox, iox.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    finally
                    {
                        SimConnectHelper.SimConnectHelper.Disconnect();
                    }
                }
            }

            return location;
        }

        private void UpdateSimVarResult(string simVarFieldName)
        {
            result = null;

            List<SimConnectVariable> failures = new List<SimConnectVariable>();

            var simVarAltitude = simVarVariables.FirstOrDefault(v => v.Key == simVarFieldName);
            SimConnectVariable request = new SimConnectVariable { Name = simVarAltitude.Value.Name, Unit = simVarAltitude.Value.DefaultUnit };
            int requestId = SimConnectHelper.SimConnectHelper.GetSimVar(request, (int)SimConnectUpdateFrequency.Once);

            if (requestId == -1)
            {
                throw new InvalidOperationException($"cannot get SimVar field {simVarFieldName}");
            }

            const int resultDelayCheckMilliseconds = 10;
            const int maxWaitForResultMilliseconds = Constants.LOCATION_TIMEOUT;

            SimConnectHelper.SimConnectHelper.GetSimVar(requestId);
            var endWaitTime = DateTime.Now.AddMilliseconds(maxWaitForResultMilliseconds);

            while (result == null && endWaitTime > DateTime.Now)
            {
                Thread.Sleep(resultDelayCheckMilliseconds); // Wait to receive the value
            }
            if (result == null)
            {
                failures.Add(request);
            }
        }

        private void SimConnect_DataReceived(object sender, SimConnectVariableValue e)
        {
            result = e;
        }
    }
}