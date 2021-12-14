using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    private GameObject confirmationWindow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBackButtonPressed(GameObject confirmationWindow)
    {
        this.confirmationWindow = confirmationWindow;
        confirmationWindow.SetActive(true);
    }

    public void OnQuit(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnStay()
    {
        confirmationWindow.SetActive(false);
    }
}
