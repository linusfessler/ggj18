using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	[SerializeField] float movementSpeed = 1f;
	[SerializeField] float rotationSpeed = 1f;

	new Rigidbody rigidbody;

	void Start() {
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.velocity = movementSpeed * transform.forward;
	}

	void FixedUpdate() {
		Rotate();
	}

	void Rotate() {
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		float z = Input.GetAxis("Rotation");
		if (x != 0 || y != 0 || z != 0) {
			transform.Rotate(rotationSpeed * Time.fixedDeltaTime * new Vector3(-y, x, -z), Space.Self);
			rigidbody.velocity = rigidbody.velocity.magnitude * transform.forward;
		}
	}

	public void AddVelocity(Vector3 velocity) {
		rigidbody.velocity += velocity;
		transform.rotation = Quaternion.LookRotation(rigidbody.velocity, transform.up);
	}
}
