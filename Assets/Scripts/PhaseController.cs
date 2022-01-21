using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    public static Phases phase = Phases.BUY_TICKET;

    private static bool thisExists = false;

    private void Awake()
    {
        if (!thisExists)
        {
            DontDestroyOnLoad(gameObject);
            thisExists = true;
        }
        else
            Destroy(gameObject);
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
}
