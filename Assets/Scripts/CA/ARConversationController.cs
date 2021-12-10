using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARConversationController : MonoBehaviour
{
    public TextMeshProUGUI CAText;
    // Start is called before the first frame update
    void Start()
    {
        ConversationController.istance.RegisterTextOutputField(CAText);

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
        ConversationController.istance.ChangeTextFields("NNN");
        ConversationController.istance.SendEventIntent("CheckSubscription");
        //Change info popup and options
        
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
