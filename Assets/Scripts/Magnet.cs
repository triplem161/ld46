using System.Collections;
using UnityEngine;

public class Magnet : MonoBehaviour {

	public MAGNET_COLOR color;

	public void MoveTo(Vector3 pPosition) {
		StartCoroutine(Moving(pPosition));
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
}
