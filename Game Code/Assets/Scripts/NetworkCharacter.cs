using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			//do nothing - we are already moving us
		} else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}
	}

	void OnPhotonSerializeView(PhotonStream aStream, PhotonMessageInfo aInfo) {
		if (aStream.isWriting) {
			// this is our player, we must send out our actual position
			aStream.SendNext(transform.position);
			aStream.SendNext(transform.rotation);
		}
		else {
			// this is someone elses player, we need to receive their player and update
			// our version of that player
			realPosition = (Vector3) aStream.ReceiveNext();
			realRotation = (Quaternion) aStream.ReceiveNext();
		}
	}

}
