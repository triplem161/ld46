using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCE_main : MonoBehaviour {

	public WorldGrid grid;

	private int _gridLayer;

	[Header("Cursor")]
	public Transform cursor;

	[Header("Magnet Spawner")]
	public Magnet[] magnetPrefabs;
	public float magnetSpawnTimer = 3;
	public int maxMagnet = 5;
	private int _magnetsCount = 0;
	private float _magnetSpawnEllapsed;

	void Awake() {
		_gridLayer = LayerMask.GetMask("GRID_COLLISION");
	}

	void Update() {
		Inputs();
		Spawner();
	}

	private void Inputs() {
		var vRayHover = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit vHitHover = new RaycastHit();
		if (Physics.Raycast(vRayHover, out vHitHover, Mathf.Infinity, _gridLayer)) {
			(int x, int y) vCoord = grid.PositionToCoord(vHitHover.point);
			Vector3 vLocalPos = grid.CoordToPosition(vCoord.x, vCoord.y) + new Vector3(0.5f, 0, -0.5f);
			cursor.position = new Vector3(vLocalPos.x, 0.6f, vLocalPos.z);
		} else {
			cursor.position = Vector3.down;
		}

		if (Input.GetMouseButtonDown(0)) {
			var vRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit vHit = new RaycastHit();
			if (Physics.Raycast(vRay, out vHit, Mathf.Infinity, _gridLayer)) {
				grid.PullOrder(vHit.point);
			}
		} else if (Input.GetMouseButtonDown(1)) {
			var vRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit vHit = new RaycastHit();
			if (Physics.Raycast(vRay, out vHit, Mathf.Infinity, _gridLayer)) {
				grid.PushOrder(vHit.point);
			}
		}
	}

	private void Spawner() {
		if (_magnetSpawnEllapsed > magnetSpawnTimer) {
			if (grid.magnetsCount < maxMagnet) {
				_magnetsCount++;
				int vRandomCube = Random.Range(0, magnetPrefabs.Length);
				Magnet vTemp = Instantiate(magnetPrefabs[vRandomCube]);
				grid.AddMagnet(vTemp);
			}

			_magnetSpawnEllapsed = 0;
		}
		_magnetSpawnEllapsed += Time.deltaTime;
	}
}
