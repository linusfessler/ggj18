using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	const float gravity = 0.01f;

	void Start() {
		
	}

	void Update() {
		
	}

	void OnTriggerStay(Collider collider) {
		Movement movement = collider.GetComponent<Movement>();
		if (movement) {
			movement.AddVelocity(gravity * (transform.position - collider.transform.position));
		}
	}
}
