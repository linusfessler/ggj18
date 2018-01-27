using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	[SerializeField] float vibrationIntensity = 1f;
	[SerializeField] float vibrationDuration = 0.25f;
	[SerializeField] Vector2 scaleRange = new Vector2(1, 20);

	float explosionScale;
	int health;
	Obstacles obstacles;
	new Rigidbody rigidbody;

	void Awake() {
		float scale = Random.Range(scaleRange.x, scaleRange.y);
		transform.localScale = scale * Vector3.one;
		health = (int) scale;
		explosionScale = scale / 10;
		obstacles = GetComponentInParent<Obstacles>();
		rigidbody = GetComponentInParent<Rigidbody>();
	}

	void Update() {
		if (obstacles.player && Vector3.Distance(transform.position, obstacles.player.position) > obstacles.maxDistance) {
			obstacles.RemoveObstacle(this);
			Destroy(gameObject);
		}
		if (rigidbody.velocity.magnitude > obstacles.maxSpeed) {
			rigidbody.velocity = obstacles.maxSpeed * rigidbody.velocity.normalized;
		}
	}

	public void Damage() {
		health -= 1;
		if (health == 0) {
			Explosion.Destroy(gameObject, transform.position, explosionScale, vibrationDuration, vibrationIntensity);
			obstacles.RemoveObstacle(this);
		}
	}
}
