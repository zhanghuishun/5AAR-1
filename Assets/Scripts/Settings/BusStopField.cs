using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BusStopField : MonoBehaviour
{
    private SerchableDropdown serchableDropdown;

    private List<StopInfo> stopInfos = null;

    private void Awake()
    {
        serchableDropdown = GetComponent<SerchableDropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetBusData());
    }

    private IEnumerator GetBusData()
    {
        Debug.Log(BusData.instance.stopsInfo);
        yield return new WaitUntil(() => BusData.instance.stopsInfoPresent);
        stopInfos = BusData.instance.stopsInfo;

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (StopInfo s in stopInfos)
            options.Add(new TMP_Dropdown.OptionData(s.name));
        serchableDropdown.UpdateDefaultOptions(options);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
