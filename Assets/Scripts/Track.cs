using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(AudioSource))]
public class Track : MonoBehaviour {


	private AudioSource music;
	public string filename;
	//Represents the keys in a track
	//Times stores the timing of the notes (we need to offset this by the travel time)
	private double[] times;
	//Notes stores the types of notes e.g. left, right, both..
	private int[] notes;
	//Total length of the track, need this for when the track is longer/shorter than the metro track
	private double trackLength;

	private int currentNote;

	// Use this for initialization
	void Awake () {
		music = GetComponent<AudioSource> ();
		currentNote = 0;
		//Should load track here
		LoadFromFile();
		trackLength = music.clip.length;
	}

	private void LoadFromFile(){
		StreamReader input = new StreamReader ("Assets/Tracks/" + filename + ".txt");

		ArrayList timesList = new ArrayList ();
		ArrayList notesList = new ArrayList ();
		while (!input.EndOfStream) {
			string line = input.ReadLine ();
			float time = float.Parse (line.Substring (0, 6));
			int type = int.Parse (line.Substring (7, 1));
			timesList.Add (time);
			notesList.Add (type);
		}

		times = (double[])timesList.ToArray (typeof(double));
		notes = (int[])notesList.ToArray (typeof(int));
	}

	public double NextNote(){
		currentNote = (currentNote + 1) % Mathf.Max (times.Length, notes.Length);
		return times [currentNote];
	}
		
	public double[] GetTimes(){
		return times;
	}

	public double GetNextTime(){
		return times[currentNote];
	}

	public double GetNextTimeRelative() {
		if (currentNote - 1 < 0)
			return times[times.Length-1];
		return times[currentNote] - times[currentNote-1];
	}

	/** Gets the time of note n in the future */
	public float GetNextTime(int n) {
		return (float)(times [(currentNote + n) % times.Length]);
	}

	public int[] GetNotes(){
		return notes;
	}

	public int GetNextNote(){
		return notes[currentNote];
	}

	public double GetLength(){
		return trackLength;
	}

	public double GetTrackStartTime(){
		return Time.time - music.time;
	}

	/** Gets the time from currentNote until note n */
	public double GetFutureTime(int n){
		if (currentNote + n >= times.Length) {
			int indOfN = (currentNote + n) % times.Length;
			double timeToEnd = (trackLength - times [currentNote]);
			int extraLoops = (int)(n / trackLength);
			if (extraLoops <= 0)
				extraLoops = 0;
			else
				extraLoops--;
			return (timeToEnd + times [indOfN] + extraLoops * trackLength);
		} else {
			return (times [currentNote + n] - times [currentNote]);
		}
	}
	
	/** Gets the note at n in the future (0 = current)*/
	public int GetFutureNote(int n){
		if (currentNote + n >= times.Length) {
			int indOfN = (currentNote + n) % times.Length;
			return notes[indOfN];
		} else {
			return notes[currentNote + n];
		}
	}
}
