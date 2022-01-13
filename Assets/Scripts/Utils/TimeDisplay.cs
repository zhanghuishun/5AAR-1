using UnityEngine;
using TMPro;
public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeDisplay.text = System.DateTime.Now.ToString("HH:mm");
    }

}