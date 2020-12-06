using System;
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using static EarthData;

public static class EarthData
{
    public const double CIRCUMFERENCE = 40_075_000.0;
    public const double RADIUS = 6_371_000.0;
    public const double METERS_PER_DEGREE_LATITUDE = CIRCUMFERENCE / 360.0;
    public const double DEGREES_LATITUDE_PER_METER = 360.0 / CIRCUMFERENCE;
           
    public const float BASE_ALTITUDE = 10.0f;
    public const double BASE_LATITUDE = 38.408125;
    public const double BASE_LONGITUDE = -110.7796288;

    public static readonly double CIRCUMFERENCE_AT_BASE_LATITUDE = 2.0 * Math.PI * Math.Cos(BASE_LATITUDE) * RADIUS;
    public static readonly double METERS_PER_DEGREE_LONGITUDE = CIRCUMFERENCE_AT_BASE_LATITUDE / 360.0;
    public static readonly double DEGREES_LONGITUDE_PER_METER = 360.0 / CIRCUMFERENCE_AT_BASE_LATITUDE;
}

public class GPS : MonoBehaviour
{
    public const float GPS_DELTA_TIME = 0.01f;

    public double Latitude = BASE_LATITUDE;
    public double Longitude = BASE_LONGITUDE;
    public double Altitude = BASE_ALTITUDE;

    public bool OverrideGPSData;

    private void Start()
    {
        RosConnection.RosSocket.Advertise<NavSatFix>("/fix");
        UpdateGPSData();
        InvokeRepeating("PublishGPSData", 1.0f, GPS_DELTA_TIME);
    }

    private void UpdateGPSData()
    {
        if (OverrideGPSData) return;
        Vector3 pos = transform.position;

        Altitude = pos.y + BASE_ALTITUDE;
        Latitude = BASE_LATITUDE + (double)pos.x * DEGREES_LATITUDE_PER_METER;
        Longitude = BASE_LONGITUDE + ((double)-pos.z) * DEGREES_LONGITUDE_PER_METER;
    }

    private void PublishGPSData()
    {
        UpdateGPSData();

        NavSatFix navData = new NavSatFix
        {
            altitude = Altitude, 
            latitude = Latitude, 
            longitude = Longitude
        };
        RosConnection.RosSocket.Publish("/fix", navData);
    }
}