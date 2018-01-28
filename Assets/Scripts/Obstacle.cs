using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	[SerializeField] float vibrationIntensity = 1f;
	[SerializeField] float vibrationDuration = 0.25f;
	[SerializeField] Vector2 scaleRange = new Vector2(10, 100);
	[SerializeField] float maxAngularVelocity = 0.5f;

	float explosionScale;
	int health;
	Obstacles obstacles;
	new Rigidbody rigidbody;

	void Awake() {
		float scale = Random.Range(scaleRange.x, scaleRange.y);
		transform.localScale = scale * Vector3.one;
		health = (int) (scale / 5);
		explosionScale = scale / 7.5f;
		obstacles = GetComponentInParent<Obstacles>();
		rigidbody = GetComponentInParent<Rigidbody>();
		rigidbody.mass = scale;

		// change color
		Renderer renderer = GetComponent<Renderer>();
		Material material = renderer.material;
		Material copy = new Material(material);
		HSBColor color = new HSBColor(copy.color);
		color.b += Random.Range(-0.5f, 0.5f);
		copy.color = color.ToColor();
		renderer.material = copy;

		StartCoroutine(PopUp());
	}

	void Update() {
		if (obstacles.player && Vector3.Distance(transform.position, obstacles.player.position) > obstacles.maxDistance) {
			obstacles.RemoveObstacle(this);
		}
		if (rigidbody.velocity.magnitude > obstacles.maxSpeed) {
			rigidbody.velocity = obstacles.maxSpeed * rigidbody.velocity.normalized;
		}

		if (rigidbody.angularVelocity.magnitude > maxAngularVelocity) {
			rigidbody.angularVelocity = maxAngularVelocity * rigidbody.angularVelocity.normalized;
		}
	}

	void OnDestroy() {
		StopAllCoroutines();
	}

	public void Damage() {
		health -= 1;
		if (health == 0) {
			Explode();
			obstacles.RemoveObstacle(this);
		}
	}

	public void Explode() {
		Explosion.Destroy(gameObject, transform.position, explosionScale, vibrationDuration, vibrationIntensity);
	}

	public IEnumerator PopUp() {
		Vector3 scale = transform.localScale;
		Vector3 step = scale / 10f;
		float scaleSquared = scale.sqrMagnitude;
		transform.localScale = Vector3.zero;
		while (transform.localScale.sqrMagnitude < 1.5f * scaleSquared) {
			transform.localScale += step;
			yield return new WaitForEndOfFrame();
		}
		while (transform.localScale.sqrMagnitude > scaleSquared) {
			transform.localScale -= step;
			yield return new WaitForEndOfFrame();
		}
		transform.localScale = scale;
	}
}
