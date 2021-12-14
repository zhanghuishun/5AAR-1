using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tmp_Text : MonoBehaviour
{
    public GameObject popup;
    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTextButtonPressed()
    {
        popup.SetActive(true);
    }

    public void OnSend()
    {
        ConversationController.istance.SendTextIntent(inputField.text);
        ClosePopup();
    }

    public void OnClose()
    {
        inputField.text = "";
        ClosePopup();
    }

    private void ClosePopup()
    {
        popup.SetActive(false);
    }
}
