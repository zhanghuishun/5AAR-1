using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimeDisplay : MonoBehaviour
{
    public GameObject timeDisplay;
    // Start is called before the first frame update
    void Start()
    {
        timeDisplay.GetComponent<TextMeshProUGUI>().text = System.DateTime.Now.ToString("HH:mm");;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}