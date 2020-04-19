using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour {

	private const float MESH_UNIT = 10;

	[Header("Grid")]
	public int gridWidth;
	public int gridHeight;

	[Header("Prefab")]
	public Magnet blueMagnet;
	public Magnet yellowMagnet;

	[Header("Grid")]
	public Renderer debugRenderer;

	private Magnet[] _map;
	private List<(int startIndex, int endIndex)> _moveOrder;

	void Awake() {
		_moveOrder = new List<(int startIndex, int endIndex)>();
		Magnet vTemp1 = Instantiate(blueMagnet);
		Magnet vTemp2 = Instantiate(yellowMagnet);

		_map = new Magnet[] {
			vTemp1, vTemp2, null, null, null,
			null, null, null, null, null,
			null, null, null, null, null,
			null, null, null, null, null,
			null, null, null, null, null
		};

		for (int i = 0; i < _map.Length; ++i) {
			if (_map[i] != null) {
				_map[i].transform.localPosition = IndexToPosition(i) + new Vector3(0.5f, 0, -0.5f);
			}
		}
	}

	void OnValidate() {
		transform.localScale = new Vector3(gridWidth / MESH_UNIT, 1, gridHeight / MESH_UNIT);
		debugRenderer.material.SetVector("GRID_SIZE", new Vector4(gridWidth, gridHeight, 0, 0));
	}

	public void PullOrder(Vector3 pClickPosition) {
		(int x, int y) vClickedCoord = PositionToCoord(pClickPosition);
		int vIndex = CoordToIndex(vClickedCoord.x, vClickedCoord.y);

		if (_map[vIndex] != null) {
			return;
		}

		// X MOVE RIGHT
		int vIncRight = 0;
		int vStartX = vClickedCoord.y * gridWidth;
		for (int x = vIndex; x >= vStartX; x--) {
			if (_map[x] != null) {
				_moveOrder.Add((x, vIndex - 1 - vIncRight));
				vIncRight++;
			}
		}

		// X MOVE LEFT
		int vIncLeft = 0;
		for (int x = vIndex; x < vStartX + gridWidth - 1; x++) {
			if (_map[x] != null) {
				_moveOrder.Add((x, vIndex + 1 + vIncLeft));
				vIncLeft++;
			}
		}

		// Y MOVE DOWN
		int vIncDown = 0;
		for (int y = vIndex; y >= 0; y -= gridWidth) {
			if (_map[y] != null) {
				_moveOrder.Add((y, vIndex - gridWidth * (vIncDown + 1)));
				vIncDown++;
			}
		}

		// Y UP
		int vIncUp = 0;
		for (int y = vIndex; y < _map.Length; y += gridWidth) {
			if (_map[y] != null) {
				_moveOrder.Add((y, vIndex + gridWidth * (vIncUp + 1)));
				vIncUp++;
			}
		}

		Move();
	}

	public void PushMagnets(Vector3 pClickPosition) {
		(int x, int y) vClickedCoord = PositionToCoord(pClickPosition);
	}

	private void Move() {
		for (int i = 0; i < _moveOrder.Count; ++i) {
			int vStartIndex = _moveOrder[i].startIndex;
			if (vStartIndex != _moveOrder[i].endIndex) {
				Vector3 vDestination = IndexToPosition(_moveOrder[i].endIndex);
				_map[vStartIndex].MoveTo(vDestination + new Vector3(0.5f, 0, -0.5f));
				ChangeIndex(vStartIndex, _moveOrder[i].endIndex);
			}
		}
		_moveOrder.Clear();
	}

	private void ChangeIndex(int pOld, int pNew) {
		_map[pNew] = _map[pOld];
		_map[pOld] = null;
	}

	private int CoordToIndex(int pX, int pY) {
		return pX + gridWidth * pY;
	}

	private (int x, int y) IndexToCoord(int pIndex) {
		return (pIndex % gridWidth, pIndex / gridWidth);
	}

	private Vector3 IndexToPosition(int pIndex) {
		(int x, int y) vCoord = IndexToCoord(pIndex);
		Vector3 vOffset = new Vector3(gridWidth / 2f, 0, gridHeight / 2f);
		Vector3 vOrigin = new Vector3(-vOffset.x, 0.5f, vOffset.z);
		return new Vector3(vOrigin.x + vCoord.x, 0.5f, vOrigin.z - vCoord.y);
	}

	private Vector3 CoordToPosition(int pX, int pY) {
		return IndexToPosition(CoordToIndex(pX, pY));
	}

	private (int x, int y) PositionToCoord(Vector3 pPosition) {
		Vector3 vOffset = new Vector3(gridWidth / 2f, 0, gridHeight / 2f);
		return (Mathf.FloorToInt(pPosition.x + vOffset.x), Mathf.FloorToInt(vOffset.z - pPosition.z));
	}

	private string GridToString() {
		string vToReturn = "";
		for (int i = 0; i < _map.Length; ++i) {
			if (i % gridWidth == 0) {
				vToReturn += "\n";
			}
			if (_map[i] != null) {
				vToReturn += "C ";
			} else {
				vToReturn += "O ";
			}
		}
		return vToReturn;
	}
}
