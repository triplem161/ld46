using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCE_main : MonoBehaviour {

	public WorldGrid grid;

	private int _gridLayer;

	void Awake() {
		_gridLayer = LayerMask.GetMask("GRID_COLLISION");
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			var vRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit vHit = new RaycastHit();
			if (Physics.Raycast(vRay, out vHit, Mathf.Infinity, _gridLayer)) {
				grid.PullMagnets(vHit.point);
			}
		}
	}
}
