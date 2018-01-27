using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	[SerializeField] float explosionScale = 0.25f;
	[SerializeField] float vibrationIntensity = 1f;
	[SerializeField] float vibrationDuration = 1f;

	Movement movement;
	ImageEffect imageEffect;

	void Start() {
		movement = GetComponentInChildren<Movement>();
		imageEffect = GetComponentInChildren<ImageEffect>();
	}

	void OnCollisionEnter(Collision collision) {
		Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
		if (obstacle) {
			obstacle.Explode();
			movement.ResetVelocity();
			imageEffect.DecreaseConnectionBrust(0.25f);
			if (imageEffect.intensity >= 1) {
				Explode();
			}
		}
	}

	public void Explode() {
		Explosion.Destroy(transform.Find("Ship").gameObject, transform.position, explosionScale, vibrationDuration, vibrationIntensity);
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Collider>().enabled = false;
	}
}
