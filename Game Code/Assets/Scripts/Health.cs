using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;
	float currentHitPoints;

	// Use this for initialization
	void Start () {
		currentHitPoints = hitPoints;
	}

	[RPC]
	public void TakeDamage(float aAmount) {
		currentHitPoints -= aAmount;

		string outstring = "Got hit, remaining health: " + currentHitPoints.ToString ();
		Debug.Log (outstring);

		if (currentHitPoints <= 0) {
			Die ();
		}
	}
	
	void Die() {

		Debug.Log("Object should be dead.");

		PhotonView pv = GetComponent<PhotonView> ();

		if (pv.instantiationId == 0) {
			Destroy (gameObject);
		} else if (pv.isMine) {
			PhotonNetwork.Destroy (gameObject);
		}
	}
}
