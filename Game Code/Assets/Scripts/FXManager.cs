using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {

	public GameObject sniperBulletFXPrefab;
	
	[RPC]
	void SniperBulletFX(Vector3 aStartPos, Vector3 aEndPos) {
		Debug.Log ("Sniper Bullet FX");

		GameObject sniperFX = (GameObject)Instantiate (sniperBulletFXPrefab, aStartPos, Quaternion.LookRotation(aEndPos - aStartPos));
		LineRenderer lr = sniperFX.transform.Find ("LineFX").GetComponent<LineRenderer> ();
		lr.SetPosition (0, aStartPos);
		lr.SetPosition (1, aEndPos);
	}
}
