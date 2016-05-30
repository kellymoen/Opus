using UnityEngine;
using System.Collections;

public class CompositionController : MonoBehaviour {
	bool paused = true;
	public Composition[] compositions;
	public GameObject sphere;
	public GameObject circle;
	private AudioSourceMetro metro;
	public int selectedComposition = 0;
	public int axisUsed = 0;
	public int nextIndex = 0;

	// Use this for initialization
	void Start () {
		Initialise ();
		//gameObject.transform.localPosition = new Vector3 (transform.position.x, transform.position.y, -2f);
	}

	void Initialise() {
		metro = GameObject.FindGameObjectWithTag ("Metronome").GetComponent<AudioSourceMetro>();
		for (int i = 0; i < compositions.Length; i++) {
			//Instantiate Circles and Spheres
			if (compositions [i] != null) {
				compositions [i].noteSprite = Instantiate (sphere);
				compositions [i].circle = Instantiate (circle).GetComponent<Circle> ();
				compositions [i].circle.xradius = 2 + i;
				compositions [i].circle.yradius = 2 + i;
				compositions [i].circle.composition = compositions [i];
				compositions [i].noteSprite.GetComponent<BeatColourChange> ().metronome = metro;
				compositions [i].gameObject.SetActive (false);
				if (i == selectedComposition)
					compositions [i].selected = true;
				nextIndex++;
			}
		}
		Debug.Log ("initialised controller");
	}
	
	// Update is called once per frame
	void Update () {
		if (!enabled)
			return;
		
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

	public void ShowComposition() {
		if (enabled) {
			for (int i = 0; i < compositions.Length; i++) {
				compositions [i].enabled = true;
				if (i == selectedComposition)
					compositions [i].selected = true;
			}
		}
	}

	public void HideComposition() {
		if (enabled) {
			for (int i = 0; i < compositions.Length; i++) {
				compositions [i].enabled = false;
				compositions [i].selected = false;
			}
		}
	}

	private Composition MakeCompositionObject(GameObject noot) {
		metro = GameObject.FindGameObjectWithTag ("Metronome").GetComponent<AudioSourceMetro>();
		Composition nootComp = noot.AddComponent<Composition> ();
		nootComp.noteSprite = Instantiate (sphere);
		nootComp.circle = Instantiate (circle).GetComponent<Circle> ();
		nootComp.circle.xradius = 2 + nextIndex;
		nootComp.circle.yradius = 2 + nextIndex;
		nootComp.circle.composition = nootComp;
		nootComp.noteSprite.GetComponent<BeatColourChange> ().metronome = metro;
		noot.gameObject.SetActive (false);
		noot.transform.SetParent (transform);
		return noot.GetComponent<Composition> ();
	}

	public Composition AddNoot(GameObject noot) {
		if (nextIndex < compositions.Length) {
			compositions [nextIndex] = MakeCompositionObject (noot);
			return compositions [nextIndex++].GetComponent<Composition>();
		}
		Debug.LogError ("Composition is full!");
		return null;
	}
}
