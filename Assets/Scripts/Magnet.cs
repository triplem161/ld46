using System.Collections;
using UnityEngine;

public class Magnet : MonoBehaviour {

	public MAGNET_COLOR color;

	public MeshRenderer cubeRenderer;

	public ParticleSystem DeathParticles;

	private CameraShake _camShake;

	private void Awake() {
		_camShake = FindObjectOfType<CameraShake>();
	}

	void OnCollisionEnter(Collision pOther) {
		Vector3 vDirection = -(pOther.transform.position - transform.position).normalized;
		pOther.gameObject.GetComponent<Alien>()?.Expulse(vDirection * 20f);
		// pOther.gameObject.GetComponent<Rigidbody>()?.AddForce(vDirection * 20f);
		// pOther.gameObject.GetComponent<Rigidbody>()?.AddExplosionForce(10f, transform.position, 2);
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

		while (vEllapsed < vDuration) {
			transform.position = Vector3.Lerp(vStart, vEnd, vEllapsed / vDuration);
			vEllapsed += Time.deltaTime;
			yield return null;
		}
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
