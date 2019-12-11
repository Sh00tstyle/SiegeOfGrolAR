using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public struct GPSLocation
{
    public double latitude;
    public double longitude;

    public GPSLocation(double pLatitude, double pLongitude)
    {
        latitude = pLatitude;
        longitude = pLongitude;
    }

    public GPSLocation(Location location)
    {
        latitude = location.latitude;
        longitude = location.longitude;
    }

    public override string ToString() // In this case the longitude and the latitude have to be swapped for the API to work correctly
    {
        return String.Format("{0},{1}", longitude.ToString(CultureInfo.InvariantCulture), latitude.ToString(CultureInfo.InvariantCulture));
    }
}