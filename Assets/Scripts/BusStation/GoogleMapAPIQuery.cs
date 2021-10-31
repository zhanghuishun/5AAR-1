using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using UnityEngine.UI;
using System.Collections.Specialized;
using UnityEngine.Networking;

public class GoogleMapAPIQuery : MonoBehaviour
{
    //import api key from .config file
    private string APIKey = GlobalConfig.GoogleMapAPIKey;
    private string keyword = "busstop";
    //set by dragging object
    public Text latitudeValue;
    public Text longitudeValue;
    public Text resultValue;
    private string radius = "150";
    // Start is called before the first frame update
    public void APIQuery()
    {
        StartCoroutine (GetJSON ());
    }

    IEnumerator GetJSON() {
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
            //parse the result using JSONParser in Utils
            string parsedResult = JSONParser.MapAPIQueryParser(result);
            resultValue.text = parsedResult;

			// geoCoded g = JsonUtility.FromJson<geoCoded>(result);
			// leg l = g.routes [0].legs [0];
			// Debug.Log (l.end_location.lat);
			// Debug.Log (l.end_location.lng);
			// // countText.text = Jsonwww.downloadHandler.text;
			// List<step> steps = l.steps;
		}

	}


}
