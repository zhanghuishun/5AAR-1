using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputFields : MonoBehaviour
{
    public SerchableDropdown destinationCord;
    public TMP_InputField tabacchiLongitude;
    public TMP_InputField tabacchiLatitude;
    public Slider volumeSlider;
    // Start is called before the first frame update

    void Start()
    {
        if (SettingsData.selectedStopSet)
        {
            destinationCord.inputField.text = SettingsData.selectedStop == null ? "" : SettingsData.selectedStop.name;
            destinationCord.UpdateSearch(destinationCord.inputField.text);
        }
        else
        {
            StartCoroutine(UpdateDestination());
        }

        if (SettingsData.tabacchiCoordinates != null)
        {
            tabacchiLongitude.text = SettingsData.tabacchiCoordinates[0];
            tabacchiLatitude.text = SettingsData.tabacchiCoordinates[1];
        }

        volumeSlider.value = SettingsData.volume;
    }

    private IEnumerator UpdateDestination()
    {
        destinationCord.inputField.interactable = false;
        destinationCord.inputField.text = "Loading...";
        yield return new WaitUntil(()=>SettingsData.selectedStopSet);
        destinationCord.inputField.text = SettingsData.selectedStop == null ? "" : SettingsData.selectedStop.name;
        destinationCord.inputField.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSaveAndQuit()
    {
        string[] tabacchiCord;
        if (tabacchiLongitude.text.Equals("") || tabacchiLatitude.text.Equals(""))
        {
            tabacchiCord = new string[2] { "", "" };
        }
        else
        {
            try
            {

                float lon = float.Parse(tabacchiLongitude.text, CultureInfo.InvariantCulture);
                float lat = float.Parse(tabacchiLatitude.text, CultureInfo.InvariantCulture);

                if (-180 <= lon && lon <= 180 && -90 <= lat && lat <= 90)
                {
                    tabacchiCord = new string[2] { tabacchiLongitude.text, tabacchiLatitude.text };
                }
                else
                {
                    tabacchiCord = null;
                }
            }
            catch (Exception)
            {
                tabacchiCord = null;
            }
        }
        
        SettingsData.SaveData(destinationCord.getSelection(), tabacchiCord, volumeSlider.value);
        SceneManager.LoadScene("SecondPage");
    }

    public void OnDontSaveAndQuit()
    {
        SceneManager.LoadScene("SecondPage");
    }
}
