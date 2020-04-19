﻿using System.Collections;
using UnityEngine;

public class Alien : MonoBehaviour {

	public WorldGrid grid;
	public Animator animator;
	public SpriteRenderer spriteRenderer;
	public ParticleSystem runParticles;
	public ParticleSystem deathParticles;

	private ALIEN_STATE state;
	private Vector3 _targetDestination;

	private CameraShake _camShake;

	void Awake() {
		(int x, int y) vCoord = grid.PositionToCoord(transform.position);
		Vector3 vLocalPos = grid.CoordToPosition(vCoord.x, vCoord.y) + new Vector3(0.5f, 0f, -0.5f);
		transform.position = new Vector3(vLocalPos.x, 1f, vLocalPos.z);
		StartCoroutine(Running());

		_camShake = FindObjectOfType<CameraShake>();
	}

	public void Expulse(Vector3 pForceDirection) {
		StopAllCoroutines();
		animator.SetBool("run", false);
		pForceDirection += Vector3.up * 5;
		GetComponent<Rigidbody>().AddForce(pForceDirection);
		StartCoroutine(Dying());
	}

	private IEnumerator Dying() {
		_camShake.Shake(0.25f);
		animator.SetBool("hurt", true);
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
		Instantiate(deathParticles,transform.position,Quaternion.identity);
	}

	private IEnumerator Running() {
		(int x, int y) vCoord = grid.PositionToCoord(transform.position);
		Vector3 vEndPos = new Vector3(0.5f, 0.5f, -0.5f);
		bool vIsUp = (Random.value < 0.5f) ? true : false;
		int vSquareCount = 0;
		if (vIsUp) {
			spriteRenderer.flipX = false;
			bool vIsGoingUp = Random.value < 0.5f;
			int vRandomY = Random.Range(0, grid.gridHeight);
			while (vRandomY == vCoord.y) {
				vRandomY = Random.Range(0, grid.gridHeight);
			}
			vSquareCount = Mathf.Abs(vRandomY - vCoord.y);
			vEndPos += grid.CoordToPosition(vCoord.x, vRandomY);
		} else {
			bool vIsGoingRight = Random.value < 0.5f;
			int vRandomX = Random.Range(0, grid.gridWidth);
			while (vRandomX == vCoord.x) {
				vRandomX = Random.Range(0, grid.gridWidth);
			}
			vSquareCount = Mathf.Abs(vRandomX - vCoord.x);
			vEndPos += grid.CoordToPosition(vRandomX, vCoord.y);
			spriteRenderer.flipX = vRandomX > vCoord.x;
		}


		Vector3 vStartPos = transform.position;
		float vDuration = vSquareCount;
		float vEllapsed = 0;

		runParticles.Play();
		animator.SetBool("run", true);
		while (vEllapsed < vDuration) {
			transform.position = Vector3.Lerp(vStartPos, vEndPos, vEllapsed / vDuration);
			vEllapsed += Time.deltaTime;
			yield return null;
		}
		transform.position = vEndPos;
		animator.SetBool("run", false);
		runParticles.Stop();
		StartCoroutine(Idle());
	}

	private IEnumerator Idle() {
		yield return new WaitForSeconds(1f);
		StartCoroutine(Running());
	}


}
