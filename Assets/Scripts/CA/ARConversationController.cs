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
        ConversationController.istance.SendEventIntent("CheckSubscription", ()=> //ConversationController.istance.ChangeTextFields("After user answer"));
            navigation.StepsInformationWrap(Phases.BUY_TICKET, () => 
                LogicFunctions.OnTabacchiShopLogic()));
        //Change info popup and options
        
        //route query and AR navigation,TODO: order problem
        //Debug.Log(navigation.testString);
        
        //wait for user action
        Debug.Log("finish phase 0");
        //check Ticket

        //load next phase
        //LogicFunctions.LoadScene(Phases.FIND_BUS_STOP);
    }

    private void FindBusStopLogic()
    {
        ConversationController.istance.ChangeTextFields("Now let's go to the bus stop");
        navigation.StepsInformationWrap(Phases.FIND_BUS_STOP, () => 
                LogicFunctions.LogicWrap());

    }

    private void TravelOnTheBusLogic()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
