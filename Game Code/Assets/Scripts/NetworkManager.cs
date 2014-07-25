using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject standyCamera;
	public SpawnSpot[] spawnSpots;

	// Use this for initialization
	void Start () {

		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot> ();

		Connect ();
	}

	void Connect() {
		PhotonNetwork.ConnectUsingSettings( "MultiFPS v1.0" );
	}

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
	}

	void OnJoinedLobby () {
		PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonJoinFailed () {
		Debug.Log ("OnPhotonJoinFailed");
	}

	void OnPhotonRandomJoinFailed () {
		Debug.Log ("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom (null);
	}

	void OnJoinedRoom () {
		Debug.Log ("OnJoinedRoom");

		SpawnMyPlayer ();
	}

	void SpawnMyPlayer () {

		if (spawnSpots == null) {
			Debug.Log ("We dont have any spawn spots!!");
			return;
		}

		SpawnSpot mySpawnSpot = spawnSpots [Random.Range (0, spawnSpots.Length)];

		GameObject myPlayerGameObject = PhotonNetwork.Instantiate("PlayerController",  mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

		((MonoBehaviour)myPlayerGameObject.GetComponent("FPSInputController")).enabled = true;;
		((MonoBehaviour)myPlayerGameObject.GetComponent("MouseLook")).enabled = true;
		((MonoBehaviour)myPlayerGameObject.GetComponent("CharacterMotor")).enabled = true;
		myPlayerGameObject.transform.FindChild ("Main Camera").gameObject.SetActive(true);


		standyCamera.SetActive(false);
	}
}
