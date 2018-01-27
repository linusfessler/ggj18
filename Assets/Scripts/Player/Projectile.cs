using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] float speed = 100;
	[SerializeField] float explosionScale = 0.5f;
	[SerializeField] float vibrationIntensity = 1f;
	[SerializeField] float vibrationDuration = 0.1f;

	void Start() {
		StartCoroutine(AutoDestroy());
	}

	void OnCollisionEnter(Collision collision) {
		Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
		if (obstacle) {
			Explosion.Destroy(gameObject, collision.contacts[0].point, explosionScale, vibrationDuration, vibrationIntensity);
			obstacle.Damage();
		}
	}

	public void Shoot(Vector3 direction) {
		GetComponent<Rigidbody>().velocity = speed * direction;
	}

	IEnumerator AutoDestroy() {
		yield return new WaitForSeconds(500 / speed);
		Destroy(gameObject);
	}
}
