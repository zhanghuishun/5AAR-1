using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ArrowPointer : MonoBehaviour
{
    public GameObject panelPrefab;
    public Transform directionsPanel;
    public GPSLocation GPSInstance;
    float lat;
    float lon;
	float destLat;
	float destLon;
	int count;
	List<step> steps;

    float brng;
    float compassBrng;
    GameObject[] panels = new GameObject[1];
    Text[] texts;

    private GoogleMapAPIQuery googleapiScript;

    void Awake(){
        googleapiScript = GetComponent<GoogleMapAPIQuery>();
        Debug.Log("-------------------------------------arrow awake");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("-------------------------------------arrow start");
        GPSInstance = GPSLocation.Instance;
        panels[0] = Instantiate(panelPrefab) as GameObject;
    }

    private bool flag = true;
    public void ClickToGetStepsInformation(){
        try{
        steps = googleapiScript.steps;
        if(steps == null) return;
        Debug.Log(steps.Count);
        Debug.Log("-------------------------------------");
        panels[0] = Instantiate(panelPrefab) as GameObject;
        texts = panels[0].GetComponentsInChildren<Text>();
        texts[0].text = "Distance here";
        //texts[1].text = steps[count].maneuver; // description
        //texts[2].text = "Step " + (count+1) + " / " + steps.Count;
        //texts[3].text = steps[count].end_location.lat + ", " + steps[count].end_location.lng;
        //open compass
        Input.compass.enabled = true;
        }
        catch(Exception e){
            Debug.LogException(e, this);
            Debug.Log("function Exception");
        }

        InvokeRepeating("StepsLoop", 1.0f, 0.5f);
    }

    public void StepsLoop()
    {
        Debug.Log("stepsloop");
        
        lat = GPSInstance.lat;
        lon = GPSInstance.lon;
        if(steps != null){
            destLat = steps[count].end_location.lat;
            destLon = steps[count].end_location.lng;
            Debug.Log("----------tempdest"+destLat+"   "+destLon+"--------------");
            float λ = destLon - lon;

            var y = Mathf.Sin(λ) * Mathf.Cos(destLat);
            var x = Mathf.Cos(lat) * Mathf.Sin(destLat) -
                Mathf.Sin(lat) * Mathf.Cos(destLat) * Mathf.Cos(λ);
            brng = Mathf.Rad2Deg * Mathf.Atan2(y, x);
            if (brng < 0)
            {
                brng = 360 + brng;
            }
            compassBrng = Input.compass.trueHeading;

            int distance = Mathf.RoundToInt(distance_metres(lat, lon, destLat, destLon));
            Debug.Log("-----------------"+distance+"-----------------");
            // constantly update distance shown
            texts[0].text = distance.ToString() + "m";

            }
        

        if (isCollide())
                    {
                        Destroy(panels[0]);
                        //panels count = panels[count+1]?
                        count++;
                        if (count < steps.Count)
                        {
                            panels[0] = Instantiate(panelPrefab);//,directionsPanel
                            texts = panels[0].GetComponentsInChildren<Text>();
                            //texts[1].text = steps[count].maneuver; // description
                            //texts[2].text = "Step " + (count+1) + " / " + steps.Count;
                            //texts[3].text = steps[count].end_location.lat + ", " + steps[count].end_location.lng;
                        }
                    }
					if (count == steps.Count){}
				
	}
    

    bool isCollide() {
		lat = Input.location.lastData.latitude;
		lon = Input.location.lastData.longitude;
		if (lat - destLat <= 0.0005f && lat - destLat >= -0.0005f) {
			if (lon - destLon <= 0.0005f && lon - destLon >= -0.0005f) {
				return true;
			}
		}

		return false;
	}

    float distance_metres (float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6378.137f; // Radius of Earth in KM
        float dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        float dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) + Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) * Mathf.Sin(dLon / 2)
            * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float d = R * c;
        return d * 1000f;
    }

	// Update is called once per frame
	void Update () {
    }
}