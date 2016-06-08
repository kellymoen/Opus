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
	public Canvas battleCanvas; // what we draw them on
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
	private bool started = false;

	private int currentSuccesses = 0;
	private int currentFailures = 0;

	// kill these variables later
	private int beatOffset = 0; // counts down to start time
	private int nextMetroBeat = 0; // metronome beat we expect
	private double emitNextNoteAt = -1;
	private double trackOffset; // == beatsToReachPlayer*BEAT_TIME

	private GameObject player;
	private AudioSourceMetro metro; // where we're counting from
	private AudioSource source;
	private Animator anim;
	private Track track;
	private Transform noteDestination; // where the notes go
	private Transform noteOrigin; // where they come from
	private string filename;

	void Start() {
		this.player = Static.GetPlayer ();
		this.metro = Static.GetMetronome ();
		this.anim = player.GetComponent<Animator> (); // FIXME animator of player controller or prefab?
		this.track = GetComponent<Track> ();
		this.source = GetComponent<AudioSource> ();
		source.volume = 0;
		filename = track.filename;
	}

	/** Returns true if calling this method has started the battle; false otherwise. */
	public bool Begin(Transform origin, Transform destination) {
		if (!started) {
			BattleStart ();
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


			return true;
		}
		return false;
	}

	void Update(){
		if (!started)
			return;
		// OnBeat handles the audio playing
		NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
		// emit a new note if it is time to do so
		if (emitNextNoteAt <= Time.time) {
			NoteMovement emitted = EmitNote ();
			emitNextNoteAt = metro.GetBeatStartTime () + metro.BEAT_TIME;
		}
		if (Input.GetButtonDown ("X Button")) {
			OnPress ();
		}
		if (note.TimeFromDestination () <= -PlayerHit.BAD) {
			note.FadeOut (0.02f);
			playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;	
		}
	}
		
	void OnDisable() {
		//PlayerHit.OnButtonPress -= OnPress;
		AudioSourceMetro.OnBeat -= OnBeat;
		started = false; // forces Begin() when reusing BattleScript
		OnDestroy ();
	}

	void OnEnable() {
		AudioSourceMetro.OnBeat += OnBeat;
		nextMetroBeat = Static.GetMetronome().GetBeat() + 1;
		//PlayerHit.OnMiss += OnMiss;
		//PlayerHit.OnButtonPress += OnPress;
	}		

	void OnBeat () {
		if (!started)
			return;
		if (beatOffset >= 0) {
			beatOffset--;
			nextMetroBeat = metro.GetBeat () + 1;
			return;
		}
		if (beatOffset == -1) {
			// this will only be true when we first start the battle
			if (source.volume == 0f) {
				track.enabled = true;
				source.enabled = true;
				source.loop = true;
				source.volume = 1f;
				source.Play ();
			}
			beatOffset = -2;
		}
		// finally
		nextMetroBeat = metro.GetBeat () + 1;
	}

	/** When the player successfully hits a note. */
	void OnPress() {
		if (!started)
			return;
		// if we have started listening			
		if (beatOffset <= 0) { 
			NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
			// first make sure we didn't miss anything
			if (note.IsInRangeOfDestination ()) {
				double score = Abs (note.TimeFromDestination ());
				Debug.Log ("Note is in range @ " + Time.time + ", score = " + score);
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
				Debug.Log ("GOAL["+playerInputIndex+"]:   "+ (note.TargetTime () + note.StartTime ()) + 
						 "\nACTUAL: "+ (score <= PlayerHit.GOOD ? "good or better":"bad") +
					     "\n@ TIME: " + Time.time);
			} else {
				Debug.Log ("================================" +
					"\nGOAL["+playerInputIndex+"]:   "+ (note.TargetTime () + note.StartTime ()) + 
					     "\n@ TIME: " + Time.time);
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
	 * visual representation of the note is enabled. */
	private NoteMovement EmitNote() {
		double targetTime = track.GetFutureTime(1);
		NoteMovement currNote = loadedNotes [activeNoteIndex].GetComponent<NoteMovement> ();
		//int idx = buttons [Random.Range (0, buttons.Length - 1)];
		currNote.StartNote (targetTime);
		anim.SetTrigger("noise");
		//currNote.StartNote (targetTime,buttons[idx]);
		//activeButtons [idx] = buttonNames [idx];
		activeNoteIndex = (activeNoteIndex + 1) % loadedNotes.Length;
		track.NextNote ();
		return currNote;
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

	private double Abs(double val) {
		if (val < 0)
			return 0-val;
		return val;
	}
}
