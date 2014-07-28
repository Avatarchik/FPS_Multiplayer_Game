using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	/*
	 * 
	 * this only applies to "my player" aka the local machine
	 * 
	 */

	float speed = 10f;
	Vector3 direction = Vector3.zero; //forward/back and left/right

	float jumpSpeed = 6f;
	float verticalVelocity = 0f;

	CharacterController cc;
	Animator anim;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		// WASD foward/back and left/right movement is stored in direction
		direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (direction.magnitude > 1f) {
			direction = direction.normalized;
		}

		anim.SetFloat ("Speed", direction.magnitude);


		if (cc.isGrounded && Input.GetButtonDown ("Jump")) {
				verticalVelocity = jumpSpeed;
		}
	}

	// Called once per physics loop
	// Do all movement and physics stuff here
	void FixedUpdate () {
		Vector3 lDistance = direction * speed * Time.deltaTime;

		if (cc.isGrounded && verticalVelocity < 0) {
			anim.SetBool ("Jumping", false);
			verticalVelocity = Physics.gravity.y * Time.deltaTime;
		} 
		else {
			if(Mathf.Abs(verticalVelocity) > jumpSpeed * 0.75f) {
				anim.SetBool("Jumping", true);
			}
		}

		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		lDistance.y = verticalVelocity * Time.deltaTime;

		cc.Move (lDistance);
	}
}













