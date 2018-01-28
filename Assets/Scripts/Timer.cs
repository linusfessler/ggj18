using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public float seconds = 0;
	Text text;

	void Start() {
		text = GetComponent<Text>();
	}
	
	void Update() {
		seconds += Time.deltaTime;
		TimeSpan t = TimeSpan.FromSeconds(seconds);
		text.text = string.Format("<color=red>{0:D2}:{1:D2}.{2:D3}</color>", 
			t.Minutes, 
			t.Seconds, 
			t.Milliseconds);
	}
}
