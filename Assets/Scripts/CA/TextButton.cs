using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextButton : MonoBehaviour
{
    public GameObject multipleChoichesPopup;
    public List<GameObject> choichesButtons;
    private List<TextMeshProUGUI> choichesButtonTexts;
    public GameObject customTextPopup;
    public TMP_InputField inputField;

    private void Awake()
    {
        choichesButtonTexts = new List<TextMeshProUGUI>();
        foreach (GameObject o in choichesButtons)
            choichesButtonTexts.Add(o.GetComponentInChildren<TextMeshProUGUI>());
    }

    // Start is called before the first frame update
    void Start()
    {
        ConversationController.Instance.ChangeOptionsConsumer(ChangeOptions);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTextButtonPressed()
    {
        multipleChoichesPopup.SetActive(true);
    }

    public void OnChoicheButtonPressed(int index)
    {
        ConversationController.Instance.SendTextIntent(choichesButtonTexts[index].text);
        CloseMultipleChoichesPopup();
    }

    public void OnCustomButtomPressed()
    {
        multipleChoichesPopup.SetActive(false);
        customTextPopup.SetActive(true);
    }

    public void OnSend()
    {
        ConversationController.Instance.SendTextIntent(inputField.text);
        CloseCustomTextPopup();
    }

    public void CloseMultipleChoichesPopup()
    {
        multipleChoichesPopup.SetActive(false);
    }

    public void CloseCustomTextPopup()
    {
        inputField.text = "";
        customTextPopup.SetActive(false);
    }

    public void ChangeOptions(string[] options)
    {
        int toChange;
        if (options == null)
        {
            toChange = 0;
        }
        else
        {
            toChange = Mathf.Min(options.Length, choichesButtonTexts.Count);
        }

        Debug.Log(toChange);

        for (int i=0; i < choichesButtonTexts.Count; i++)
        {
            if (i < toChange)
            {
                choichesButtons[i].SetActive(true);
                choichesButtonTexts[i].text = options[i];
            }
            else
            {
                choichesButtons[i].SetActive(false);
            }
        }
    }
}
