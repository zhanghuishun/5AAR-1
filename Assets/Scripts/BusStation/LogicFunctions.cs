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
    private bool cnaTriggerBusToDestination = false;
    private DateTime datevalue1;
    private DateTime datevalue2;
    [SerializeField] private TextMeshProUGUI Instruction;
    // Start is called before the first frame update
    void Start()
    {
        GoogleAPIScript = GetComponent<GoogleMapAPIQuery>();
        GPSInstance = GPSLocation.Instance;
        utils = Utils.Instance;
        ConversationController.istance.RegisterTextOutputField(Instruction);
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
        busInformation = GoogleAPIScript.busInformation;
        //check if user is near bus station
        //Debug.Log("stationInfo"+ JsonUtility.ToJson(busInformation, true));
        float stopLat = busInformation.departure_stop.location.lat;
        float stopLng = busInformation.departure_stop.location.lng;
        int stopDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, stopLat, stopLng));
        Debug.Log("distance of user to the bus station"+stopDistance);
        if(stopDistance <= 15){
            isOnBusStop();
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
        //Debug.Log("current time:" + DateTime.Now);
        double minutesDouble = (datevalue2 - datevalue1).TotalMinutes;
        int minutes = Mathf.RoundToInt((float)minutesDouble);
        if(minutes > 1) ConversationController.istance.ChangeTextFields("the bus is arriving in " +minutes+ "min");
        //Debug.Log("the bus is arriving in " +minutes+ "min");
        canTriggerBusIsArriving = true;
        //wait
        //CA: The bus is arrving in less one min, tell me when you are on the bus
        
        //wait for reply then OnTheBus()
    }
    private void LostWhenFindingBusStop()
    {
        //confort
        //relocate        
        Debug.Log("get too far from bus station");
    }
    private void OnTheBus()
    {
        //validate the ticket
        //answer his potential questions
        //remind the user is arrving the destination
        cnaTriggerBusToDestination = true;
        
    }
    public void OnTabacchiShopLogic()
    {
        ConversationController.istance.ChangeTextFields("Ask the shop keeper to buy the ticket, tell me when you get the ticket");
    }
    public void LoadScene(Phases phase){
        PhaseController.phase = phase;
        SceneManager.LoadScene("ARScene");
    }
    // Update is called once per frame
    void Update()
    {
        if(canTriggerBusIsArriving == true){
            if ((datevalue2 - DateTime.Now).TotalMinutes < 1){
                ConversationController.istance.ChangeTextFields("The bus is arrving in less one min, tell me when you are on the bus");
                canTriggerBusIsArriving = false;
            }
        }
        if(cnaTriggerBusToDestination == true){
            destLat = float.Parse(InputFieldSubmit.destinationCoordinates[0]);
            destLng = float.Parse(InputFieldSubmit.destinationCoordinates[1]);
            destDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, destLat, destLng));
            if(destDistance < 500){
                ConversationController.istance.ChangeTextFields("The bus is arrving the destination, please prepare to get off the bus");
                cnaTriggerBusToDestination = false;
            }
        }
    }
}
