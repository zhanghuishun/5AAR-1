using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitialConversationController : MonoBehaviour
{
    public TextMeshProUGUI CAText;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject nextButton;

    // Start is called before the first frame update
    void Start()
    {
        //Do I need to remove it?
        ConversationController.istance.RegisterTextOutputField(CAText);

        ConversationController.istance.SendEventIntent("Introduction");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void YesButton()
    {
        yesButton.SetActive(false);
        noButton.SetActive(false);
        ConversationController.istance.SendTextIntent("Yes", ChangeButtons);
        //ChangeButtons();
    }

    public void NoButton()
    {
        yesButton.SetActive(false);
        noButton.SetActive(false);
        ConversationController.istance.SendTextIntent("No", ChangeButtons);
        //ChangeButtons();
    }

    private void ChangeButtons()
    {
        //yesButton.SetActive(false);
        //noButton.SetActive(false);
        nextButton.SetActive(true);
    }
}
