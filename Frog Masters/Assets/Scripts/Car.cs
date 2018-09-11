using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

	public Rigidbody2D rb;

	public float speed;
	public bool right;


	void Start () {
		speed = 5.0f; 
	}

	void FixedUpdate () {
		if (right) {
			rb.velocity = new Vector2 (speed, 0f);
		} else {
			rb.velocity = new Vector2 (-speed, 0f);
		}
	}

	void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "Destroy") {
			Destroy (gameObject);
		}
	}
}
