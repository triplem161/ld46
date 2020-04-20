using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCE_Menu : MonoBehaviour
{
    public void GoToArcade() {
        Settings.ARCADE_MODE = true;
        SceneManager.LoadScene(1);
    }

    public void GoToTutorial() {
        Settings.ARCADE_MODE = false;
        SceneManager.LoadScene(1);
    }

    public void Quit() {
        Application.Quit();
    }
}
