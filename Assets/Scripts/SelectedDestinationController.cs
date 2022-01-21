using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class SelectedDestinationController : MonoBehaviour
{
    public GameObject selecdetDestinationLabel;
    public TMP_Text selectedDestination;
    public GameObject popup;
    public GameObject loadingPopup;

    private bool waitingForSelectedStop;
    private bool selectedStopPresent;

    // Start is called before the first frame update
    void Start()
    {
        selectedStopPresent = false;
        waitingForSelectedStop = false;
        if (SettingsData.selectedStopSet)
        {
            SetText();
        }
        else
        {
            selecdetDestinationLabel.SetActive(false);
            selectedDestination.gameObject.SetActive(false);
            StartCoroutine(DelayedSetText());
        }
    }

    private IEnumerator DelayedSetText()
    {
        waitingForSelectedStop = true;
        yield return new WaitUntil(() => SettingsData.selectedStopSet);
        SetText();
        waitingForSelectedStop = false;
    }

    private void SetText()
    {
        if (SettingsData.selectedStop == null)
        {
            selecdetDestinationLabel.SetActive(false);
            selectedDestination.gameObject.SetActive(false);
            selectedStopPresent = false;
        }
        else
        {
            selecdetDestinationLabel.SetActive(true);
            selectedDestination.gameObject.SetActive(true);
            selectedDestination.text = SettingsData.selectedStop.name;
            selectedStopPresent = true;
        }
    }

    public void LoadARScene(int phase)
    {
        if (waitingForSelectedStop)
        {
            StartCoroutine(WaitAndLoadARScene(phase));
        }
        else
        {
            _LoadARScene(phase);
        }
    }

    public void _LoadARScene(int phase)
    {
        if (selectedStopPresent)
        {
            PhaseController.phase = (Phases)phase;
            SceneManager.LoadScene("ARScene");
        }
        else
        {
            popup.SetActive(true);
        }
    }

    private IEnumerator WaitAndLoadARScene(int phase)
    {
        loadingPopup.SetActive(true);
        yield return new WaitUntil(() => !waitingForSelectedStop);
        loadingPopup.SetActive(false);
        _LoadARScene(phase);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
