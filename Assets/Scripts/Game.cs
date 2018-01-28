using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	[SerializeField] GameObject ship;
	[SerializeField] GameObject asteroids;

	void Awake() {
		ship.SetActive(false);
		asteroids.SetActive(false);
		Camera.main.GetComponent<ImageEffect>().intensity = 1;
	}

	public void Connect(float seconds) {
		StartCoroutine(ConnectAfterSeconds(seconds));
	}

	IEnumerator ConnectAfterSeconds(float seconds) {
		yield return new WaitForSeconds(seconds);
		ship.SetActive(true);
		asteroids.SetActive(true);
		Camera.main.GetComponent<ImageEffect>().intensity = 0;
	}
}
