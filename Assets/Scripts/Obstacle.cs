using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	[SerializeField] float vibrationIntensity = 1f;
	[SerializeField] float vibrationDuration = 0.25f;

	float explosionScale;
	int health;

	void Start() {
		health = (int) transform.localScale.x;
		explosionScale = transform.localScale.x / 10;
	}

	void Update() {
		
	}

	public void Damage() {
		health -= 1;
		if (health == 0) {
			Destroy(gameObject);
			Explosion.Destroy(gameObject, transform.position, explosionScale, vibrationDuration, vibrationIntensity);
		}
	}
}
