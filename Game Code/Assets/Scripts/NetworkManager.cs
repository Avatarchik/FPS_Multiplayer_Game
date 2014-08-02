using UnityEngine;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public GameObject standyCamera;
	public SpawnSpot[] spawnSpots;

	public bool offlineMode = false;
	bool connecting = false;

	List<string> chatMessages;
	int maxChatMessages = 5;

	public float respawnTimer = 0;

	// Use this for initialization
	void Start () {
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot> ();
		PhotonNetwork.player.name = PlayerPrefs.GetString ("Username", "Big Ass Hoob");

		chatMessages = new List<string> ();
	}

	void Update() {
		if (respawnTimer > 0) {
			respawnTimer -= Time.deltaTime;

			if(respawnTimer <= 0) {
				SpawnMyPlayer();
			}
		}
	}


	void OnDestroy() {
		PlayerPrefs.SetString ("Username", PhotonNetwork.player.name);
	}

	public void AddChatMessage(string m) {
		GetComponent<PhotonView> ().RPC ("AddChatMessage_RPC", PhotonTargets.AllBuffered, m);
	}

	[RPC]
	void AddChatMessage_RPC(string m) {
		while (chatMessages.Count >= maxChatMessages) {
			chatMessages.RemoveAt(0);
		}

		chatMessages.Add (m);
	}

	void Connect() {
		PhotonNetwork.ConnectUsingSettings( "MultiFPS v1.1" );
	}

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());

		if (PhotonNetwork.connected == false && connecting == false) {
			GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Username:");
			PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name);
			GUILayout.EndHorizontal();

			if(GUILayout.Button("Single Player")){
				connecting = true;
				PhotonNetwork.offlineMode = true;
				OnJoinedLobby();
			}
			if(GUILayout.Button("Multiplayer")) {
				connecting = true;
				Connect();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		if(PhotonNetwork.connected == true && connecting == false) {
			GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();

			foreach (string msg in chatMessages) {
				GUILayout.Label(msg);
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
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
		connecting = false;
		SpawnMyPlayer ();
	}

	void SpawnMyPlayer () {

		AddChatMessage ("Spawning player: " + PhotonNetwork.player.name);

		if (spawnSpots == null) {
			Debug.Log ("We dont have any spawn spots!!");
			return;
		}

		SpawnSpot mySpawnSpot = spawnSpots [Random.Range (0, spawnSpots.Length)];

		GameObject myPlayerGameObject = PhotonNetwork.Instantiate("PlayerController",  mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

		//((MonoBehaviour)myPlayerGameObject.GetComponent("FPSInputController")).enabled = true;;
		((MonoBehaviour)myPlayerGameObject.GetComponent("MouseLook")).enabled = true;
		((MonoBehaviour)myPlayerGameObject.GetComponent("PlayerMovement")).enabled = true;
		((MonoBehaviour)myPlayerGameObject.GetComponent("PlayerShooting")).enabled = true;
		myPlayerGameObject.transform.FindChild ("Main Camera").gameObject.SetActive(true);


		standyCamera.SetActive(false);
	}
}
