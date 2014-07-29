using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;
	float currentHitPoints;

	// Use this for initialization
	void Start () {
		currentHitPoints = hitPoints;
	}


	public void TakeDamage(float aAmount) {
		currentHitPoints -= aAmount;

		if (currentHitPoints <= 0) {
			Die ();
		}
	}

	void Die() {
		Destroy (gameObject);
	}
}
