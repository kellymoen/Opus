using UnityEngine;
using System.Collections;

/** NoteBattleScripts are (will be) created when the player enters combat with a notesprite,
 * and destroyed once the combat is over (whether successful or failed). 
 */
public class NoteBattleScript : MonoBehaviour {
	public AudioSourceMetro metro;
	public int successesToWin = 3; // number of notes to get correct in a row
	public int missesToFail = 3; // number of notes to fail in a row before it escapes
	public int delayStart = 4; // number of beats to wait before listening

	private int currentSuccesses = 0;
	private int currentFailures = 0;
	private int lastEnteredBeat = 0;

	private int beatOffset;

	// Use this for initialization
	void Start () {
		beatOffset = metro.GetBeat ();
	}

	void OnEnable() {
		if (metro == null) {
			Debug.Log ("NoteBattleScript needs a metro to count time with!");
			transform.gameObject.SetActive (false);
		}
	}

	void OnDisable() {
		PlayerHit.OnButtonPress -= Listen;
	}
	
	// Update is called once per frame
	void Update () {
		if (metro.GetBeat() - beatOffset == delayStart)
			PlayerHit.OnButtonPress += Listen;
	}

	void Listen(double score) {
		int beat = metro.GetBeat ();

		score = Mathf.Abs ((float)score);

		if (score >= PlayerHit.GOOD && lastEnteredBeat+1 == beat) {
			currentSuccesses ++;
			currentFailures = 0;
		} else {
			currentSuccesses = 0;
			currentFailures ++;
		}
		lastEnteredBeat = beat;

		if (currentSuccesses == successesToWin) {
			Debug.Log ("Success! Battle won!");
			transform.gameObject.SetActive (false);
		}

		if (currentFailures == missesToFail) {
			Debug.Log ("Failure! Battle lost :(");
			transform.gameObject.SetActive (false);
		}
	}
	/*
	public void EmitNote() {
		GameObject note = (GameObject)Instantiate (Resources.Load("LeftNote"));
		Canvas c = GameObject.FindObjectOfType<Canvas> ("BattleCanvas");
		note.transform.SetParent (c);
	} */
}
