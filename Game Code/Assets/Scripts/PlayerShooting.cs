using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	public float damage = 25f;
	public float fireRate = 0.5f;
	float cooldown = 0f;

	// Update is called once per frame
	void Update () {
		cooldown -= (Time.deltaTime + .001f);

		if(Input.GetButtonDown("Fire1")) {
			Fire ();
		}

	}

	void Fire() {
		if (cooldown > 0) {
			return;
		}

		Debug.Log ("Firing our weapon!");

		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		Transform hitTransform;
		Vector3 hitPoint;

		hitTransform = FindClosestHitObject (ray, out hitPoint);

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
					pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
				}
			}
		}

		cooldown = fireRate;
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
