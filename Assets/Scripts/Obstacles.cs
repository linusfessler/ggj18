using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour {

	[SerializeField] public float maxDistance = 100;
	[SerializeField] public int maxObstacles = 100;
	[SerializeField] Movement movement;

	[HideInInspector] public Transform player;
	[HideInInspector] public float maxSpeed;

	List<Obstacle> obstacles = new List<Obstacle>();
	GameObject[] prefabs;

	void Awake() {
		player = movement.transform.parent;
		maxSpeed = movement.movementSpeed / 2f;
		prefabs = new GameObject[3];
		for (int i = 0; i < prefabs.Length; i++) {
			prefabs[i] = Resources.Load<GameObject>("Prefabs/Asteroid (" + (i+1) + ")");
		}
	}

	void Start() {
		InvokeRepeating("SpawnObstacle", 0.1f, 0.1f);
	}

	void SpawnObstacle() {
		if (!player || obstacles.Count > maxObstacles) {
			return;
		}
		Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		Vector3 position = player.position + maxDistance * direction;
		Vector3 targetDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
		Vector3 targetPosition = player.position + maxDistance * targetDirection;
		targetDirection = (targetPosition - position).normalized;
		int prefabIndex = Random.Range(0, prefabs.Length);
		GameObject obstacle = GameObject.Instantiate(prefabs[prefabIndex], position, Random.rotationUniform, transform);
		Rigidbody rigidbody = obstacle.GetComponent<Rigidbody>();
		rigidbody.velocity = Random.value * maxSpeed * targetDirection;
		rigidbody.angularVelocity = 5 * Random.value * new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f)).normalized;
		obstacles.Add(obstacle.GetComponent<Obstacle>());
	}

	public void RemoveObstacle(Obstacle obstacle) {
		obstacles.Remove(obstacle);
		SpawnObstacle();
	}
}
