using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitialConversationController : MonoBehaviour
{
    public TextMeshProUGUI CAText;

    // Start is called before the first frame update
    void Start()
    {
        //Do I need to remove it?
        ConversationController.RegisterTextOutputField(CAText);

        ConversationController.SendEventIntent("Introduction", new Dictionary<string, object>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
