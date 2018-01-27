using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	[SerializeField] Projectile projectile;
	[SerializeField] float cooldown = 0.5f;

	bool canFire = true;

	public void Fire() {
		if (canFire) {
			GameObject projectile = GameObject.Instantiate(this.projectile.gameObject, transform.position, transform.rotation);
			projectile.GetComponent<Projectile>().Shoot(projectile.transform.forward);
			StartCoroutine(Cooldown());
		}
	}

	IEnumerator Cooldown() {
		canFire = false;
		yield return new WaitForSeconds(cooldown);
		canFire = true;
	}
}
