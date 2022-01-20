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
    float stopLat;
    float stopLng;
    private bool canTriggerBusIsArriving = false;
    private bool cnaTriggerBusToDestination = false;
    private bool  isInsideBusStopArea = false;
    private DateTime datevalue1;
    private DateTime datevalue2;
    [SerializeField] private TextMeshProUGUI Instruction;
    [SerializeField] private GameObject box_busticket;
    [SerializeField] private GameObject check_busticket;
    [SerializeField] private GameObject ImageRecognition;
    public bool ticketChecked = false;
    public Container<string> busName = new Container<string>();
    private Container<int> _minutes = new Container<int>();
    public Container<int> minutes { get { _minutes.content = Mathf.RoundToInt((float)(datevalue2 - datevalue1).TotalMinutes); ; return _minutes; } }
    public Container<string> departureTime = new Container<string>();
    // Start is called before the first frame update
    void Start()
    {
        GoogleAPIScript = GetComponent<GoogleMapAPIQuery>();
        busInformation = GoogleAPIScript.busInformation;
        GPSInstance = GPSLocation.Instance;
        utils = Utils.Instance;
        ConversationController.Instance.RegisterTextOutputField(Instruction);
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
        stopLat = busInformation.departure_stop.location.lat;
        stopLng = busInformation.departure_stop.location.lng;
        int stopDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, stopLat, stopLng));
        Debug.Log("distance of user to the bus station"+stopDistance);
        if(stopDistance <= 20){
            isOnBusStop();
            isInsideBusStopArea = true;
        }
        else{
            LostWhenFindingBusStop();
        }
    }
    private void isOnBusStop()
    {
        //CA: The bus is arrving in XX time, 
        datevalue1 = DateTime.Now;
        datevalue2 = utils.TimeParse(busInformation.departure_time.text);
        busName.content = busInformation.line.short_name;
        //Debug.Log("current time:" + DateTime.Now);
        departureTime.content = busInformation.departure_time.text;
        if(minutes.content > 1) ConversationController.Instance.ChangeTextFields("the bus " +"is arriving in " + "min at ");
        //Debug.Log("the bus is arriving in " +minutes+ "min");
        canTriggerBusIsArriving = true;
        //wait
        //CA: The bus is arrving in less one min, tell me when you are on the bus
        
        //wait for reply then OnTheBus()
    }

    private void LostWhenFindingBusStop()
    {
        //TODO: send the event intent to find the destination again
        Debug.Log("get too far from bus station");
    }
    private void OnTheBus()
    {
        //validate the ticket
        //answer his potential questions
        //remind the user is arrving the destination
        cnaTriggerBusToDestination = true;
        
    }
    //called by diagflow TabacchiReached event
    public void TicketRecognitionLogic()
    {
        //active the recognition script and box
        ImageRecognition.SetActive(true);//automatic call image event manager
        box_busticket.SetActive(true);
    }
    private IEnumerator WaitForSecondsAndDisableBox(int seconds){
        yield return new WaitForSeconds(seconds);
        ConversationController.Instance.SendEventIntent("TicketRecognized" , disableBusBox);
    }

    private void disableBusBox()
    {
        ImageRecognition.SetActive(false);
        check_busticket.SetActive(false);
        box_busticket.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isInsideBusStopArea == true) {
            stopLat = busInformation.departure_stop.location.lat;
            stopLng = busInformation.departure_stop.location.lng;
            int stopDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, stopLat, stopLng));
            if(stopDistance > 20){
                isInsideBusStopArea = false;
                canTriggerBusIsArriving = false;
                LostWhenFindingBusStop();
            }
        }
        if(canTriggerBusIsArriving == true){
            if ((datevalue2 - DateTime.Now).TotalMinutes < 1){
                ConversationController.Instance.ChangeTextFields("The bus is arrving in less one min, tell me when you are on the bus");
                canTriggerBusIsArriving = false;
            }
        }
        if(cnaTriggerBusToDestination == true){
            destLat = float.Parse(InputFieldSubmit.destinationCoordinates[0]);
            destLng = float.Parse(InputFieldSubmit.destinationCoordinates[1]);
            destDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, destLat, destLng));
            if(destDistance < 500){
                ConversationController.Instance.SendEventIntent("LastStopAproaching");
                cnaTriggerBusToDestination = false;
            }
        }
        if(ticketChecked == true)
        {
            check_busticket.SetActive(true);
            //TODO: wait for seconds for user
            StartCoroutine(WaitForSecondsAndDisableBox(1));
            ticketChecked = false;
        }
    }
}
