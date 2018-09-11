using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpawner : MonoBehaviour {

	public float spawnDelay;

	public GameObject plane;
//	public float nextTimeToSpawn = 0f;
	public bool right;
	public bool spawn = false;

	void Start () {
//
//		//Random.InitState (GetComponent<NetworkingClient> ().seed);
//		if(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>() != null)
//			Random.InitState(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>().seed);
//		else
//			Random.InitState(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingClient>().seed);
//
//
//		spawnDelay = Random.Range (0f, 3.0f);
//		nextTimeToSpawn = spawnDelay;
	}

	void Update () {
//		if (nextTimeToSpawn <= Time.time) {
		if (spawn) {
			SpawnPlane ();
			spawn = false;
		}
//			spawnDelay = Random.Range (3.0f, 4.0f);
//			nextTimeToSpawn = Time.time + spawnDelay;
//		}
	}

	void SpawnPlane () {
		GameObject planeSpawn = Instantiate (plane, transform.position, transform.rotation);
		if (right) {
			planeSpawn.GetComponent<Car> ().right = true;
		} else {
			planeSpawn.GetComponent<Car> ().right = false;
		}
	}

}
