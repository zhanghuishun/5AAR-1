using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class LogicFunctions : MonoBehaviour
{
    transitDetails busInformation;
    private GPSLocation GPSInstance;
    private GoogleMapAPIQuery GoogleAPIScript;
    private Utils utils;
    private int destDistance;
    float destLat;
    float destLng;
    private bool canTriggerBusIsArriving = false;
    public bool cnaTriggerBusToDestination = false;
    public bool  isInsideBusStopArea = false;
    [SerializeField] private TextMeshProUGUI Instruction;
    [SerializeField] private GameObject box_busticket;
    [SerializeField] private GameObject check_busticket;
    [SerializeField] private GameObject checked_sign;
    [SerializeField] private GameObject ImageRecognition;
    public bool ticketChecked = false;
    private bool isFarAwayFirstTime = true;
    private ArrowNavigation navigation;
    private DateTime oldTime;

    private Container<int> _distanceFromLastStop = new Container<int>();
    public Container<int> distanceFromLastStop
    {
        get
        {
            _distanceFromLastStop.content = Mathf.RoundToInt(utils.CalculateDistanceMeters(
                GPSInstance.lat, GPSInstance.lng, GoogleAPIScript.endStopLat, GoogleAPIScript.endStopLng));
            ; return _distanceFromLastStop;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GoogleAPIScript = GetComponent<GoogleMapAPIQuery>();
        busInformation = GoogleAPIScript.busInformation;
        GPSInstance = GPSLocation.Instance;
        utils = Utils.Instance;
        ConversationController.Instance.RegisterTextOutputField(Instruction);
        navigation = GetComponent<ArrowNavigation>();
    }
    public void AfterArrivingBusStopLogic(){
        StartCoroutine(AfterArrivingBusStop());
        
    }
    private IEnumerator AfterArrivingBusStop(){
        int maxWait = 3;
        while(GoogleAPIScript.walkingSteps.Count == 0 && maxWait > 0){
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if(maxWait <= 0) yield return 0;
        //check if user is near bus station
        //Debug.Log("stationInfo"+ JsonUtility.ToJson(busInformation, true));
        
        canTriggerBusIsArriving = false;
        isInsideBusStopArea = false;
        int stopDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, GoogleAPIScript.startStopLat, GoogleAPIScript.startStopLng));
        Debug.Log("distance of user to the bus station"+stopDistance);
        if(stopDistance <= 20){
            isOnBusStop();
        }
        else{
            LostWhenFindingBusStop();
        }
    }
    private void isOnBusStop()
    {
        //CA: The bus is arrving in XX time, 
        //Debug.Log("the bus is arriving in " +minutes+ "min");
        canTriggerBusIsArriving = true;
        isInsideBusStopArea = true;

        //wait
        //CA: The bus is arrving in less one min, tell me when you are on the bus
        
        //wait for reply then OnTheBus()
    }

    private void LostWhenFindingBusStop()
    {
        //TODO: send the "LostWhenFindingBusStop" event intent to find the destination again
        ConversationController.Instance.SendEventIntent("UserMovedAway" , () =>
            navigation.ShowBusStopNavigation(() => 
                ConversationController.Instance.SendEventIntent("BusStopReached", () => 
                    AfterArrivingBusStopLogic()
                )
            )
        );
            
        Debug.Log("get too far from bus station");
    }

    //called by diagflow TabacchiReached event
    public void TicketRecognitionLogic()
    {
        //active the recognition script and box
        ImageRecognition.SetActive(true);//automatic call image event manager
        check_busticket.SetActive(true);
        checked_sign.SetActive(false);
        //box_busticket.SetActive(true);
    }
    private IEnumerator WaitForSecondsAndDisableBox(int seconds){
        yield return new WaitForSeconds(seconds);
        ConversationController.Instance.SendEventIntent("TicketRecognized" , disableBusBox);
    }

    private void disableBusBox()
    {
        ImageRecognition.SetActive(false);
        checked_sign.SetActive(false);
        check_busticket.SetActive(false);
        box_busticket.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInsideBusStopArea == true)
        {
            int stopDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, GoogleAPIScript.startStopLat, GoogleAPIScript.startStopLng));
            Debug.Log("distance to the current bus stop"+stopDistance);
            if (stopDistance > 30 && isFarAwayFirstTime)
            {   
                //start timer
                Debug.Log("start timer");
                oldTime = DateTime.Now;
                isFarAwayFirstTime = false;
                
            }
            else if(stopDistance > 30 && !isFarAwayFirstTime)
            {
                Debug.Log("second time");
                if(DateTime.Now.Subtract(oldTime).Seconds > 10)
                {
                    Debug.Log("more than 10s");
                    isInsideBusStopArea = false;
                    canTriggerBusIsArriving = false;
                    LostWhenFindingBusStop();
                }
            }
            else
            {
                Debug.Log("less than 30m");
                //once less than 30m, reset timer
                oldTime = DateTime.Now;
                isFarAwayFirstTime = true;
            }
        }

        if (GoogleAPIScript.minutes.content <= 2 && canTriggerBusIsArriving == true)
        {
            ConversationController.Instance.SendEventIntent("BusArriving");
            canTriggerBusIsArriving = false;
        }

        if (cnaTriggerBusToDestination == true)
        {
            Debug.Log("distance to next destination stop"+ distanceFromLastStop.content); 
            if (distanceFromLastStop.content < 200)
            {
                ConversationController.Instance.SendEventIntent("LastStopAproaching");
                cnaTriggerBusToDestination = false;
            }
        }
        if (ticketChecked == true)
        {
            ticketChecked = false;
            checked_sign.SetActive(true);
            //TODO: wait for seconds for user
            StartCoroutine(WaitForSecondsAndDisableBox(1));
            ticketChecked = false;
        }
    }
}
