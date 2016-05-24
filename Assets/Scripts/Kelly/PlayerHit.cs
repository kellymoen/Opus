using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void GetQuality(double diff);
public delegate void OnHit();

public class PlayerHit : MonoBehaviour {
	public static event GetQuality OnButtonPress;
//	public static event OnHit OnBad;
//	public static event OnHit OnGood;
//	public static event OnHit OnGreat;

	public static double BAD = 0.1;
	public static double GOOD = 0.03;
	public static double GREAT = 0;

	public AudioSourceMetro metronome;
	public Text display;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.X)) {
			double diff = Time.time - metronome.GetBeatStartTime ();
			diff -= 0.03; // 30ms delay, 
			if (diff > 0.2728) //greater than a half beat means the player is likely early
				diff -= 0.5455;
			Debug.Log (diff);
			if (diff < -BAD) {
				display.text = "BAD: EARLY";
//				OnBad ();
				OnButtonPress (BAD);
			} else if (diff < -GOOD) {
				display.text = "GOOD: EARLY";
//				OnGood ();
				OnButtonPress (GOOD);
			} else if (diff < GREAT) {
				display.text = "GREAT: EARLY";
//				OnGreat ();
				OnButtonPress (GREAT);
			} else if (diff > BAD) {
				display.text = "BAD: LATE";
//				OnBad ();
				OnButtonPress (BAD);
			} else if (diff > GOOD) {
				display.text = "GOOD: LATE";
//				OnGood ();
				OnButtonPress (GOOD);
			} else if (diff > GREAT) {
				display.text = "GREAT: LATE";
//				OnGreat ();
				OnButtonPress (GREAT);
			}
		}
	}
}
