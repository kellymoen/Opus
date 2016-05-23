using UnityEngine;
using System.Collections;

/** NoteBattleScripts are (will be) created/enabled when the player enters combat with a notesprite,
 * and destroyed/disabled once the combat is over (whether successful or failed). 
 */
public class NoteBattleScript : MonoBehaviour {
	public AudioSourceMetro metro;
	public Transform noteDestination;
	public Transform noteOrigin;
	public int successesToWin = 3; // number of notes to get correct in a row
	public int missesToFail = 12; // number of notes to fail in a row before it escapes
	public int delayStart = 4; // number of beats to wait before listening to input
	public int beatsToReachPlayer = 4; // number of beats each note takes to reach the player;
									// essentially the speed
	[Range(0.0f,16f)]
	public int maxActiveNotes = 4; // helpful but not essential!

	// fields for calculating where we are in the rhythm
	private int currentSuccesses = 0;
	private int currentFailures = 0;

	private int beatOffset; // counts down to start time
	private int nextMetroBeat = 0; // metronome beat we expect

	// fields for the notemovement scripts
	private GameObject player;
	// fields for managing our active notes
	private GameObject[] activeNotes;
	private int playerInputIndex = 0;
	private int activeNoteIndex = 0;

	// Use this for initialization
	void Start () {
		// first, make sure we know when to start
		if (delayStart < beatsToReachPlayer) {
			delayStart = 0;
		} else {
			delayStart -= beatsToReachPlayer;
		}
		beatOffset = delayStart;
		player = GameObject.FindGameObjectWithTag ("Player");
		activeNotes = new NoteMovement[maxActiveNotes];
		if (noteDestination == null) {
			noteDestination = player.transform;
		}
		if (noteOrigin == null) {
			Debug.LogError ("NoteBattleScript must have a place for notes to spawn from!");
		}
	}

	void OnEnable() {
		player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<Movement> ().Tether (gameObject); // TODO is this right? we'll find out soon!
		if (metro == null) {
			Debug.Log ("NoteBattleScript needs a metro to count time with!");
		} else {
			PlayerHit.OnButtonPress += OnPress;
			AudioSourceMetro.OnBeat += OnBeat;
			PlayerHit.OnMiss += OnMiss;
		}
		Debug.Log ("SO" +
		"\n\tsuccessesToWin = " + successesToWin +
		"\n\tmissesToFail = " + missesToFail +
		"\n\tdelayStart = " + delayStart +
		"\n\tbeatsToReachPlayer = " + beatsToReachPlayer +
		"\n\tmaxActiveNotes = " + maxActiveNotes +
		"\n\tcurrentSuccesses = " + currentSuccesses +
		"\n\tcurrentFailures = " + currentFailures +
		"\n\tbeatOffset = " + beatOffset +
		"\n\tnextMetroBeat = " + nextMetroBeat +
		"\n\tplayer = " + player +
		"\n\tactiveNotes = " + activeNotes +
		"\n\tplayerInputIndex = " + playerInputIndex +
		"\n\tactiveNoteIndex = " + activeNoteIndex);
		// some initialisation goes here
		activeNotes = new GameObject[maxActiveNotes];
		for (int i = 0; i < activeNotes.Length; i++) {
			activeNotes [i] = CreateNote ();
		}
	}

	void OnDisable() {
		PlayerHit.OnButtonPress -= OnPress;
		AudioSourceMetro.OnBeat -= OnBeat;
	}

	void OnBeat () {
		Debug.Log ("new beat with " + nextMetroBeat);
		// if we're resting, return
		if (beatOffset > 0) {
			nextMetroBeat++;
			beatOffset--;
		} else {
			EmitNote ();
		}
	}

	void OnMiss() {
		if (beatOffset <= 0) {
			Debug.Log ("Missed a note!" + nextMetroBeat);
			currentFailures++;
			nextMetroBeat = metro.GetBeat () + 1;
		}
		CheckEnd ();
	}

