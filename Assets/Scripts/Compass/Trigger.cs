using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject ARCompass;
    public UnityARCompass.ARCompassIOS ARCompassIOS;
    GameObject instance;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void onClickTrigger(){
        Debug.Log("11111111");
        ARCompass.SetActive(true);
        ARCompassIOS.startLat = 45.521592f;
        ARCompassIOS.startLon = 9.226934f;
        ARCompassIOS.endLat = 45.522870f;
        ARCompassIOS.endLon = 9.223684f;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
