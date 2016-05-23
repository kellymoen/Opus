using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/** I mean, it *works*. I wouldn't say it's particularly good.*/
[RequireComponent(typeof(RawImage))]
public class NoteMovement : MonoBehaviour {
	public AudioSourceMetro metro;
	public Transform destination;
	public Transform origin;
	private float relativeHeight = 2f;
	private float totalDistance;
	private float speed;
	private int index = -1;
	private float targetTime; // = the amount of time it should take to reach destination
	string button;
	string newbutton;

	void Start() {
		transform.position = origin.transform.position;
		totalDistance = DistanceFromDestination ();
	}

	public void ResetNote(float newTargetTime) {
		GetComponentInChildren<Text> ().text = "";
		targetTime = newTargetTime;
		transform.position = origin;
	}

	public void StartNote(float newTargetTime) {
		ResetNote (newTargetTime);
		transform.position = Vector3.Lerp (origin, destination, targetTime - Time.time);
	}

	public float GetTarget() { 
		return targetTime;
	}

	public void SetSpeed(int beatsToReachDestination) {
		speed = (float)(PlayerHit.HalfBeat / beatsToReachDestination);
	}

	void Update() {
		if (!transform.gameObject.activeSelf)
			return;
		//transform.LookAt (destination);
		//transform.position += transform.forward * (((float)PlayerHit.HalfBeat / 32f) / Time.deltaTime);
		//transform.position = new Vector3(transform.position.x, relativeHeight, transform.position.z);
		transform.LookAt (2 * (transform.position - Camera.main.transform.position));

		if (IsInRangeOfDestination() && targetTime >= Time.time) {
			GetComponentInChildren<Text> ().text = "DONE";
			if (Input.GetKeyDown (button)) {
				if (newbutton != null) {
					button = newbutton;
					newbutton = null;
				}
			}
		}
	}

	public float DistanceFromDestination() {
		return Vector3.Distance (transform.position, destination.position);
	}

	public bool IsInRangeOfDestination() {
		return Vector3.Distance (transform.position, destination.position) <= relativeHeight;
	}

	public void Set(AudioSourceMetro m, Transform destination, Transform origin, string button) {
		this.destination = destination;
		this.metro = m;
		this.origin = origin;
		this.button = button;
	}

	public void ChangeButton(string b) {
		newbutton = b;
	}

}