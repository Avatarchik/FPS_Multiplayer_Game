using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	/*
	 * 
	 * this only applies to "my player" aka the local machine
	 * 
	 */

	float speed = 10f;
	Vector3 direction = Vector3.zero;

	CharacterController cc;


	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Updating the direction");
		direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
	}

	// Called once per physics loop
	// Do all movement and physics stuff here
	void FixedUpdate () {
			Debug.Log ("Oh how you move me");
			cc.SimpleMove (direction * speed);
	}
}
