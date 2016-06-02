using UnityEngine;
using System.Collections;

public delegate void BattleEvent();

/** AIBattleScripts are (will be) created/enabled when the player enters combat with a notesprite,
 * and destroyed/disabled once the combat is over (whether successful or failed).
 */
[RequireComponent(typeof(Track))]
[RequireComponent(typeof(AudioSource))]
public class AIBattleScript : MonoBehaviour {

	public static event BattleEvent GoodHit;
	public static event BattleEvent BadHit;
	public static event BattleEvent GreatHit;
	public static event BattleEvent BattleEnd;
	public static event BattleEvent BattleStart;
	public static event BattleEvent Win;
	public static event BattleEvent Loss;

	public Texture[] buttons;
	public string[] buttonNames;

	private bool started = false;

	private string filename;
	public Transform noteDestination; // where the notes go
	public Transform noteOrigin; // where they come from
	public Canvas battleCanvas; // what we draw them on
	private GameObject player;
	private AudioSourceMetro metro; // where we're counting from
	private AudioSource source;
	private Animator anim;
	private Track track;

	public int successesToWin = 8; // number of notes to get correct in a row
	public int missesToFail = 12; // number of notes to fail in a row before it escapes
	//public int delayStart = 0; // number of beats to wait before listening to input; de//faults to 0 if less than beatsToReachPlayer
	public int beatsToReachPlayer = 4; // number of beats each note takes to reach the player;
									// essentially the speed
	[Range(0.0f,16f)]
	public int maxLoadedNotes = 4; // how many notes we can have LOADED (not necessarily active) at any time
	// fields for managing our loaded notes
	private GameObject[] loadedNotes;
	private string[] activeButtons;
	private int playerInputIndex = 0;
	private int activeNoteIndex = 0;

	private int currentSuccesses = 0;
	private int currentFailures = 0;

	private int beatOffset = 0; // counts down to start time
	private int nextMetroBeat = 0; // metronome beat we expect
	private double emitNextNoteAt = -1;

	void Start() {
		this.player = Static.GetPlayer ();
		this.anim = player.GetComponent<Animator> (); // FIXME animator of player controller or prefab?
		this.metro = Static.GetMetronome ();
		this.track = GetComponent<Track> ();
		this.source = GetComponent<AudioSource> ();
		//source.Play ();
		//source.loop = true;
		source.volume = 0;
		filename = track.filename;
	}

	/** Returns true if calling this method has started the battle; false otherwise. */
	public bool Begin(Transform origin, Transform destination) {
		if (!started) {
			this.source = GetComponent<AudioSource> ();
			this.player = Static.GetPlayer ();
			this.noteOrigin = origin;
			this.noteDestination = destination;
			nextMetroBeat = metro.GetBeat () + 1;
			/*
			if (delayStart < beatsToReachPlayer) {
				delayStart = 0;
			} else {
				delayStart -= beatsToReachPlayer;
			}
			beatOffset = delayStart; */

			SetCanvas ();
			// some initialisation goes here
			loadedNotes = new GameObject[maxLoadedNotes];
			for (int i = 0; i < loadedNotes.Length; i++) {
				loadedNotes [i] = CreateNote ();
			}

			track.enabled = true;
			source.enabled = true;
			source.volume = 1f;
			started = true;
			emitNextNoteAt = metro.BEAT_TIME;
			return true;
		}
		return false;
	}

	void Update(){
		if (!started)
			return;
		NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
		if (!note.gameObject.activeSelf)
			playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;
		
		if (emitNextNoteAt <= Time.time) {
			EmitNote ((float)track.GetNextTime());
			GetComponentInChildren<Animator> ().SetTrigger ("noise");	
			emitNextNoteAt = Time.time + track.GetFutureTime (1);
			track.NextNote ();
			// Debug.Log ("next @ " + emitNextNoteAt + ", index[" + activeNoteIndex + "]");
		}
		if (Input.GetButtonDown ("X Button")) {
			OnPress ();
		}
	}

	/** we should never disable AIBattleScripts as they are currently, only create and destroy them.*/
	void OnDisable() {
		//PlayerHit.OnButtonPress -= OnPress;
		AudioSourceMetro.OnBeat -= OnBeat;
	}

	/** OnBeat is 'called' from AudioSourceMetro */
	void OnBeat () {
		if (beatOffset >= 0) {
			beatOffset--;
			nextMetroBeat++;
			return;
		}
		nextMetroBeat++;
	}

