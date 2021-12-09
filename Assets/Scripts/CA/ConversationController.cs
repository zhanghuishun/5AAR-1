using Syrus.Plugins.DFV2Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationController : MonoBehaviour
{
    private static DialogFlowV2Client client;

    //TODO implement a way to get session name (UUID)
    public static string sessionName = "123456789";

    private static List<Text> textOutputFields;
    private static List<TextMeshProUGUI> textPROOutputFields;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        client = GetComponent<DialogFlowV2Client>();
        client.ChatbotResponded += OnResponse;
        client.DetectIntentError += LogError;

        textOutputFields = new List<Text>();
        textPROOutputFields = new List<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void SendTextIntent(string text)
    {
        client.DetectIntentFromText(text, sessionName);
    }

    public static void SendAudioIntent(AudioClip clip)
    {
        byte[] audioBytes = WavUtility.FromAudioClip(clip);
        string audioString = Convert.ToBase64String(audioBytes);
        client.DetectIntentFromAudio(audioString, sessionName);
    }

    public static void SendEventIntent(string eventName, Dictionary<string, object> parameters)
    {
        client.DetectIntentFromEvent(eventName, parameters, sessionName);
    }

    public static void RegisterTextOutputField(Text field)
    {
        textOutputFields.Add(field);
    }

    public static void UnregisterTextOutputField(Text field)
    {
        textOutputFields.Remove(field);
    }

    public static void RegisterTextOutputField(TextMeshProUGUI field)
    {
        textPROOutputFields.Add(field);
    }

    public static void UnregisterTextOutputField(TextMeshProUGUI field)
    {
        textPROOutputFields.Remove(field);
    }

    private void OnResponse(DF2Response response)
    {
        Debug.Log(name + " said: \"" + response.queryResult.fulfillmentText + "\"");
        foreach (Text field in textOutputFields)
            field.text = response.queryResult.fulfillmentText;

        foreach (TextMeshProUGUI field in textPROOutputFields)
            field.text = response.queryResult.fulfillmentText;
    }

    private void LogError(DF2ErrorResponse errorResponse)
    {
        Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(), errorResponse.error.message));
    }
}
