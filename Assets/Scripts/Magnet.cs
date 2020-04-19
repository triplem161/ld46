using System.Collections;
using UnityEngine;

public class Magnet : MonoBehaviour {

	public MAGNET_COLOR color;

	public MeshRenderer cubeRenderer;

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
		Destroy(gameObject);
	}
}
