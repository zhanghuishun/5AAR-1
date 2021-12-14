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

    private void Awake()
    {
        ConversationController.istance.RegisterTextOutputField(CAText);
        navigation = GetComponent<ArrowNavigation>();
        LogicFunctions = GetComponent<LogicFunctions>();

        InterfaceMethods.AddMethod("FIND_TABACCHI_SHOP", FindTabacchi);
        InterfaceMethods.AddMethod("CHECK_TICKET", () => LogicFunctions.TicketRecognitionLogic());
        InterfaceMethods.AddMethod("FIND_BUS_STOP", FindBusStop);
    }

    // Start is called before the first frame update
    void Start()
    {
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
        //TODO write comments on how the application works with dialogflow
    }

    private void FindTabacchi()
    {
        navigation.ShowNavigationInformation(Phases.BUY_TICKET, () => 
            ConversationController.istance.SendEventIntent("TabacchiReached"));
    }

    private void FindBusStopLogic()
    {
        ConversationController.istance.SendEventIntent("GoToBusStop");
    }

    private void FindBusStop()
    {
        Debug.Log("FindBusStop called");
        navigation.ShowNavigationInformation(Phases.FIND_BUS_STOP, () => 
            ConversationController.istance.SendEventIntent("BusStopReached", () => 
               LogicFunctions.AfterArrivingBusStopLogic() ));
    }

    private void TravelOnTheBusLogic()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
