using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NavigationTest : MonoBehaviour
{
    [SerializeField] LineRenderer _lineRenderer;

    const string apiKey = "5b3ce3597851110001cf6248296d7129fb1343a4bf2e43eca3d631a1";
    const string baseURI = "https://api.openrouteservice.org/v2/directions/";

    // Start is called before the first frame update
    [ContextMenu("Request")]
    void GetRequest()
    {

        StartCoroutine(GetNavigation(
            new GPSLocation(6.617551, 52.041095),
            new GPSLocation(6.617584, 52.042336)
            ));

    }
    [System.Serializable]
    public struct TestCoordinates
    {
        public float[] coordinates;
    }


    [System.Serializable]
    public struct GPSLocation
    {
        public double lattitude;
        public double longitude;


        public GPSLocation(double lattitude, double longitude) : this()
        {
            this.lattitude = lattitude;
            this.longitude = longitude;
        }

    }

    // Update is called once per frame
    IEnumerator GetNavigation(GPSLocation pStart, GPSLocation pEnd)
    {
        string request = String.Format("driving-car?api_key={0}&start={1}&end={2}", apiKey, LocationToString(pStart), LocationToString(pEnd));
        using (UnityWebRequest webRequest = UnityWebRequest.Get(baseURI + request))
        {
            //webRequest.SetRequestHeader("Authorization", apiKey);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                AddLineRenderer(JsonConvert.DeserializeObject<NavigationResponse>(webRequest.downloadHandler.text));
            }
        }
    }

    public void AddLineRenderer(NavigationResponse pNavigationResponse)
    {
        Feature road = pNavigationResponse.features.FirstOrDefault();
        Vector3[] locations = new Vector3[road.geometry.coordinates.Count];
        for (int i = 0; i < road.geometry.coordinates.Count; i++)
            locations[i] = new Vector3((float)road.geometry.coordinates[i][0] * 180, 0, (float)road.geometry.coordinates[i][1] * 360);

        _lineRenderer.positionCount = locations.Length;
        _lineRenderer.SetPositions(locations);
    }

    string LocationToString(GPSLocation location)
    {
        return String.Format("{0},{1}", location.lattitude.ToString(CultureInfo.InvariantCulture), location.longitude.ToString(CultureInfo.InvariantCulture));
    }


}
