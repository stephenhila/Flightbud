﻿using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Models;
using Sylvan.Data;
using Sylvan.Data.Csv;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    // TODO: Implement AirportData using Sylvan CSV Reader
    public class AirportDataSylvanDataSource : IMapRegionData<MapItemBase>
    {
        public async Task<List<MapItemBase>> Get(Position center, double radius, CancellationToken ct = default)
        {
            MapSpan region = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(radius));
            List<MapItemBase> airports = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.AIRPORT_DATA_RESOURCE))
            {
                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    var reader = await Sylvan.Data.Csv.CsvDataReader.CreateAsync(streamReader);
                    airports = new List<MapItemBase>();

                    while (await reader.ReadAsync(ct))
                    {
                        string airportTypeValue = reader.GetString(2);
                        double latitudeValue = reader.GetDouble(4);
                        double longitudeValue = reader.GetDouble(5);

                        if (latitudeValue < center.Latitude + (region.LatitudeDegrees / 2)
                             && latitudeValue > center.Latitude - (region.LatitudeDegrees / 2)
                             && longitudeValue < center.Longitude + (region.LongitudeDegrees / 2)
                             && longitudeValue > center.Longitude - (region.LongitudeDegrees / 2)
                             && (airportTypeValue == "small_airport"
                              || airportTypeValue == "medium_airport"
                              || airportTypeValue == "large_airport"))
                        {
                            Airport airport = new Airport
                            {
                                Id = reader.GetDouble(0),
                                Code = reader.GetString(1),
                                Type = airportTypeValue,
                                Name = reader.GetString(3),
                                Latitude = latitudeValue,
                                Longitude = longitudeValue,
                                Country = reader.GetString(8),
                            };
                            airports.Add(airport);
                        }
                    }
                }
            }

            return airports;
        }
    }

    /// <summary>
    /// Facade to manage data from the Airports data file.
    /// </summary>
    public class AirportDataCsvReaderDataSource : IMapRegionData<MapItemBase>
    {
        public async Task<List<MapItemBase>> Get(Position center, double radius, CancellationToken ct = default)
        {
            MapSpan region = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(radius));
            List<MapItemBase> airports = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.AIRPORT_DATA_RESOURCE))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    if (reader != null)
                    {
                        using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "," }))
                        {
                            csvReader.Read();
                            csvReader.ReadHeader();
                            airports = new List<MapItemBase>();
                            while (await csvReader.ReadAsync())
                            {
                                Airport airport = csvReader.GetRecord<Airport>();

                                if (airport.Latitude < center.Latitude + (region.LatitudeDegrees / 2)
                                     && airport.Latitude > center.Latitude - (region.LatitudeDegrees / 2)
                                     && airport.Longitude < center.Longitude + (region.LongitudeDegrees / 2)
                                     && airport.Longitude > center.Longitude - (region.LongitudeDegrees / 2)
                                     && (airport.Type == "small_airport"
                                      || airport.Type == "medium_airport"
                                      || airport.Type == "large_airport"))
                                {
                                    airports.Add(airport);
                                }
                            }
                        }
                    }
                }
            }
            return airports;
        }
    }
}
