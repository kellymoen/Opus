using UnityEngine;
using System.Collections;

public delegate void BattleEvent();
public delegate void BattleStop(bool isWin);

/** AIBattleScripts are (will be) created/enabled when the player enters combat with a notesprite,
 * and destroyed/disabled once the combat is over (whether successful or failed).
 */
[RequireComponent(typeof(Track))]
[RequireComponent(typeof(AudioSource))]
public class AIBattleScript : MonoBehaviour {

	public static event BattleEvent GoodHit;
	public static event BattleEvent BadHit;
	public static event BattleEvent GreatHit;
	public static event BattleStop BattleEnd;
	public static event BattleEvent BattleStart;
	public static event BattleEvent OnMiss;

	public Texture[] buttons;
	public string[] buttonNames;
	public Canvas battleCanvas; // what we draw them on
	public int successesToWin = 8; // number of notes to get correct in a row
	public int missesToFail = 12; // number of notes to fail in a row before it escapes
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
	private double metroTrackOffset; // how far out the next metro is from the next track
	private static double cheatLevel;

	private int currentSuccesses = 0;
	private int currentFailures = 0;

	// kill these variables later
	private int beatOffset = 0; // counts down to start time
	private int nextMetroBeat = 0; // metronome beat we expect
	private double emitNextNoteAt = -1;
	private double trackStartTime; // == Time.time+beatsToReachPlayer*BEAT_TIME

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
		this.anim = GetComponentInChildren<Animator> ();
		this.source = GetComponent<AudioSource> ();
		this.track = GetComponent<Track> ();
		AudioSourceMetro.OnBeat += OnBeat;
		source.volume = 0;
		filename = track.filename;
	}

	/** Returns true if calling this method has started the battle; false otherwise. */
	public bool Begin(Transform origin, Transform destination) {
		if (!started) {
			if (BattleStart != null)
				BattleStart ();
			this.source = GetComponent<AudioSource> ();
			this.player = Static.GetPlayer ();
			this.noteOrigin = origin;
			this.noteDestination = destination;
			this.metro = Static.GetMetronome ();
			this.track = GetComponent<Track> ();
			nextMetroBeat = metro.GetBeat () + 1;
			trackStartTime = metro.GetBeatStartTime() + (1 * metro.BEAT_TIME);
			emitNextNoteAt = track.GetFutureTime(1) + track.GetTrackStartTime() + 1.091f;
			SetCanvas ();
			// some initialisation goes here
			loadedNotes = new GameObject[maxLoadedNotes];
			for (int i = 0; i < loadedNotes.Length; i++) {
				loadedNotes [i] = CreateNote ();
			}
			started = true;
			return true;
		}
		return false;
	}

	void Update(){
		if (Input.GetKeyDown ("y")) {
			Debug.Log ("CHEAT MODE " + cheatLevel);
			cheatLevel += 0.005;
		}
		if (!started)
			return;
		// OnBeat handles the audio playing
		NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
		// when starting play
		if (source.volume == 0) {
			source.volume = 1;
		}
		if (source.volume == 0 || !source.isPlaying)
			return;
		// emit a new note if it is time to do so
		if (emitNextNoteAt <= Time.time) {
			NoteMovement emitted = EmitNote ();
			emitNextNoteAt = Time.time + track.GetFutureTime(1);
		}
	
		if (Input.GetButtonDown (Static.LB) || Input.GetButtonDown(Static.RB)) {
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

	public void OnBeat () {
		if (!started)
			return;
		if (beatOffset >= 0) {
			beatOffset--;
			nextMetroBeat = metro.GetBeat () + 1;
			return;
		}
		// finally
		nextMetroBeat = metro.GetBeat () + 1;
		metroTrackOffset = (metro.GetBeatStartTime () + metro.BEAT_TIME) - track.GetNextTimeRelative ();
	}

	/** When the player tries to hit a note. */
	void OnPress() {
		if (!started)
			return;
		player.GetComponent<Animator>().SetTrigger("noise");
		// if we have started listening			
		if (beatOffset <= 0) {
			NoteMovement note = loadedNotes [playerInputIndex].GetComponent<NoteMovement> ();
			// first make sure we didn't miss anything
			if (note.IsInRangeOfDestination () && ButtonCheck(note.GetButton())) {
				double score = Abs (note.TimeFromDestination ());
				if (score <= PlayerHit.GREAT + cheatLevel) {
					GreatHit ();
					currentSuccesses++;
				} else {//if (score <= PlayerHit.GOOD + cheatLevel) {
					GoodHit ();
					currentSuccesses++;
				} //else if (score <= PlayerHit.BAD) {
				//	BadHit ();
				//	currentFailures++;
				//	currentSuccesses = (currentSuccesses - 1 >= 0) ? currentSuccesses-1 : 0; // straight-up ending their streak seemed a little harsh
			//	}
				playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;
				note.FadeOut (0.5f);
			} else {
				// button press when note is out of range
				playerInputIndex = (playerInputIndex + 1) % loadedNotes.Length;
				currentFailures++;
				OnMiss();
				note.Missed ();
				note.FadeOut (0.5f);
			}
		}
		CheckEnd ();
	}

	/** Decides whether the button press is good. 
	We treat 1 = X, 2 = Y, and anything greater than those as 'both at the same time' */
	private bool ButtonCheck(int note) {
		return true;
		if ((note > 2) && Input.GetButtonDown (Static.LB) && Input.GetButtonDown (Static.RB)) {
			Debug.Log ("Yes, it's a double button!" + note);
			return true;
		} else if ((note == 1) && Input.GetButtonDown (Static.LB)) {
			Debug.Log ("Yes, it's a left button!"+ note);
			return true;
		} else if ((note == 2) && Input.GetButtonDown (Static.RB)) {
			Debug.Log ("Yes, it's a right button!"+ note);
			return true;
		}
		Debug.Log ("Nope, no buttons here.");
		return false;
	}

	/** Gets the right button texture for the note. */
	private Texture TextureCheck(int note) {
		if ((note > 2) && Input.GetButtonDown (Static.LB) && Input.GetButtonDown (Static.RB))
			return NoteMovement.bothTexture;
		else if ((note == 1) && Input.GetButtonDown (Static.LB))
			return NoteMovement.leftTexture;
		else
			return NoteMovement.rightTexture;
	}

	private void CheckEnd() {
		// did they win?
		if (currentSuccesses >= successesToWin) {
			OnWin ();
			OnDestroy ();
		}
		if (currentFailures >= missesToFail) {
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
		double targetTime = Time.time + 1.091f;
		NoteMovement currNote = loadedNotes [activeNoteIndex].GetComponent<NoteMovement> ();
		int futureNote = 1;
		currNote.StartNote (targetTime, futureNote, TextureCheck(futureNote));
		anim.SetTrigger("noise");
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
		noteScript.Initialise (noteDestination,noteOrigin);
		return note;
	}


	public void OnDestroy() {
		if (this != null && loadedNotes != null) {
			for (int i = 0; i < loadedNotes.Length; i++) {
				if (loadedNotes [i] != null)
					Destroy (loadedNotes [i]);
			}
		}
		this.enabled = false;
	}

	/* Once we have things to do when the player loses to the sprite, put them here. */
	private void OnLose () {
		player.GetComponent<PlayerManagerScript>().endBattle(false);
		source.volume = 0;
		source.Stop ();
		source.loop = false;
		BattleEnd (false);
	}

	/* Once we have things to do when the player wins against the sprite, put them here. */
	private void OnWin () {
		player.GetComponent<PlayerManagerScript>().endBattle(true);
		anim.SetTrigger ("capture");
		BattleEnd (true);
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
			if (battleCanvas == null) {
				Debug.LogError ("Please ensure the scene contains a Canvas named \"Battle Canvas\", or assign the appropriate one to this script!");// what we draw them on
			}
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
