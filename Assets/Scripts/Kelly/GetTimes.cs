using UnityEngine;
using System.Collections;

public class GetTimes : MonoBehaviour {
	private GameObject[] notes;
	private double[] times;
	private Track track;
	public int maxLoadedNotes = 4; // how many notes we can have LOADED (not necessarily active) at any time
	// fields for managing our loaded notes
	private GameObject[] loadedNotes;
	private double emitNextNoteAt;
	private int activeNoteIndex = 0;
	public Transform a;
	public Transform b;
	// Use this for initialization
	void Start () {
		track = GetComponent<Track> ();
		times = track.GetTimes ();
		loadedNotes = new GameObject[times.Length];
		emitNextNoteAt = times[activeNoteIndex] + 1.091f;
		for (int i = 0; i < loadedNotes.Length; i++) {
			loadedNotes [i] = CreateNote ();
		}
	}

	private GameObject CreateNote() {
		GameObject note = Instantiate (GameObject.Find("Note")) as GameObject;
		GameObject battleCanvas = GameObject.Find ("Battle Canvas");
		note.SetActive (false);
		note.transform.SetParent (battleCanvas.transform, true);
		NoteMovement noteScript = note.GetComponent<NoteMovement> ();
		noteScript.Initialise (a,b);
		return note;
	}
	/** When a new beat is detected (rough method for now, sorry!) a new
	 * visual representation of the note is enabled. */
	private NoteMovement EmitNote() {
		double targetTime = Time.time + 1.091f;
		NoteMovement currNote = loadedNotes [activeNoteIndex].GetComponent<NoteMovement> ();
		int futureNote = 1;
		currNote.StartNote (targetTime, futureNote, TextureCheck(futureNote));
		activeNoteIndex = (activeNoteIndex + 1) % loadedNotes.Length;
		track.NextNote ();
		return currNote;
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


	// Update is called once per frame
	void Update () {
		if (emitNextNoteAt <= Time.time) {
			NoteMovement emitted = EmitNote ();
			emitNextNoteAt = times[activeNoteIndex] + 1.091f;
		}
	}
}