	/** When the player successfully hits a note. */
	void OnPress() {
		if (!started)
			return;
		NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
		double score = note.TimeFromDestination ();
		// if we have started listening			
		if (beatOffset <= 0) { 
			// first make sure we didn't miss anything
			if (note.IsInRangeOfDestination ()) {
				if (score <= PlayerHit.GREAT) {
					GreatHit ();
					currentSuccesses++;
				} else if (score <= PlayerHit.GOOD) {
					GoodHit ();
					currentSuccesses++;
				} else if (score <= PlayerHit.BAD) {
					BadHit ();
					currentFailures++;
					currentSuccesses = 0; // HACK reset successes on every fail
				}
				playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;
				note.FadeOut (0.5f);
				Debug.Log ("We were aiming for " + (note.TargetTime () + note.StartTime ()) + ", got a button press at " + Time.time + ", and died.");
			} else {
				// button press when note is out of range
				playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;
				//currentFailures++; easy mode
				note.FadeOut (0.5f);
			}
		}
		CheckEnd ();
	}

	private void CheckEnd() {
		// did they win?
		if (currentSuccesses >= successesToWin) {
			Debug.Log ("Success! Battle won!");
			OnWin ();
			OnDestroy ();
		}
		if (currentFailures >= missesToFail) {
			Debug.Log ("Failure! Battle lost :(");
			OnLose ();
			OnDestroy ();
		}

		if (currentFailures >= missesToFail || currentSuccesses >= successesToWin) {
			for (int i = 0; i < loadedNotes.Length; i++) {
				if (loadedNotes [i] != null)
					Destroy (loadedNotes [i]);
			}
		}
	}

	/** When a new beat is detected (rough method for now, sorry!) a new
	 * visual representation of the note is enabled. It (should)
	 * slowly move towards the player so that it arrives on another beat. */
	private void EmitNote(float targetTime) {
		NoteMovement currNote = loadedNotes [activeNoteIndex].GetComponent<NoteMovement> ();
		//int idx = buttons [Random.Range (0, buttons.Length - 1)];
		currNote.StartNote (targetTime);
		anim.SetTrigger("noise");
		//currNote.StartNote (targetTime,buttons[idx]);
		//activeButtons [idx] = buttonNames [idx];
		activeNoteIndex = (activeNoteIndex + 1) % loadedNotes.Length;
	}


	/** Creates a note with fields initialised to noteDestination, noteOrigin, and to listen
	 * to button X */
	private GameObject CreateNote() {
		GameObject note = Instantiate (GameObject.Find("Note")) as GameObject;
		note.SetActive (false);
		note.transform.SetParent (battleCanvas.transform, true);
		NoteMovement noteScript = note.GetComponent<NoteMovement> ();
		noteScript.Initialise (noteDestination,noteOrigin,"X");
		return note;
	}


	public void OnDestroy() {
		if (this != null && loadedNotes != null) {
			for (int i = 0; i < loadedNotes.Length; i++) {
				if (loadedNotes [i] != null)
					Destroy (loadedNotes [i]);
			}
		}
	}

	void OnEnable() {
		//PlayerHit.OnButtonPress += OnPress;
		AudioSourceMetro.OnBeat += OnBeat;
		//PlayerHit.OnMiss += OnMiss;
	}

	/* Once we have things to do when the player loses to the sprite, put them here. */
	private void OnLose () {
		player.GetComponent<PlayerManagerScript>().endBattle(false);
		BattleEnd ();
		Loss ();
	}

	/* Once we have things to do when the player wins against the sprite, put them here. */
	private void OnWin () {
		player.GetComponent<PlayerManagerScript>().endBattle(true);
		BattleEnd ();
		GetComponentInChildren<Animator> ().SetTrigger ("capture");
		Win ();
	}

	private void SetCanvas() {
		if (battleCanvas == null) {
			Canvas[] cs = GameObject.FindObjectsOfType<Canvas> ();
			for (int i = 0; i < cs.Length; i++) {
				if (cs [i].name.Equals ("Battle Canvas")) {
					battleCanvas = cs [i];
					break;
				}
			}
			if (battleCanvas == null)
				Debug.LogError ("Please ensure the scene contains a Canvas named \"Battle Canvas\", or assign the appropriate one to this script!");// what we draw them on
		}
		if (battleCanvas == null)
			Debug.LogError ("Canvas cannot be null");
		battleCanvas.transform.LookAt (player.transform);
	}
}
