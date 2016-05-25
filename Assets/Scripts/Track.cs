using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Track : MonoBehaviour {

	private AudioSource music;
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
		times = new double[]{0.5455,1.0910,1.6365};
		notes = new int[]{0,1,0};
		trackLength = music.clip.length;
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
