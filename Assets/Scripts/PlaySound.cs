using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [Header("Sounds")]
    public GameObject wooshSound;

    public void Play() {
        Instantiate(wooshSound);
    }
}
