using UnityEngine;

public class Score : MonoBehaviour {

	private void OnEnable() {
		EventsManager.Instance.StartListening<ScoreEvent>("score:update", UpdateScore);
	}

	private void OnDisable() {
		EventsManager.Instance.StopListening<ScoreEvent>("score:update", UpdateScore);
	}

	private void UpdateScore(ScoreEvent pEvent) {

	}
}
