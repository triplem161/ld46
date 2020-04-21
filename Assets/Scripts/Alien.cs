using System.Collections;
using UnityEngine;

public class Alien : MonoBehaviour {

	public SCE_main manager;
	public WorldGrid grid;
	public Animator animator;
	public SpriteRenderer spriteRenderer;
	public ParticleSystem runParticles;
	public ParticleSystem deathParticles;
	public Transform shadow;

	[Header("Movement")]
	public float speed = 0.5f;

	[Header("Sounds")]
	public GameObject alienHurt;
	public GameObject alienDeath;

	private CameraShake _camShake;

	private int _magnetLayer;

	void Awake() {
		_magnetLayer = LayerMask.GetMask("MAGNET_COLLISION");

		(int x, int y) vCoord = grid.PositionToCoord(transform.position);
		Vector3 vLocalPos = grid.CoordToPosition(vCoord.x, vCoord.y) + new Vector3(0.5f, 0f, -0.5f);
		transform.position = new Vector3(vLocalPos.x, 1f, vLocalPos.z);
		grid.CeilBlackList(0, 0, new Vector2Int(0, 0), 1);

		_camShake = FindObjectOfType<CameraShake>();
		manager = FindObjectOfType<SCE_main>();
		shadow.localScale = Vector3.zero;
	}

	void OnCollisionEnter(Collision pOther) {
		if (pOther.gameObject.CompareTag("ROBOT")) {
			Vector3 vDirection = (pOther.transform.position - transform.position).normalized;
			Expulse(vDirection);
		}
	}

	public void Expulse(Vector3 pForceDirection) {
		StopAllCoroutines();
		animator.SetBool("run", false);
		pForceDirection += Vector3.up * 5;
		GetComponent<Rigidbody>().AddForce(pForceDirection);
		StartCoroutine(Dying());
	}

	public void Fall() {
		StartCoroutine(Falling());
	}

