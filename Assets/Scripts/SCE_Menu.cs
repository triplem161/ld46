using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCE_Menu : MonoBehaviour
{
    [Header("Sounds")]
    public GameObject clickSound;

    public void GoToArcade() {
        Instantiate(clickSound);
        Settings.ARCADE_MODE = true;
        SceneManager.LoadScene(1);
    }

    public void GoToTutorial() {
        Instantiate(clickSound);
        Settings.ARCADE_MODE = false;
        SceneManager.LoadScene(1);
    }

    public void Quit() {
        Instantiate(clickSound);
        Application.Quit();
    }
}
