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
			1, 0, 0, 0, 1,
			0, 0, 0, 0, 0,
			0, 0, 1, 0, 0,
			0, 0, 0, 0, 0,
			1, 0, 0, 0, 1
		};

		GameObject vTemp;
		for (int i = 0; i < _map.Length; ++i) {
			if (_map[i] > 0) {
				vTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
				vTemp.transform.localPosition = ToLocalPosition(i);
			}
		}
	}

	void OnValidate() {
		transform.localScale = new Vector3(gridWidth / MESH_UNIT, 1, gridHeight / MESH_UNIT);
		debugRenderer.material.SetVector("GRID_SIZE", new Vector4(gridWidth, gridHeight, 0, 0));
	}

	private int FromCoord(int pX, int pY) {
		return pX + gridWidth * pY;
	}

	private (int, int) FromIndex(int pIndex) {
		return (pIndex % gridWidth, pIndex / gridWidth);
	}

	private Vector3 ToLocalPosition(int pIndex) {
		Vector3 vOffset = new Vector3(MESH_UNIT / gridWidth, 0, MESH_UNIT / gridHeight);
		return new Vector3(FromIndex(pIndex).Item1, 0.5f, FromIndex(pIndex).Item2) - vOffset;
	}
}
