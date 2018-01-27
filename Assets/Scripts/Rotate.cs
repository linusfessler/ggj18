using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	[SerializeField] float angularVelocity = 1f;

	void Start () {
		GetComponent<Rigidbody> ().angularVelocity = angularVelocity * Vector3.up;
	}
}
