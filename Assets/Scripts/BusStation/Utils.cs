using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class Utils : MonoBehaviour
{
    private static Utils _instance;

    public static Utils Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    public DateTime TimeParse(string timeStr){
        DateTime parsedTime;

        string hhmmss = timeStr +":00";
        Debug.Log("time:" + hhmmss);
        string[] formats = {"HH:mm:ss"};
        if (DateTime.TryParseExact(hhmmss, formats,
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out parsedTime))
            return parsedTime;
        else{
            Debug.Log("time parse error");
            return parsedTime;
        }
    }

    public List<step> Convert(List<innerStep> steps){
        Debug.Log(JsonUtility.ToJson(steps[0], true));
        List<step> newSteps = new List<step>();
        foreach(innerStep step in steps){
            step newStep = new step();
            newStep.end_location = step.end_location;
            newStep.start_location = step.start_location;
            newStep.travel_mode = step.travel_mode;
            newStep.dis = step.dis;
            newStep.dur = step.dur;
            newSteps.Add(newStep);
        }
            Debug.Log(JsonUtility.ToJson(newSteps, true));
        return newSteps;
    }

    public float CalculateDistanceMeters (float lat1, float lng1, float lat2, float lng2)
    {
        float R = 6378.137f; // Radius of Earth in KM
        float dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        float dlng = lng2 * Mathf.PI / 180 - lng1 * Mathf.PI / 180;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) + Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) * Mathf.Sin(dlng / 2)
            * Mathf.Sin(dlng / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float d = R * c;
        return d * 1000f;
    }

}
