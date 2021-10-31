using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSLocation : MonoBehaviour
{
    public Text GPSStatus;
    public Text latitudeValue;
    public Text longitudeValue;
    public Text altitudeValue;
    public Text horizontalAccuracyValue;
    public Text timestampValue;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPSLoc());
    }
    // reference : https://www.youtube.com/watch?v=JWccDbm69Cg
    IEnumerator GPSLoc(){
        if(!Input.location.isEnabledByUser) yield break;

        Input.location.Start();

        int maxWait = 20;

        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0){
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        //service didn't init in 20 sec
        if(maxWait < 1){
            GPSStatus.text = "Time out";
            yield break;
        }
        //connection failed
        if(Input.location.status == LocationServiceStatus.Failed){
            GPSStatus.text = "Unable to determin device location";
            yield break;
        }else{
            GPSStatus.text = "Running";
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
        }
    }
    private void UpdateGPSData(){
        if(Input.location.status == LocationServiceStatus.Running){
            //Access granted to gps values and it has been init
            GPSStatus.text = "Running";
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            latitudeValue.text = Input.location.lastData.latitude.ToString();
            longitudeValue.text = Input.location.lastData.longitude.ToString();
            altitudeValue.text = Input.location.lastData.altitude.ToString();
            horizontalAccuracyValue.text = Input.location.lastData.horizontalAccuracy.ToString();
            timestampValue.text = Input.location.lastData.timestamp.ToString();
            
        }else{
            // service is stopped
            GPSStatus.text = "Stop";
        }
    }
}
