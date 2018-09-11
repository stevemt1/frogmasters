using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;


//https://docs.unity3d.com/Manual/UNetUsingTransport.html

public class NetworkingHost : MonoBehaviour {

	public int myReliableChannelID;
	public int myUnreliableChannelID;
	public int hostID;
	public string myIP;
	public string CurrentMenu = "Lobby";

	[HideInInspector]
	public NetworkingHost Instance;

	public GameObject frog;
	public GameObject frogU;
	private string externalIP;
	public float time;
	public float time2;
	public int numberOfPlayers = 1;
	public bool gameStarted = false;
	public int playerNumber = 0;
	public bool GUIenabled = true;
	public bool gameOver = false;

	public float timedelay = 0f;
	public float spawndelay;
	public float movedelay = 0f;

	public string IP = "";
	public string GameName = "";

	public List<int> connectionIDs = new List<int>();
	public List<int> lagtimes = new List<int> ();
	public List<GameObject> froglist;
//	public int seed = 100;
	public float counter = 1;
	public Text textlog;


	private string CreateLobbyURL;
	private string DeleteLobbyURL;


	void Start () {
		IP = GameObject.FindGameObjectWithTag ("Login").GetComponent<Login> ().IPAddress;

		StartCoroutine(NetworkSetup());

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
		hostID = NetworkTransport.AddHost (topology, 3002);
		Debug.Log (hostID);

	}

	public IEnumerator NetworkSetup()
	{
		Network.Connect("127.0.0.1");

		while (Network.player.externalIP == "UNASSIGNED_SYSTEM_ADDRESS")
		{
			time += Time.deltaTime + 0.01f;

			if (time > 10)
			{
				Debug.LogError(" Unable to obtain external ip: Are you sure you are connected to the internet?");
			}

			yield return null;
		}

		myIP = Network.player.externalIP;
		Network.Disconnect();
	}

	void OnGUI()
	{
		if (GUIenabled) {
			if (CurrentMenu == "Lobby")
				LobbyGUI ();
		}
		if (gameOver)
			StartCoroutine ("DeleteLobby");
	}

	void LobbyGUI(){
		GUI.Box (new Rect (280, 120, (Screen.width/4) + 200, (Screen.height/4) + 250), "Server");

		GUI.Label (new Rect (390, 200, 220, 25), "Game Name");
		GameName = GUI.TextField (new Rect (390, 225, 220, 25), GameName);

		GUI.Label (new Rect (390, 250, 400, 25), "Currently " + numberOfPlayers + " Frog(s) are waiting to starts!");

		if (numberOfPlayers > 1)
			GUI.Label (new Rect (390, 300, 400, 25), "Press Space to Start Racing Frogs!");


		if (GUI.Button (new Rect (390, 350, 120, 25), "Create Game")) {
			StartCoroutine ("CreateLobby");
		}

		GUI.Label (new Rect (390, 400, 220, 25), "My ExternalIP: " + myIP);
	}
	
