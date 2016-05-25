using UnityEngine;
using System.Collections;

/** NoteBattleScripts are (will be) created/enabled when the player enters combat with a notesprite,
 * and destroyed/disabled once the combat is over (whether successful or failed). 
 */
public class NoteBattleScript : MonoBehaviour {
	public AudioSourceMetro metro; // where we're counting from
	public Transform noteDestination; // where the notes go
	public Transform noteOrigin; // where they come from
	public Canvas battleCanvas; // what we draw them on
	public Track track;
	private GameObject player;

	public int successesToWin = 3; // number of notes to get correct in a row
	public int missesToFail = 12; // number of notes to fail in a row before it escapes
	public int delayStart = 4; // number of beats to wait before listening to input; defaults to 0 if less than beatsToReachPlayer
	public int beatsToReachPlayer = 4; // number of beats each note takes to reach the player;
									// essentially the speed
	[Range(0.0f,16f)]
	public int maxLoadedNotes = 4; // how many notes we can have LOADED (not necessarily active) at any time
	// fields for managing our loaded notes
	private GameObject[] loadedNotes;
	private int playerInputIndex = 0;
	private int activeNoteIndex = 0;

	private int currentSuccesses = 0;
	private int currentFailures = 0;

	private int beatOffset = 0; // counts down to start time
	private int nextMetroBeat = 0; // metronome beat we expect
	private double emitNextNoteAt = 0;


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
		if (noteDestination == null) {
			noteDestination = player.transform;
		}
		if (noteOrigin == null) {
			Debug.LogError ("NoteBattleScript must have a place for notes to spawn from!");
		}
	}

	void Update(){
		if (emitNextNoteAt <= Time.time) {
			int noteType = track.GetNextNote ();
			track.NextNote ();
			// emit one note every beat; each note takes 4 beats to arrive
			EmitNote (track.GetFutureTime (4));
			emitNextNoteAt = track.GetFutureTime (1);
		}
	}

	void OnEnable() {
		player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<Movement> ().Tether (transform.gameObject);
		if (metro == null) {
			Debug.Log ("NoteBattleScript needs a metro to count time with!");
		}
		PlayerHit.OnButtonPress += OnPress;
		AudioSourceMetro.OnBeat += OnBeat;
		PlayerHit.OnMiss += OnMiss;

		// some initialisation goes here
		loadedNotes = new GameObject[maxLoadedNotes];
		for (int i = 0; i < loadedNotes.Length; i++) {
			loadedNotes [i] = CreateNote ();
		}
	}

	/** we should never disable NoteBattleScripts as they are currently, only create and destroy them.*/
	void OnDisable() {
		PlayerHit.OnButtonPress -= OnPress;
		AudioSourceMetro.OnBeat -= OnBeat;
	}

	/** OnBeat is 'called' from AudioSourceMetro */
	void OnBeat () {
		Debug.Log ("new beat with " + nextMetroBeat +" and track number "+ track.GetNextTime());
		// if we're resting, return
		if (beatOffset > 0) {
			beatOffset--;
		} else {
			track.NextNote ();
		}
		nextMetroBeat++;
	}

	/** When the player misses a beat, update failures accordingly. 
	  * (OnMiss is 'called' from PlayerHit) */
	void OnMiss() {
		if (beatOffset <= 0) {
			Debug.Log ("Missed a note!" + nextMetroBeat);
			currentFailures++;
		}
		CheckEnd ();
	}

	/** When the player successfully hits a note. */
	void OnPress(double score) {
		Debug.Log ("press");
		score = (double)Mathf.Abs ((float)score);
		// if we have started listening
		if (beatOffset <= 0) { 
			// first make sure we didn't miss anything
			int beat = metro.GetBeat ();
			NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
			if (note.IsInRangeOfDestination ()) {
				if (score >= PlayerHit.BAD) {
					note.BadHit ();
				} else {
					note.GoodHit ();
				}
				playerInputIndex = playerInputIndex + 1 % loadedNotes.Length - 1;
				Debug.Log ("active: " + activeNoteIndex +", playerIndex: "+ playerInputIndex);
			} else {
				// button press when note is out of range
			}
		}
		CheckEnd ();
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

	/** When a new beat is detected (rough method for now, sorry!) a new
	 * visual representation of the note is enabled. It (should)
	 * slowly move towards the player so that it arrives on another beat. */
	private void EmitNote(float goalTime) {
		NoteMovement currNote = loadedNotes [activeNoteIndex].GetComponent<NoteMovement> ();
		if (loadedNotes [activeNoteIndex].activeSelf) {
		}
		else {
			currNote.StartNote (goalTime);
		}
		activeNoteIndex = activeNoteIndex + 1 % loadedNotes.Length - 1;
	}

	/** Creates a note with fields initialised to metro, noteDestination, noteOrigin, and to listen
	 * to button X */
	private GameObject CreateNote() {
		GameObject note = Instantiate (Resources.Load ("Notes/Note")) as GameObject;
		NoteMovement noteScript = note.GetComponent<NoteMovement> ();
		noteScript.Initialise (metro,noteDestination,noteOrigin,"X");
		noteScript.ResetNote ((float)(beatsToReachPlayer * PlayerHit.HalfBeat * 2));
		note.transform.SetParent (battleCanvas.transform);
		note.SetActive (false);
		return note;
	}


	// 
	public void OnDestroy() {
		if (player != null) {
			player.GetComponent<Movement> ().Untether ();
		}
		for (int i = 0; i < loadedNotes.Length; i++) {
			if (loadedNotes [i] != null) {
				Destroy (loadedNotes [i]);
			}
		}
	}

	/* Once we have things to do when the player loses to the sprite, put them here. */
	private void OnLose () {
		player.GetComponent<Movement>().Untether();
	}

	/* Once we have things to do when the player wins against the sprite, put them here. */
	private void OnWin () {
		player.GetComponent<Movement>().Untether();
	}
}
