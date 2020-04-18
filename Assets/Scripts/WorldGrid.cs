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

	void Awake() {
		Magnet vTemp1 = Instantiate(blueMagnet);
		Magnet vTemp2 = Instantiate(yellowMagnet);

		_map = new Magnet[] {
			vTemp1, null, null, null, null,
			vTemp2, null, null, null, null,
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

	public void PullMagnets(Vector3 pClickPosition) {
		(int x, int y) vClickedCoord = PositionToCoord(pClickPosition);
		int vIndex = CoordToIndex(vClickedCoord.x, vClickedCoord.y);

		if (_map[vIndex] != null) {
			return;
		}

		// //NOTE(marion): search magnet on X
		// int vXOffset = CoordToIndex(vClickedCoord.x, vClickedCoord.y) / gridWidth * gridWidth;
		// for (int x = 0; x < gridWidth; x++) {
		//	if (_map[vXOffset + x] != null) {
		//		int vInc = (vXOffset + x < vClickedCoord.x) ? -1 : 1;
		//		int vFinalXCoord = Mathf.Clamp(vClickedCoord.x + vInc, 0, gridWidth - 1);

		//		_map[vXOffset + x]?.MoveTo(CoordToPosition(vFinalXCoord, vClickedCoord.y)
		//								   + new Vector3(0.5f, 0, -0.5f));

		//		_map[CoordToIndex(vFinalXCoord, vClickedCoord.y)] = _map[vXOffset + x];
		//		_map[vXOffset + x] = null;
		//	}
		// }


		// Y MOVE DOWN
		int vIncDown = 0;
		for (int y = vIndex; y >= 0; y -= gridWidth) {
			if (_map[y] != null) {
				Debug.Log("MOVING DOWN");
				_map[y].MoveTo(CoordToPosition(vClickedCoord.x, vClickedCoord.y - 1 - vIncDown)
							   + new Vector3(0.5f, 0, -0.5f));
				ChangeIndex(y, vIndex - gridWidth * (vIncDown + 1));
				vIncDown++;
			}
		}

		// Y UP
		int vIncUp = 0;
		for (int y = vIndex; y < _map.Length; y += gridWidth) {
			if (_map[y] != null) {
				Debug.Log("MOVING UP");
				_map[y].MoveTo(CoordToPosition(vClickedCoord.x, vClickedCoord.y + 1 + vIncUp)
							   + new Vector3(0.5f, 0, -0.5f));
				ChangeIndex(y, vIndex + gridWidth * (vIncUp + 1));
				vIncUp++;
			}
		}

		Debug.Log(GridToString());
	}

	public void PushMagnets(Vector3 pClickPosition) {
		(int x, int y) vClickedCoord = PositionToCoord(pClickPosition);
	}

	public void MoveMagnets(Vector3 pClickPosition) {
		(int, int) vClickedCoord = PositionToCoord(pClickPosition);

		//NOTE(marion): search obstacle on width
		int vXOffset = CoordToIndex(vClickedCoord.Item1, vClickedCoord.Item2) / gridWidth * gridWidth;
		for (int x = 0; x < gridWidth; x++) {
			if (_map[vXOffset + x] != null) {
				int vInc = (vXOffset + x < vClickedCoord.Item1) ? -1 : 1;
				int vFinalXCoord = Mathf.Clamp(vClickedCoord.Item1 + vInc, 0, gridWidth - 1);
				_map[vXOffset + x]?.MoveTo(CoordToPosition(vFinalXCoord, vClickedCoord.Item2)
										   + new Vector3(0.5f, 0, -0.5f));
				_map[CoordToIndex(vFinalXCoord, vClickedCoord.Item2)] = _map[vXOffset + x];
				_map[vXOffset + x] = null;
			}
		}

		//NOTE(marion): search obstacle on height
		int vYOffset = CoordToIndex(vClickedCoord.Item1, vClickedCoord.Item2) % gridWidth;
		for (int y = 0; y < gridHeight; y++) {
			if (_map[vYOffset + y * gridHeight] != null) {
				int vInc = (vYOffset + y * gridHeight < vClickedCoord.Item2) ? -1 : 1;
				int vFinalYCoord = Mathf.Clamp(vClickedCoord.Item2 + vInc, 0, gridHeight - 1);
				_map[vYOffset + y * gridHeight].MoveTo(CoordToPosition(vClickedCoord.Item1, vFinalYCoord)
													   + new Vector3(0.5f, 0, -0.5f));
				_map[CoordToIndex(vClickedCoord.Item1, vFinalYCoord)] = _map[vYOffset + y * gridHeight];
				_map[vYOffset + y * gridHeight] = null;
			}
		}
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
				vToReturn += "_ ";
			}
		}
		return vToReturn;
	}
}
