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
    private string stopsTableID = "2a52d51d-66fe-480b-a101-983aa2f6cbc3";
    private string queryParameter = "";//" WHERE id_amat=10001";

    private StopInfo[] stopInfos = null;

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
        UnityWebRequest www = new UnityWebRequest("https://dati.comune.milano.it/api/3/action/datastore_search_sql?sql=SELECT * from \"" + stopsTableID + "\"" + queryParameter);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error fetching the bus stops");
            Debug.Log(www.downloadHandler.data);
        }
        else
        {
            string stringResult = Encoding.UTF8.GetString(www.downloadHandler.data);
            CKAN_SQLQueryResult queryResult = JsonUtility.FromJson<CKAN_SQLQueryResult>(stringResult);

            stopInfos = queryResult.result.records;

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (StopInfo s in stopInfos)
                options.Add(new TMP_Dropdown.OptionData(s.name));
            serchableDropdown.UpdateDefaultOptions(options);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class CKAN_SQLQueryResult
{
    public string help;
    public bool success;
    public CKAN_result result;
}

[Serializable]
public class CKAN_result
{
    public StopInfo[] records;
    public CKAN_field[] fields;
    public string sql;
}

[Serializable]
public class StopInfo
{
    [SerializeField] private string ubicazione;
    public string name { get { return ubicazione; } }

    [SerializeField] private float LONG_X_4326;
    public float lon { get { return LONG_X_4326; } }

    [SerializeField] private string id_amat;
    public string id { get { return id_amat; } }

    [SerializeField] private string _full_text;
    public string fullText { get { return _full_text; } }

    [SerializeField] private float LAT_Y_4326;
    public float lat { get { return LAT_Y_4326; } }

    [SerializeField] private string Location;
    public string location { get { return Location; } }

    [SerializeField] private string linee;
    public string lines { get { return linee; } }

    public string _id;
}

[Serializable]
public class CKAN_field
{
    public string type;
    public string id;
}
