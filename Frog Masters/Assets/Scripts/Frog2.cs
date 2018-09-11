using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Frog2 : MonoBehaviour {

	public Rigidbody2D rb;
	public bool onLog = false;
	public bool onWater = false;
	public Text Frog2Points;
	private RaycastHit2D hit;
	private int layerMask = 1 << 8;
	private static int points = 0;
	public bool canMove;

	//Tongue variables
	private GameObject tonguePivot;
	private bool canTongue;
	public float maxSize = 0.4f;
	public float growthRate;
	public float waitTime;

	void Start()
	{
		Frog2Points.text = "Points: 0";
		tonguePivot = transform.GetChild (0).gameObject;
		tonguePivot.SetActive (false);
		canTongue = true;
		canMove = true;
	}

	void Update () {
		hit = Physics2D.Raycast (transform.position, Vector2.up, 0.3f, layerMask);
		Debug.DrawRay (transform.position, Vector2.up, Color.black);

		//Movement
		Quaternion newRotation = transform.rotation;
		if (Input.GetKeyDown (KeyCode.W) && canMove) {
			transform.position = new Vector2 (transform.position.x, transform.position.y + 1);
			newRotation.eulerAngles = new Vector3 (0, 0, 0);
		}
		if (Input.GetKeyDown (KeyCode.D) && canMove) {
			newRotation.eulerAngles = new Vector3 (0, 0, 270);
			if (rb.position.x + 1 > 10)
				return;
			else
				transform.position = new Vector2 (transform.position.x+1, transform.position.y);
		}
		if (Input.GetKeyDown (KeyCode.A) && canMove) {
			newRotation.eulerAngles = new Vector3 (0, 0, 90);
			if (rb.position.x -1 < -10)
				return;
			else
				transform.position = new Vector2 (transform.position.x-1, transform.position.y);
		}
		if (Input.GetKeyDown (KeyCode.S) && canMove) {
			newRotation.eulerAngles = new Vector3 (0, 0, 180);
			if (rb.position.y -1 < -4.5)
				return;
			else
				transform.position = new Vector2 (transform.position.x, transform.position.y-1);
		}
		transform.rotation = newRotation;

		//Shoot Tongue
		if (Input.GetKeyDown(KeyCode.Space) && canTongue){
			canMove = false;
			canTongue = false;
			tonguePivot.SetActive (true);
			StartCoroutine (ScaleTongue ());
		}



		if (hit.collider != null && onWater == true) {
			return;
		} else {
			if (onWater) {
				//SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				transform.position = new Vector2 (-5f, -4.5f);
			}
		}



	}

	//Scales the tongue (This is a coroutine for Update())
	IEnumerator ScaleTongue(){
		while (tonguePivot.transform.localScale.y < maxSize) {
			tonguePivot.transform.localScale += new Vector3 (0, 1, 0) * Time.deltaTime * growthRate;
			yield return null;
		}

		yield return new WaitForSeconds(0.05f);

		while (tonguePivot.transform.localScale.y > 0.007f) {
			if ((tonguePivot.transform.localScale - new Vector3 (0, 1, 0) * Time.deltaTime * growthRate).y < 0.007f) {
				tonguePivot.transform.localScale -= new Vector3 (0, 0.01f, 0);
			} else {
				tonguePivot.transform.localScale -= new Vector3 (0, 1, 0) * Time.deltaTime * growthRate;
			}
			yield return null;
		}

		tonguePivot.SetActive (false);
		canTongue = true;
		canMove = true;

	}


	void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "Car" || other.tag == "Tongue") {
			//SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			transform.position = new Vector2 (-5f, -4.5f);
		}
		if (other.tag == "Wood") {
			transform.parent = other.transform;
			onLog = true;
		}
		if (other.tag == "Road") {
			onLog = false;
			onWater = false;
			transform.parent = null;
		}
		if (other.tag == "Water")
		{
			onWater = true;
		}


		if (other.tag == "Goal") {
			onLog = false;
			onWater = false;
			transform.parent = null;
			//SceneManager.LoadScene ("Win");
			points++;
			Frog2Points.text = "Points: " + points.ToString();
			transform.position = new Vector2 (-5f, -4.5f);
		}
	}

    public static int returnPoints()
    {
        return points;
    }

}
