using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	const float gravity = 0.1f;

	void Start() {
		
	}

	void Update() {
		
	}

	/*void OnTriggerStay(Collider collider) {
		Movement movement = collider.GetComponentInChildren<Movement>();
		if (movement) {
			movement.AddVelocity(gravity * (transform.position - collider.transform.position));
		}
	}*/

	void OnCollisionEnter(Collision collision) {
		Explosion explosion = GetComponent<Collider>().GetComponent<Explosion>();
		if (explosion) {
			explosion.enabled = true;
		}
	}
}
