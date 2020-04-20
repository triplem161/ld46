using System.Collections;
using UnityEngine;

public class Magnet : MonoBehaviour {

	public MAGNET_COLOR color;

	public MeshRenderer cubeRenderer;

	public bool isMoving;

	private Transform cubeMeshTransform;

	public ParticleSystem DeathParticles;

	private CameraShake _camShake;

	private void Awake() {
		_camShake = FindObjectOfType<CameraShake>();
		cubeMeshTransform = cubeRenderer.transform;
	}

	private void Start() {
		StartCoroutine(Falling());
	}

	IEnumerator Falling() {
		float vEllapsed = 0;
		float vDuration = 0.25f;

		cubeMeshTransform.localScale = new Vector3(1, 1, 1.5f) *100f;

		while (vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			cubeMeshTransform.localPosition = Vector3.up * Mathf.Lerp(10.0f, 0.0f, vEllapsed / vDuration);
			yield return null;
		}

		Instantiate(DeathParticles, transform.position, Quaternion.identity);

		vEllapsed = 0;
		vDuration = 0.25f;

		while (vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			cubeMeshTransform.localScale = Vector3.Lerp(new Vector3(1, 1, 1.5f) * 100f, new Vector3(1,1, 0.5f) * 100f, vEllapsed / vDuration);
			yield return null;
		}

		vEllapsed = 0;
		vDuration = 0.25f;

		while (vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			cubeMeshTransform.localScale = Vector3.Lerp(new Vector3(1, 1, 0.5f) * 100f, Vector3.one*100f, vEllapsed / vDuration);
			yield return null;
		}
	}

	void OnCollisionEnter(Collision pOther) {
		Vector3 vDirection = -(pOther.transform.position - transform.position).normalized;
		pOther.gameObject.GetComponent<Alien>()?.Expulse(vDirection * 20f);
		pOther.gameObject.GetComponent<Enemy>()?.Expulse(vDirection * 20f);
	}

	public void MoveTo(Vector3 pPosition) {
		StartCoroutine(Moving(pPosition));
	}

	public void Destroy() {
		StartCoroutine(Destroying());
	}

	private IEnumerator Moving(Vector3 pPosition) {
		Vector3 vStart = transform.position;
		Vector3 vEnd = pPosition;
		float vDuration = 0.2f;
		float vEllapsed = 0;

		isMoving = true;
		while (vEllapsed < vDuration) {
			transform.position = Vector3.Lerp(vStart, vEnd, vEllapsed / vDuration);
			vEllapsed += Time.deltaTime;
			yield return null;
		}
		isMoving = false;
		transform.position = vEnd;
	}

	private IEnumerator Destroying() {
		float vDuration = 0.5f;
		float vEllapsed = 0;

		while (vEllapsed < vDuration) {
			cubeRenderer.materials[0].SetFloat("_dissolve", Mathf.Lerp(0, 1, vEllapsed / vDuration));
			vEllapsed += Time.deltaTime;
			yield return null;
		}

		_camShake.Shake(0.1f);


		Instantiate(DeathParticles, transform.position, Quaternion.identity);

		vDuration = 0.25f;
		vEllapsed = 0;

		float vInitScale = transform.localScale.x;

		while (vEllapsed < vDuration) {
			transform.localScale = Vector3.one * Mathf.Lerp(vInitScale, 0, vEllapsed / vDuration);
			vEllapsed += Time.deltaTime;
			yield return null;
		}

		Destroy(gameObject);
	}
}
