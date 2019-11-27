using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public struct GPSLocation
{
    public double longitude;
    public double latitude;

    public GPSLocation(double pLongitude, double pLatitude)
    {
        longitude = pLongitude;
        latitude = pLatitude;
    }

    public GPSLocation(Location location)
    {
        longitude = location.longitude;
        latitude = location.latitude;
    }

    public override string ToString() // In this case the longitude and the latitude have to be swapped for the API to work correctly
    {
        return String.Format("{0},{1}", longitude.ToString(CultureInfo.InvariantCulture), latitude.ToString(CultureInfo.InvariantCulture));
    }
}