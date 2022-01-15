using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARConversationController : MonoBehaviour
{
    public TextMeshProUGUI CAText;
    public GameObject subscriptionPopup;
    private ArrowNavigation navigation;
    private LogicFunctions LogicFunctions;

    private Container<string> destination = new Container<string>();

    private void Awake()
    {
        ConversationController.Instance.RegisterTextOutputField(CAText);
        navigation = GetComponent<ArrowNavigation>();
        LogicFunctions = GetComponent<LogicFunctions>();

        InterfaceMethods.AddMethod("HELP_SUBSCRIPTION", ShowSubscriptionPopup);
        InterfaceMethods.AddMethod("FIND_TABACCHI_SHOP", FindTabacchi);
        InterfaceMethods.AddMethod("CHECK_TICKET", () => LogicFunctions.TicketRecognitionLogic());
        InterfaceMethods.AddMethod("FIND_BUS_STOP", FindBusStop);

        Parameters.AddParameter("destination", destination);
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

    private void ShowSubscriptionPopup()
    {
        subscriptionPopup.SetActive(true);
    }

    private void BuyTicketLogic()
    {
        ConversationController.Instance.SendEventIntent("CheckSubscription");
        //TODO write comments on how the application works with dialogflow
    }

    private void FindTabacchi()
    {
        destination.content = "tabacchi shop";
        navigation.ShowNavigationInformation(Phases.BUY_TICKET, () => 
            ConversationController.Instance.SendEventIntent("TabacchiReached"));
    }

    private void FindBusStopLogic()
    {
        ConversationController.Instance.SendEventIntent("GoToBusStop");
    }

    private void FindBusStop()
    {
        destination.content = "bus stop";
        Debug.Log("FindBusStop called");
        navigation.ShowNavigationInformation(Phases.FIND_BUS_STOP, () => 
            ConversationController.Instance.SendEventIntent("BusStopReached", () => 
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
