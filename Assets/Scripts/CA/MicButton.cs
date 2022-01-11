using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MicButton : MonoBehaviour
{
    private bool micConnected = false;
    private int minFreq;
    private int maxFreq;

    private AudioSource goAudioSource;

    public GameObject micOnSprite;
    public GameObject micOffSprite;
    public float minimumLevel = 1e-06f;
    private float quietTime = 0;
    public float quietTimeMax = 1.5f;
    private bool isRecording = false;

    private void Awake()
    {
        goAudioSource = GetComponent<AudioSource>();
        goAudioSource.mute = true;
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
        if (isRecording)
        {
            //Debug.Log(MicLoudness.micLoudness);

            if(MicLoudness.micLoudness < minimumLevel)
            {
                quietTime += Time.deltaTime;
            }

            if(quietTime >= quietTimeMax)
            {
                quietTime = 0;
                StopMicrophone();
            }
        }
    }

    public void OnMicButtonPress()
    {
        //If there is a microphone  
        if (micConnected)
        {
            //If the audio from any microphone isn't being captured  
            if (!isRecording)
            {
                isRecording = true;
                //Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource  
                goAudioSource.clip = Microphone.Start(null, false, 20, maxFreq);
                //goAudioSource.Play();
                //micText.text = "Stop";
                ToggleMic(true);
            }
            else //Recording is in progress  
            {
                StopMicrophone();
            }
        }
        else // No microphone  
        {
            //Print a red "Microphone not connected!" message at the center of the screen  
            GUI.contentColor = Color.red;
        }

    }

    private void StopMicrophone()
    {
        isRecording = false;
        //Stop the audio recording
        Microphone.End(null);   
        goAudioSource.Play(); //Playback the recorded audio
        //micText.text = "Mic";
        ToggleMic(false);
        //ConversationController.Instance.SendAudioIntent(goAudioSource.clip);
    }

    private void ToggleMic(bool state)
    {
        micOnSprite.SetActive(state);
        micOffSprite.SetActive(!state);
    }
}
