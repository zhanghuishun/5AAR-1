using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using UnityEngine.UI;
using System.Collections.Specialized;
using UnityEngine.Networking;
using System.Xml;
using System.IO;
using System.Globalization;
using Syrus.Plugins.DFV2Client;
using System;

public class GoogleMapAPIQuery : MonoBehaviour
{
    GPSLocation GPSInstance;
    Utils utils;
    //import api key from .config file
    private string APIKey;
    private string keyword = "tabacchi";
    [HideInInspector]
    public Text resultValue;
    private string radius = "500";
    [HideInInspector]
    public List<step> walkingSteps;
    [HideInInspector]
    public List<step> walkingToTabacchiSteps;
    [HideInInspector]
    public transitDetails busInformation;
    [HideInInspector]
    public loc tabacchiLoc = new loc();
    private List<result> tabacchiResults;
    private int tabacchiIndex = 0;
    //public TextAsset xmlRawFile;
    public Container<string> busName = new Container<string>();
    public DateTime datevalue2;
    public Container<string> departureTime = new Container<string>();
    private Container<int> _minutes = new Container<int>();
    public Container<int> minutes { get { _minutes.content = Mathf.RoundToInt((float)(datevalue2 - DateTime.Now).TotalMinutes); return _minutes; } }
    
    void Awake(){

    }
    void  Start(){
        GPSInstance = GPSLocation.Instance;
        utils = Utils.Instance;
        APIKey = GlobalConfig.GoogleMapAPIKey;
        tabacchiResults = new List<result>();
        /*XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlRawFile.text));

        //set google map api key
        string xmlPathPattern = "//keys/GoogleMapAPIKey";
        XmlNodeList nodelist = xmlDoc.SelectNodes(xmlPathPattern);
        XmlNode node = nodelist[0];
        APIKey = node.InnerXml;
        Debug.Log(APIKey);*/
    }
    
    public IEnumerator TabacchiInOrder(bool isFirstTabacchi) {
        //if it is the first time, if user set their shop, follow the setting, or use the first result
        if(isFirstTabacchi){
            //wait for GPS location
            yield return new WaitForSecondsRealtime(4);
            //store tabacchi results into tabacchiResults
            yield return StartCoroutine(GetTabacchiJSON());
            tabacchiIndex = 0;
            if(InputFieldSubmit.tabacchiCoordinates[0] != "") {
                tabacchiLoc.lat = float.Parse(InputFieldSubmit.tabacchiCoordinates[0]);
                tabacchiLoc.lng = float.Parse(InputFieldSubmit.tabacchiCoordinates[1]);
            }
            else{
                tabacchiLoc.lat = tabacchiResults[tabacchiIndex].geometry.location.lat;//index = 0
                tabacchiLoc.lng = tabacchiResults[tabacchiIndex].geometry.location.lng;
                tabacchiIndex++;
            }
        }
        //if it is not the first time, use tabacchi index to get result
        else {
            try{
                tabacchiLoc.lat = tabacchiResults[tabacchiIndex].geometry.location.lat;
                tabacchiLoc.lng = tabacchiResults[tabacchiIndex].geometry.location.lng;
                tabacchiIndex++;
                }
            catch(System.Exception e){
                DF2Context[] newContext = new DF2Context[1];
                newContext[0] = new DF2Context("TabacchiReached-Closed-followup", 2, new Dictionary<string, object>());
                ConversationController.Instance.ResetContext(newContext);
                ConversationController.Instance.SendTextIntent("No");
            }
        }    
        yield return StartCoroutine(GetWalkRouteJSON (tabacchiLoc.lat, tabacchiLoc.lng));
        }
        
    public void RouteToBusStationQuery()
    {
        //get value from settings scene
        float destLat = float.Parse(InputFieldSubmit.destinationCoordinates[0]);
        float destLng = float.Parse(InputFieldSubmit.destinationCoordinates[1]);
        StartCoroutine (GetBusRouteJSON (destLat, destLng));//45.5168268f, 9.2166683f
    }

