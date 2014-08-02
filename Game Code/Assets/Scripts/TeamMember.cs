using UnityEngine;
using System.Collections;

public class TeamMember : MonoBehaviour {

	int _teamID = 0;

	public int teamID {
		get { return _teamID; }
	}

	[RPC]
	void SetTeamID (int id) {
		_teamID = id;

		SkinnedMeshRenderer mySkin = this.transform.GetComponentInChildren<SkinnedMeshRenderer>();
		
		if (mySkin == null) {
			Debug.Log("No skin on the player");
		}
		
		if(teamID == 1) {
			mySkin.material.color = Color.red;
		}
		if(teamID == 2) {
			mySkin.material.color = new Color(.5f, 1f, .5f);
		}
	}
}
