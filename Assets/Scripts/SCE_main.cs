using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCE_main : MonoBehaviour {

	public WorldGrid grid;

	private int _gridLayer;

	[Header("Game mode")]
	public bool arcadeMode = false;

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

	[Header("Tutorial")]
	public GameObject tutorial01;
	public GameObject tutorial02;
	public GameObject tutorial03;
	public GameObject tutorial04;

	[Header("Arcade")]
	public TMP_Text mainText;
	public AnimationCurve mainTextAlphaCurve;

	private bool _spawnInfinite = false;
	private bool _hasInputs = false;

	void Awake() {
		_gridLayer = LayerMask.GetMask("GRID_COLLISION");
	}

	private void Start() {
		if(arcadeMode) {
			_spawnInfinite = true;
			StartCoroutine(Arcade());
		}
		else {
			StartCoroutine(Tutorial());
		}
		
	}

	IEnumerator Arcade() {
		StartCoroutine(ArcadeTextIntro());

		for (int i = 0; i < 8; i++) {
			int vRandomCube = Random.Range(0, magnetPrefabs.Length);
			SpawnCube(vRandomCube);
			yield return new WaitForSeconds(0.1f);
		}

		//SPAWN ALIEN

		_hasInputs = true;
	}

	IEnumerator ArcadeTextIntro() {
		mainText.gameObject.SetActive(true);

		float vEllapsed = 0f;
		float vDuration = 1f;

		mainText.text = "Ready?";

		while (vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			mainText.transform.localScale = Vector3.one * Mathf.Lerp(0, 1, vEllapsed / vDuration);
			mainText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), mainTextAlphaCurve.Evaluate(vEllapsed / vDuration));
			yield return null;
		}

		vEllapsed = 0f;
		vDuration = 0.5f;

		mainText.text = "START";

		while (vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			mainText.transform.localScale = Vector3.one * Mathf.Lerp(0, 1, vEllapsed / vDuration);
			mainText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), mainTextAlphaCurve.Evaluate(vEllapsed / vDuration));
			yield return null;
		}

		mainText.gameObject.SetActive(false);
	}

	IEnumerator Tutorial() {
		tutorial01.SetActive(true);

		for(int i=0;i<4;i++) {
			SpawnCube(1);
			yield return new WaitForSeconds(0.5f);
		}

		_hasInputs = true;

		while (grid.magnetsCount > 0) {
			yield return null;
		}

		_hasInputs = false;

		tutorial01.SetActive(false);

		yield return new WaitForSeconds(0.5f);

		tutorial02.SetActive(true);

		for (int i = 0; i < 4; i++) {
			SpawnCube(0);
			yield return new WaitForSeconds(0.2f);
		}

		for (int i = 0; i < 4; i++) {
			SpawnCube(1);
			yield return new WaitForSeconds(0.2f);
		}

		for (int i = 0; i < 4; i++) {
			SpawnCube(2);
			yield return new WaitForSeconds(0.2f);
		}

		_hasInputs = true;

		while (grid.magnetsCount > 0) {
			yield return null;
		}

		tutorial02.SetActive(false);

		yield return new WaitForSeconds(0.5f);

		tutorial03.SetActive(true);

		yield return new WaitForSeconds(5f);

		tutorial03.SetActive(false);

		_spawnInfinite = true;

		yield return new WaitForSeconds(30f);

		tutorial04.SetActive(true);

		yield return new WaitForSeconds(5f);

		tutorial03.SetActive(false);
	}

	void Update() {
		if(_hasInputs)
			Inputs();

		if(_spawnInfinite)
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
				int vRandomCube = Random.Range(0, magnetPrefabs.Length);
				SpawnCube(vRandomCube);
			}

			_magnetSpawnEllapsed = 0;
		}
		_magnetSpawnEllapsed += Time.deltaTime;
	}

	private void SpawnCube(int pIndex) {
		Magnet vTemp = Instantiate(magnetPrefabs[pIndex]);
		grid.AddMagnet(vTemp);
	}
}
