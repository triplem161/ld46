using UnityEngine;

public class WorldGrid : MonoBehaviour {

	private const float MESH_UNIT = 10;

	[Header("Grid")]
	public int gridWidth;
	public int gridHeight;

	[Header("Grid")]
	public Renderer debugRenderer;

	private int[] _map;

	void Awake() {
		_map = new int[] {
			1, 0, 0, 0, 0,
			0, 0, 0, 0, 0,
			0, 0, 0, 0, 0,
			0, 0, 0, 0, 0,
			0, 0, 0, 0, 1
		};

		GameObject vTemp;
		for (int i = 0; i < _map.Length; ++i) {
			if (_map[i] > 0) {
				vTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
				vTemp.transform.localPosition = IndexToPosition(i) + new Vector3(0.5f, 0, -0.5f);
			}
		}
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			var vRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit vHit = new RaycastHit();
			int vLayer = LayerMask.GetMask("GRID_COLLISION");
			if (Physics.Raycast(vRay, out vHit, Mathf.Infinity, vLayer)) {

				// Debug.Log("Clicked at: " + vHit.point);
				// Debug.Log("Clicked at coord: " + PositionToCoord(vHit.point));
				// GameObject vTemp;
				// vTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
				// (int, int) vCoord = PositionToCoord(vHit.point);
				// vTemp.transform.localPosition = CoordToPosition(vCoord.Item1, vCoord.Item2);
			}
		}
	}

	void OnValidate() {
		transform.localScale = new Vector3(gridWidth / MESH_UNIT, 1, gridHeight / MESH_UNIT);
		debugRenderer.material.SetVector("GRID_SIZE", new Vector4(gridWidth, gridHeight, 0, 0));
	}

	private int CoordToIndex(int pX, int pY) {
		return pX + gridWidth * pY;
	}

	private (int, int) IndexToCoord(int pIndex) {
		return (pIndex % gridWidth, pIndex / gridWidth);
	}

	private Vector3 IndexToPosition(int pIndex) {
		(int, int) vCoord = IndexToCoord(pIndex);
		Vector3 vOffset = new Vector3(gridWidth / 2f, 0, gridHeight / 2f);
		Vector3 vOrigin = new Vector3(-vOffset.x, 0.5f, vOffset.z);
		return new Vector3(vOrigin.x + vCoord.Item1, 0.5f, vOrigin.z - vCoord.Item2);
	}

	private Vector3 CoordToPosition(int pX, int pY) {
		return IndexToPosition(CoordToIndex(pX, pY));
	}

	private (int, int) PositionToCoord(Vector3 pPosition) {
		Vector3 vOffset = new Vector3(gridWidth / 2f, 0, gridHeight / 2f);
		return (Mathf.FloorToInt(pPosition.x + vOffset.x), Mathf.FloorToInt(vOffset.z - pPosition.z));
	}
}
