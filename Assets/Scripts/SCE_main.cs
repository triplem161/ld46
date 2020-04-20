using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCE_main : MonoBehaviour {

	public WorldGrid grid;

	private int _gridLayer;

	[Header("Cursor")]
	public Transform attractCursor;
	public Transform repulseCursor;
	public Cursor cursorVisuals;

	[Header("Alien")]
	public Alien alien;

	[Header("Magnet Spawner")]
	public Magnet[] magnetPrefabs;
	public float initMagnetSpawnTimer = 3;
	public float endMagnetSpawnTimer = 1;
	private float _magnetSpawnTimer = 0;
	public int maxMagnet = 5;
	private int _magnetsCount = 0;
	private float _magnetSpawnEllapsed;
	[Space]
	public float durationBeforeMaxSpawnRate = 120;
	private float _spawnRateTimer = 0;
	private float _initSpawnTimer = 0;

	[Header("Tutorial")]
	public GameObject tutorial01;
	public GameObject tutorial02;
	public GameObject tutorial03;
	public GameObject tutorial04;

	[Header("Arcade")]
	public TMP_Text alertText;
	public AnimationCurve mainTextAlphaCurve;

	[Header("Game Over")]
	public GameObject gameOverUI;


	private bool _spawnInfinite = false;
	private bool _hasInputs = false;

	void Awake() {
		alien.gameObject.SetActive(false);
		_gridLayer = LayerMask.GetMask("GRID_COLLISION");
	}

	private void Start() {
		if(Settings.ARCADE_MODE) {
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

		alien.gameObject.SetActive(false);

		_hasInputs = true;
	}

	IEnumerator ArcadeTextIntro() {

		float vEllapsed = 0f;
		float vDuration = 1f;

		alertText.text = "Ready?";

		while (vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			alertText.transform.localScale = Vector3.one * Mathf.Lerp(0, 1, vEllapsed / vDuration);
			alertText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), mainTextAlphaCurve.Evaluate(vEllapsed / vDuration));
			yield return null;
		}

		vEllapsed = 0f;
		vDuration = 0.5f;

		alertText.text = "START";

		while (vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			alertText.transform.localScale = Vector3.one * Mathf.Lerp(0, 1, vEllapsed / vDuration);
			alertText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), mainTextAlphaCurve.Evaluate(vEllapsed / vDuration));
			yield return null;
		}

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
		SpawnAlien();
		yield return new WaitForSeconds(5f);

		tutorial03.SetActive(false);

		StartCoroutine(ArcadeTextIntro());

		for (int i = 0; i < 8; i++) {
			int vRandomCube = Random.Range(0, magnetPrefabs.Length);
			SpawnCube(vRandomCube);
			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(30f);

		tutorial04.SetActive(true);

		yield return new WaitForSeconds(5f);

		tutorial03.SetActive(false);
	}

	void Update() {
		if(_hasInputs)
			Inputs();

		if(_spawnInfinite) {
			ManageSpawnTime();
			Spawner();
		}

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

	private void ManageSpawnTime() {
		if(_spawnRateTimer < durationBeforeMaxSpawnRate) {
			_spawnRateTimer += Time.deltaTime;
			_magnetSpawnTimer = Mathf.Lerp(initMagnetSpawnTimer, endMagnetSpawnTimer, _spawnRateTimer / durationBeforeMaxSpawnRate);
		}
	}

	private void Spawner() {
		if (_magnetSpawnEllapsed > _magnetSpawnTimer) {
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

	private void SpawnAlien() {
		int vRandomIndex = Random.Range(0, grid.gridHeight * grid.gridWidth);
		while (!grid.IsEmptyAt(vRandomIndex)) {
			vRandomIndex = Random.Range(0, grid.gridHeight * grid.gridWidth);
		}
		alien.transform.position = grid.IndexToPosition(vRandomIndex);
		alien.gameObject.SetActive(true);
		alien.Fall();
	}

	public void GameOver() {
		_hasInputs = false;
		_spawnInfinite = false;
		gameOverUI.SetActive(true);
	}
}
