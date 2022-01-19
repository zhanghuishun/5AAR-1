using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputFields : MonoBehaviour
{
    public SerchableDropdown destinationCord;
    public TMP_InputField tabacchiLongitude;
    public TMP_InputField tabacchiLatitude;
    // Start is called before the first frame update

    void Start()
    {
        if (InputFieldSubmit.selectedStopSet)
        {
            destinationCord.inputField.text = InputFieldSubmit.selectedStop == null ? "" : InputFieldSubmit.selectedStop.name;
        }
        else
        {
            StartCoroutine(UpdateDestination());
        }

        if (InputFieldSubmit.tabacchiCoordinates != null)
        {
            tabacchiLongitude.text = InputFieldSubmit.tabacchiCoordinates[0];
            tabacchiLatitude.text = InputFieldSubmit.tabacchiCoordinates[1];
        }
    }

    private IEnumerator UpdateDestination()
    {
        destinationCord.inputField.interactable = false;
        destinationCord.inputField.text = "Loading...";
        yield return new WaitUntil(()=>InputFieldSubmit.selectedStopSet);
        destinationCord.inputField.text = InputFieldSubmit.selectedStop == null ? "" : InputFieldSubmit.selectedStop.name;
        destinationCord.inputField.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSaveAndQuit()
    {
        string[] tabacchiCord;
        try
        {
            float lon = float.Parse(tabacchiLongitude.text);
            float lat = float.Parse(tabacchiLatitude.text);

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


        InputFieldSubmit.SaveData(destinationCord.getSelection(), tabacchiCord);
        SceneManager.LoadScene("SecondPage");
    }

    public void OnDontSaveAndQuit()
    {
        SceneManager.LoadScene("SecondPage");
    }
}
