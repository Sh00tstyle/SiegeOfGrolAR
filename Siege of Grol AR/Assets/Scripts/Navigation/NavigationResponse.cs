using System;
using System.Collections.Generic;

[System.Serializable]
public class Step
{
    public double distance;
    public double duration;
    public int type;
    public string instruction;
    public string name;
    public List<int> way_points;
}

[System.Serializable]
public class Segment
{
    public double distance;
    public double duration;
    public List<Step> steps;
}

[System.Serializable]
public class Summary
{
    public double distance;
    public double duration;
}

[System.Serializable]
public class Properties
{
    public List<Segment> segments;
    public Summary summary;
    public List<int> way_points;
}

[System.Serializable]
public class Geometry
{
    public List<Test.GPSLocation> coordinates;
    public string type;
}

[System.Serializable]
public class Feature
{
    public List<double> bbox;
    public string type;
    public Properties properties;
    public Geometry geometry;
}

[System.Serializable]
public class Query
{
    public List<List<double>> coordinates;
    public string profile;
    public string format;
}

[System.Serializable]
public class Engine
{
    public string version;
    public DateTime build_date;
}

[System.Serializable]
public class Metadata
{
    public string attribution;
    public string service;
    public long timestamp;
    public Query query;
    public Engine engine;
}

[System.Serializable]
public class NavigationResponse
{
    public string type;
    public List<Feature> features;
    public List<double> bbox;
    public Metadata metadata;
}