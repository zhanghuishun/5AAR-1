using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.Networking;

public class BusData : MonoBehaviour
{
    public static BusData instance = null;

    private string stopsTableID = "2a52d51d-66fe-480b-a101-983aa2f6cbc3";
    private string stopsTableQueryParameter = "";//" WHERE id_amat=10001";

    private string routesTableID = "d4408a03-ef86-40df-a4a9-e738d05e03e4";
    private string routesTableQueryParameter = " WHERE mezzo LIKE 'BUS'";

    private string routesCompositionID = "d4530625-dd71-4e8c-8eb4-bf4e26f2500e";
    private string routesCompositionQueryParameter = "";

    public List<StopInfo> stopsInfo;
    public bool stopsInfoPresent = false;

    public List<RouteInfo> routesInfo = null;
    public bool routesInfoPresent = false;

    public List<RouteComposition> routesComposition = null;
    public bool routesCompositionPresent = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(GetStopsInfo());
        StartCoroutine(GetRoutesInfo());
        StartCoroutine(GetRoutesComposition());
    }

    private IEnumerator GetStopsInfo()
    {
        UnityWebRequest www = new UnityWebRequest("https://dati.comune.milano.it/api/3/action/datastore_search_sql?sql=SELECT * from \"" + stopsTableID + "\"" + stopsTableQueryParameter);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error fetching the bus stops");
            Debug.Log(www.downloadHandler.data);
        }
        else
        {
            string stringResult = Encoding.UTF8.GetString(www.downloadHandler.data);
            CKAN_SQLQueryResult_StopInfo queryResult = JsonUtility.FromJson<CKAN_SQLQueryResult_StopInfo>(stringResult);

            yield return new WaitUntil(() => routesInfoPresent);

            stopsInfo = queryResult.result.records.FindAll(s =>
            {
                foreach (RouteInfo r in routesInfo)
                {
                    if (s.lines.Contains(r.line)) return true;
                }
                return false;
            });
            stopsInfoPresent = true;
        }
    }

    private IEnumerator GetRoutesInfo()
    {
        UnityWebRequest www = new UnityWebRequest("https://dati.comune.milano.it/api/3/action/datastore_search_sql?sql=SELECT * from \"" + routesTableID + "\"" + routesTableQueryParameter);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error fetching the bus stops");
            Debug.Log(www.downloadHandler.data);
        }
        else
        {
            string stringResult = Encoding.UTF8.GetString(www.downloadHandler.data);
            CKAN_SQLQueryResult_RouteInfo queryResult = JsonUtility.FromJson<CKAN_SQLQueryResult_RouteInfo>(stringResult);

            routesInfo = queryResult.result.records;
            routesInfoPresent = true;
        }
    }

    private IEnumerator GetRoutesComposition()
    {
        UnityWebRequest www = new UnityWebRequest("https://dati.comune.milano.it/api/3/action/datastore_search_sql?sql=SELECT * from \"" + routesCompositionID + "\"" + routesCompositionQueryParameter);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error fetching the bus stops");
            Debug.Log(Encoding.UTF8.GetString(www.downloadHandler.data));
        }
        else
        {
            string stringResult = Encoding.UTF8.GetString(www.downloadHandler.data);
            CKAN_SQLQueryResult_RouteComposition queryResult = JsonUtility.FromJson<CKAN_SQLQueryResult_RouteComposition>(stringResult);

            routesComposition = queryResult.result.records;
            routesCompositionPresent = true;
        }
    }

    public List<StopInfo> GetStopsWithSameLines(string stopName)
    {
        if (!stopsInfoPresent) return null;
        if (!routesCompositionPresent) return null;

        StopInfo originalStop = stopsInfo.Find(s => s.name.Equals(stopName));

        List<RouteComposition> routes = routesComposition.FindAll(rc => rc.stopId.Equals(originalStop.id));

        List<RouteComposition> stopsIdsRoutes = new List<RouteComposition>();
        foreach (RouteComposition rc in routes)
        {
            stopsIdsRoutes.AddRange(routesComposition.FindAll(x => x.routeId.Equals(rc.routeId) && x.ordinalPosition>rc.ordinalPosition));
        }

        List<string> stopsId = new List<string>(stopsIdsRoutes.Count);
        foreach(RouteComposition rc in stopsIdsRoutes)
        {
            stopsId.Add(rc.stopId);
        }

        return stopsInfo.FindAll(x => stopsId.Contains(x.id));
    }

    public StopInfo GetStopsFromName(string stopName)
    {
        if (!stopsInfoPresent) return null;
        
        return stopsInfo.Find(s => s.name.Equals(stopName));
    }
}

[Serializable]
public class CKAN_SQLQueryResult_RouteComposition
{
    public string help;
    public bool success;
    public CKAN_result_RouteComposition result;
}

[Serializable]
public class CKAN_result_RouteComposition
{
    public List<RouteComposition> records;
    public CKAN_field[] fields;
    public string sql;
}

[Serializable]
public class RouteComposition
{
    public string _id;

    [SerializeField] private string percorso;
    public string routeId { get { return percorso; } }

    [SerializeField] private int num;
    public int ordinalPosition { get { return num; } }

    [SerializeField] private string id_ferm;
    public string stopId { get { return id_ferm; } }
}

[Serializable]
public class CKAN_SQLQueryResult_RouteInfo
{
    public string help;
    public bool success;
    public CKAN_result_RouteInfo result;
}

[Serializable]
public class CKAN_result_RouteInfo
{
    public List<RouteInfo> records;
    public CKAN_field[] fields;
    public string sql;
}

[Serializable]
public class RouteInfo
{
    public string _id;

    [SerializeField] private string linea;
    public string line { get { return linea; } }

    [SerializeField] private string mezzo;
    public string mean { get { return mezzo; } }

    [SerializeField] private string percorso;
    public string routeId { get { return percorso; } }

    [SerializeField] private string verso;
    public string direction { get { return verso; } }

    [SerializeField] private string nome;
    public string name { get { return nome; } }

    [SerializeField] private string tipo_perc;
    public string routeType { get { return tipo_perc; } }

    [SerializeField] private string lung_km;
    public string kmLength { get { return lung_km; } }

    [SerializeField] private string num_ferm;
    public string numberOfStops { get { return num_ferm; } }

    [SerializeField] private float LONG_X_4326;
    public float lon { get { return LONG_X_4326; } }

    [SerializeField] private float LAT_Y_4326;
    public float lat { get { return LAT_Y_4326; } }

    [SerializeField] private string Location;
    public string location { get { return Location; } }
}

[Serializable]
public class CKAN_SQLQueryResult_StopInfo
{
    public string help;
    public bool success;
    public CKAN_result_StopInfo result;
}

[Serializable]
public class CKAN_result_StopInfo
{
    public List<StopInfo> records;
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
