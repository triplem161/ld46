﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour {

	public TMP_Text scoreAmountText;
	public TMP_Text scoreBonusText;
	public AnimationCurve bonusTextCurve;
	[Space]
	public TMP_Text multiplierText;
	[Space]
	public TMP_Text alertText;
	public AnimationCurve alertTextCurve;
	public AnimationCurve alertTextScale;

	private List<string> _alertList;
	private float _alertTimer = 0;

	private int _score;

	[Header("Sounds")]
	public GameObject alertScoreSound;

	private void Start() {
		_alertList = new List<string>();
		scoreAmountText.text = _score.ToString("000000");
		StartCoroutine(AlertProcess());
	}

	private void OnEnable() {
		EventsManager.Instance.StartListening<ScoreEvent>("score:update", UpdateScore);
	}

	private void OnDisable() {
		EventsManager.Instance.StopListening<ScoreEvent>("score:update", UpdateScore);
	}

	private void UpdateScore(ScoreEvent pEvent) {

		int vScoreAdd = 100;
		vScoreAdd *= pEvent.comboCounter;
		vScoreAdd += (pEvent.destroyedMagnet*pEvent.lineDestroyed - 4*pEvent.lineDestroyed) * 50;
		vScoreAdd += (pEvent.lineDestroyed - 1) * 100;

		_score += vScoreAdd;
		scoreAmountText.text = _score.ToString("000000");

		StartCoroutine(ShowingBonus(vScoreAdd));

		multiplierText.text = "x" + pEvent.comboCounter;

		if(pEvent.comboCounter > 1) {
			_alertList.Add("Combo x" + pEvent.comboCounter);
		}

		if (pEvent.lineDestroyed > 1) {
			_alertList.Add("Lines x" + pEvent.lineDestroyed);
		}

		if (pEvent.destroyedMagnet / pEvent.lineDestroyed > 4) {
			_alertList.Add("Magnet x" + (pEvent.destroyedMagnet / pEvent.lineDestroyed));
		}

		// Debug.Log("Line: " + pEvent.destroyedMagnet / pEvent.lineDestroyed);
	}


	IEnumerator ShowingBonus(int pAmount) {
		float vEllapsed = 0f;
		float vDuration = 1f;

		scoreBonusText.text = "+" + pAmount;

		while(vEllapsed < vDuration) {
			vEllapsed += Time.deltaTime;
			scoreBonusText.transform.localScale = Vector3.one * Mathf.Lerp(0, 1, bonusTextCurve.Evaluate(vEllapsed / vDuration));
			yield return null;
		}

		scoreBonusText.text = " ";
	}

	IEnumerator AlertProcess() {
		while(true) {
			while (_alertList.Count > 0) {

				Instantiate(alertScoreSound);

				// Debug.Log("ALERRT");
				alertText.text = _alertList[0];

				float vEllapsed = 0f;
				float vDuration = 0.75f;

				while (vEllapsed < vDuration) {
					vEllapsed += Time.deltaTime;
					alertText.transform.localScale = Vector3.one * Mathf.Lerp(0, 1, alertTextScale.Evaluate(vEllapsed / vDuration));
					alertText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), alertTextCurve.Evaluate(vEllapsed/vDuration));
					yield return null;
				}

				_alertList.RemoveAt(0);
				yield return null;
			}
			yield return null;
		}
	}

}
