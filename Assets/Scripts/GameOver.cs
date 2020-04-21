using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("Sounds")]
    public GameObject clickSound;

    public void Restart() {
        Instantiate(clickSound);
        SceneManager.LoadScene(1);
    }

    public void Menu() {
        Instantiate(clickSound);
        SceneManager.LoadScene(0);
    }
}
