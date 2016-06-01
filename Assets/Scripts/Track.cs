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
	void Start () {
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
	public float GetFutureTime(int n){
		if (currentNote + n >= times.Length) {
			int indOfN = (currentNote + n) % times.Length;
			float timeToEnd = (float)(trackLength - times [currentNote]);
			return (float)(timeToEnd + times [indOfN]);
		} else {
			return (float)(times [currentNote + n] - times [currentNote]);
		}
	}
}