	private IEnumerator Falling() {
		float vEllapsed = 0;
		float vDuration = 0.5f;

		Vector3 vStartPos = transform.position + Vector3.up * 15f;
		Vector3 vEndPos = transform.position;

		animator.SetBool("hurt", true);
		while (vEllapsed < vDuration) {
			transform.position = Vector3.Lerp(vStartPos, vEndPos, vEllapsed / vDuration);
			vEllapsed += Time.deltaTime;
			shadow.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.3f, 0.5f, 1f), vEllapsed / vDuration);
			yield return null;
		}
		Instantiate(alienHurt);
		Instantiate(deathParticles, transform.position, Quaternion.identity);
		animator.SetBool("hurt", false);
		StartCoroutine(Idle());
	}

	private IEnumerator Dying() {
		Instantiate(alienDeath);
		_camShake.Shake(0.25f);
		animator.SetBool("hurt", true);
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
		Instantiate(deathParticles,transform.position,Quaternion.identity);

		manager.GameOver();
	}

	private IEnumerator Running() {
		(int x, int y) vCoord = grid.PositionToCoord(transform.position);
		Vector3 vEndPos = new Vector3(0.5f, 0.5f, -0.5f);
		bool vIsUp = (Random.value < 0.5f) ? true : false;
		int vSquareCount = 0;
		int vRandomX = vCoord.x;
		int vRandomY = vCoord.y;
		if (vIsUp) {
			spriteRenderer.flipX = false;
			// vRandomY = Random.Range(0, grid.gridHeight);
			while (vRandomY == vCoord.y) {
				vRandomY = Random.Range(0, grid.gridHeight);
			}
			vSquareCount = Mathf.Abs(vRandomY - vCoord.y);
			vEndPos += grid.CoordToPosition(vCoord.x, vRandomY);
		} else {
			// vRandomX = Random.Range(0, grid.gridWidth);
			while (vRandomX == vCoord.x) {
				vRandomX = Random.Range(0, grid.gridWidth);
			}
			vSquareCount = Mathf.Abs(vRandomX - vCoord.x);
			vEndPos += grid.CoordToPosition(vRandomX, vCoord.y);
			spriteRenderer.flipX = vRandomX > vCoord.x;
		}

		Vector3 vStartPos = transform.position;
		Vector3 vDirection = (vEndPos - vStartPos).normalized;

		grid.CeilBlackList(vCoord.x, vCoord.y, new Vector2Int((int)vDirection.x, (int)vDirection.z), 4);

		float vDuration = vSquareCount * speed;
		float vEllapsed = 0;

		runParticles.Play();
		animator.SetBool("run", true);

		RaycastHit vHit = new RaycastHit();
		bool vHasFoundMagnet = false;
		// bool vQuickIdle = false;
		bool vPushMagnet = false;
		Magnet vMagnetToPush = null;
		(int x, int y) vMagnetCoord = (0, 0);
		while (vEllapsed < vDuration) {
			transform.position = Vector3.Lerp(vStartPos, vEndPos, vEllapsed / vDuration);
			if (Physics.Raycast(transform.position, vDirection, out vHit, 1.2f, _magnetLayer) && !vHasFoundMagnet) {
				vMagnetToPush = vHit.collider.GetComponent<Magnet>();
				if (!vMagnetToPush.isMoving) {
					vMagnetCoord = grid.PositionToCoord(vHit.point);
					(int x, int y) vNewEndCoord = (vMagnetCoord.x - (int)vDirection.x, vMagnetCoord.y + (int)vDirection.z);

					int vCellDistance = Mathf.Abs((vRandomX - vNewEndCoord.x) + (vRandomY - vNewEndCoord.y));
					vEndPos = grid.CoordToPosition(vNewEndCoord.x, vNewEndCoord.y) + new Vector3(0.5f, 0.5f, -0.5f);

					vDuration -= vCellDistance * speed;
					// vQuickIdle = vCellDistance <= 0;
					vHasFoundMagnet = true;

					vPushMagnet = vCellDistance > 0;
				}
			}
			vEllapsed += Time.deltaTime;
			yield return null;
		}
		transform.position = vEndPos;
		animator.SetBool("run", false);
		runParticles.Stop();
		if (vPushMagnet) {
			StartCoroutine(PushMagnet(vMagnetToPush, vDirection, vMagnetCoord));
		} else {
			StartCoroutine(Idle());
		}
	}

	private IEnumerator Idle() {
		(int x, int y) vCoord = grid.PositionToCoord(transform.position);
		grid.CeilBlackList(vCoord.x, vCoord.y, new Vector2Int(0, 0), 1);
		yield return new WaitForSeconds(1);
		StartCoroutine(Running());
	}

	private IEnumerator PushMagnet(Magnet pMagnet, Vector3 pDirection, (int x, int y) pMagnetCoord) {
		if (!pMagnet.isMoving) {
			(int x, int y) vPushCoord = (pMagnetCoord.x + (int)pDirection.x, pMagnetCoord.y - (int)pDirection.z);
			if (grid.IsEmptyAt(vPushCoord.x, vPushCoord.y)) {
				pMagnet.MoveTo(grid.CoordToPosition(vPushCoord.x, vPushCoord.y) + new Vector3(0.5f, 0, -0.5f));
				grid.ChangeIndex(grid.CoordToIndex(pMagnetCoord.x, pMagnetCoord.y), grid.CoordToIndex(vPushCoord.x, vPushCoord.y));
			}
			animator.SetBool("hurt", true);
			_camShake.Shake(0.1f);
			Instantiate(alienHurt);
			GameObject vParticles = Instantiate(deathParticles.gameObject, pMagnet.transform.position, Quaternion.identity) as GameObject;
			vParticles.transform.localScale = Vector3.one * 0.5f;
			GetComponent<Rigidbody>().AddForce(Vector3.up * 1.5f);
			yield return new WaitForSeconds(0.5f);
			animator.SetBool("hurt", false);
		}
		StartCoroutine(Idle());
	}
}