	void Update()
	{


//		if (gameStarted && time2 >= counter) {
//			time2 = 0f;
//			SendFrogLocations ();
//		}

		time2 += Time.deltaTime;

		int recHostId; 
		int connectionId; 
		int channelId; 
		byte[] recBuffer = new byte[1024]; 
		int bufferSize = 1024;
		int dataSize;
		byte error;
		NetworkEventType recData = NetworkTransport.Receive (out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
		switch (recData) {
		case NetworkEventType.Nothing:         //1
			break;
		case NetworkEventType.ConnectEvent:    //2
			connectionIDs.Add (connectionId);
//			SendMessage (hostID, connectionId, myUnreliableChannelID, "number: " + connectionIDs.Count);
			SendToClientsR ("number: " + connectionIDs.Count);
			Debug.Log ("Someone Connected:" + connectionId);
			numberOfPlayers++;

			if (gameStarted) {
				if (froglist.Contains(null)) {
					if (froglist [connectionId] == null) {
						SendMessage (hostID, connectionId, myReliableChannelID, "players:" + numberOfPlayers.ToString ());
						SendMessage (hostID, connectionId, myReliableChannelID, "reassign:" + connectionId.ToString ());
						SendMessage (hostID, connectionId, myReliableChannelID, "change");
						froglist [connectionId] = Instantiate (frog, new Vector3 (-5.0f, -4.5f, 0f), Quaternion.identity);
						foreach (int connection in connectionIDs) {
							if (connection != connectionId)
								SendMessage (hostID, connection, myUnreliableChannelID, "oldfrog:" + connectionId.ToString ());
						}
						SendMessage (hostID, connectionId, myReliableChannelID, "timer:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<countdownTimer> ().timeRemaining.ToString ());

						textlog = GameObject.FindGameObjectWithTag ("gamelog").GetComponent<Text> ();
						textlog.text = "Player " + (connectionId + 1).ToString () + " Connected.";
					}
				} else {
					SendMessage (hostID, connectionId, myReliableChannelID, "players:" + numberOfPlayers.ToString ());
					SendMessage (hostID, connectionId, myReliableChannelID, "change");
					froglist.Add (Instantiate (frog, new Vector3 (-5.0f, -4.5f, 0f), Quaternion.identity));
					foreach (int connection in connectionIDs) {
						if (connection != connectionId)
							SendMessage (hostID, connection, myUnreliableChannelID, "addfrog");
					}
					SendMessage (hostID, connectionId, myReliableChannelID, "timer:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<countdownTimer> ().timeRemaining.ToString ());

					textlog = GameObject.FindGameObjectWithTag ("gamelog").GetComponent<Text> ();
					textlog.text = "Player " + (connectionId + 1).ToString () + " Connected.";
				}
			}
			break;
		case NetworkEventType.DataEvent:       //3
			Stream stream = new MemoryStream (recBuffer);
			BinaryFormatter formatter = new BinaryFormatter ();
			string message = formatter.Deserialize (stream) as string;
			Debug.Log ("incoming message event received: " + message);
//			if (message.Contains ("rtt")) {
//				Debug.Log ("adding rtts");
//				string[] rttmessage = message.Split (':');
//				if (lagtimes.Count <= numberOfPlayers)
//					lagtimes.Add (Int32.Parse (rttmessage [1]));
//				else if (lagtimes.Count > numberOfPlayers) {
//					lagtimes.Clear ();
//					lagtimes.Add (Int32.Parse (rttmessage [1]));
//				}
//				Debug.Log (lagtimes.Count);
//			}
			if (gameStarted) 
			{
				if (message.Contains("SPACE")){
					foreach (int connection in connectionIDs){
						if (connection != connectionId)
							SendMessage (hostID, connection, myUnreliableChannelID, "Tongue:" + connectionId.ToString());
					}
					froglist [connectionId].GetComponent<Frog> ().ShootTongue ();
				}
				if (message.Contains("UP")){

					//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
					froglist[connectionId].transform.position  = new Vector2(froglist[connectionId].transform.position.x, froglist[connectionId].transform.position.y + 1);
					froglist[connectionId].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 0);
				}				
				if (message.Contains("DOWN")){

					//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
					if (froglist [connectionId].GetComponent<Rigidbody2D> ().position.y -1 < -4.5)
						;
					else
						froglist[connectionId].transform.position  = new Vector2(froglist[connectionId].transform.position.x, froglist[connectionId].transform.position.y - 1);
					froglist[connectionId].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 180);
				}				
				if (message.Contains("RIGHT")){

					//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
					if (froglist [connectionId].GetComponent<Rigidbody2D> ().position.x + 1 > 10)
						;
					else
						froglist[connectionId].transform.position  = new Vector2(froglist[connectionId].transform.position.x + 1, froglist[connectionId].transform.position.y);
					froglist[connectionId].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 270);
				}
				if (message.Contains("LEFT")){

					//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
					if (froglist [connectionId].GetComponent<Rigidbody2D> ().position.x - 1 < -10)
						;
					else
						froglist[connectionId].transform.position  = new Vector2(froglist[connectionId].transform.position.x - 1, froglist[connectionId].transform.position.y);
					froglist[connectionId].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 90);
				}
			}
			break;
		case NetworkEventType.DisconnectEvent: //4
			Debug.Log ("Player disconnected");
			Debug.Log (connectionId);
			connectionIDs.Remove (connectionId);
			Destroy (froglist [connectionId]);
			froglist [connectionId] = null;
			numberOfPlayers--;
			foreach (int connection in connectionIDs) {
				SendToClientsR ("gone:" + connectionId.ToString());
			}
			textlog = GameObject.FindGameObjectWithTag ("gamelog").GetComponent<Text> ();
			textlog.text = "Player " + (connectionId + 1).ToString () + " Disconnected.";
			if (numberOfPlayers < 2){
				Debug.Log ("Deleted");
				StartCoroutine ("DeleteLobby");
			}
			break;
		}
		if (numberOfPlayers > 1 && Input.GetKeyDown(KeyCode.Space) && !gameStarted) {
			//				SendMessage (hostID, connectionId, myUnreliableChannelID, "Seed: 100"); 
//			SendToClientsUnR ("Seed: " + seed.ToString());
			//SendToClientsUnR ("change");
			SceneManager.LoadScene ("GameScene");
			//				froglist = GetComponent<GameManager> ().froglist;
			InstantiateFrogs();
			for (int i = 0; i < numberOfPlayers; i++) {
				froglist [i].GetComponent<Frog> ().playerNumber = i;
//				if (froglist [i].GetComponent<Frog>().playerNumber == 0)
//					froglist [i].GetComponent<Frog> ().localplayer = true;
			}
            SendToClientsR("change");
            gameStarted = true;
			GUIenabled = false;
		}

