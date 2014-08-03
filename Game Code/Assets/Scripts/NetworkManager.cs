using UnityEngine;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public GameObject standyCamera;

	public SpawnSpot[] spawnSpots;
	List<SpawnSpot> myTeamSpawnSpots;

	public bool offlineMode = false;
	bool connecting = false;

	List<string> chatMessages;
	int maxChatMessages = 5;

	public float respawnTimer = 0;

	bool hasPickedTeam = false;
	int teamID = 0;

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
				SpawnMyPlayer(teamID);
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
		PhotonNetwork.ConnectUsingSettings( "MultiFPS v1.2" );
	}

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());

		if (PhotonNetwork.connected == false && connecting == false) {
			// We have not yet connected so ask fro online/offline mode
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
			if(hasPickedTeam) {
				// we are fully connected, make sure to display the chat box
				GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
	
				foreach (string msg in chatMessages) {
					GUILayout.Label(msg);
				}
	
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
			else {
				// Player has not yet selected a team
				GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();

				if(GUILayout.Button("Red Team")){
					SpawnMyPlayer(1);
				}
				if(GUILayout.Button("Green Team")) {
					SpawnMyPlayer(2);
				}
				if(GUILayout.Button("Random")) {
					SpawnMyPlayer(Random.Range(1,3));
				}
				if(GUILayout.Button("FFA")) {
					SpawnMyPlayer(0);
				}

				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
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
	}

	void SpawnMyPlayer (int teamID) {
		
		if (hasPickedTeam == false) {
			hasPickedTeam = true;
			this.teamID = teamID;

			myTeamSpawnSpots = new List<SpawnSpot>();
			foreach (SpawnSpot ss in spawnSpots) {
				if (ss.teamID == teamID || teamID == 0) {
					myTeamSpawnSpots.Add(ss);
				}
			}
		}

		AddChatMessage ("Spawning player: " + PhotonNetwork.player.name);

		if (spawnSpots == null || myTeamSpawnSpots.Count == 0) {
			Debug.LogError ("We dont have any spawn spots!!");
			return;
		}

		Debug.Log("lets get our spot! We have " + myTeamSpawnSpots.Count.ToString() );
		SpawnSpot mySpawnSpot = myTeamSpawnSpots[Random.Range (0, myTeamSpawnSpots.Count)];

		GameObject myPlayerGameObject = PhotonNetwork.Instantiate("PlayerController",  mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

		//((MonoBehaviour)myPlayerGameObject.GetComponent("FPSInputController")).enabled = true;;
		((MonoBehaviour)myPlayerGameObject.GetComponent("MouseLook")).enabled = true;
		((MonoBehaviour)myPlayerGameObject.GetComponent("PlayerMovement")).enabled = true;
		((MonoBehaviour)myPlayerGameObject.GetComponent("PlayerShooting")).enabled = true;
		
		myPlayerGameObject.GetComponent<PhotonView>().RPC ("SetTeamID", PhotonTargets.AllBuffered, teamID);		

		myPlayerGameObject.transform.FindChild ("Main Camera").gameObject.SetActive(true);
		standyCamera.SetActive(false);
	}
}
