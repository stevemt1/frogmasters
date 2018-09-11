using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSpawner : MonoBehaviour {

	public float spawnDelay;

	public GameObject wood;
//	public float nextTimeToSpawn = 0f;
	public bool right;
	public bool spawn = false;

	void Start () {

		//Random.InitState (GetComponent<NetworkingClient> ().seed);
//		if(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>() != null)
//			Random.InitState(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>().seed);
//		else
//			Random.InitState(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingClient>().seed);
//
//		spawnDelay = Random.Range (0f, 3.0f);
//
//
//		nextTimeToSpawn = spawnDelay;
	}

	void Update () {
//		if (nextTimeToSpawn <= Time.time) {
		if (spawn) {
			SpawnWood ();
			spawn = false;

		}
//			spawnDelay = Random.Range (3.0f, 8.0f);
//			nextTimeToSpawn = Time.time + spawnDelay;
	}
//	}

	void SpawnWood () {
		GameObject woodSpawn = Instantiate (wood, transform.position, transform.rotation);
		if (right) {
			woodSpawn.GetComponent<Wood> ().right = true;
		} else {
			woodSpawn.GetComponent<Wood> ().right = false;
		}
	}

}