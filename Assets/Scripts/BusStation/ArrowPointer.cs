using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ArrowPointer : MonoBehaviour
{
    [SerializeField] private GameObject PanelPrefab;
    private GPSLocation GPSInstance;
    private GoogleMapAPIQuery GoogleAPIScript;

    [SerializeField] private GameObject CompassObject;
    [SerializeField] private UnityARCompass.ARCompassIOS ARCompassIOS;

    
    float lat;
    float lon;
	float destLat;
	float destLon;
	int count;
	List<step> steps;
    float brng;
    float compassBrng;
    GameObject panel;
    Text[] texts;

    
    //compass related 

    void Awake(){
        GoogleAPIScript = GetComponent<GoogleMapAPIQuery>();
        //DistanceCalculatorInstance = DistanceCalculator.Instance;
        //Debug.Log("Distancecalculator-----------"+DistanceCalculatorInstance.tempWords);
        GPSInstance = GPSLocation.Instance;
        Debug.Log("GPSInstance-----------"+GPSInstance.status);

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void ClickToGetStepsInformation(){
        try{
        steps = GoogleAPIScript.steps;
        foreach(var x in steps){
            Debug.Log(x.end_location.lat+","+x.end_location.lng);
        }
        if(steps == null) return;
        //active compass
        //ARCompass.SetActive(true);
        CompassObject.SetActive(true);

        panel = Instantiate(PanelPrefab) as GameObject;
        texts = panel.GetComponentsInChildren<Text>();
        texts[0].text = "Distance here";
        //texts[1].text = steps[count].maneuver; // description
        texts[2].text = "Step " + (count+1) + " / " + steps.Count;
        texts[3].text = steps[count].end_location.lat + ", " + steps[count].end_location.lng;
        
        }
        catch(Exception e){
            Debug.LogException(e, this);
            Debug.Log("function Exception");
        }
        //StepLoop starts in 0.1s and repeating running every 0.5s
        InvokeRepeating("StepsLoop", 0.1f, 0.5f);
    }

    public void StepsLoop()
    {
        Debug.Log("stepsloop");
        
        lat = GPSInstance.lat;
        lon = GPSInstance.lon;
        destLat = steps[count].end_location.lat;
        destLon = steps[count].end_location.lng;
        //update compass direction
        //Debug.Log("ARCompassIOS :" + ARCompassIOS.startLat);
        ARCompassIOS.startLat = lat;
        ARCompassIOS.startLon = lon;
        ARCompassIOS.endLat = destLat;
        ARCompassIOS.endLon = destLon;

        Debug.Log("----------tempdest"+destLat+"   "+destLon+"--------------");
        //Debug.Log("DistanceInstance :" + DistanceCalculatorInstance.CalculateDistanceMeters(lat, lon, destLat, destLon));

        //int distance = Mathf.RoundToInt(DistanceCalculatorInstance.CalculateDistanceMeters(lat, lon, destLat, destLon));
        int distance = Mathf.RoundToInt(CalculateDistanceMeters(lat, lon, destLat, destLon));
        Debug.Log("-----------------"+distance+"-----------------");
        // constantly update distance shown
        texts[0].text = distance.ToString() + "m";

        

        if (isCollide())
                    {
                        Destroy(panel);
                        //panels count = panels[count+1]?
                        count++;
                        if (count < steps.Count)
                        {
                            panel = Instantiate(PanelPrefab);//,directionsPanel
                            texts = panel.GetComponentsInChildren<Text>();
                            //texts[1].text = steps[count].maneuver; // description
                            texts[2].text = "Step " + (count+1) + " / " + steps.Count;
                            texts[3].text = steps[count].end_location.lat + ", " + steps[count].end_location.lng;
                        }
                    }
                    //when arriving the dest, cancel this invokerepeating. 
					if (count == steps.Count)
                    {
                        CompassObject.SetActive(false);
                        //Destroy(CompassObject);
                        CancelInvoke();
                    }
				
	}
    

    bool isCollide() {
		lat = Input.location.lastData.latitude;
		lon = Input.location.lastData.longitude;
        //collide within 10m
		if (lat - destLat <= 0.0001f && lat - destLat >= -0.0001f) {
			if (lon - destLon <= 0.0001f && lon - destLon >= -0.0001f) {
				return true;
			}
		}

		return false;
	}

    public float CalculateDistanceMeters (float lat1, float lon1, float lat2, float lon2)
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