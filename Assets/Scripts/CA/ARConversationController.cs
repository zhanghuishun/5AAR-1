using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARConversationController : MonoBehaviour
{
    public TextMeshProUGUI CAText;
    // Start is called before the first frame update
    void Start()
    {
        ConversationController.RegisterTextOutputField(CAText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
