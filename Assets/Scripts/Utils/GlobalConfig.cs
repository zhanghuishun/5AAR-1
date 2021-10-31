using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System.Xml;

public class GlobalConfig : MonoBehaviour
{
    public TextAsset xmlRawFile;
    public static string GoogleMapAPIKey;
    void Start()
    {
        string data = xmlRawFile.text;
        parseXmlFile(data);
    }
    void parseXmlFile(string xmlData)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlData));
        
        //set google map api key
        string xmlPathPattern = "//keys/GoogleMapAPIKey";
        XmlNodeList nodelist = xmlDoc.SelectNodes(xmlPathPattern);
        XmlNode node = nodelist[0];
        GoogleMapAPIKey = node.InnerXml;
        Debug.Log(GoogleMapAPIKey);
    }
    
    
}
    // private void OnGUI()
    // {
    //     GUILayout.Label(dic["Login"]["Password"]);
    // }