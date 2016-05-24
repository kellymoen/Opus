using UnityEngine;
using System.Collections;

public class NoteMovement : MonoBehaviour {
	public AudioSourceMetro metro;
	public Transform destination;
	public Transform origin;

	void Update() {
		if (!transform.gameObject.activeSelf)
			return;
		transform.LookAt (destination);
		transform.position = transform.position + (transform.forward * (DistanceFromDestination () / Time.deltaTime));
		transform.LookAt(Camera.main.transform.position, -Vector3.up);
		if (Vector3.Distance(transform.position,destination.position) < 0.02) {
			transform.gameObject.SetActive (false);
			Destroy(transform.gameObject);
		}
	}

	public float DistanceFromDestination() {
		return Vector3.Distance (transform.position, destination.position);
	}
}
