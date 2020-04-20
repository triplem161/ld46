using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public WorldGrid grid;
	public SCE_main main;
	public Transform shadow;
	public SpriteRenderer spriteRenderer;

	[Header("Movement")]
	public float speed = 0.5f;

	private int _magnetLayer;

	void Awake() {
		_magnetLayer = LayerMask.GetMask("MAGNET_COLLISION");
	}

	public void Spawn() {
		StartCoroutine(Falling());
	}

	public void Expulse(Vector3 pForceDirection) {
		StopAllCoroutines();
		// animator.SetBool("run", false);
		pForceDirection += Vector3.up * 5;
		GetComponent<Rigidbody>().AddForce(pForceDirection);
		StartCoroutine(Dying());
	}

	private IEnumerator Falling() {
		float vEllapsed = 0;
		float vDuration = 0.5f;

		Vector3 vStartPos = transform.position + Vector3.up * 15f;
		Vector3 vEndPos = transform.position;

		while (vEllapsed < vDuration) {
			transform.position = Vector3.Lerp(vStartPos, vEndPos, vEllapsed / vDuration);
			vEllapsed += Time.deltaTime;
			shadow.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.3f, 0.5f, 1f), vEllapsed / vDuration);
			yield return null;
		}
		StartCoroutine(Idle());
	}

	private IEnumerator Dying() {
		GetComponent<Collider>().enabled = false;
		main.robotsCount--;
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
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
			while (vRandomY == vCoord.y) {
				vRandomY = Random.Range(0, grid.gridHeight);
			}
			vSquareCount = Mathf.Abs(vRandomY - vCoord.y);
			vEndPos += grid.CoordToPosition(vCoord.x, vRandomY);
		} else {
			while (vRandomX == vCoord.x) {
				vRandomX = Random.Range(0, grid.gridWidth);
			}
			vSquareCount = Mathf.Abs(vRandomX - vCoord.x);
			vEndPos += grid.CoordToPosition(vRandomX, vCoord.y);
			spriteRenderer.flipX = vRandomX > vCoord.x;
		}

		Vector3 vStartPos = transform.position;
		Vector3 vDirection = (vEndPos - vStartPos).normalized;

		float vDuration = vSquareCount * speed;
		float vEllapsed = 0;

		RaycastHit vHit = new RaycastHit();
		bool vHasFoundMagnet = false;
		Magnet vMagnetToPush = null;
		while (vEllapsed < vDuration) {
			transform.position = Vector3.Lerp(vStartPos, vEndPos, vEllapsed / vDuration);
			if (Physics.Raycast(transform.position, vDirection, out vHit, 1.2f, _magnetLayer) && vHasFoundMagnet) {
				vMagnetToPush = vHit.collider.GetComponent<Magnet>();
				if (!vMagnetToPush.isMoving) {
					(int x, int y) vMagnetCoord = grid.PositionToCoord(vHit.point);
					(int x, int y) vNewEndCoord = (vMagnetCoord.x - (int)vDirection.x, vMagnetCoord.y + (int)vDirection.z);
					int vCellDistance = Mathf.Abs((vRandomX - vNewEndCoord.x) + (vRandomY - vNewEndCoord.y));
					vEndPos = grid.CoordToPosition(vNewEndCoord.x, vNewEndCoord.y) + new Vector3(0.5f, 0.5f, -0.5f);
					vDuration -= vCellDistance * speed;
					vHasFoundMagnet = true;
				}
			}
			vEllapsed += Time.deltaTime;
			yield return null;
		}
		transform.position = vEndPos;
		// animator.SetBool("run", false);
		// runParticles.Stop();
		StartCoroutine(Idle());
	}

	private IEnumerator Idle() {
		yield return new WaitForSeconds(1);
		StartCoroutine(Running());
	}
}
