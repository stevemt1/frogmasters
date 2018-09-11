﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Frog : MonoBehaviour {

	public int playerNumber;
	public Rigidbody2D rb;
	public bool onLog = false;
	public bool onWater = false;
	public Text Frog1Points;
	private RaycastHit2D hit;
	private int layerMask = 1 << 8;
	public int points = 0;
	public bool canMove;
//	public string dirmove;
	private Vector3 spawnPoint;
	public bool localplayer = false;
	public Quaternion newRotation;

	//Tongue variables
	private GameObject tonguePivot;
	private bool canTongue;
	public float maxSize = 0.4f;
	public float growthRate;
	public float waitTime;

	//audio
	public AudioSource audioManag;
	public AudioClip tongueSound;
	public AudioClip carSquish;

	void Start()
	{
		//Set points to zero, deactivate tongue.
//		Frog1Points.text = "Points: 0";
		newRotation = transform.rotation;
		tonguePivot = transform.GetChild (0).gameObject;
		tonguePivot.SetActive (false);
		canTongue = true;
		canMove = true;
		spawnPoint = transform.position;

		audioManag = GetComponent<AudioSource> ();

	}

	void Update () {
		// Raycasts for wood
		hit = Physics2D.Raycast (transform.position, Vector2.up, 0.3f, layerMask);
		Debug.DrawRay (transform.position, Vector2.up, Color.black);

		//Movement
//		Quaternion newRotation = transform.rotation;
		if (localplayer && Input.GetKeyDown(KeyCode.UpArrow) && canMove) {
			transform.position = new Vector2 (transform.position.x, transform.position.y + 1);
			newRotation.eulerAngles = new Vector3 (0, 0, 0);
		}
		if (localplayer && Input.GetKeyDown(KeyCode.RightArrow) && canMove){
			newRotation.eulerAngles = new Vector3 (0, 0, 270);
			if (rb.position.x + 1 > 10)
				return;
			else
				transform.position = new Vector2 (transform.position.x+1, transform.position.y);
		}
		if (localplayer && Input.GetKeyDown(KeyCode.LeftArrow) && canMove) {
			newRotation.eulerAngles = new Vector3 (0, 0, 90);
			if (rb.position.x -1 < -10)
				return;
			else
				transform.position = new Vector2 (transform.position.x-1, transform.position.y);
		}
		if (localplayer && Input.GetKeyDown(KeyCode.DownArrow) && canMove){
			newRotation.eulerAngles = new Vector3 (0, 0, 180);
			if (rb.position.y - 1 < -4.5)
				return;
			else
				transform.position = new Vector2 (transform.position.x, transform.position.y - 1);
		}
		transform.rotation = newRotation;

		//Shoot Tongue
		if (Input.GetKeyDown(KeyCode.Space) && localplayer)
			ShootTongue();

		//Reset position if hitting water
		if (hit.collider != null && onWater == true) {
			return;
		} else {
			if (onWater) {
				//SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				respawn();
			}
		}

	}

	public void ShootTongue(){
		if (canTongue){
			audioManag.PlayOneShot (tongueSound);
			canMove = false;
			canTongue = false;
			tonguePivot.SetActive (true);
			StartCoroutine (ScaleTongue ());
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
		if (other.tag == "frogdest") {
			respawn ();
		}
		if (other.tag == "Destroy") {
			respawn ();
		}	
		if (other.tag == "Car" || other.tag == "Tongue") {
			//SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			if(other.tag == "Car"){
				audioManag.PlayOneShot(carSquish);
			}
			respawn();
		}
		if (other.tag == "Wood") {
			transform.parent = other.transform;
			onLog = true;
		}
		if (other.tag == "Road" || other.tag == "Checkpoint1" || other.tag=="Checkpoint2") {
			onLog = false;
			onWater = false;
			transform.parent = null;
		}
		if (other.tag == "Water")
		{
			onLog = false;
			onWater = true;
		}

		if (other.tag == "Goal") {
			onLog = false;
			onWater = false;    
			transform.parent = null;
			//SceneManager.LoadScene ("Win");
			points++;
//			Frog1Points.text = "Points: " + points.ToString();
			//transform.position = spawnPoint;
			transform.position = new Vector2(-0.5f, -4.5f);
		}
	}

    public int returnPoints()
    {
        return points;
    }

    public void respawn()
    {
        if(transform.position.y >= 23.5)
        {
            transform.position = new Vector2(-0.5f, 23.5f);
        }
        else if(transform.position.y >= 9.5f)
        {
            transform.position = new Vector2(-0.5f, 9.5f);
        }
        else
        {
            transform.position = new Vector2(-0.5f, -4.5f);
        }
    }
}
