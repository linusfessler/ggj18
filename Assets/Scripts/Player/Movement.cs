using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	[SerializeField] float movementSpeed = 1f;
	[SerializeField] float rotationSpeed = 1f;
	[SerializeField] float rotationOffsetDegrees = 5;
	[SerializeField] float rotationOffsetSpeed = 2.5f;

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
			Quaternion offsetFromParent = Quaternion.identity;
			offsetFromParent *= Quaternion.AngleAxis(rotationOffsetDegrees * y, transform.parent.right);
			offsetFromParent *= Quaternion.AngleAxis(rotationOffsetDegrees * x, transform.parent.up) * Quaternion.AngleAxis(rotationOffsetDegrees * x, -transform.parent.forward);
			offsetFromParent *= Quaternion.AngleAxis(rotationOffsetDegrees * z, -transform.parent.forward);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, offsetFromParent * transform.parent.rotation, rotationOffsetSpeed * Time.fixedDeltaTime);
			transform.parent.Rotate(rotationSpeed * Time.fixedDeltaTime * new Vector3(y, x, -z), Space.Self);
			rigidbody.velocity = rigidbody.velocity.magnitude * transform.parent.forward;
		} else {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, rotationOffsetSpeed * Time.fixedDeltaTime);
		}
	}

	public void AddVelocity(Vector3 velocity) {
		rigidbody.velocity += velocity;
		Vector3 up = Vector3.Cross(transform.parent.forward, velocity);
		//transform.parent.Rotate(rotationSpeed * Time.fixedDeltaTime * velocity, Space.Self);
		transform.parent.rotation = Quaternion.LookRotation(rigidbody.velocity, transform.parent.up);
	}
}
