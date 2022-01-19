using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class InputFieldSubmit : MonoBehaviour
{
    private static InputFieldSubmit instance = null;

    //dest start from home:"45.516826", "9.216668"
    public static StopInfo selectedStop = null;
    private static string stopName = null;
    public static bool selectedStopSet = false;
    public static string[] destinationCoordinates { get => new string[2] { selectedStop.lon.ToString(), selectedStop.lon.ToString() }; } // = new string[2] {"45.4827681", "9.2322856"}; // lambrate station,
    public static string[] tabacchiCoordinates = null;// {"45.480759", "9.224494"}; //tabacchi near BCL

    private static string selectedStopNameKey = "selectedStop";
    private static string tabacchiLonKey = "tabacchiLon";
    private static string tabacchiLatKey = "tabacchiLat";

    public void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(transform.gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey(tabacchiLonKey) && PlayerPrefs.HasKey(tabacchiLatKey))
        {
            tabacchiCoordinates = new string[2] { PlayerPrefs.GetString(tabacchiLonKey), PlayerPrefs.GetString(tabacchiLatKey) };
        }

        if (PlayerPrefs.HasKey(selectedStopNameKey))
        {
            stopName = PlayerPrefs.GetString(selectedStopNameKey);
        }


        //Debug.Log("print "+destinationCord.placeholder.GetComponent<TextMeshProUGUI>().text);
        //destinationCord.placeholder.GetComponent<TextMeshProUGUI>().text = destinationCoordinates[0]+","+destinationCoordinates[1];
        //tabacchiCord.placeholder.GetComponent<TextMeshProUGUI>().text = tabacchiCoordinates[0]+","+tabacchiCoordinates[1];
        
    }

    private static IEnumerator GetSelectedStop()
    {
        yield return new WaitUntil(() => BusData.instance.stopsInfoPresent);
        selectedStop = BusData.instance.GetStopsFromName(stopName);
        selectedStopSet = true;
    }

    public static void SaveData(string stopName, string[] tabacchiCoordinates)
    {
        if (tabacchiCoordinates != null)
        {
            InputFieldSubmit.tabacchiCoordinates = new string[2] { tabacchiCoordinates[0], tabacchiCoordinates[1] };
            PlayerPrefs.SetString(tabacchiLonKey, tabacchiCoordinates[0]);
            PlayerPrefs.SetString(tabacchiLatKey, tabacchiCoordinates[1]);
        }

        if (stopName != null)
        {
            selectedStopSet = false;
            InputFieldSubmit.stopName = stopName;
            if (BusData.instance.stopsInfoPresent)
            {
                selectedStop = BusData.instance.GetStopsFromName(stopName);
                selectedStopSet = true;
            }
            else
            {
                selectedStop = null;
                instance.StartCoroutine(GetSelectedStop());
            }
            PlayerPrefs.SetString(selectedStopNameKey, stopName);
        }

        PlayerPrefs.Save();
    }

    public void Start()
	{
        StartCoroutine(GetSelectedStop());
        //destinationCord.onEndEdit.AddListener(delegate{LockInput(destinationCoordinates, destinationCord);});
        //tabacchiCord.onEndEdit.AddListener(delegate{LockInput(tabacchiCoordinates, tabacchiCord);});
    }

}