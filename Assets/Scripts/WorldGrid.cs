using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour {

	private const float MESH_UNIT = 10;

	[Header("Grid")]
	public int gridWidth;
	public int gridHeight;

	[Header("Debug")]
	public Renderer debugRenderer;

	[HideInInspector]
	public int magnetsCount;

	private Magnet[] _map;
	private List<(int startIndex, int endIndex)> _moveOrder;

	private int _comboCounter;
	private int _lineDestroyed;

	void Awake() {
		_moveOrder = new List<(int startIndex, int endIndex)>();
		_map = new Magnet[gridWidth * gridHeight];
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

		// X ATTRACT FROM LEFT
		int vIncRight = 0;
		int vStartX = vClickedCoord.y * gridWidth;
		for (int x = vIndex; x >= vStartX; x--) {
			if (_map[x] != null) {
				_moveOrder.Add((x, vIndex - 1 - vIncRight));
				vIncRight++;
			}
		}

		// X ATTRACT FROM RIGHT
		int vIncLeft = 0;
		for (int x = vIndex; x < vStartX + gridWidth; x++) {
			if (_map[x] != null) {
				_moveOrder.Add((x, vIndex + 1 + vIncLeft));
				vIncLeft++;
			}
		}

		// Y ATTRACT FROM TOP
		int vIncDown = 0;
		for (int y = vIndex; y >= 0; y -= gridWidth) {
			if (_map[y] != null) {
				_moveOrder.Add((y, vIndex - gridWidth * (vIncDown + 1)));
				vIncDown++;
			}
		}

		// Y ATTRACT FROM BOTTOM
		int vIncUp = 0;
		for (int y = vIndex; y < _map.Length; y += gridWidth) {
			if (_map[y] != null) {
				_moveOrder.Add((y, vIndex + gridWidth * (vIncUp + 1)));
				vIncUp++;
			}
		}

		Move();
	}

	public void PushOrder(Vector3 pClickPosition) {
		(int x, int y) vClickedCoord = PositionToCoord(pClickPosition);
		int vIndex = CoordToIndex(vClickedCoord.x, vClickedCoord.y);

		if (_map[vIndex] != null) {
			return;
		}

		// X PUSH TO LEFT
		int vIncRight = 0;
		int vStartX = vClickedCoord.y * gridWidth;
		for (int x = vStartX; x < vIndex; x++) {
			if (_map[x] != null) {
				_moveOrder.Add((x, vStartX + vIncRight));
				vIncRight++;
			}
		}

		// X PUSH TO RIGHT
		int vIncLeft = 0;
		for (int x = vStartX + gridWidth - 1; x >= vIndex; x--) {
			if (_map[x] != null) {
				_moveOrder.Add((x, vStartX + gridWidth - 1 - vIncLeft));
				vIncLeft++;
			}
		}

		// Y PUSH TO TOP
		int vIncDown = 0;
		for (int y = vClickedCoord.x; y < vIndex; y += gridWidth) {
			if (_map[y] != null) {
				_moveOrder.Add((y, vClickedCoord.x + gridWidth * vIncDown));
				vIncDown++;
			}
		}

		// Y PUSH TO BOTTOM
		int vIncUp = 0;
		int vLastIndex = gridHeight * gridWidth - (gridWidth - vClickedCoord.x);
		for (int y = vLastIndex; y >= vIndex; y -= gridWidth) {
			if (_map[y] != null) {
				_moveOrder.Add((y, vLastIndex - gridWidth * vIncUp));
				vIncUp++;
			}
		}

		Move();
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
		CheckLine();
	}

	// REALLY UGLY IMPROVE LATER
	private void CheckLine() {
		List<int> _indexToDestroy = new List<int>();
		int vColorCount = 0;
		MAGNET_COLOR vXColor = MAGNET_COLOR.NONE;
		MAGNET_COLOR vYColor = MAGNET_COLOR.NONE;

		//Check horizontal line
		for (int x = 0; x < _map.Length; x++) {
			//every new line we reset increment
			if (x % gridWidth == 0) {
				if (vColorCount > 3) {
					Debug.Log("destroy at new line");
					//remove from x -1 for color count
					for (int i = x - 1; i >= x - vColorCount; i--) {
						if (_indexToDestroy.IndexOf(i) == -1) {
							_indexToDestroy.Add(i);
							_lineDestroyed++;
						}
					}
				}
				vColorCount = 0;
			}
			if (_map[x] != null) {
				if (_map[x].color != vXColor) {
					if (vColorCount > 3) {
						Debug.Log("destroy at different color");
						//remove from x - 1 for color count
						for (int i = x - 1; i >= x - vColorCount; i--) {
							if (_indexToDestroy.IndexOf(i) == -1) {
								_indexToDestroy.Add(i);
								_lineDestroyed++;
							}
						}
					}
					vXColor = _map[x].color;
					vColorCount = 1;
				} else {
					vColorCount++;
				}
			} else {
				if (vColorCount > 3) {
					//remove from x - 1 for color count
					Debug.Log("destroy at empty");
					for (int i = x - 1; i >= x - vColorCount; i--) {
						if (_indexToDestroy.IndexOf(i) == -1) {
							_indexToDestroy.Add(i);
							_lineDestroyed++;
						}
					}
				}
				vColorCount = 0;
			}

			if (x == _map.Length - 1) {
				if (vColorCount > 3) {

					//remove from x - 1 for color count
					for (int i = x; i > x - vColorCount; i--) {
						if (_indexToDestroy.IndexOf(i) == -1) {
							_indexToDestroy.Add(i);
							_lineDestroyed++;
						}
					}
				}
			}
		}
		vColorCount = 0;

		// CHecking vertical line
		for (int x = 0; x < gridWidth; x++) {
			int y;
			for (y = 0; y < gridHeight; y++) {
				int vIndex = CoordToIndex(x, y);
				if (_map[vIndex] != null) {
					if (_map[vIndex].color != vYColor) {
						if (vColorCount > 3) {
							for (int i = 0; i < vColorCount; ++i) {
								if (_indexToDestroy.IndexOf(i) == -1) {
									_indexToDestroy.Add(CoordToIndex(x, y - 1 - i)); // -1 here
									_lineDestroyed++;
								}
							}
						}
						vYColor = _map[vIndex].color;
						vColorCount = 1;
					} else {
						vColorCount++;
					}
				} else {
					if (vColorCount > 3) {
						for (int i = 0; i < vColorCount; ++i) {
							if (_indexToDestroy.IndexOf(i) == -1) {
								_indexToDestroy.Add(CoordToIndex(x, y - 1 - i)); // -1 here
								_lineDestroyed++;
							}
						}
					}
					vColorCount = 0;
				}
			}
			if (vColorCount > 3) {
				for (int i = 0; i < vColorCount; ++i) {
					if (_indexToDestroy.IndexOf(i) == -1) {
						_indexToDestroy.Add(CoordToIndex(x, y - 1 - i));
						_lineDestroyed++;
					}
				}
			}
			vColorCount = 0;
		}

		if (_indexToDestroy.Count > 0) {
			_comboCounter++;
			DestroyMagnet(_indexToDestroy);
		} else {
			_comboCounter = 0;
		}
	}

	private void DestroyMagnet(List<int> pList) {
		EventsManager.Instance.Trigger<ScoreEvent>("score:update", new ScoreEvent(pList.Count, _comboCounter, _lineDestroyed));
		_lineDestroyed = 0;
		foreach (int vIndex in pList) {
			Debug.Log(vIndex);
			_map[vIndex].Destroy();
			magnetsCount--;
			_map[vIndex] = null;
		}
	}

	public void AddMagnet(Magnet pToAdd) {
		bool vIsEmpty = false;
		int vIndex = 0;
		while (!vIsEmpty) {
			vIndex = Random.Range(0, _map.Length);
			vIsEmpty = _map[vIndex] == null;
		}
		_map[vIndex] = pToAdd;
		_map[vIndex].transform.localPosition = IndexToPosition(vIndex) + new Vector3(0.5f, 0, -0.5f);
		magnetsCount++;
		CheckLine();
	}

	private void ChangeIndex(int pOld, int pNew) {
		_map[pNew] = _map[pOld];
		_map[pOld] = null;
	}

	public int CoordToIndex(int pX, int pY) {
		return pX + gridWidth * pY;
	}

	public (int x, int y) IndexToCoord(int pIndex) {
		return (pIndex % gridWidth, pIndex / gridWidth);
	}

	public Vector3 IndexToPosition(int pIndex) {
		(int x, int y) vCoord = IndexToCoord(pIndex);
		Vector3 vOffset = new Vector3(gridWidth / 2f, 0, gridHeight / 2f);
		Vector3 vOrigin = new Vector3(-vOffset.x, 0.5f, vOffset.z);
		return new Vector3(vOrigin.x + vCoord.x, 0.5f, vOrigin.z - vCoord.y);
	}

	public Vector3 CoordToPosition(int pX, int pY) {
		return IndexToPosition(CoordToIndex(pX, pY));
	}

	public (int x, int y) PositionToCoord(Vector3 pPosition) {
		Vector3 vOffset = new Vector3(gridWidth / 2f, 0, gridHeight / 2f);
		return (Mathf.FloorToInt(pPosition.x + vOffset.x), Mathf.FloorToInt(vOffset.z - pPosition.z));
	}

	private string GridToString() {
		Debug.ClearDeveloperConsole();
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