		if (gameStarted) 
		{
			if (Input.GetKeyDown (KeyCode.Space)) {
				froglist [0].GetComponent<Frog> ().ShootTongue ();
				SendToClientsUnR ("Tongue:" + 0);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow)){
				//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
				froglist[0].transform.position  = new Vector2(froglist[0].transform.position.x, froglist[0].transform.position.y + 1);
				froglist[0].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 0);
				SendScores ();
			}				
			if (Input.GetKeyDown(KeyCode.DownArrow)){
				//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
				if (froglist [0].GetComponent<Rigidbody2D> ().position.y -1 < -4.5)
					;
				else
					froglist[0].transform.position  = new Vector2(froglist[0].transform.position.x, froglist[0].transform.position.y - 1);
				froglist[0].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 180);
				SendScores ();
			}				
			if (Input.GetKeyDown(KeyCode.RightArrow)){
				//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
				if (froglist [0].GetComponent<Rigidbody2D> ().position.x + 1 > 10)
					;
				else
					froglist[0].transform.position  = new Vector2(froglist[0].transform.position.x + 1, froglist[0].transform.position.y);
				froglist[connectionId].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 270);
				SendScores ();
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow)){
				//froglist[connectionId].GetComponent<Frog>().dirmove = direction;
				if (froglist [0].GetComponent<Rigidbody2D> ().position.x - 1 < -10)
					;
				else
					froglist[0].transform.position  = new Vector2(froglist[0].transform.position.x - 1, froglist[0].transform.position.y);
				froglist[0].GetComponent<Frog>().newRotation.eulerAngles = new Vector3 (0, 0, 90);
				SendScores ();
			}
			if (movedelay >= 0.25f) {
				SendFrogLocations ();
				movedelay = 0;
			}

			movedelay += Time.deltaTime;
				
			if (timedelay >= 10000) {

				timedelay = 0f;
				
				int index1 = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager>().randomint;
				int index2 = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager>().randomint2;
				int index3 = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager>().randomint3;
				int index4 = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager>().randomint4;
				int index5 = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager>().randomint5;
				int index6 = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager>().randomint6;


				if (GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index1].GetComponent<CarSpawner>() == null) {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index1].GetComponent<WoodSpawner> ().spawn = true;
				} else {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index1].GetComponent<CarSpawner> ().spawn = true;
				}
				SendToClientsR ("Spawn:1:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index1].transform.position.ToString ().Replace ("(", "").Replace (" ", "").Replace (")", ""));


				if (GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index2].GetComponent<CarSpawner>() == null) {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index2].GetComponent<WoodSpawner> ().spawn = true;
				} else {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index2].GetComponent<CarSpawner> ().spawn = true;
				}

				SendToClientsR ("Spawn:1:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn1 [index2].transform.position.ToString ().Replace ("(", "").Replace (" ", "").Replace (")", ""));


				if (GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index3].GetComponent<CarSpawner>() == null) {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index3].GetComponent<WoodSpawner> ().spawn = true;
				} else {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index3].GetComponent<CarSpawner> ().spawn = true;
				}

				SendToClientsR ("Spawn:2:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index3].transform.position.ToString ().Replace ("(", "").Replace (" ", "").Replace (")", ""));


				if (GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index4].GetComponent<CarSpawner>() == null) {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index4].GetComponent<WoodSpawner> ().spawn = true;
				} else {
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index4].GetComponent<CarSpawner> ().spawn = true;
				}

				SendToClientsR ("Spawn:2:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn2 [index4].transform.position.ToString ().Replace ("(", "").Replace (" ", "").Replace (")", ""));


				GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn3 [index5].GetComponent<PlaneSpawner>().spawn = true;
				SendToClientsR ("Spawn:3:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn3 [index5].transform.position.ToString ().Replace ("(", "").Replace (" ", "").Replace (")", ""));

				GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn3 [index6].GetComponent<PlaneSpawner>().spawn = true;
				SendToClientsR ("Spawn:3:" + GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameManager> ().areaspawn3 [index6].transform.position.ToString ().Replace ("(", "").Replace (" ", "").Replace (")", ""));

			}
			spawndelay = UnityEngine.Random.Range (100f, 500f);
			timedelay += spawndelay;
		}
	}

	void SendToClientsUnR(string message)
	{
//		int laggiest = 0;
//		int laggiestconn = 0;
		foreach (int connectionId in connectionIDs)
		{
//			SendMessage (hostID, connectionId, myUnreliableChannelID, "rtt");
//			if (lagtimes [lagtimes.Count - 1] > laggiest) {
//				laggiest = lagtimes [lagtimes.Count - 1];
//				laggiestconn = connectionId;
//			}
				SendMessage(hostID, connectionId, myUnreliableChannelID, message);
		}
//
//		SendMessage (hostID, laggiestconn, myUnreliableChannelID, message);
//
//		foreach (int connectionId in connectionIDs) {
//			int difference = laggiest - lagtimes [connectionId];
//			yield return new WaitForSeconds (difference);
//			if (connectionId != laggiestconn)
//				SendMessage (hostID, connectionId, myUnreliableChannelID, message);
//
//		}

	}

	void SendToClientsR(string message)
	{
		foreach (int connectionId in connectionIDs)
		{
				SendMessage(hostID, connectionId, myReliableChannelID, message);
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

	void SendFrogLocations(){

		if (gameStarted) {
			
			string message = "";
			for (int i = 0; i < numberOfPlayers; i++) {
				if (froglist[i] != null)
					message += "loc:" + i.ToString() + ":" + froglist[i].transform.position.ToString().Replace("(", "").Replace(" ", "").Replace(")", "") + ":" +froglist[i].GetComponent<Frog>().newRotation.eulerAngles.ToString().Replace("(", "").Replace(" ", "").Replace(")", "") + '\n';
			}
//			StartCoroutine (SendToClientsUnR (message));
			SendToClientsUnR (message);
		}
	}

	void SendScores(){
		if (gameStarted) {

			string message = "";
			for (int i = 0; i < numberOfPlayers; i++) {
				if (froglist[i] != null)
					message += "scores:" + i.ToString () + ":" + froglist [i].GetComponent<Frog> ().points.ToString () + '\n';
			}
//			StartCoroutine (SendToClientsUnR (message));
			SendToClientsUnR (message);
		}
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

	IEnumerator CreateLobby()
	{
		CreateLobbyURL = "http://" + IP + "/CreateLobby.php";
		WWWForm form = new WWWForm ();
		form.AddField ("GameName", GameName);
		form.AddField ("IP", myIP);

		WWW CreateLobbyWWW = new WWW (CreateLobbyURL, form);
		yield return CreateLobbyWWW;

		if (CreateLobbyWWW.error != null) {
			Debug.LogError (CreateLobbyWWW.error);
			Debug.LogError ("Cannot Create Game");
		}
		else {
			string CreateLobbyReturn = CreateLobbyWWW.text;
			if (CreateLobbyReturn == "Success") {
				Debug.Log ("Success; Game Created");
			}
		}
	}

	IEnumerator DeleteLobby()
	{
		DeleteLobbyURL = "http://" + IP + "/DeleteLobby.php";
		WWWForm form = new WWWForm ();
		form.AddField ("GameName", GameName);

		WWW DeleteLobbyWWW = new WWW (DeleteLobbyURL, form);
		yield return DeleteLobbyWWW;

		if (DeleteLobbyWWW.error != null) {
			Debug.LogError (DeleteLobbyWWW.error);
			Debug.LogError ("Cannot Delete Game");
		}
		else {
			string DeleteLobbyReturn = DeleteLobbyWWW.text;
			if (DeleteLobbyReturn == "Success") {
				Debug.Log ("Success; Game Deleted");
			}
		}
	}

}
