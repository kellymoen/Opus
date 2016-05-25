using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void GetQuality(double diff);
public delegate void Event();

public class PlayerHit : MonoBehaviour {
	public static event GetQuality OnButtonPress;
	public static event Event OnMiss;
//	public static event OnHit OnBad;
//	public static event OnHit OnGood;
//	public static event OnHit OnGreat;

	public static double BAD = 0.15;
	public static double GOOD = 0.06;
	public static double GREAT = 0;
	public static double HalfBeat = 0.2728;
	private int nextBeat = 0;

	public AudioSourceMetro metronome;
	public Text display;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("X Button")) {
			double diff = Time.time - metronome.GetBeatStartTime ();
			if (nextBeat < metronome.GetBeat ()) {
				nextBeat = metronome.GetBeat ();
				OnMiss ();
			} else if (nextBeat == metronome.GetBeat ()) {
				nextBeat++;
			}
			diff -= 0.080; // 30ms delay, 
			if (diff > 0.2728) //greater than a half beat means the player is likely early
				diff -= 0.5455;
			Debug.Log (diff);
			if (diff < -BAD) {
				display.text = "BAD: EARLY";
				//	OnBad ();
				if (OnButtonPress != null)
					OnButtonPress(-BAD);
			} else if (diff < -GOOD) {
				display.text = "GOOD: EARLY";
				//	OnGood ();
				if (OnButtonPress != null)
					OnButtonPress (-GOOD);
			} else if (diff < GREAT) {
				display.text = "GREAT: EARLY";
				//	OnGreat ();
				if (OnButtonPress != null)
					OnButtonPress (-GREAT);
			} else if (diff > BAD) {
				display.text = "BAD: LATE";
				//	OnBad ();
				if (OnButtonPress != null)
					OnButtonPress (BAD);
			} else if (diff > GOOD) {
				display.text = "GOOD: LATE";
				//	OnGood ();
				if (OnButtonPress != null)
					OnButtonPress (GOOD);
			} else if (diff > GREAT) {
				display.text = "GREAT: LATE";
				//	OnGreat ();
				if (OnButtonPress != null)
					OnButtonPress (GREAT);
			}
		}
	}
}
