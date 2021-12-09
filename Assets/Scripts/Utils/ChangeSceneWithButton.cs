using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeSceneWithButton : MonoBehaviour {
 
    public void LoadScene(string nameScene) {
        SceneManager.LoadScene(nameScene);
 
    }

    public void LoadARScene(int phase)
    {
        PhaseController.phase = (Phases)phase;
        SceneManager.LoadScene("ARScene");
    }
 
}