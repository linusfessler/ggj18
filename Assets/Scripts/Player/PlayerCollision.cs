using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	[SerializeField] float explosionScale = 0.25f;
	[SerializeField] float vibrationIntensity = 1f;
	[SerializeField] float vibrationDuration = 1f;

	void OnCollisionEnter(Collision collision) {
		Explosion.Destroy(transform.Find("Ship").gameObject, transform.position, explosionScale, vibrationDuration, vibrationIntensity);
		GetComponent<Rigidbody>().velocity = Vector3.zero;
	}
}
