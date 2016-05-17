using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHit : MonoBehaviour {

	public AudioSourceMetro metronome;
	public Text display;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.X)) {
			double diff = Time.time - metronome.GetBeatStartTime ();
			if (diff > 0.2728) //greater than a half beat means the player is likely early
				diff -= 0.5455;
			Debug.Log (diff);
			if (diff < -0.08)
				display.text = "BAD: EARLY";
			else if (diff < -0.02)
				display.text = "GOOD: EARLY";
			else if (diff < 0)
				display.text = "GREAT: EARLY";
			else if (diff > 0.08)
				display.text = "BAD: LATE";
			else if (diff > 0.02)
				display.text = "GOOD: LATE";
			else if (diff > 0.00)
				display.text = "GREAT: LATE";
			
		}
	}
}
