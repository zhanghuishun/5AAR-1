using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using UnityEngine.UI;
using System.Collections.Specialized;
using UnityEngine.Networking;

public class GoogleMapAPIQuery : MonoBehaviour
{
    GPSLocation GPSLocation;
    //import api key from .config file
    private string APIKey;
    private string keyword = "busstop";
    //set by dragging object
    public Text latitudeValue;
    public Text longitudeValue;
    public Text resultValue;
    private string radius = "150";

    void  Start(){
        GPSLocation = GetComponent<GPSLocation>();
        APIKey = GlobalConfig.GoogleMapAPIKey;
        latitudeValue = GPSLocation.latitudeValue;
        longitudeValue = GPSLocation.longitudeValue;
    }

    public void RouteQuery()
    {
        StartCoroutine (GetBusStationJSON ());
        StartCoroutine (GetRouteJSON ());
    }

    IEnumerator GetBusStationJSON() {
		string baseURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?";
		string keyword = "keyword="+this.keyword;
        string location = "location="+latitudeValue.text+"%2C"+longitudeValue.text;
        string radius = "radius="+this.radius;
		string apiKey = "key="+APIKey;
		string api = baseURL + keyword + "&" + location + "&" + radius + "&" + apiKey;
		Debug.Log(api);
        UnityWebRequest www = UnityWebRequest.Get(api);
		yield return www.Send();
		if(www.isNetworkError) {
			Debug.Log(www.error);
		}
		else {
			// Show results as text
			string result = www.downloadHandler.text;
			Debug.Log (result);
            //parse the result
            attribution attr = JsonUtility.FromJson<attribution>(result);
            //resultValue.text = attr.stations[0].name;
		}

	}

    IEnumerator GetRouteJSON() {
        string baseURL = "https://maps.googleapis.com/maps/api/directions/json?";
        //string origin = "origin=" + "45.5219%2C9.2216939";
		string origin = "origin="+ latitudeValue.text+"%2C"+longitudeValue.text;
        //TODO: set the dest
        string dest = "destination=" + "45.522714%2C9.2216527";
		string mode = "mode="+"walking";
		string apiKey = "key="+APIKey;
		string api = baseURL + origin + "&" + dest + "&" + mode + "&" + apiKey;
        Debug.Log(api);
		UnityWebRequest www = UnityWebRequest.Get(api);
		yield return www.Send();
		if(www.isNetworkError) {
			Debug.Log(www.error);
		}
		else {
			string result = www.downloadHandler.text;
			Debug.Log (result);
			geoCoded g = JsonUtility.FromJson<geoCoded>(result);
			leg l = g.routes [0].legs [0];
			Debug.Log (l.end_location.lat);
			Debug.Log (l.end_location.lng);
			// countText.text = Jsonwww.downloadHandler.text;
			var steps = new List<step>(l.steps);
            
			Debug.Log (steps[0].end_location.lng);
			Debug.Log (steps[0].end_location.lat);
            resultValue.text = steps[0].end_location.lng + "  "+steps[0].end_location.lat;
        }
    }
}