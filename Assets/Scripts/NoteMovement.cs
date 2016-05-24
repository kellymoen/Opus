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
	private int index = -1;
	private float targetTime; // = the amount of time it should take to reach destination
	string button;

	void Start() {
		transform.position = origin.position;
	}

	public void Initialise(AudioSourceMetro m, Transform destination, Transform origin, string button) {
		this.destination = destination;
		this.metro = m;
		this.origin = origin;
		this.button = button;
	}

	// called to put note back at origin
	public void ResetNote(float newTargetTime) {
		GetComponentInChildren<Text> ().text = "";
		targetTime = newTargetTime;
		transform.position = origin.position;
		transform.position = origin.transform.position;
		totalDistance = DistanceFromDestination ();
	}

	// called to enable the note and start moving
	public void StartNote(float newTargetTime) {
		ResetNote (newTargetTime);
		transform.gameObject.SetActive (true);
		PlayerHit.OnButtonPress += OnHit;
	}

	public float GetTarget() { 
		return targetTime;
	}

	void Update() {
		if (!transform.gameObject.activeSelf)
			return;
		
		transform.LookAt (destination);
		//Debug.Log (destination.position+", "+transform.position+","+((totalDistance / targetTime)));
		transform.position += transform.forward * 0.5f;
		transform.position = new Vector3(transform.position.x, relativeHeight, transform.position.z);
		transform.LookAt (2 * (transform.position - Camera.main.transform.position));

		if (IsInRangeOfDestination() && targetTime - 0.03 >= Time.time) {
			GetComponentInChildren<Text> ().text = "DONE";
			GetComponent<RawImage> ().color = Color.white;
		}
	}

	public void OnHit(double score) {
		if (!transform.gameObject.activeSelf)
			return;
		score = (double)Mathf.Abs ((float)score);
		// now we see if the player hit the button in time
		if (score <= PlayerHit.GOOD && IsInRangeOfDestination()) {
			GetComponent<UnityEngine.UI.RawImage> ().color = Color.green;
		} else if (score > PlayerHit.GOOD && IsInRangeOfDestination()) {
			GetComponent<UnityEngine.UI.RawImage> ().color = Color.red;
		} else if (!IsInRangeOfDestination()) {
			GetComponent<UnityEngine.UI.RawImage> ().color = Color.yellow;
		}
	}

	public float DistanceFromDestination() {
		return Vector3.Distance (transform.position, destination.position);
	}

	public bool IsInRangeOfDestination() {
		return Vector3.Distance (transform.position, destination.position) <= relativeHeight + 3f;
	}
}