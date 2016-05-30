using UnityEngine;
using System.Collections;

public class CompositionController : MonoBehaviour {
	bool paused = true;
	public Composition[] compositions;
	public GameObject sphere;
	public GameObject circle;
	public int selectedComposition = 0;
	public int axisUsed = 0;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < compositions.Length; i++) {
			//Instantiate Circles and Spheres
			compositions[i].noteSprite = Instantiate(sphere);
			compositions [i].circle = Instantiate (circle).GetComponent<Circle>();
			compositions [i].circle.xradius = 2 + i;
			compositions [i].circle.yradius = 2 + i;
			compositions [i].circle.composition = compositions [i];
			compositions [i].noteSprite.GetComponent<BeatColourChange> ().metronome = compositions[i].metro;
			if (i == selectedComposition)
				compositions [i].selected = true;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Start")) {
			paused = !paused;
		}

		if (Input.GetAxis ("Vertical") > 0) {
			if (axisUsed <= 0) {
				compositions [selectedComposition].selected = false;
				selectedComposition++;
				if (selectedComposition > compositions.Length - 1)
					selectedComposition = compositions.Length - 1;
				compositions [selectedComposition].selected = true;
				axisUsed = 10;
			} else {
				axisUsed--;
			}
		} else if (Input.GetAxis ("Vertical") < 0) {
			if (axisUsed <= 0) {
			compositions [selectedComposition].selected = false;
			selectedComposition--;
			if (selectedComposition <  0)
				selectedComposition = 0;
			compositions [selectedComposition].selected = true;
				axisUsed = 10;
			} else {
				axisUsed--;
			}
		}

		if (Input.GetAxis ("Horizontal") > 0) {
			if (axisUsed <= 0) {
				compositions [selectedComposition].moveRight ();
				axisUsed = 10;
			} else {
				axisUsed--;
			}
		} else if (Input.GetAxis ("Horizontal") < 0) {
			if (axisUsed <= 0) {
				compositions [selectedComposition].moveLeft ();
				axisUsed = 10;
			} else {
				axisUsed--;
			}
		}

		if (Input.GetButtonDown ("X Button")) {
			compositions [selectedComposition].toggle ();
		}
		if (Input.GetButtonDown ("C")) {
			compositions [selectedComposition].increaseBars ();
		}

		if (Input.GetButtonDown ("Z")) {
			compositions [selectedComposition].decreaseBars ();
		}



		if (paused) {
			AudioListener.pause = true;
		} else {
			AudioListener.pause = false;
		}
	
	}
}
