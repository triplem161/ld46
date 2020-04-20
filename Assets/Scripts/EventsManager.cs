
using System.Collections.Generic;
using System;

public class EventsManager {
	private static EventsManager _instance;
	public delegate void EventDelegate<T>(T pEvent) where T : GameEvent;
	private Dictionary<string, Delegate> _delegates;

	public static EventsManager Instance {
		get {
			if (_instance == null) {
				_instance = new EventsManager();
				_instance.Init();
			}
			return _instance;
		}
	}

	private void Init() {
		_delegates = new Dictionary<string, Delegate>();
	}

	public void StartListening<T>(string pReference, EventDelegate<T> pListener) where T : GameEvent {
		Delegate vTempDelegate;
		if (_delegates.TryGetValue(pReference, out vTempDelegate)) {
			_delegates[pReference] = Delegate.Combine(vTempDelegate, pListener);
		} else {
			_delegates[pReference] = pListener;
		}
	}

	public void StopListening<T>(string pReference, EventDelegate<T> pListener) where T : GameEvent {
		Delegate vTempDelegate;
		if (_delegates.TryGetValue(pReference, out vTempDelegate)) {
			Delegate vCurrentDelegate = Delegate.Remove(vTempDelegate, pListener);
			if (vCurrentDelegate == null) {
				_delegates.Remove(pReference);
			} else {
				_delegates[pReference] = vCurrentDelegate;
			}
		}
	}

	public void Trigger<T>(string pReference,T pInfo) where T : GameEvent {
		if (pInfo == null) {
			throw new ArgumentNullException("e");
		}
		Delegate vTempDelete;
		if (_delegates.TryGetValue(pReference, out vTempDelete)) {
			EventDelegate<T> callback = vTempDelete as EventDelegate<T>;
			if (callback != null) {
				callback(pInfo);
			}
		}
	}
}

public class GameEvent{}

public class ScoreEvent: GameEvent {
	public int destroyedMagnet;
	public int comboCounter;
	public int lineDestroyed;

	public ScoreEvent(int pMagnetsDestroyed, int pComboCounter, int pLineDestroyed) {
		destroyedMagnet = pMagnetsDestroyed;
		comboCounter = pComboCounter;
		lineDestroyed = pLineDestroyed;
	}
}
