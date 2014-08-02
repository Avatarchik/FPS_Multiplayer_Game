using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	WeaponData weaponData;
	float cooldown = 0f;

	FXManager fxManager;
	Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
		fxManager = GameObject.FindObjectOfType<FXManager> ();

		if (fxManager == null) {
			Debug.LogError ("Couldn't find an FXManager");
		}
	}

	// Update is called once per frame
	void Update () {
		cooldown -= (Time.deltaTime + .001f);

		if(Input.GetButtonDown("Fire1")) {
			Fire ();
		}
	}

	void Fire() {
		if (weaponData == null) {
			weaponData = gameObject.GetComponentInChildren<WeaponData> ();
			if (weaponData == null) {
				Debug.Log ("Did not find any WeaponData in our children");
				return;
			}
		}

		if (cooldown > 0) {
			return;
		}

		anim.SetBool ("Jumping", false);

		Debug.Log ("Firing our weapon!");

		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		Transform hitTransform;
		Vector3 hitPoint;

		hitTransform = FindClosestHitObject (ray, out hitPoint);

		// hit something
		if (hitTransform != null) {
			Debug.Log("We hit: " + hitTransform.name);

			Health h = hitTransform.GetComponent<Health>();

			while(h == null && hitTransform.parent) {
				hitTransform = hitTransform.parent;
				h = hitTransform.GetComponent<Health>();
			}

			//Once we get here hitTransform may not be what we started with
			// EX: hit the graphics but the crate has the health

			if (h != null) {
				// Equivalent of running h.TakeDamage(damage); but is sent across the network
				PhotonView pv = h.GetComponent<PhotonView>();

				if(pv == null) {
					Debug.Log ("No PhotonView found");
				}
				else {
					TeamMember tm = hitTransform.GetComponent<TeamMember>();	
					TeamMember myTm = this.GetComponent<TeamMember>();	

					if(tm==null || tm.teamID == 0 || myTm == null || myTm.teamID == 0 || tm.teamID != myTm.teamID) {
						pv.RPC("TakeDamage", PhotonTargets.AllBuffered, weaponData.damage);
					}
				}
			}

			if (fxManager != null) {
				DoGunFX(hitPoint);
			}
		}
		// we hit empty space
		else {
			if (fxManager != null) {
				hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 100f);
				DoGunFX(hitPoint);
			}
		}

		cooldown = weaponData.fireRate;
	}

	void DoGunFX(Vector3 hitPoint) {
		fxManager.GetComponent<PhotonView> ().RPC ("SniperBulletFX", PhotonTargets.All, weaponData.transform.position, hitPoint);
	}

	Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint) {
		RaycastHit[] hits = Physics.RaycastAll (ray);

		Transform closestHit = null;
		float distance = 0f;
		hitPoint = Vector3.zero;

		foreach (RaycastHit hit in hits) {
			if(hit.transform != this.transform && (closestHit == null || hit.distance < distance)) {
				closestHit = hit.transform;
				distance = hit.distance;
				hitPoint = hit.point;
			}
		}

		return closestHit;
	}
}
