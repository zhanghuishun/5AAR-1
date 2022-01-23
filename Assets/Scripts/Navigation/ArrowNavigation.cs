using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Globalization;

public class ArrowNavigation : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject safetyAlert;
    private GPSLocation GPSInstance;
    private GoogleMapAPIQuery GoogleAPIScript;
    private Utils utils;
    private Action afterDestinationCallback = null;
    [SerializeField] private GameObject CompassPerfab;
    [SerializeField] private UnityARCompass.ARCompassIOS ARCompassIOS;
    [SerializeField] private TextMeshProUGUI Instruction;
    [SerializeField] private Firework Firework;
    private bool CheckponintArrived = false;
    float lat;
    float lng;
	float destLat;
	float destLng;
	int count;
	List<step> steps;
    float brng;
    float compassBrng;
    TextMeshProUGUI textMeshProUGUI;

    
    //compass related 
    GameObject compass;
    public GameObject ARCamera;
    private int forwardOffset = 1;
    private int upOffset = 1;
    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        GoogleAPIScript = GetComponent<GoogleMapAPIQuery>();
        ConversationController.Instance.RegisterTextOutputField(Instruction);
        utils = Utils.Instance;
        GPSInstance = GPSLocation.Instance;
    }
    public void ShowTabacchiNavigation(Action callback, bool isFirstTabacchi){
        StartCoroutine(TabacchiStepsInformation(isFirstTabacchi));
        afterDestinationCallback = callback;
    }
    public void ShowBusStopNavigation(Action callback){
        StartCoroutine(BusStopStepsInformation());
        afterDestinationCallback = callback;
    }
    IEnumerator TabacchiStepsInformation(bool isFirstTabacchi)
    {
        yield return StartCoroutine(GoogleAPIScript.TabacchiInOrder(isFirstTabacchi));
        yield return new WaitForSecondsRealtime(1);
        yield return StartCoroutine(ClickToGetStepsInformation());

    }
    IEnumerator BusStopStepsInformation()
    {
        
        float destLat = float.Parse(SettingsData.destinationCoordinates[0], CultureInfo.InvariantCulture);
        float destLng = float.Parse(SettingsData.destinationCoordinates[1], CultureInfo.InvariantCulture);
        //Debug.Log(destLat.ToString()+destLng.ToString());
        yield return StartCoroutine (GoogleAPIScript.GetBusRouteJSON (destLat, destLng));//45.5168268f, 9.2166683f
        yield return new WaitForSecondsRealtime(1);
        yield return StartCoroutine(ClickToGetStepsInformation());
    }
    IEnumerator ClickToGetStepsInformation()
    {
        //show safety alert
        safetyAlert.SetActive(true);
        int maxWait = 3;
        while(GoogleAPIScript.walkingSteps.Count == 0 && maxWait > 0){
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if(maxWait <= 0) yield return 0;
        steps = GoogleAPIScript.walkingSteps;
        //instantiate prefab
        compass = Instantiate(CompassPerfab) as GameObject;
        //panel = Instantiate(PanelPrefab) as GameObject;
        panel.SetActive(true);
        textMeshProUGUI = panel.GetComponent<TextMeshProUGUI>();
        Debug.Log(textMeshProUGUI.text);
        Debug.Log(JsonUtility.ToJson(steps[0], true));
        Instruction.text = utils.RemoveSpecialCharacters(steps[count].html_instructions);
        textMeshProUGUI.text = "Distance here";
        
        //StepLoop starts in 0.1s and repeating running every 0.5s
        InvokeRepeating("StepsLoop", 0.1f, 0.5f);
    }

    private int oldDistance = -1;
    private int minDistanceReached = int.MaxValue;
    private float timeCounter = 0;
    private bool firstDistanceTresholdReached = false;
    private bool secondDistanceTresholdReached = false;
    public int firstDistanceTreshold = 100;
    public int secondDistanceTreshold = 150;
    private bool firstTimeTresholdReached = false;
    private bool secondTimeTresholdReached = false;
    public int firstTimeTreshold = 4*60;
    public int secondTimeTreshold = 6*60;
    public void StepsLoop()
    {
        lat = GPSInstance.lat;
        lng = GPSInstance.lng;
        destLat = steps[count].end_location.lat;
        destLng = steps[count].end_location.lng;
        //update compass direction
        ARCompassIOS.startLat = lat;
        ARCompassIOS.startLng = lng;
        ARCompassIOS.endLat = destLat;
        ARCompassIOS.endLng = destLng;
        int distance = Mathf.RoundToInt(utils.CalculateDistanceMeters(lat, lng, destLat, destLng));
        // constantly update distance shown
        textMeshProUGUI.text = distance.ToString() + "m";

        if (distance < minDistanceReached)
        {
            firstDistanceTresholdReached = false;
            secondDistanceTresholdReached = false;
            minDistanceReached = distance;
        }
        else if (!firstDistanceTresholdReached && distance > minDistanceReached + firstDistanceTreshold)
        {
            firstDistanceTresholdReached = true;
            ConversationController.Instance.SendEventIntent("MovingWrongDirection");
        }
        else if (firstDistanceTresholdReached && !secondDistanceTresholdReached && distance > minDistanceReached + secondDistanceTreshold)
        {
            secondDistanceTresholdReached = true;
            ConversationController.Instance.SendEventIntent("MovingWrongDirection");
        }

        if (oldDistance != -1)
        {
            if(distance == oldDistance)
            {
                timeCounter += 0.5f;
            }
            else
            {
                timeCounter = 0;
            }
        }
        oldDistance = distance;

        if (!firstTimeTresholdReached && distance > minDistanceReached + firstDistanceTreshold)
        {
            firstTimeTresholdReached = true;
            ConversationController.Instance.SendEventIntent("NotMoving");
        }
        else if (firstTimeTresholdReached && !secondTimeTresholdReached && distance > minDistanceReached + secondDistanceTreshold)
        {
            secondTimeTresholdReached = true;
            ConversationController.Instance.SendEventIntent("NotMoving");
        }

        if (isCollide(0.0001f))
        {
            //show checkpoint firework
            count++;
            panel.SetActive(false);
            compass.SetActive(false);
            Firework.Instance.Explosion(ARCamera.transform.position + ARCamera.transform.forward * forwardOffset);
            //ConversationController.Instance.ChangeTextFields("you are arriving the checkpoint " + count + "/" + steps.Count);
            if (count < steps.Count)
            {
                panel.SetActive(true);
                compass.SetActive(true);
                //Debug.Log(utils.RemoveSpecialCharacters(steps[count].html_instructions));
                ConversationController.Instance.ChangeTextFields(utils.RemoveSpecialCharacters(steps[count].html_instructions));
            }            
        }
        //when arriving the dest, cancel this invokerepeating. 
        if (count == steps.Count)
        {
            Destroy(compass);
            panel.SetActive(false);
            //Destroy(CompassObject);
            if(afterDestinationCallback != null) {afterDestinationCallback.Invoke();};
            CancelInvoke();
        }
				
	}

    bool isCollide(float meters) {
		lat = Input.location.lastData.latitude;
		lng = Input.location.lastData.longitude;
        //collide within 10m
		if (lat - destLat <= meters && lat - destLat >= -1*meters) {
			if (lng - destLng <= meters && lng - destLng >= -1*meters) {
				return true;
			}
		}

		return false;
	}
    public bool isAlreadyReachedDestination()
    {
        float destLat = float.Parse(SettingsData.destinationCoordinates[0], CultureInfo.InvariantCulture);
        float destLng = float.Parse(SettingsData.destinationCoordinates[1], CultureInfo.InvariantCulture);
        //already reached
        Debug.Log(lat+","+lng);
        Debug.Log(destLat+","+destLng);
        if(isCollide(0.0002f)){
            Debug.Log("collide");
            return true;
        }
        else
        {
            Debug.Log("not collide");
            return false;
        }

    }
	// Update is called once per frame
	void Update () {      
        if(compass != null) {
            compass.transform.rotation = ARCompassIOS.TrueHeadingRotation;
            //the position is always on the front of camera with a offset
            compass.transform.position = ARCamera.transform.position + ARCamera.transform.forward * forwardOffset;
            //Debug.Log("compass rotation"+compass.transform.rotation);
            
        }
        
    }
}