using UnityEngine;

public class WorldGrid : MonoBehaviour {

	public int gridWidth;
	public int gridHeight;

	private bool[] _map;

	void Awake() {
		_map = new bool[gridWidth * gridHeight];
	}

	private int FromCoord(int pX, int pY) {
		return pX + gridWidth * pY;
	}

	private (int, int) FromIndex(int pIndex) {
		return (pIndex % gridWidth, pIndex / gridWidth);
	}

}
