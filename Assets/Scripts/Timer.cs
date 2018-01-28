using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public float seconds = 0;
	Text text;
	bool stopped;

	void Start() {
		text = GetComponent<Text>();
	}

	void OnEnable() {
		stopped = false;
	}
	
	void Update() {
		if (!stopped) {
			seconds += Time.deltaTime;
			text.text = asString ();
		}
	}

	public string asString() {
        return Highscores.Format(seconds);
	}

	public void Stop() {
		stopped = true;
	}
}
