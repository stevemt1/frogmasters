using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

	public static string Username = "";
	public static string Password = "";
	public string IPAddress = "";
	public int attempt = 0;

	public string CurrentMenu = "Login";
	public string myIP;
	public bool loggedin = false;
	public bool GUIenabled = true;

	private string CreateAccountURL;
	private string LoginAccountURL;
	private string ConfirmUsername = "";
	private string ConfirmPassword = "";
	private string CUser = "";
	private string CPass = "";

	public float time;

	public Rect windowRect1 = new Rect(20,20, 150, 50);

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
		StartCoroutine(NetworkSetup());
		Debug.Log (myIP);
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
			if (CurrentMenu == "Login") {
				LoginGUI ();
			} 
			if (CurrentMenu == "Lobby") {
				LobbyGUI ();
			}
			if (CurrentMenu == "LoginFailed") {
				LoginFailGUI ();
			}
			if (CurrentMenu == "CreateAccount") {
				CreateAccountGUI ();
			}
			if (CurrentMenu == "CreateAccountFailed") {
				CreateAccountFailGUI ();
			}

			if (loggedin)
				CurrentMenu = "Lobby";
		}
	}

	void LoginGUI()
	{
		GUI.Box (new Rect (280, 120, (Screen.width/4) + 200, (Screen.height/4) + 250), "Login");

		if (GUI.Button(new Rect(370, 360, 120, 25), "Create Account"))
		{
			CurrentMenu = "CreateAccount";

		}
		if (GUI.Button(new Rect(510, 360, 120, 25), "Log In"))
		{
			StartCoroutine ("LoginAccount");
			attempt++;
		}

		GUI.Label (new Rect (390, 200, 220, 25), "IP Address");
		IPAddress = GUI.TextField (new Rect (390, 225, 220, 25), IPAddress);
		GUI.Label (new Rect (390, 250, 220, 25), "Username");
		Username = GUI.TextField (new Rect (390, 275, 220, 25), Username);
		GUI.Label (new Rect (390, 300, 220, 25), "Password");
		Password = GUI.PasswordField (new Rect (390, 325, 220, 25), Password, '*');
		GUI.Label (new Rect (390, 400, 220, 25), "My ExternalIP: " + myIP);
	}

	void CreateAccountGUI()
	{
		GUI.Box (new Rect (280, 120, (Screen.width/4) + 200, (Screen.height/4) + 250), "Create Account");

		if (GUI.Button(new Rect(370, 460, 120, 25), "Create Account"))
		{
			if (ConfirmPassword == CPass && ConfirmUsername == CUser) {
				Debug.Log (IPAddress);

				StartCoroutine("CreateAccount");
			}
			else{
				CurrentMenu = "CreateAccountFailed";
//				Debug.Log ("Usernames or Passwords do not match");
			}
		}
		if (GUI.Button(new Rect(510, 460, 120, 25), "Back"))
		{
			CurrentMenu = "Login";
		}
		GUI.Label (new Rect (390, 145, 220, 25), "IP Address");
		IPAddress = GUI.TextField (new Rect (390, 170, 220, 25), IPAddress);
		GUI.Label (new Rect (390, 200, 220, 25), "Username");
		CUser = GUI.TextField (new Rect (390, 225, 220, 25), CUser);
		GUI.Label (new Rect (390, 255, 220, 25), "Password");
		CPass = GUI.PasswordField (new Rect (390, 280, 220, 25), CPass, '*');
		GUI.Label (new Rect (390, 310, 220, 25), "Confirm Username");
		ConfirmUsername = GUI.TextField (new Rect (390, 340, 220, 25), ConfirmUsername);
		GUI.Label (new Rect (390, 370, 220, 25), "Confirm Password");
		ConfirmPassword = GUI.PasswordField (new Rect (390, 400, 220, 25), ConfirmPassword, '*');
		GUI.Label (new Rect (390, 425, 220, 25), "My ExternalIP: " + myIP);
	}

	void CreateAccountFailGUI()
	{
		GUI.Box (new Rect (280, 120, (Screen.width/4) + 200, (Screen.height/4) + 250), "Create Account");

		if (GUI.Button(new Rect(370, 460, 120, 25), "Create Account"))
		{
			if (ConfirmPassword == CPass && ConfirmUsername == CUser) {
				Debug.Log (IPAddress);

				StartCoroutine("CreateAccount");
			}
			else{
				CurrentMenu = "CreateAccountFailed";
//				Debug.Log ("Usernames or Passwords do not match");
			}
		}
		if (GUI.Button(new Rect(510, 460, 120, 25), "Back"))
		{
			CurrentMenu = "Login";
		}
		GUI.Label (new Rect (390, 145, 220, 25), "IP Address");
		IPAddress = GUI.TextField (new Rect (390, 170, 220, 25), IPAddress);
		GUI.Label (new Rect (390, 200, 220, 25), "Username");
		CUser = GUI.TextField (new Rect (390, 225, 220, 25), CUser);
		GUI.Label (new Rect (390, 255, 220, 25), "Password");
		CPass = GUI.PasswordField (new Rect (390, 280, 220, 25), CPass, '*');
		GUI.Label (new Rect (390, 310, 220, 25), "Confirm Username");
		ConfirmUsername = GUI.TextField (new Rect (390, 340, 220, 25), ConfirmUsername);
		GUI.Label (new Rect (390, 370, 220, 25), "Confirm Password");
		ConfirmPassword = GUI.PasswordField (new Rect (390, 400, 220, 25), ConfirmPassword, '*');
		GUI.Label (new Rect (390, 425, 220, 25), "My ExternalIP: " + myIP);
		GUI.Label (new Rect (390, 495, 220, 25), "Username/Password do not match");

	}

	void LobbyGUI()
	{
		GUI.Box (new Rect (280, 120, (Screen.width/4) + 200, (Screen.height/4) + 250), "Lobby");

		if (GUI.Button(new Rect(370, 360, 120, 25), "Start Host"))
		{
			SceneManager.LoadScene ("Lobby");
			GUIenabled = false;
		}
		if (GUI.Button(new Rect(510, 360, 120, 25), "Join Host"))
		{
			SceneManager.LoadScene ("Lobby2");
			GUIenabled = false;
		}
		if (GUI.Button (new Rect (440, 400, 120, 25), "Back")) 
		{
			loggedin = false;
			attempt = 0;
			CurrentMenu = "Login";
		}
	}

	void LoginFailGUI()
	{
		GUI.Box (new Rect (280, 120, (Screen.width/4) + 200, (Screen.height/4) + 250), "Login");

		if (GUI.Button(new Rect(370, 360, 120, 25), "Create Account"))
		{
			CurrentMenu = "CreateAccount";

		}
		if (GUI.Button(new Rect(510, 360, 120, 25), "Log In"))
		{
			StartCoroutine ("LoginAccount");
		}

		GUI.Label (new Rect (390, 200, 220, 25), "IP Address");
		IPAddress = GUI.TextField (new Rect (390, 225, 220, 25), IPAddress);
		GUI.Label (new Rect (390, 250, 220, 25), "Username");
		Username = GUI.TextField (new Rect (390, 275, 220, 25), Username);
		GUI.Label (new Rect (390, 300, 220, 25), "Password");
		Password = GUI.PasswordField (new Rect (390, 325, 220, 25), Password, '*');
		GUI.Label (new Rect (390, 400, 220, 25), "My ExternalIP: " + myIP);
		GUI.Label (new Rect (390, 450, 220, 25), "Incorrect Username or Password");

	}


	IEnumerator CreateAccount()
	{
		CreateAccountURL = "http://" + IPAddress + "/CreateAccountT.php";
		WWWForm form = new WWWForm ();
		form.AddField ("Username", CUser);
		form.AddField ("Password", CPass);

		WWW CreateAccountWWW = new WWW (CreateAccountURL, form);
		yield return CreateAccountWWW;

		if (CreateAccountWWW.error != null) {
			Debug.LogError (CreateAccountWWW.error);
			Debug.LogError ("Cannot Connect to Account Creation");
		}
		else {
			string CreateAccountReturn = CreateAccountWWW.text;
			if (CreateAccountReturn == "Success") {
				Debug.Log ("Success; Account Created");
				CurrentMenu = "Login";
			}
			if (CreateAccountReturn == "AlreadyUsed") {
				Debug.Log ("Username used");
			}
		}
	}

	IEnumerator LoginAccount()
	{
		LoginAccountURL = "http://" + IPAddress + "/LoginAccountT.php";
		WWWForm form = new WWWForm ();
		form.AddField ("Username", Username);
		form.AddField ("Password", Password);

		WWW LoginWWW = new WWW (LoginAccountURL, form);
		yield return LoginWWW;

		if (LoginWWW.error != null)
			Debug.LogError ("Cannot Connect to Account Login");
		else {
			string LoginReturn = LoginWWW.text;
			Debug.Log (LoginReturn);
			if (LoginReturn == "Success") {
				Debug.Log ("Success; Logged in");
				loggedin = true;
			}
			if (LoginReturn == "DoesNotExist") {
				CurrentMenu = "LoginFailed";
				Debug.Log ("Incorrect Username or Password");
			}
		}
	}
}
