using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    private const double EarthRadius = 6378137.0;

    public static float ClampAngle(float pAngle, float pMin, float pMax)
    {
        if (pAngle < -360F)
            pAngle += 360F;

        if (pAngle > 360F)
            pAngle -= 360F;

        return Mathf.Clamp(pAngle, pMin, pMax);
    }

    public static MapPoint GPS2MapPoint(double pLatitude, double pLongitude)
    {
        // Based on: http://jackofalltradesdeveloper.blogspot.com/2012/03/how-to-project-point-from-geography-to.html
        // Projects a point from a web mercator (WG84 elipsoid) on a plane in a cartesian coordinate system
        double radiansPerDegree = Math.PI / 180.0;
        double rad = pLongitude * radiansPerDegree;
        double fSin = Math.Sin(rad);

        double x = pLatitude * radiansPerDegree * EarthRadius;
        double y = EarthRadius / 2.0 * Math.Log((1.0 + fSin) / (1.0 - fSin));

        return new MapPoint(x, y);
    }

    public static GPSLocation MapPoint2GPS(double pX, double pY)
    {
        // Based on: https://en.wikipedia.org/wiki/Mercator_projection
        // Projects a point for a XY plane on a web mercator (WG84 elipsoid)
        double degreesPerRadians = 180.0 / Math.PI;

        double latitude = pX / EarthRadius;
        latitude *= degreesPerRadians;
        
        double longitude = 2.0 * Math.Atan(Math.Exp(pY / EarthRadius)) - Math.PI / 2.0;
        longitude *= degreesPerRadians;

        return new GPSLocation(latitude, longitude);
    }

}
