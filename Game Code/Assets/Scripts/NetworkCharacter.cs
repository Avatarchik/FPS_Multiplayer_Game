using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	float realAimAngle;

	Animator anim;

	bool gotFirstUpdate = false;

	void Start () {
		CacheComponents ();
	}

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator> ();

		if (anim == null) {
			Debug.Log ("This character is missing an Animator component");
		}
	}

	void CacheComponents() {
		if (anim == null) {
			anim = GetComponent<Animator>();

			if(anim == null) {
				Debug.LogError("The character prefab does not have an Animator component");
			}
		}

		// Cache more components here
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			//do nothing - we are already moving us
		} 
		else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
			anim.SetFloat("AimAngle", Mathf.Lerp(anim.GetFloat("AimAngle"), realAimAngle, 0.1f));
		}
	}

	void OnPhotonSerializeView(PhotonStream aStream, PhotonMessageInfo aInfo) {
		CacheComponents ();

		if (aStream.isWriting) {
			// this is our player, we must send out our actual position
			aStream.SendNext(transform.position);
			aStream.SendNext(transform.rotation);
			aStream.SendNext(anim.GetFloat("Speed"));
			aStream.SendNext(anim.GetBool("Jumping"));
			aStream.SendNext (anim.GetFloat("AimAngle"));
		}
		else {
			// this is someone elses player, we need to receive their player and update
			// our version of that player
			realPosition = (Vector3) aStream.ReceiveNext();
			realRotation = (Quaternion) aStream.ReceiveNext();
			anim.SetFloat("Speed", (float)aStream.ReceiveNext());
			anim.SetBool("Jumping", (bool)aStream.ReceiveNext());
			realAimAngle  = (float) aStream.ReceiveNext();

			if(gotFirstUpdate == false) {
				transform.position = realPosition;
				transform.rotation = realRotation;
				anim.SetFloat("AimAngle", realAimAngle);
				gotFirstUpdate = true;
			}
		}
	}

}
