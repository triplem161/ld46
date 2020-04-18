using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideCube : MonoBehaviour { 

    // Start is called before the first frame update
    void Start() {
        foreach(Transform vChild in transform) {
            vChild.GetComponent<Renderer>().material.SetVector("_random_speed_seed", new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
            vChild.GetComponent<Renderer>().material.SetVector("_random_emission_seed", new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        }
    }


}
