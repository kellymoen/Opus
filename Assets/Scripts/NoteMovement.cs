using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/** I mean, it *works*. I wouldn't say it's particularly good.*/
[RequireComponent(typeof(RawImage))]
public class NoteMovement : MonoBehaviour {
	// note: magenta = note is in range of destination
	// red = bad hit
	// green = good or better hit
	// orange/yellow = too early
	// black = too late
	public AudioSourceMetro metro;
	public Transform destination;
	public Transform origin;
	private float relativeHeight = 2f;
	private float totalDistance;
	private int index = -1;
	string button;
	private float startTime;
	private float targetTime; // = the amount of time it should take to reach destination (startTime + endTime = totalTime)
	private bool colored = false;

	void Start() {
		transform.position = origin.position;
	}

	/** Initialise should be called as soon as a NoteMovement script is created to
	 * set up the fields that will be the same throughout its lifetime:
	 * where it comes from, where it is going, where it gets the beat from, and
	 * the button it should listen out for.
	 * (This last one should be moved to StartNote once we decide whether the button
	 * to press should generate randomly or based on Track values.) */
	public void Initialise(AudioSourceMetro m, Transform destination, Transform origin, string button) {
		this.destination = destination;
		this.metro = m;
		this.origin = origin;
		this.button = button;
		totalDistance = Vector3.Distance (origin.transform.position, destination.transform.position);
	}

	/* called to enable the note and start it moving; also calls ResetNote */
	public void StartNote(float newTargetTime) {
		this.startTime = Time.time;
		ResetNote (newTargetTime);
		transform.gameObject.SetActive (true);
	}

	/* called to put note back at origin */
	public void ResetNote(float newTargetTime) {
		GetComponentInChildren<Text> ().text = "";
		colored = false;
		GetComponent<RawImage> ().color = Color.white;
		targetTime = newTargetTime;
		transform.position = origin.transform.position;
	}

	public float GetTarget() { 
		return targetTime;
	}

	// manages the note's inexorable march forward
	void Update() {
		if (!transform.gameObject.activeSelf)
			return;
		
		transform.LookAt (destination);
		//Debug.Log (destination.position+", "+transform.position+","+((totalDistance / targetTime)));
		transform.position += transform.forward * (totalDistance / targetTime) * Time.deltaTime;
		transform.position = new Vector3(transform.position.x, relativeHeight, transform.position.z);
		transform.LookAt (2 * (transform.position - Camera.main.transform.position));

		if (TimeFromDestination() == 0) {
			if (!colored)
				GetComponent<RawImage> ().color = Color.magenta; // NOW
		} else if (TimeFromDestination() < 0 - PlayerHit.BAD) {
			GetComponentInChildren<Text> ().text = "missed";
			transform.gameObject.SetActive (false);
		}
	}

	public void GreatHit() {
		GetComponent<UnityEngine.UI.RawImage> ().color = Color.green;
		colored = true;
	}

	public void GoodHit() {
		GetComponent<UnityEngine.UI.RawImage> ().color = Color.green;
		colored = true;
	}

	public void BadHit() {
		GetComponent<UnityEngine.UI.RawImage> ().color = Color.red;
		colored = true;
	}

	public void EarlyHit() {
		GetComponent<UnityEngine.UI.RawImage> ().color = Color.yellow;
		colored = true;
	}

	public float TimeFromDestination() {
		return (startTime + targetTime) - Time.time;
	}

	public bool IsInRangeOfDestination() {
		return (startTime + targetTime - PlayerHit.BAD) < Time.time;
	}
}