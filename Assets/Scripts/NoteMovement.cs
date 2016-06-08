﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/** I mean, it *works*. I wouldn't say it's particularly good.*/
[RequireComponent(typeof(RawImage))]
public class NoteMovement : MonoBehaviour {
	// note: grey = note is in range of destination
	// red = bad hit
	// orange/yellow = too early
	// green = good or better hit
	// black = too late
	private Vector3 destination;
	private Vector3 origin;
	private Quaternion rotation;
	private float relativeHeight = 4f;
	private double startTime;
	private double targetTime; // = the amount of time it should take to reach destination (startTime + targetTime = endTime)
	private float totalDistance;
	string button;
	private bool colored = false;
	private bool fadeout = false;
	public GameObject cam;
	public Color great = new Color (237, 214, 108, 255);
	public Color good = new Color (255, 131, 131, 255);
	public Color bad = Color.black;
	private float enteredRangeAt;

	/** Initialise should be called as soon as a NoteMovement script is created to
	 * set up the fields that will be the same throughout its lifetime:
	 * where it comes from, where it is going, where it gets the beat from, and
	 * the button it should listen out for.
	 * (This last one should be moved to StartNote once we decide whether the button
	 * to press should generate randomly or based on Track values.) */
	public void Initialise(Transform destination, Transform origin, string button) {
		if (destination == null || origin == null)
			Debug.LogError ("Cannot assign a null destination/origin.");
		this.button = button;
		this.destination = new Vector3(destination.position.x, (destination.position.y+relativeHeight), destination.position.z);
		this.origin = new Vector3 (origin.position.x, (destination.position.y+relativeHeight), origin.position.z);
		transform.position = origin.position;
		totalDistance = Mathf.Abs(Vector3.Distance (destination.transform.position, origin.transform.position));
	}

	/* called to put note back at origin and start it moving again */
	//public void StartNote (float newTargetTime, RawImage button) {
	public void StartNote (double newTargetTime) {
		transform.position = origin;
		GetComponent<RawImage> ().color = new Color (255, 255, 255, 255);
		gameObject.GetComponent<CanvasRenderer> ().SetColor (Color.white);
		startTime = Time.time;
		targetTime = newTargetTime;
		colored = false;
		fadeout = false;
		gameObject.SetActive (true);
		enteredRangeAt = 0;
		transform.LookAt (cam.transform);
		rotation = transform.rotation;
	}

	public double GetTarget() { 
		return targetTime;
	}

	// manages the note's inexorable march forward
	void Update() {
		// check if we are active
		if ((fadeout && GetComponent<RawImage> ().color.a > 0)
			|| destination == null 
			|| gameObject.activeSelf == false 
			|| !isActiveAndEnabled) {
			return;
		} else if (GetComponent<RawImage> ().color.a == 0) {
			gameObject.SetActive (false);
			return;
		}

		if (targetTime == 0) {
			targetTime = 0.000000001f;
		}
		transform.LookAt (destination);
		transform.position += transform.forward * (totalDistance / (float)targetTime) * Time.deltaTime;
		transform.rotation = rotation;

		if (TimeFromDestination() < PlayerHit.BAD + PlayerHit.BAD*0.5 + 0.3) {
		//	if (!colored)
		//		GetComponent<RawImage> ().color = Color.grey; // NOW
		}

		if (enteredRangeAt == 0 && Vector3.Distance(transform.position,destination) <= 0.001f)
			enteredRangeAt = Time.time;
	}

	public void GreatHit() {
		if (fadeout || colored == true)
			return;
		ChangeColor (great);
	}

	public void GoodHit() {
		if (fadeout || colored == true)
			return;
		ChangeColor (good);
	}

	public void BadHit() {
		if (fadeout || colored == true)
			return;
		ChangeColor (bad);
	}

	public void EarlyHit() {
		if (fadeout)
			return;
		GetComponent<RawImage> ().color = Color.yellow;
	}

	private void ChangeColor(Color c) {
		GetComponent<RawImage> ().color = c;
		colored = true;
	}

	public double TimeFromDestination() {
		return (startTime + targetTime) - Time.time;
	}

	public bool IsInRangeOfDestination() {
		if (fadeout || !gameObject.activeSelf)
			return false;
		return (startTime + targetTime - PlayerHit.BAD) <= Time.time
			&& Time.time < startTime + targetTime + PlayerHit.BAD;
	}

	public void FadeOut(float seconds) {
		fadeout = true;
		GetComponent<RawImage> ().CrossFadeAlpha (0, seconds, false);
	}

	public bool isGoingOut() {
		return fadeout;
	}

	public bool hasStarted() {
		return gameObject.activeSelf && !colored && !fadeout;
	}

	public double TargetTime() {
		return targetTime;
	}

	public double StartTime() {
		return startTime;
	}

	public Vector3 Destination() {
		return destination;
	}
}