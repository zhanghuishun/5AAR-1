using Syrus.Plugins.DFV2Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARConversationController : MonoBehaviour
{
    public TextMeshProUGUI CAText;
    public TextButton textButtonScript;
    public Button[] CAButtons;
    public GameObject subscriptionPopup;
    public GameObject stopButtonPopup;
    public GameObject ticketMachinePopup;
    private ArrowNavigation navigation;
    private LogicFunctions LogicFunctions;
    private GoogleMapAPIQuery GoogleAPIScript;

    private bool hasTicket = true;

    private Container<string> destination = new Container<string>();

    private void Awake()
    {
        ConversationController.Instance.RegisterTextOutputField(CAText);
        navigation = GetComponent<ArrowNavigation>();
        LogicFunctions = GetComponent<LogicFunctions>();
        GoogleAPIScript = GetComponent<GoogleMapAPIQuery>();

        InterfaceMethods.AddMethod("HELP_SUBSCRIPTION", ShowSubscriptionPopup);
        InterfaceMethods.AddMethod("HELP_STOP_BUTTON", ShowStopButtonPopup);
        InterfaceMethods.AddMethod("HELP_TICKET_MACHINE", ShowTicketMachinePopup);
        InterfaceMethods.AddMethod("FIND_TABACCHI_SHOP", FindTabacchi);
        InterfaceMethods.AddMethod("FIND_ANOTHER_TABACCHI_SHOP", FindAnotherTabacchi);
        InterfaceMethods.AddMethod("CHECK_TICKET", () => LogicFunctions.TicketRecognitionLogic());
        InterfaceMethods.AddMethod("FIND_BUS_STOP", FindBusStop);
        InterfaceMethods.AddMethod("INSIDE_THE_BUS", InsideTheBusLogic);
        InterfaceMethods.AddMethod("GOT_OFF_THE_BUS", AlreadyGetOffTheBusLogic);
        InterfaceMethods.AddMethod("FINAL_REWARD", FinalActions);
    }

    private void Start()
    {
        Parameters.AddParameter("timeToBus", GoogleAPIScript.minutes.ToString);
        Parameters.AddParameter("busNumber", GoogleAPIScript.busName.ToString);
        Parameters.AddParameter("busArrivalTime", GoogleAPIScript.departureTime.ToString);
        Parameters.AddParameter("destination", destination.ToString);
        Parameters.AddParameter("distance", LogicFunctions.distanceFromLastStop.ToString);

        switch (PhaseController.phase)
        {
            case Phases.BUY_TICKET: BuyTicketLogic(); break;
            case Phases.FIND_BUS_STOP: FindBusStopLogic(); break;
            case Phases.TRAVEL_ON_THE_BUS: TravelOnTheBusLogic(); break;
            default: break;
        }
    }

    private void ShowSubscriptionPopup()
    {
        subscriptionPopup.SetActive(true);
    }

    private void ShowStopButtonPopup()
    {
        stopButtonPopup.SetActive(true);
    }

    private void ShowTicketMachinePopup()
    {
        ticketMachinePopup.SetActive(true);
    }

    private void BuyTicketLogic()
    {
        ConversationController.Instance.SendEventIntent("CheckSubscription");
        //TODO write comments on how the application works with dialogflow
    }

    private void FindTabacchi()
    {
        destination.content = "tabacchi shop";
        navigation.ShowTabacchiNavigation(OnTabacchiReached, true);
    }

    private void OnTabacchiReached()
    {
        ConversationController.Instance.SendEventIntent("TabacchiReached");
        ConversationController.Instance.DoSomethingOnInactivity(60 * 4, () => ConversationController.Instance.SendEventIntent("Inactivity"));
    }

    private void FindAnotherTabacchi()
    {
        destination.content = "tabacchi shop";
        navigation.ShowTabacchiNavigation(OnTabacchiReached, false);
    }

    private void FindBusStopLogic()
    {
        ConversationController.Instance.SendEventIntent("GoToBusStop");
    }

    private void FindBusStop()
    {
        destination.content = "bus stop";
        Debug.Log("FindBusStop called");
        navigation.ShowBusStopNavigation(() => 
            ConversationController.Instance.SendEventIntent("BusStopReached", () => 
               LogicFunctions.AfterArrivingBusStopLogic()));
    }

    private void TravelOnTheBusLogic()
    {
        
    }

    private void InsideTheBusLogic()
    {
        LogicFunctions.cnaTriggerBusToDestination = true;
        LogicFunctions.isInsideBusStopArea = false;
        if (hasTicket)
        {
            ConversationController.Instance.SendEventIntent("OnTheBusTicket");
            ConversationController.Instance.DoSomethingOnInactivity(60 * 2, () => ConversationController.Instance.SendEventIntent("Inactivity"));
        }
        else
        {
            ConversationController.Instance.SendEventIntent("OnTheBusNoTicket");
        }
    }
    private void AlreadyGetOffTheBusLogic()
    {
        if(navigation.isAlreadyReachedDestination())
        {
            ConversationController.Instance.SendEventIntent("FinalDestinationReached");
        }
        else
        {
            ConversationController.Instance.ResetContext(new DF2Context[1] { new DF2Context("Moving", 2) });
            textButtonScript.ChangeOptions(new string[2] {"Where are we going?", "Help" });
            navigation.ShowBusStopNavigation(() => 
            ConversationController.Instance.SendEventIntent("BusStopReached", () => 
               LogicFunctions.AfterArrivingBusStopLogic()));
        }

    }

    private void FinalActions()
    {
        foreach(Button b in CAButtons)
        {
            b.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
