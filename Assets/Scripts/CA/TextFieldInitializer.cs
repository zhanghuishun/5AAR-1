using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextFieldInitializer : MonoBehaviour
{
    public string text = "";

    public List<Text> textFields = new List<Text>();
    public List<TextMeshProUGUI> textPROFields = new List<TextMeshProUGUI>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Text t in textFields)
            t.text = text;

        foreach (TextMeshProUGUI t in textPROFields)
            t.text = text;

        TTSController.Speak(text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
