using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] float speed = 100;

	void Start() {
		StartCoroutine(AutoDestroy());
	}

	void OnCollisionEnter(Collision collision) {
		Destroy(collision.gameObject);
		Destroy(gameObject);
	}

	public void Shoot() {
		GetComponent<Rigidbody>().velocity = speed * transform.forward;
	}

	IEnumerator AutoDestroy() {
		yield return new WaitForSeconds(500 / speed);
		Destroy(gameObject);
	}
}
