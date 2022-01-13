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
    public void ShowNavigationInformation(Phases phase, Action callback){
        StartCoroutine(StepsInformation(phase));
        afterDestinationCallback = callback;
    }
    IEnumerator StepsInformation(Phases phase)
    {
        if(phase == Phases.BUY_TICKET)
        {
            yield return StartCoroutine(GoogleAPIScript.TabacchiInOrder());
        }
        else if(phase == Phases.FIND_BUS_STOP) 
        {
            float destLat = float.Parse(InputFieldSubmit.destinationCoordinates[0], CultureInfo.InvariantCulture);
            float destLng = float.Parse(InputFieldSubmit.destinationCoordinates[1], CultureInfo.InvariantCulture);
            //Debug.Log(destLat.ToString()+destLng.ToString());
            yield return StartCoroutine (GoogleAPIScript.GetBusRouteJSON (destLat, destLng));//45.5168268f, 9.2166683f
        }
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
        if (isCollide())
        {
            //show checkpoint firework
            count++;
            panel.SetActive(false);
            compass.SetActive(false);
            Firework.Instance.Explosion(ARCamera.transform.position + ARCamera.transform.forward * forwardOffset);
            ConversationController.Instance.ChangeTextFields("you are arriving the checkpoint " + count + "/" + steps.Count);
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

    bool isCollide() {
		lat = Input.location.lastData.latitude;
		lng = Input.location.lastData.longitude;
        //collide within 10m
		if (lat - destLat <= 0.0001f && lat - destLat >= -0.0001f) {
			if (lng - destLng <= 0.0001f && lng - destLng >= -0.0001f) {
				return true;
			}
		}

		return false;
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