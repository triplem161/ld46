using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCE_main : MonoBehaviour {

	public WorldGrid grid;

	private int _gridLayer;

	[Header("Cursor")]
	public Transform attractCursor;
	public Transform repulseCursor;
	public Cursor cursorVisuals;

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
		if (Input.GetMouseButtonDown(0)) {
			var vRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit vHit = new RaycastHit();
			if (Physics.Raycast(vRay, out vHit, Mathf.Infinity, _gridLayer)) {
				(int x, int y) vCoord = grid.PositionToCoord(vHit.point);
				Vector3 vLocalPos = grid.CoordToPosition(vCoord.x, vCoord.y) + new Vector3(0.5f, 0, -0.5f);
				attractCursor.position = new Vector3(vLocalPos.x, 0.6f, vLocalPos.z);
				repulseCursor.gameObject.SetActive(false);
				attractCursor.gameObject.SetActive(false);
				attractCursor.gameObject.SetActive(true);
				grid.PullOrder(vHit.point);
				cursorVisuals.Attract();
			} else {
				attractCursor.transform.position = Vector3.down;
			}
		} else if (Input.GetMouseButtonDown(1)) {
			var vRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit vHit = new RaycastHit();
			if (Physics.Raycast(vRay, out vHit, Mathf.Infinity, _gridLayer)) {
				(int x, int y) vCoord = grid.PositionToCoord(vHit.point);
				Vector3 vLocalPos = grid.CoordToPosition(vCoord.x, vCoord.y) + new Vector3(0.5f, 0, -0.5f);
				repulseCursor.position = new Vector3(vLocalPos.x, 0.6f, vLocalPos.z);
				attractCursor.gameObject.SetActive(false);
				repulseCursor.gameObject.SetActive(false);
				repulseCursor.gameObject.SetActive(true);
				grid.PushOrder(vHit.point);
				cursorVisuals.Repulse();
			} else {
				repulseCursor.transform.position = Vector3.down;
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
