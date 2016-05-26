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
	private double emitNextNoteAt = -1;


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
		OnEnable ();
	}

	void OnEnable() {
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

	void Update(){
		 if (emitNextNoteAt <= Time.time) {
			EmitNote (track.GetFutureTime(4));
			emitNextNoteAt = Time.time + track.GetFutureTime (1);
			activeNoteIndex = (activeNoteIndex + 1) % loadedNotes.Length;
			track.NextNote ();
			Debug.Log ("next @ " + emitNextNoteAt + ", index[" + activeNoteIndex + "]");
		}
	}

	/** we should never disable NoteBattleScripts as they are currently, only create and destroy them.*/
	void OnDisable() {
		PlayerHit.OnButtonPress -= OnPress;
		AudioSourceMetro.OnBeat -= OnBeat;
	}

	/** OnBeat is 'called' from AudioSourceMetro */
	void OnBeat () {
		// if we're resting, return
		if (beatOffset > 0) {
			beatOffset--;
		} else {
		}
		nextMetroBeat++;
	}

	/** When the player misses a beat, update failures accordingly. 
	  * (OnMiss is 'called' from PlayerHit) */
	void OnMiss() {
		/*if (beatOffset <= 0) {
			Debug.Log ("Missed note "+ playerInputIndex +".");
			currentFailures++;
		}
		CheckEnd (); */
	}

	/** When the player successfully hits a note. */
	void OnPress(double score) {
		if (score < 0) {
			score *= -1;
		}
		// if we have started listening
		if (beatOffset <= 0) { 
			// first make sure we didn't miss anything
			int beat = metro.GetBeat ();
			NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
			if (note.IsInRangeOfDestination ()) {
				if (score <= PlayerHit.GREAT) {
					note.GreatHit ();
					currentSuccesses++;
				} else if (score <= PlayerHit.GOOD) {
					note.GoodHit ();
					currentSuccesses++;
				} else if (score <= PlayerHit.BAD) {
					note.BadHit ();
					currentFailures++;
				}
				playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;
				note.FadeOut (1);
			} else {
				// button press when note is out of range
				playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;
				currentFailures++;
				note.FadeOut (1);
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

	/** When a new beat is detected (rough method for now, sorry!) a new
	 * visual representation of the note is enabled. It (should)
	 * slowly move towards the player so that it arrives on another beat. */
	private void EmitNote(float targetTime) {
		NoteMovement currNote = loadedNotes [activeNoteIndex].GetComponent<NoteMovement> ();
		currNote.StartNote (targetTime);
	}

	/** Creates a note with fields initialised to noteDestination, noteOrigin, and to listen
	 * to button X */
	private GameObject CreateNote() {
		GameObject note = Instantiate (Resources.Load ("Notes/Note")) as GameObject;
		note.transform.SetParent (battleCanvas.transform);
		NoteMovement noteScript = note.GetComponent<NoteMovement> ();
		noteScript.Initialise (noteDestination,noteOrigin,"X");
		return note;
	}


	public void OnDestroy() {
		player.GetComponent<PlayerManagerScript>().endBattle(true);
		for (int i = 0; i < loadedNotes.Length; i++) {
			Destroy(loadedNotes[i]);
		}
	}

	/* Once we have things to do when the player loses to the sprite, put them here. */
	private void OnLose () {
		player.GetComponent<PlayerManagerScript>().endBattle(false);
	}

	/* Once we have things to do when the player wins against the sprite, put them here. */
	private void OnWin () {
		player.GetComponent<PlayerManagerScript>().endBattle(true);
	}
}
