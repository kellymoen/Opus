using UnityEngine;
using System.Collections;

/** NoteBattleScripts are (will be) created/enabled when the player enters combat with a notesprite,
 * and destroyed/disabled once the combat is over (whether successful or failed). 
 */
public class NoteBattleScript : MonoBehaviour {
	public AudioSourceMetro metro;
	public Transform noteDestination;
	public Transform noteOrigin;
	public Canvas battleCanvas;
	public int successesToWin = 3; // number of notes to get correct in a row
	public int missesToFail = 12; // number of notes to fail in a row before it escapes
	public int delayStart = 4; // number of beats to wait before listening to input
	public int beatsToReachPlayer = 4; // number of beats each note takes to reach the player;
									// essentially the speed
	[Range(0.0f,16f)]
	public int maxActiveNotes = 4; // helpful but not essential!

	public Track track;

	// fields for calculating where we are in the rhythm
	private int currentSuccesses = 0;
	private int currentFailures = 0;

	private int beatOffset = 0; // counts down to start time
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
		if (noteDestination == null) {
			noteDestination = player.transform;
		}
		if (noteOrigin == null) {
			Debug.LogError ("NoteBattleScript must have a place for notes to spawn from!");
		}
	}

	void Update(){
		if (Time.time - track.GetTrackStartTime() >= track.GetNextTime ()) {
			int noteType = track.GetNextNote ();
			track.NextNote ();
			//EmitNote ();
			Debug.Log("NOTE");
		}
	}

	void OnEnable() {
		player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<Movement> ().Tether (gameObject); // TODO is this right? we'll find out soon!
		if (metro == null) {
			Debug.Log ("NoteBattleScript needs a metro to count time with!");
		}
		PlayerHit.OnButtonPress += OnPress;
		AudioSourceMetro.OnBeat += OnBeat;
		PlayerHit.OnMiss += OnMiss;
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
			beatOffset--;
		} else {
			EmitNote ();
		}
		nextMetroBeat++;
	}

	void OnMiss() {
		if (beatOffset <= 0) {
			Debug.Log ("Missed a note!" + nextMetroBeat);
			currentFailures++;
		}
		CheckEnd ();
	}

	/** When the player hits a note. */
	void OnPress(double score) {
		double beatStartTime = metro.GetBeatStartTime ();
		if (beatOffset <= 0) {
			// first make sure we didn't miss anything
			int beat = metro.GetBeat ();
			if (beat > nextMetroBeat) {
				currentFailures += beat - nextMetroBeat; // FIXME not great
			}
			playerInputIndex = playerInputIndex + 1 % activeNotes.Length - 1;
			if (activeNotes [activeNoteIndex].GetComponent<NoteMovement> ().IsInRangeOfDestination ()) {
				activeNotes [activeNoteIndex].SetActive (false);
				activeNoteIndex = (activeNoteIndex + 1 % activeNotes.Length - 1);
				Debug.Log ("active: " + activeNoteIndex);
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
	 * visual representation of the note is created. It (should)
	 * slowly move towards the player so that it arrives on another beat. */
	private void EmitNote() {
		NoteMovement currNote = activeNotes [activeNoteIndex].GetComponent<NoteMovement> ();
		if (activeNotes [activeNoteIndex].activeSelf) {
			if (currNote.IsInRangeOfDestination()) {
				currNote.gameObject.SetActive (false);
			}
		}
		else {
			currNote.StartNote (Time.time + (float)(beatsToReachPlayer * PlayerHit.HalfBeat * 2));
			activeNotes [activeNoteIndex].SetActive (true);
		}
	}

	/** Creates a note with fields initialised to metro, noteDestination, noteOrigin, and to listen
	 * to button X */
	private GameObject CreateNote() {
		GameObject note = Instantiate (Resources.Load ("Notes/LeftNote")) as GameObject;
		NoteMovement noteScript = note.GetComponent<NoteMovement> ();
		noteScript.Initialise (metro,noteDestination,noteOrigin,"X");
		noteScript.ResetNote ((float)(beatsToReachPlayer * PlayerHit.HalfBeat * 2));
		note.transform.SetParent (battleCanvas.transform);
		note.SetActive (false);
		return note;
	}


	// 
	public void OnDestroy() {
		player.GetComponent<Movement>().Untether();
		for (int i = 0; i < activeNotes.Length; i++) {
			Destroy(activeNotes[i]);
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
