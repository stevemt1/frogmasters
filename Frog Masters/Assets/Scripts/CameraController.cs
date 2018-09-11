using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform player;
	public Vector3 offset;
	public List<GameObject> froglist;
	public int playerNumber;
	public CameraController Instance;

	void Start() {
		/*froglist = GameObject.FindGameObjectWithTag("client").GetComponent<NetworkingClient> ().froglist;
		playerNumber = GameObject.FindGameObjectWithTag("client").GetComponent<NetworkingClient> ().playerNumber;*/

        if(GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>() != null)
        {
            froglist = GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>().froglist;
            playerNumber = GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingHost>().playerNumber;
        }
        else
        {
            froglist = GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingClient>().froglist;
            playerNumber = GameObject.FindGameObjectWithTag("host").GetComponent<NetworkingClient>().playerNumber;
        }
        
		Assign ();
	}

	void Assign(){
        for (int i = 0; i < froglist.Count; i++) {
			if (playerNumber == froglist [i].GetComponent<Frog> ().playerNumber) {
                player = froglist[i].transform;
			}
		}
	}
    
	void Update () {
		transform.position = new Vector3 (0, player.position.y + offset.y, offset.z);
	}
}
