using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicButton : MonoBehaviour
{
    private bool micConnected = false;
    private int minFreq;
    private int maxFreq;
    private AudioSource goAudioSource;
    public Text micText; //Will change to sprite

    private void Awake()
    {
        goAudioSource = this.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
                micText.text = "Stop";
            }
            else //Recording is in progress  
            {
                Microphone.End(null); //Stop the audio recording  
                goAudioSource.Play(); //Playback the recorded audio
                micText.text = "Mic";
                ConversationController.SendAudioIntent(goAudioSource.clip);
            }
        }
        else // No microphone  
        {
            //Print a red "Microphone not connected!" message at the center of the screen  
            GUI.contentColor = Color.red;
        }

    }
}
