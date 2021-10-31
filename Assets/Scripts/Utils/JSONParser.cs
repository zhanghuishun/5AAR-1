using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class JSONParser : MonoBehaviour
{
    //TODO: decide what result to show or to use, for now just get the name of first result
    public static string MapAPIQueryParser(string json){
        JObject o = JObject.Parse(json);
        string name = o["results"][0]["name"].ToString();
        return name;
    }
}
