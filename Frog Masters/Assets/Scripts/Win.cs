using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win: MonoBehaviour {


	void Start(){
		if (GameObject.FindGameObjectWithTag ("host").GetComponent<NetworkingHost> () != null)
			GameObject.FindGameObjectWithTag ("host").GetComponent<NetworkingHost> ().gameOver = true;
	}

	public void DeleteAll(){
		foreach (GameObject o in Object.FindObjectsOfType<GameObject>()) {
			Destroy(o);
		}
	}

	void Update (){

		if (Input.GetKeyDown (KeyCode.R)) {
			DeleteAll ();
			SceneManager.LoadScene ("TitleScreen");
		}
	}
}
