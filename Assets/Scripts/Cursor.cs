using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

    public WorldGrid grid;
    [Space]
    public Transform cursorX;
    public Transform cursorZ;
    [Space]
    public Color neutralColor;
    public Color attractColor;
    public Color repulseColor;

    private int _gridLayer;

    void Awake() {
        _gridLayer = LayerMask.GetMask("GRID_COLLISION");
    }

    // Start is called before the first frame update
    void Start() {
        cursorX.localScale = new Vector3(1, grid.gridHeight, 1);
        cursorZ.localScale = new Vector3(grid.gridWidth, 1, 1);
    }

    // Update is called once per frame
    void Update() {
        var vRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit vHit = new RaycastHit();
        if (Physics.Raycast(vRay, out vHit, Mathf.Infinity, _gridLayer)) {
            (int x, int y) vCoord = grid.PositionToCoord(vHit.point);
            Vector3 vLocalPos = grid.CoordToPosition(vCoord.x, vCoord.y) + new Vector3(0.5f, 0, -0.5f);
            cursorX.position = new Vector3(vLocalPos.x, 0.51f, 0);
            cursorZ.position = new Vector3(0,0.51f, vLocalPos.z);

            cursorX.gameObject.SetActive(true);
            cursorZ.gameObject.SetActive(true);
        }
        else {
            cursorX.gameObject.SetActive(false);
            cursorZ.gameObject.SetActive(false);
        }
    }

    public void Attract() {
        StartCoroutine(ChangingColor(attractColor));
    }

    public void Repulse() {
        StartCoroutine(ChangingColor(repulseColor));
    }

    IEnumerator ChangingColor(Color pColor) {
        cursorX.GetComponent<Renderer>().material.SetColor("_BaseColor", pColor);
        cursorZ.GetComponent<Renderer>().material.SetColor("_BaseColor", pColor);

        yield return new WaitForSeconds(0.25f);

        cursorX.GetComponent<Renderer>().material.SetColor("_BaseColor", neutralColor);
        cursorZ.GetComponent<Renderer>().material.SetColor("_BaseColor", neutralColor);
    }
}
