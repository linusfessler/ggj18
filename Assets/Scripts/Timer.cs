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
		TimeSpan t = TimeSpan.FromSeconds(seconds);
		return string.Format("<color=red>{0:D2}:{1:D2}.{2:D3}</color>", 
			t.Minutes, 
			t.Seconds, 
			t.Milliseconds);
	}

	public void Stop() {
		stopped = true;
	}
}
