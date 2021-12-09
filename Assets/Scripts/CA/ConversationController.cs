using Syrus.Plugins.DFV2Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationController : MonoBehaviour
{
    public static ConversationController istance { private set; get; }

    private  DialogFlowV2Client client;

    //TODO implement a way to get session name (UUID)
    public  string sessionName = "123456789";

    private  List<Text> textOutputFields;
    private  List<TextMeshProUGUI> textPROOutputFields;

    public  bool textFieldsCanBeOverwritten { private set; get;}

    private void Awake()
    {
        istance = this;

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
        textFieldsCanBeOverwritten = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendTextIntent(string text)
    {
        client.DetectIntentFromText(text, sessionName);
    }

    public void SendAudioIntent(AudioClip clip)
    {
        byte[] audioBytes = WavUtility.FromAudioClip(clip);
        string audioString = Convert.ToBase64String(audioBytes);
        client.DetectIntentFromAudio(audioString, sessionName);
    }

    public void SendEventIntent(string eventName, Dictionary<string, object> parameters)
    {
        client.DetectIntentFromEvent(eventName, parameters, sessionName);
    }

    public void SendEventIntent(string eventName)
    {
        SendEventIntent(eventName, new Dictionary<string, object>());
    }

    public void RegisterTextOutputField(Text field)
    {
        textOutputFields.Add(field);
    }

    public void UnregisterTextOutputField(Text field)
    {
        textOutputFields.Remove(field);
    }

    public void RegisterTextOutputField(TextMeshProUGUI field)
    {
        textPROOutputFields.Add(field);
    }

    public void UnregisterTextOutputField(TextMeshProUGUI field)
    {
        textPROOutputFields.Remove(field);
    }

    private void OnResponse(DF2Response response)
    {
        StartCoroutine(_ChangeTextFields(response.queryResult.fulfillmentText));
    }

    private IEnumerator _ChangeTextFields(String text)
    {
        yield return new WaitUntil(() => textFieldsCanBeOverwritten);

        //Debug.Log(name + " said: \"" + response.queryResult.fulfillmentText + "\"");
        foreach (Text field in textOutputFields)
            field.text = text;

        foreach (TextMeshProUGUI field in textPROOutputFields)
            field.text = text;

        textFieldsCanBeOverwritten = false;
        yield return new WaitForSecondsRealtime(5);
        textFieldsCanBeOverwritten = true;
    }

    public void ChangeTextFields(String text)
    {
        StartCoroutine(_ChangeTextFields(text));
    }

    private void LogError(DF2ErrorResponse errorResponse)
    {
        Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(), errorResponse.error.message));
    }
}