	/** When the player hits a note. */
	void OnPress(double score) {
		double beatStartTime = metro.GetBeatStartTime ();

		// if we're not resting
		if (beatOffset <= 0) {
			// first make sure we didn't miss anything
			int beat = metro.GetBeat ();
			if (beat > nextMetroBeat) {
				currentFailures += beat - nextMetroBeat; // FIXME not great
				//Debug.Log(currentFailures +" on "+beat+" when we expected "+nextBeat);
			}

			// now we see if the player hit the button in time
			if (score >= PlayerHit.GOOD &&
				(activeNotes [playerInputIndex] != null && activeNotes [playerInputIndex].GetComponentInChildren<NoteMovement>().IsInRangeOfDestination ())) {
				currentSuccesses++;
				//Debug.Log ("Good or better hit. " + currentSuccesses + " to " + currentFailures);
				if (activeNotes [playerInputIndex] != null)
					activeNotes [playerInputIndex].GetComponent<UnityEngine.UI.RawImage> ().color = Color.green;
				currentFailures = 0; // easy mode :P

			} else if (score < PlayerHit.GOOD) {
				currentFailures++;
				//Debug.Log ("Not good hit. " + currentSuccesses + " to " + currentFailures);
				if (activeNotes [playerInputIndex] != null)
					activeNotes [playerInputIndex].GetComponent<UnityEngine.UI.RawImage> ().color = Color.red;
				currentSuccesses = 0;
			} else {
				Debug.Log ("reutrn");
				return;
			}
			playerInputIndex = playerInputIndex+1 % activeNotes.Length-1;
			CheckEnd ();
		}
	}

	private void CheckEnd() {
		// did they win?
		if (currentSuccesses >= successesToWin) {
			Debug.Log ("Success! Battle won!");
			transform.gameObject.SetActive (false);
			OnWin ();
		}
		if (currentFailures >= missesToFail) {
			Debug.Log ("Failure! Battle lost :(");
			transform.gameObject.SetActive (false);
			OnLose ();
		}
	}

	double Round(double d) {
		return ((int)(d * 10000d))/10000d;
	}

	/* Once we have things to do when the player loses to the sprite, put them here. */
	private void OnLose () {
		player.GetComponent<Movement>().Untether();
	}

	/* Once we have things to do when the player wins against the sprite, put them here. */
	private void OnWin () {
		player.GetComponent<Movement>().Untether();
	}



	/** When a new beat is detected (rough method for now, sorry!) a new
	 * visual representation of the note is created. It (should)
	 * slowly move towards the player so that it arrives on another beat. */
	private void EmitNote() {
		if (activeNoteIndex == activeNotes.Length) {
			if (activeNotes [0] != null && activeNotes[0].gameObject.activeSelf) {
				return;
			} else {
				activeNoteIndex = 0;
			}
		}

		activeNotes [activeNoteIndex] = CreateNote ();

		activeNoteIndex++;
	}

	private GameObject CreateNote() {
		// 
		GameObject note = Instantiate (Resources.Load ("Notes/LeftNote")) as GameObject;
		NoteMovement noteScript = note.GetComponent<NoteMovement> ();
		noteScript.Set (metro,noteDestination,noteOrigin,"X");
		Canvas[] cses = GameObject.FindObjectsOfType<Canvas> ();
		Canvas c = null;
		for (int i = 0; i < cses.Length; i++) {
			if (cses [i].name.Equals ("Battle Canvas")) {
				c = cses [i];
				break; 
			}
		}
		if (c == null) {
			Debug.LogError ("Please have a 'Battle Canvas' canvas in the scene!");
		}
		note.transform.SetParent (c.transform);
		return note;
	}

	public void OnDestroy() {
		for (int i = 0; i < activeNotes.Length; i++) {
			activeNotes[i].Destroy();
		}
	}
}