    IEnumerator GetTabacchiJSON() {
		string baseURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?";
		string keyword = "keyword="+this.keyword;
        string location = "location="+GPSInstance.lat.ToString("G", CultureInfo.InvariantCulture)+"%2C"+GPSInstance.lng.ToString("G", CultureInfo.InvariantCulture);
        string radius = "radius="+this.radius;
		string apiKey = "key="+APIKey;
		string api = baseURL + keyword + "&" + location + "&" + radius + "&" + apiKey;
		Debug.Log(api);
        UnityWebRequest www = UnityWebRequest.Get(api);
        yield return www.Send();
        //TODO handle case where no tabacchi is found nearby
        if (www.isNetworkError) {
			Debug.Log(www.error);
		}
		else {
			// Show results as text
			string result = www.downloadHandler.text;
			Debug.Log (result);
            //parse the result
            attribution attr = JsonUtility.FromJson<attribution>(result);
            //Debug.Log(JsonUtility.ToJson(attr, true));
            int maxWait = 3;
            while(attr.results.Count == 0 && maxWait > 0){
                yield return new WaitForSeconds(1);
                maxWait--;
            }
            if(maxWait <= 0) yield return 0;
            tabacchiResults = attr.results;
		}

	}

    public IEnumerator GetBusRouteJSON(float destLat, float destLng) {
        //wait for GPS location
        yield return new WaitForSecondsRealtime(4);
        string baseURL = "https://maps.googleapis.com/maps/api/directions/json?";
        //string origin = "origin=" + "45.5219%2C9.2216939";
        string origin = "origin=" + GPSInstance.lat.ToString("G", CultureInfo.InvariantCulture) + "%2C" + GPSInstance.lng.ToString("G", CultureInfo.InvariantCulture);
#if (UNITY_EDITOR)
        origin = "origin=45.4806988%2C9.2245384";
#endif
        //TODO: set the dest
        string dest = "destination=" + destLat.ToString("G", CultureInfo.InvariantCulture) + "%2C" + destLng.ToString("G", CultureInfo.InvariantCulture);
		string mode = "mode=transit";
        string transit_mode = "transit_mode=bus";
        string transit_routing_preference="transit_routing_preference=fewer_transfers";
		string apiKey = "key="+APIKey;
		string api = baseURL + origin + "&" + dest + "&" + mode + "&" + apiKey + "&" + transit_mode + "&" + transit_routing_preference;
        Debug.Log(api);
		UnityWebRequest www = UnityWebRequest.Get(api);
		yield return www.Send();
		if(www.isNetworkError) {
			Debug.Log(www.error);
		}
		else {
			string result = www.downloadHandler.text;
			geoCoded g = JsonUtility.FromJson<geoCoded>(result);
			leg l = g.routes [0].legs [0];
            //steps include walking part and bus part
            foreach (step step in l.steps)
            {
                if(step.travel_mode == "WALKING"){
                    //convert innnerstep type to step
                    walkingSteps = utils.Convert(step.steps);
                }
                else if(step.travel_mode == "TRANSIT"){
                    busInformation = step.transit_details;
                    break;
                }
            }
            //send parameters to CA
            datevalue2 = utils.TimeParse(busInformation.departure_time.text);
            busName.content = busInformation.line.short_name;
            departureTime.content = busInformation.departure_time.text;
        }
    }
    IEnumerator GetWalkRouteJSON(float destLat, float destLng) {
        string baseURL = "https://maps.googleapis.com/maps/api/directions/json?";
        //string origin = "origin=" + "45.5219%2C9.2216939";
        string origin = "origin=" + GPSInstance.lat.ToString("G", CultureInfo.InvariantCulture) + "%2C" + GPSInstance.lng.ToString("G", CultureInfo.InvariantCulture);
#if (UNITY_EDITOR)
        origin = "origin=45.480198%2C9.2262149";
#endif
        //TODO: set the dest
        string dest = "destination=" + destLat +"%2C" + destLng;
		string mode = "mode=walking";
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
            Debug.Log(result);
            geoCoded g = JsonUtility.FromJson<geoCoded>(result);
			leg l = g.routes [0].legs [0];
            //steps include walking part and bus part
			walkingSteps = l.steps;
        }
    }
}