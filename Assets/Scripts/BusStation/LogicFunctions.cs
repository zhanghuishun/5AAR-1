using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class LogicFunctions : MonoBehaviour
{
    transitDetails busInformation;
    private GPSLocation GPSInstance;
    private GoogleMapAPIQuery GoogleAPIScript;
    private Utils utils;
    private int destDistance;
    [SerializeField] private TextMeshProUGUI Instruction;
    // Start is called before the first frame update
    void Start()
    {
        GoogleAPIScript = GetComponent<GoogleMapAPIQuery>();
        GPSInstance = GPSLocation.Instance;
        utils = Utils.Instance;
        ConversationController.istance.RegisterTextOutputField(Instruction);
    }
    public void LogicWrap(){
        StartCoroutine(Logic());
        
    }
    private IEnumerator Logic(){
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
            OnBusStation();
        }
        else{
            LostWhenFindingStation();
        }
    }
    private void OnBusStation()
    {
        //CA: The bus is arrving in XX time, 
        DateTime datevalue1 = DateTime.Now;
        DateTime datevalue2 = utils.TimeParse(busInformation.departure_time.text);
        //Debug.Log("current time:" + DateTime.Now);
        double minutesDouble = (datevalue2 - datevalue1).TotalMinutes;
        int minutes = Mathf.RoundToInt((float)minutesDouble);
        if(minutes > 1) Debug.Log("the bus is arriving in " +minutes+ "min");

        //wait
        //CA: The bus is arrving in less one min, tell me when you are on the bus
        do{}
        while((datevalue2 - DateTime.Now).TotalMinutes > 1);
        Debug.Log("The bus is arrving in less one min, tell me when you are on the bus");
        
        //wait for reply then OnTheBus()
    }
    private void LostWhenFindingStation()
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
        float destLat = float.Parse(InputFieldSubmit.destinationCoordinates[0]);
        float destLng = float.Parse(InputFieldSubmit.destinationCoordinates[1]);
        do{destDistance = Mathf.RoundToInt(utils.CalculateDistanceMeters(GPSInstance.lat, GPSInstance.lng, destLat, destLng));}
        while(destDistance > 500);
        //if distance < 500m
        Debug.Log("The bus is arrving the destination, please prepare to get off the bus");
    }
    public void OnTabacchiShopLogic()
    {
        ConversationController.istance.ChangeTextFields("Ask the shop keeper to buy the ticket");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
