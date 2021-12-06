using Newtonsoft.Json;
using Syrus.Plugins.DFV2Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public InputField inputField;
    public Text outputField;
    public Button micButton;

    private string sessionName = "123456789";
    private DialogFlowV2Client client;

    private bool micConnected = false;
    private int minFreq;
    private int maxFreq;
    private AudioSource goAudioSource;
    private Text micButtonText;

    // Start is called before the first frame update
    void Start()
    {
        client = GetComponent<DialogFlowV2Client>();

        client.ChatbotResponded += OnResponse;
        client.DetectIntentError += LogError;

        inputField.text = "How's the weather?";

        if (Microphone.devices.Length <= 0)
        {
            //Throw a warning message at the console if there isn't  
            Debug.LogWarning("Microphone not connected!");
        }
        else //At least one microphone is present  
        {
            //Set 'micConnected' to true  
            micConnected = true;

            //Get the default microphone recording capabilities  
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (minFreq == 0 && maxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate  
                maxFreq = 44100;
            }

            //Get the attached AudioSource component  
            goAudioSource = this.GetComponent<AudioSource>();
        }

        micButtonText = micButton.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSend()
    {
        client.DetectIntentFromText(inputField.text, sessionName);
    }

    private void OnResponse(DF2Response response)
    {
        Debug.Log(JsonConvert.SerializeObject(response, Formatting.Indented));
        Debug.Log(name + " said: \"" + response.queryResult.fulfillmentText + "\"");
        outputField.text = response.queryResult.fulfillmentText;
    }

    private void LogError(DF2ErrorResponse errorResponse)
    {
        Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(), errorResponse.error.message));
    }

    public void OnMicButtonPress()
    {
        //If there is a microphone  
        if (micConnected)
        {
            //If the audio from any microphone isn't being captured  
            if (!Microphone.IsRecording(null))
            {
                //Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource  
                goAudioSource.clip = Microphone.Start(null, true, 20, maxFreq);
                micButtonText.text = "Stop";
            }
            else //Recording is in progress  
            {
                Microphone.End(null); //Stop the audio recording  
                goAudioSource.Play(); //Playback the recorded audio  
                micButtonText.text = "Mic";
                sendAudio(goAudioSource.clip);
            }
        }
        else // No microphone  
        {
            //Print a red "Microphone not connected!" message at the center of the screen  
            GUI.contentColor = Color.red;
        }

    }

    private void sendAudio(AudioClip clip)
    {
        byte[] audioBytes = WavUtility.FromAudioClip(clip);
        string audioString = Convert.ToBase64String(audioBytes);
        client.DetectIntentFromAudio(audioString, sessionName);
    }
}