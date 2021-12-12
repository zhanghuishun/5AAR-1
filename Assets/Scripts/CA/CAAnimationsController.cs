using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAAnimationsController : MonoBehaviour
{
    public static CAAnimationsController istance { private set; get; }

    public GameObject loading;

    private void Awake()
    {
        istance = this;
        loading.SetActive(false);
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
    }
}
