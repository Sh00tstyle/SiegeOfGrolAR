using System;
using System.Collections.Generic;


[Serializable]
public class Geometry
{
    public List<List<double>> coordinates;
    public string type;
}

[Serializable]
public class Feature
{
    public List<double> bbox;
    public string type;
    public Geometry geometry;
}

[Serializable]
public class NavigationResponse
{
    public string type;
    public List<Feature> features;
    public List<double> bbox;
}