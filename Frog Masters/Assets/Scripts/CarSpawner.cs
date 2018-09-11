using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour {

	public float spawnDelay;

	public GameObject car;
//	public float nextTimeToSpawn = 0f;
	public bool right;
	public bool spawn = false;

	void Start () {

		//Random.InitState (GetComponent<NetworkingClient> ().seed);
//		if(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>() != null)
//			Random.InitState(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>().seed);
//		else
//			Random.InitState(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingClient>().seed);
		

//		spawnDelay = Random.Range (0f, 3.0f);
//		nextTimeToSpawn = spawnDelay;
	}

	void Update () {
//		if (nextTimeToSpawn <= Time.time) {
		if (spawn) {
			SpawnCar ();
			spawn = false;
		}
//			spawnDelay = Random.Range (1.0f, 3.0f);
//			nextTimeToSpawn = Time.time + spawnDelay;
//		}
	}

	void SpawnCar () {
		GameObject carSpawn = Instantiate (car, transform.position, transform.rotation);
		if (right) {
			carSpawn.GetComponent<Car> ().right = true;
		} else {
			carSpawn.GetComponent<Car> ().right = false;
		}
	}

}
