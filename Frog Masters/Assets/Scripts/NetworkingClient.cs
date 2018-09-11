using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class NetworkingClient : MonoBehaviour {

	public int connectionID;
	public int myReliableChannelID;
	public int myUnreliableChannelID;
	public int hostID;
	public int port = 3002;
	public int numberOfPlayers = 1;
	public GameObject frog;
	public GameObject frogU;

	public string CurrentMenu = "Lobby2";
	public static string IPAddress = "";
	public bool GUIenabled = true;

	public bool playerassigned = false;
	public int playerNumber = 0;
	public List<GameObject> froglist;
	public int seed;
	public int rtt;

	public string IP = "";
	private string CheckLobbiesURL;

	public List<string> Games;
	public Text textlog;

	[HideInInspector]
	public NetworkingClient Instance;

	void Start () {
		IP = GameObject.FindGameObjectWithTag ("Login").GetComponent<Login> ().IPAddress;
		
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		NetworkTransport.Init ();
		ConnectionConfig config = new ConnectionConfig ();
		myReliableChannelID = config.AddChannel (QosType.Reliable);
		myUnreliableChannelID = config.AddChannel (QosType.Unreliable);




		HostTopology topology = new HostTopology (config, 4);
		hostID = NetworkTransport.AddHost (topology, port);

		Debug.Log ("Socket ID: " + hostID);
	}

	public void Connect(){
		byte error;
		connectionID = NetworkTransport.Connect (hostID, IPAddress, port, 0, out error);

		//Debug.Log ("Connected to Server ID: " + connectionID);
	}
	
	void Update () {
		int recHostId;
		int connectionId;
		int channelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;
		rtt = NetworkTransport.GetCurrentRTT(hostID,connectionID,out error);
		if (rtt > 2500) {
			SendMessage(hostID, connectionID, myUnreliableChannelID, "Disconnect");
			NetworkTransport.Disconnect(hostID, connectionID, out error);
		}
		NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
		switch (recData)
		{
		case NetworkEventType.Nothing:         //1
			break;
		case NetworkEventType.ConnectEvent:    //2
			Debug.Log ("Connecting");
			break;
		case NetworkEventType.DataEvent:       //3
			Stream stream = new MemoryStream (recBuffer);
			BinaryFormatter formatter = new BinaryFormatter ();
			string message = formatter.Deserialize (stream) as string;
			Debug.Log ("incoming message event received: " + message);
//			Debug.Log ("RTT: " + rtt + "ms" );
//			if (message.Contains ("rtt")) {
//				SendMessage (hostID, connectionID, myReliableChannelID, "rtt:" + rtt.ToString ());
//			}
			if (message.Contains ("players")) {
				string[] numberofplayers = message.Split (':');
				numberOfPlayers = Int32.Parse (numberofplayers [1]);
			}
			if (message.Contains ("timer")) {
				string[] time = message.Split (':');
				float timer = float.Parse (time [1]);
				GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<countdownTimer> ().timeRemaining = timer;
			}
			if (message.Contains ("oldfrog")) {
				string[] oldfrog = message.Split (':');
				int old = Int32.Parse (oldfrog[1]);
				playerNumber = old;
				Debug.Log ("PlayerNumber: " + playerNumber);
				froglist [old] = Instantiate (frog, new Vector3 (-5.0f, -4.5f, 0f), Quaternion.identity);
				textlog = GameObject.FindGameObjectWithTag ("gamelog").GetComponent<Text> ();
				textlog.text = "Player " + (old + 1).ToString () + " Connected.";
			}
			if (message.Contains ("addfrog")) {
				froglist.Add (Instantiate (frog, new Vector3 (-5.0f, -4.5f, 0f), Quaternion.identity));
				textlog = GameObject.FindGameObjectWithTag ("gamelog").GetComponent<Text> ();
				textlog.text = "Player " + (numberOfPlayers).ToString () + " Connected.";
			}
			if (message.Contains ("number:")) {
				if (!playerassigned) {
					playerNumber = Int32.Parse (message.Replace ("number: ", ""));
					numberOfPlayers += playerNumber;
					playerassigned = true;
				} else {
					numberOfPlayers++;
				}
			}
			if (message.Contains ("reassign")) {
				string[] oldfrog = message.Split (':');
				int old = Int32.Parse (oldfrog [1]);
				playerNumber = old;
			}
			if (message.Contains ("change")) {
				SceneManager.LoadScene ("GameScene");
//				froglist = GetComponent<GameManager> ().froglist;
				InstantiateFrogs ();
				for (int i = 0; i < numberOfPlayers; i++) {
					froglist [i].GetComponent<Frog> ().playerNumber = i;
					if (froglist [i].GetComponent<Frog> ().playerNumber == playerNumber)
						froglist [i].GetComponent<Frog> ().localplayer = true;
				}
				GUIenabled = false;
			}
//			if (message.Contains ("single")) {
//				string[] position = message.Replace("single:", "").Split (',');
//				float x = float.Parse(position[0]);
//				float y = float.Parse(position[1]);
//				froglist[playerNumber].transform.position = new Vector2(x, y);
//			}
			if (message.Contains ("gone")) {
				string[] todelete = message.Split (':');
				int playertodelete = Int32.Parse (todelete [1]);
				Destroy (froglist [playertodelete]);
				froglist [playertodelete] = null;
				numberOfPlayers--;

				textlog = GameObject.FindGameObjectWithTag ("gamelog").GetComponent<Text> ();
				textlog.text = "Player " + (playertodelete + 1).ToString () + " Disconnected.";
			}

			if (message.Contains ("loc")) {

				string[] locations = message.Split ('\n');
				foreach (string location in locations) {
					if (location != "") {
						string[] playerlocations = location.Split (':');
						int playernum = Int32.Parse (playerlocations [1]);
						string[] position = playerlocations [2].Split (',');
						float x = float.Parse (position [0]);
						float y = float.Parse (position [1]);
						string[] rotation = playerlocations [3].Split (',');
						float z = float.Parse (rotation [2]);
						froglist [playernum].GetComponent<Frog> ().newRotation.eulerAngles = new Vector3 (0, 0, z);
						froglist [playernum].transform.position = new Vector2 (x, y);
					}
				}
			}

			if (message.Contains ("scores")) {

				string[] scores = message.Split ('\n');
				foreach (string score in scores) {
					if (score != "") {
						string[] playerscores = score.Split (':');
						int playerNumber = Int32.Parse (playerscores [1]);
						int numberscore = Int32.Parse (playerscores [2]);
						froglist [playerNumber].GetComponent<Frog> ().points = numberscore;
					}
				}
			}

			if (message.Contains ("Spawn")) {
				string[] spawners = message.Split (':');
				string area = spawners [1];
				string[] spawnloc = spawners [2].Split (',');
				float x = float.Parse (spawnloc [0]);
				float y = float.Parse (spawnloc [1]);

				if (area == "3") {
					GameObject[] spawnlist = GameObject.FindGameObjectsWithTag ("Area3");
					foreach (GameObject spawn in spawnlist) {
						if (spawn.transform.position.x == x && spawn.transform.position.y == y) {
							spawn.GetComponent<PlaneSpawner> ().spawn = true;
						}
					}

				} else {
					GameObject[] spawnlist = GameObject.FindGameObjectsWithTag ("Area" + area);
					foreach (GameObject spawn in spawnlist) {
						if (spawn.transform.position.x == x && spawn.transform.position.y == y) {
							if (spawn.GetComponent<CarSpawner> () != null)
								spawn.GetComponent<CarSpawner> ().spawn = true;
							if (spawn.GetComponent<WoodSpawner> () != null)
								spawn.GetComponent<WoodSpawner> ().spawn = true;
//						if (spawn.GetComponent<PlaneSpawner>() != null)
//							spawn.GetComponent<PlaneSpawner> ().spawn = true;
						}
					}
				}

			}

			if (message.Contains ("Tongue")) {
				string[] tongue = message.Split (':');
				int persontonguing = Int32.Parse (tongue [1]);
				froglist [persontonguing].GetComponent<Frog> ().ShootTongue ();
			}
			break;
		case NetworkEventType.DisconnectEvent: //4
			Debug.Log("Disconnected, Game Over");

			break;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
            SendMessage (hostID, connectionID, myReliableChannelID,"UP");
		if (Input.GetKeyDown(KeyCode.DownArrow))
			SendMessage (hostID, connectionID, myReliableChannelID,"DOWN");
		if (Input.GetKeyDown(KeyCode.RightArrow))
			SendMessage (hostID, connectionID, myReliableChannelID,"RIGHT");
		if (Input.GetKeyDown(KeyCode.LeftArrow))
			SendMessage (hostID, connectionID, myReliableChannelID,"LEFT");
		if (Input.GetKeyDown(KeyCode.Space))
			SendMessage (hostID, connectionID, myReliableChannelID,"SPACE");

	}


	void OnGUI()
	{
		StartCoroutine ("CheckLobbies");
		if (GUIenabled) {
			if (CurrentMenu == "Lobby2")
				Lobby2GUI ();
		}

	}


	void Lobby2GUI(){
		GUI.Box (new Rect (280, 120, (Screen.width/4) + 200, (Screen.height/4) + 250), "Join Server");

		if (GUI.Button(new Rect(370, 360, 120, 25), "Join Server")){
			Connect ();
		}

		if (playerassigned) {
			GUI.Label (new Rect (390, 290, 220, 25), "You are Player " + (playerNumber + 1));
		}


		GUI.Label (new Rect (390, 200, 220, 25), "IP Address");
		IPAddress = GUI.TextField (new Rect (390, 225, 220, 25), IPAddress);

		int GameNameNum = 250;
		int IPNum = 260;

		for (int i = 0; i < Games.Count; i++) {
			GUI.Label (new Rect (390, GameNameNum, 220, 100), Games[i]);
			GameNameNum += 20;
			IPNum += 20;
		}


	}

	byte[] StringToByteBuffer(string str)
	{
		int bufferSize = 1024;
		byte[] buffer = new byte[bufferSize];
		Stream stream = new MemoryStream(buffer);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, str);

		return buffer;
	}

	void SendMessage(int hostID, int connectionId, int channel, string message)
	{
		byte error;

		byte[] buffer = StringToByteBuffer(message);

		NetworkTransport.Send(hostID, connectionId, channel, buffer, buffer.Length, out error);
	}

	void SendMessage(int hostID, int connectionId, int channel, byte[] buffer)
	{
		byte error;
		NetworkTransport.Send(hostID, connectionId, channel, buffer, buffer.Length, out error);
	}
		

	void InstantiateFrogs(){
		Vector3 location = new Vector3 (-5.0f, -4.5f, 0f);
		GameObject frogprefab;
		for (int i = 0; i < numberOfPlayers; i++) {
			if (i == playerNumber)
				frogprefab = Instantiate (frogU, location, Quaternion.identity);
			else
				frogprefab = Instantiate (frog, location, Quaternion.identity);
			DontDestroyOnLoad(frogprefab);
			froglist.Add(frogprefab);
			location.x += 3.0f;
		}
	}


	IEnumerator CheckLobbies()
	{
		CheckLobbiesURL = "http://" + IP + "/CheckLobbies.php";

		WWW CheckLobbiesWWW = new WWW (CheckLobbiesURL);
		yield return CheckLobbiesWWW;

		if (CheckLobbiesWWW.error != null) {
			Debug.LogError (CheckLobbiesWWW.error);
			Debug.LogError ("Cannot Create Game");
		}
		else {
			Games.Clear ();
			string CheckLobbiesReturn = CheckLobbiesWWW.text;
			Games.Add (CheckLobbiesReturn);
		}
	}


}
