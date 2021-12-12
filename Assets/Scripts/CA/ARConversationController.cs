using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARConversationController : MonoBehaviour
{
    public TextMeshProUGUI CAText;
    private ArrowNavigation navigation;
    private LogicFunctions LogicFunctions;

    // Start is called before the first frame update
    void Start()
    {
        ConversationController.istance.RegisterTextOutputField(CAText);
        navigation = GetComponent<ArrowNavigation>();
        LogicFunctions = GetComponent<LogicFunctions>();

        switch (PhaseController.phase)
        {
            case Phases.BUY_TICKET: BuyTicketLogic();  break;
            case Phases.FIND_BUS_STOP: FindBusStopLogic();  break;
            case Phases.TRAVEL_ON_THE_BUS: TravelOnTheBusLogic();  break;
            default: break;
        }
    }

    private void BuyTicketLogic()
    {
        ConversationController.istance.SendEventIntent("CheckSubscription");
        //Change info popup and options
        ConversationController.istance.ChangeTextFields("guide you to tabacchi shop now");
        //route query and AR navigation,TODO: order problem
        //Debug.Log(navigation.testString);
        navigation.StepsInformationWrap(() => LogicFunctions.OnTabacchiShopLogic());

        //ConversationController.istance.ChangeTextFields("Now go inside and buy a ticket, tell me when you already get the ticket");
        //wait for user action
        Debug.Log("finish phase 0");


        //load next phase
        //PhaseController.phase = Phases.FIND_BUS_STOP;
        //SceneManager.LoadScene("ARScene");
    }

    private void FindBusStopLogic()
    {

    }

    private void TravelOnTheBusLogic()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
