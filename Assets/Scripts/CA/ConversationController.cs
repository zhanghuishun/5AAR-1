using Syrus.Plugins.DFV2Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConversationController : MonoBehaviour
{
    public static ConversationController Instance { private set; get; } = null;

    private  DialogFlowV2Client client;

    //TODO implement a way to get session name (UUID)
    private string sessionName = Guid.NewGuid().ToString();

    private  List<Text> textOutputFields;
    private  List<TextMeshProUGUI> textPROOutputFields;
    private Action<string[]> optionsConsumer = null;

    private readonly object textFieldsLock = new object();
    private bool textLock = false;
    public bool textFieldsOverwritten { private set; get; }
    private Action afterWriteCallback = null;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(transform.gameObject);

            client = GetComponent<DialogFlowV2Client>();
            client.ChatbotResponded += OnResponse;
            client.DetectIntentError += LogError;

            textOutputFields = new List<Text>();
            textPROOutputFields = new List<TextMeshProUGUI>();
            textFieldsOverwritten = true;
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void SendTextIntent(string text, Action callback = null)
    {
        StartCoroutine(_SendTextIntent(text, callback));
    }

    private IEnumerator _SendTextIntent(string text, Action callback = null)
    {
        if (textFieldsLock != null)
        {
            bool canGo = false;
            new Thread(() =>
            {
                Monitor.Enter(textFieldsLock);
                try
                {
                    while (textLock)
                        Monitor.Wait(textFieldsLock);
                    canGo = true;
                    afterWriteCallback = callback;
                    textLock = true;
                    Monitor.PulseAll(textFieldsLock);
                }
                finally
                {
                    Monitor.Exit(textFieldsLock);
                }
            }).Start();
            CAAnimationsController.istance.SetLoading(true);
            yield return new WaitUntil(() => canGo);
            //lock (textFieldsLock)
            ///yield return new WaitUntil(() => mockLock);
            //{
            ///mockLock = false;
            textFieldsOverwritten = false;
            client.DetectIntentFromText(text, sessionName);
            yield return new WaitUntil(() => textFieldsOverwritten);
            //}
            ///mockLock = true;
        }
    }

    public void SendAudioIntent(AudioClip clip, Action callback = null)
    {
        StartCoroutine(_SendAudioIntent(clip, callback));
    }

    private IEnumerator _SendAudioIntent(AudioClip clip, Action callback = null)
    {
        byte[] audioBytes = WavUtility.FromAudioClip(clip);
        string audioString = Convert.ToBase64String(audioBytes);
        if (textFieldsLock != null)
        {
            bool canGo = false;
            new Thread(() =>
            {
                Monitor.Enter(textFieldsLock);
                try
                {
                    while (textLock)
                        Monitor.Wait(textFieldsLock);
                    canGo = true;
                    afterWriteCallback = callback;
                    textLock = true;
                    Monitor.PulseAll(textFieldsLock);
                }
                finally
                {
                    Monitor.Exit(textFieldsLock);
                }
            }).Start();
            CAAnimationsController.istance.SetLoading(true);
            yield return new WaitUntil(() => canGo);
            //lock (textFieldsLock)
            ///yield return new WaitUntil(() => mockLock);
            //{
            //textFieldsOverwritten = false;
            client.DetectIntentFromAudio(audioString, sessionName);
            yield return new WaitUntil(() => textFieldsOverwritten);
            //}
            ///mockLock = true;
        }
    }

    public void SendEventIntent(string eventName, Dictionary<string, object> parameters, Action callback = null)
    {
        StartCoroutine(_SendEventIntent(eventName, parameters, callback));
        
    }

    private IEnumerator _SendEventIntent(string eventName, Dictionary<string, object> parameters, Action callback = null)
    {
        if (textFieldsLock != null)
        {
            bool canGo = false;
            Debug.Log("pre-enter evnet intent");
            new Thread(() =>
            {
                Monitor.Enter(textFieldsLock);
                Debug.Log("Got lock");
                try
                {
                    Debug.Log(textLock);
                    while (textLock)
                        Monitor.Wait(textFieldsLock);
                    canGo = true;
                    afterWriteCallback = callback;
                    textLock = true;
                    Monitor.PulseAll(textFieldsLock);
                }
                finally
                {
                    Monitor.Exit(textFieldsLock);
                }
            }).Start();
            CAAnimationsController.istance.SetLoading(true);
            yield return new WaitUntil(() => canGo);
            //lock (textFieldsLock)
            ///yield return new WaitUntil(() => mockLock);
            //{
            ///mockLock = false;
            Debug.Log("enter evnet intent");
            Debug.Log(Thread.CurrentThread.ManagedThreadId.ToString());
            textFieldsOverwritten = false;
            client.DetectIntentFromEvent(eventName, parameters, sessionName);
            yield return new WaitUntil(() => textFieldsOverwritten);
            //Debug.Log("finish event intent");

            //}
            ///mockLock = true;
        }
    }

    public void SendEventIntent(string eventName, Action callback = null)
    {
        SendEventIntent(eventName, new Dictionary<string, object>(), callback);
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

    public void ChangeOptionsConsumer(Action<string[]> fun)
    {
        optionsConsumer = fun;
    }

    private void OnResponse(DF2Response response)
    {
        //Debug.Log(Thread.CurrentThread.ManagedThreadId.ToString());
        string responseText = response.queryResult.fulfillmentText;
        StartCoroutine(_OverwriteTextFields(responseText));

        CustomPayload cp = GetCustomPayload(response);
        string method = response.queryResult.action;
        if (method!=null && InterfaceMethods.list.ContainsKey(method)) InterfaceMethods.list[method].Invoke();
        if (optionsConsumer != null) optionsConsumer.Invoke(cp?.options);
    }

    private CustomPayload GetCustomPayload(DF2Response response)
    {
        if (response.queryResult.fulfillmentMessages.Length <= 1) return null;
        string s = response.queryResult.fulfillmentMessages[1]["payload"].ToString();
        return JsonUtility.FromJson<CustomPayload>(s);
    }

    private IEnumerator _OverwriteTextFields(string text)
    {
        //Debug.Log(name + " said: \"" + response.queryResult.fulfillmentText + "\"");
        Debug.Log(text);
        foreach (Text field in textOutputFields)
            field.text = text;

        foreach (TextMeshProUGUI field in textPROOutputFields)
            field.text = text;

        TTSController.Speak(text);

        if (afterWriteCallback != null) afterWriteCallback.Invoke();
        CAAnimationsController.istance.SetLoading(false);

        yield return new WaitForSecondsRealtime(10);
        new Thread(() =>
        {
            Monitor.Enter(textFieldsLock);
            try
            {
                textFieldsOverwritten = true;
                textLock = false;
                Monitor.PulseAll(textFieldsLock);
            }
            finally
            {
                Monitor.Exit(textFieldsLock);
            }
        }).Start();
        Debug.Log("finish overwrite");

    }

    public void ChangeTextFields(string text, Action callback = null)
    {
        StartCoroutine(_ChangeTextFields(text, callback));

    }

    private IEnumerator _ChangeTextFields(string text, Action callback = null)
    {
        if (textFieldsLock != null)
        {
            //lock (textFieldsLock)
            bool canGo = false;
            new Thread(() =>
            {
                Monitor.Enter(textFieldsLock);
                try
                {
                    while (textLock)
                        Monitor.Wait(textFieldsLock);
                    canGo = true;
                    afterWriteCallback = callback;
                    textLock = true;
                    Monitor.PulseAll(textFieldsLock);
                }
                finally
                {
                    Monitor.Exit(textFieldsLock);
                }
            }).Start();
            CAAnimationsController.istance.SetLoading(true);
            yield return new WaitUntil(() => canGo);
            ///yield return new WaitForSecondsRealtime(1);
            ///yield return new WaitUntil(() => mockLock);
            //{
            ///mockLock = false;
            Debug.Log("enter _change text fields");
            Debug.Log(Thread.CurrentThread.ManagedThreadId.ToString());
            textFieldsOverwritten = false;
            StartCoroutine(_OverwriteTextFields(text));
            Debug.Log("textFieldsOverwritten:" + textFieldsOverwritten);
            yield return new WaitUntil(() => textFieldsOverwritten);
            //Debug.Log("finish _change text fields");
            //}
            ///mockLock = true;
        }
    }
    /*public void ChangeTextFieldsPriority(string text)
    {
        StartCoroutine(_ChangeTextFieldsPriority(text));

    }

    private IEnumerator _ChangeTextFieldsPriority(string text)
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
    }*/

    private void LogError(DF2ErrorResponse errorResponse)
    {
        Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(), errorResponse.error.message));
        ChangeTextFields("ERROR");
    }
}

public class CustomPayload
{
    public string[] options;
}

public class InterfaceMethods
{
    public static readonly Dictionary<string, Action> list = new Dictionary<string, Action>
    {
        { "HELP_SUBSCRIPTION", () =>{ } },  //start the jurney towords the nearest tabacchi shop
        { "FIND_TABACCHI_SHOP", () =>{ } },  //start the jurney towords the nearest tabacchi shop
        { "FIND_BUS_STOP", () =>{ } }, //start the jurney towords the bus stop
        { "FIND_ANOTHER_TABACCHI_SHOP", () =>{ } }, //start the jurney towords another tabacchi shop (because the first was closed)
        { "CHECK_TICKET", () =>{ } } //the ticket got recognized
    };

    public static bool AddMethod(string interfaceName, Action method)
    {
        if (list.ContainsKey(interfaceName))
        {
            list[interfaceName] = method;
            return true;
        }

        return false;
    }
}
