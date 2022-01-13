using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class InputFieldSubmit : MonoBehaviour
{
    //dest start from home:"45.516826", "9.216668"
    public static string[] destinationCoordinates = new string[2] {"45.4827681", "9.2322856"}; // lambrate station,
    public static string[] tabacchiCoordinates = new string[2] {"45.480759", "9.224494"}; //tabacchi near BCL
    public TMP_InputField destinationCord;
    public TMP_InputField tabacchiCord;

    public void Awake()
    {
        Debug.Log("print "+destinationCord.placeholder.GetComponent<TextMeshProUGUI>().text);
        destinationCord.placeholder.GetComponent<TextMeshProUGUI>().text = destinationCoordinates[0]+","+destinationCoordinates[1];
        tabacchiCord.placeholder.GetComponent<TextMeshProUGUI>().text = tabacchiCoordinates[0]+","+tabacchiCoordinates[1];
        DontDestroyOnLoad(transform.gameObject);
    }
    public void LockInput(string[] coordinates, TMP_InputField inputField)
    {
        coordinates = inputField.text.Split(',');
    }
    public void Start()
	{
		destinationCord.onEndEdit.AddListener(delegate{LockInput(destinationCoordinates, destinationCord);});
		tabacchiCord.onEndEdit.AddListener(delegate{LockInput(tabacchiCoordinates, tabacchiCord);});

	}

}