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

		times = new double[]{0.5455,1.0910,1.6365};
		notes = new int[]{0,1,0};
		trackLength = music.clip.length;
	}

	private void LoadFromFile(){
		StreamReader input = new StreamReader ("Tracks/" + filename);

		ArrayList timesList = new ArrayList ();
		ArrayList notesList = new ArrayList ();
		while (!input.EndOfStream) {
			string line = input.ReadLine ();
			float time = float.Parse (line.Substring (0, 6));
			int type = int.Parse (line.Substring (8, 1));
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
		float timeCount = 0;
		int timeIdx = currentNote;
		for (int i = 0; i < n; i++) {
			timeCount += (float)times [timeIdx];
			timeIdx = (timeIdx + 1) % times.Length;
		}
		return timeCount;
	}
}
