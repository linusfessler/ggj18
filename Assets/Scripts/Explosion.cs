using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	void Start() {
		StartCoroutine(AutoDestroy());
	}

	IEnumerator AutoDestroy() {
		yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration);
		Destroy(gameObject);
	}

	public static void Destroy(GameObject gameObject, Vector3 position, float scale, float vibrationDuration, float vibrationIntensity) {
		GameObject prefab = Resources.Load<GameObject>("Prefabs/Explosion");
		GameObject explosion = GameObject.Instantiate(prefab, position, Random.rotationUniform);
		explosion.transform.localScale = scale * Vector3.one;
		explosion.GetComponent<MonoBehaviour>().StartCoroutine(Vibration.Vibrate(vibrationDuration, vibrationIntensity));
		Destroy(gameObject);
	}
}
