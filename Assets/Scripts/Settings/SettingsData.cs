using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SettingsData : MonoBehaviour
{
    private static SettingsData instance = null;

    //dest start from home:"45.516826", "9.216668"
    public static StopInfo selectedStop = null;
    private static string stopName = null;
    public static bool selectedStopSet = false;
    //public static float volumeDB { get => volume <= 0.0001f ? -80 : Mathf.Log10(volume * 20); }
    public static float volume = 0.75f;
    public static string[] destinationCoordinates { get => new string[2] { selectedStop.lat.ToString(), selectedStop.lon.ToString() }; } // = new string[2] {"45.4827681", "9.2322856"}; // lambrate station,
    public static string[] tabacchiCoordinates = null;// {"45.480759", "9.224494"}; //tabacchi near BCL

    private static string selectedStopNameKey = "selectedStop";
    private static string tabacchiLonKey = "tabacchiLon";
    private static string tabacchiLatKey = "tabacchiLat";
    private static string volumeKey = "volume";

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

        if (PlayerPrefs.HasKey(volumeKey))
        {
            volume = PlayerPrefs.GetFloat(volumeKey);
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

    public static void SaveData(string stopName, string[] tabacchiCoordinates, float volume)
    {
        if (tabacchiCoordinates != null)
        {
            SettingsData.tabacchiCoordinates = new string[2] { tabacchiCoordinates[0], tabacchiCoordinates[1] };
            PlayerPrefs.SetString(tabacchiLonKey, tabacchiCoordinates[0]);
            PlayerPrefs.SetString(tabacchiLatKey, tabacchiCoordinates[1]);
        }

        if (stopName != null)
        {
            selectedStopSet = false;
            SettingsData.stopName = stopName;
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

        if (0 <= volume && volume <= 1)
        {
            SettingsData.volume = volume;
            PlayerPrefs.SetFloat(volumeKey, volume);
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