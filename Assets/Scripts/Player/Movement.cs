using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	[SerializeField] float movementSpeed = 1f;
	[SerializeField] float rotationSpeed = 1f;

	new Rigidbody rigidbody;

	void Start() {
		rigidbody = GetComponentInParent<Rigidbody>();
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
			Quaternion offset = Quaternion.AngleAxis(x, transform.right) * Quaternion.AngleAxis(y, -transform.up) * Quaternion.AngleAxis(z, -transform.forward);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, rotationSpeed);
			transform.parent.Rotate(rotationSpeed * Time.fixedDeltaTime * new Vector3(-y, x, -z), Space.Self);
			rigidbody.velocity = rigidbody.velocity.magnitude * transform.forward;
		} else {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, rotationSpeed * Time.fixedDeltaTime);
		}
	}

	public void AddVelocity(Vector3 velocity) {
		rigidbody.velocity += velocity;
		transform.parent.rotation = Quaternion.LookRotation(rigidbody.velocity, transform.up);
	}
}
