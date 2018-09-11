using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public int randomint;
	public int randomint2;
	public int randomint3;
	public int randomint4;
	public int randomint5;
	public int randomint6; 
	public GameObject[] areaspawn1;
	public GameObject[] areaspawn2;
	public GameObject[] areaspawn3;



	void Start () {
		areaspawn1 = GameObject.FindGameObjectsWithTag ("Area1");
		areaspawn2 = GameObject.FindGameObjectsWithTag ("Area2");
		areaspawn3 = GameObject.FindGameObjectsWithTag ("Area3");
		randomint = Random.Range (0, areaspawn1.Length-1);
		randomint2 = Random.Range (0, areaspawn1.Length-1);
		randomint3 = Random.Range (0, areaspawn2.Length-1);
		randomint4 = Random.Range (0, areaspawn2.Length-1);
		randomint5 = Random.Range (0, areaspawn3.Length-1);
		randomint6 = Random.Range (0, areaspawn3.Length-1);
	}
	
	void Update () {
		randomint = Random.Range (0, areaspawn1.Length-1);
		randomint2 = Random.Range (0, areaspawn1.Length-1);
		randomint3 = Random.Range (0, areaspawn2.Length-1);
		randomint4 = Random.Range (0, areaspawn2.Length-1);
		randomint5 = Random.Range (0, areaspawn3.Length-1);
		randomint6 = Random.Range (0, areaspawn3.Length-1);
	}
}
