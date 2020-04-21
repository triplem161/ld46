using UnityEngine;
using System.Collections;

public class SFX : MonoBehaviour {

    public AudioClip[] Sounds;

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        int rand = Random.RandomRange(0,Sounds.Length);
        GetComponent<AudioSource>().clip = Sounds[rand];
        GetComponent<AudioSource>().Play();

        Destroy(gameObject, GetComponent<AudioSource>().clip.length);
    }
}
