using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionGenerator : MonoBehaviour
{
    public float angleFromCoordinate(float lat1, float long1, float lat2,
        float long2) {
    lat1 = (Mathf.PI/180) * lat1;
    long1 = (Mathf.PI/180) * long1;
    lat2 = (Mathf.PI/180) * lat2;
    long2 = (Mathf.PI/180) * long2;

    float dLon = (long2 - long1);

    float y = Mathf.Sin(dLon) * Mathf.Cos(lat2);
    float x = Mathf.Cos(lat1) * Mathf.Sin(lat2) - Mathf.Sin(lat1)
            * Mathf.Cos(lat2) * Mathf.Cos(dLon);

    float brng = Mathf.Atan2(y, x);

    brng = Mathf.Rad2Deg * brng;
    brng = (brng + 360) % 360;
    brng = 360 - brng; // count degrees counter-clockwise - remove to make clockwise

    return brng;
}
}
