using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAAnimationsController : MonoBehaviour
{
    public static CAAnimationsController istance { private set; get; }

    public GameObject loading;
    public List<Button> interactionButtons;

    private void Awake()
    {
        istance = this;
        loading.SetActive(false);

        foreach (Button button in interactionButtons)
            button.interactable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLoading(bool b)
    {
        loading.SetActive(b);

        foreach (Button button in interactionButtons)
            button.interactable = !b;
    }
}
