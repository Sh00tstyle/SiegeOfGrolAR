using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationDatabase", menuName = "Siege of Grol/LocationDatabase")]
public class LocationDatabase : ScriptableObject
{
    public Location[] locations;
}

[Serializable]
public class Location
{
    public string locationName;

    public string characterName;

    public double latitude;

    public double longitude;

    public GameObject locationPrefab;
}