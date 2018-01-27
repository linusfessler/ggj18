using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour {

	Weapon leftWeapon;
	Weapon rightWeapon;

	void Start() {
		leftWeapon = transform.GetChild(0).GetComponent<Weapon>();
		rightWeapon = transform.GetChild(1).GetComponent<Weapon>();
	}

	void Update() {
		if (Input.GetAxis("FireLeft") > 0) {
			leftWeapon.Fire();
		}
		if (Input.GetAxis("FireRight") > 0) {
			rightWeapon.Fire();
		}
	}
}
