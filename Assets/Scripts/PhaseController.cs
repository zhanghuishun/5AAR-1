using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    public static Phases phase = Phases.BUY_TICKET;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum Phases
{
    BUY_TICKET = 0,
    FIND_BUS_STOP = 1,
    TRAVEL_ON_THE_BUS = 2
}
