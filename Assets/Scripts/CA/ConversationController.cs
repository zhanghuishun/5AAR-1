using Syrus.Plugins.DFV2Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
public class ConversationController : MonoBehaviour
{
    public static ConversationController istance { private set; get; }

    private  DialogFlowV2Client client;

    //TODO implement a way to get session name (UUID)
    public  string sessionName = "123456789";

    private  List<Text> textOutputFields;
    private  List<TextMeshProUGUI> textPROOutputFields;

    private readonly object textFieldsLock = new object();
    private bool mockLock = true;
    public bool textFieldsOverwritten { private set; get; }

    private void Awake()
    {
        istance = this;

        DontDestroyOnLoad(transform.gameObject);

        client = GetComponent<DialogFlowV2Client>();
        client.ChatbotResponded += OnResponse;
        client.DetectIntentError += LogError;

        textOutputFields = new List<Text>();
        textPROOutputFields = new List<TextMeshProUGUI>();
        textFieldsOverwritten = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //textFieldsOverwritten = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SendTextIntent(string text)
    {
        StartCoroutine(_SendTextIntent(text));
    }

    private IEnumerator _SendTextIntent(string text)
    {
        if (textFieldsLock != null) 
        {
            //lock (textFieldsLock)
            yield return new WaitUntil(() => mockLock);
            //{
            mockLock = false;
            textFieldsOverwritten = false;
                client.DetectIntentFromText(text, sessionName);
                yield return new WaitUntil(() => textFieldsOverwritten);
            //}
            mockLock = true;
        }
    }

    public void SendAudioIntent(AudioClip clip)
    {
        StartCoroutine(_SendAudioIntent(clip));
    }

    private IEnumerator _SendAudioIntent(AudioClip clip)
    {
        byte[] audioBytes = WavUtility.FromAudioClip(clip);
        string audioString = Convert.ToBase64String(audioBytes);
        if (textFieldsLock != null) 
        {
            //lock (textFieldsLock)
            yield return new WaitUntil(() => mockLock);
            //{
            textFieldsOverwritten = false;
                client.DetectIntentFromAudio(audioString, sessionName);
                yield return new WaitUntil(() => textFieldsOverwritten);
            //}
            mockLock = true;
        }
    }

    public void SendEventIntent(string eventName, Dictionary<string, object> parameters)
    {
        StartCoroutine(_SendEventIntent(eventName, parameters));
        
    }

    private IEnumerator _SendEventIntent(string eventName, Dictionary<string, object> parameters)
    {
        if (textFieldsLock != null) 
        {
            //lock (textFieldsLock)
            yield return new WaitUntil(() => mockLock);
            //{
            mockLock = false;
                //Debug.Log("enter evnet intent");
                Debug.Log(Thread.CurrentThread.ManagedThreadId.ToString());
                textFieldsOverwritten = false;
                client.DetectIntentFromEvent(eventName, parameters, sessionName);
                yield return new WaitUntil(() => textFieldsOverwritten);
                //Debug.Log("finish event intent");

            //}
            mockLock = true;
        }
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
        Debug.Log(Thread.CurrentThread.ManagedThreadId.ToString());
        StartCoroutine(_OverwriteTextFields(response.queryResult.fulfillmentText));
    }

    private IEnumerator _OverwriteTextFields(String text)
    {
        //Debug.Log(name + " said: \"" + response.queryResult.fulfillmentText + "\"");
        Debug.Log(text);
        foreach (Text field in textOutputFields)
            field.text = text;

        foreach (TextMeshProUGUI field in textPROOutputFields)
            field.text = text;

        yield return new WaitForSecondsRealtime(3);
        textFieldsOverwritten = true;
        Debug.Log("finish overwrite");

    }

    public void ChangeTextFields(String text)
    {
        StartCoroutine(_ChangeTextFields(text));

    }

    private IEnumerator _ChangeTextFields(String text)
    {
        if (textFieldsLock != null)
        {
            //lock (textFieldsLock)
            yield return new WaitForSecondsRealtime(1);
            yield return new WaitUntil(() => mockLock);
            //{
            mockLock = false;
                //Debug.Log("enter _change text fields");
                Debug.Log(Thread.CurrentThread.ManagedThreadId.ToString());
                textFieldsOverwritten = false;
                StartCoroutine(_OverwriteTextFields(text));
                yield return new WaitUntil(() => textFieldsOverwritten);
                //Debug.Log("finish _change text fields");
            //}
            mockLock = true;
        }
    }
    public void ChangeTextFieldsPriority(String text)
    {
        StartCoroutine(_ChangeTextFieldsPriority(text));

    }

    private IEnumerator _ChangeTextFieldsPriority(String text)
    {
        if (textFieldsLock != null)
        {
            //lock (textFieldsLock);
            yield return new WaitUntil(() => mockLock);
            //{
            mockLock = false;
            Debug.Log("enter _change text fields");
            Debug.Log(Thread.CurrentThread.ManagedThreadId.ToString());
            textFieldsOverwritten = false;
            StartCoroutine(_OverwriteTextFields(text));
            yield return new WaitUntil(() => textFieldsOverwritten);
            Debug.Log("finish _change text fields");
            //}
            mockLock = true;
        }
    }

    private void LogError(DF2ErrorResponse errorResponse)
    {
        Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(), errorResponse.error.message));
        ChangeTextFields("ERROR");
    }
}
