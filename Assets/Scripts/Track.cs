using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour {

	//This should probably be changed to retrieve the audio from the object it is attached to
	public AudioClip music;
	//Represents the keys in a track
	//Times stores the timing of the notes (we need to offset this by the travel time)
	private double[] times;
	//Notes stores the types of notes e.g. left, right, both..
	private int[] notes;
	//Total length of the track, need this for when the track is longer/shorter than the metro track
	private double trackLength;

	// Use this for initialization
	void Start () {
		//Should load track here
		times = new double[]{0.5455,1.0910,1.6365};
		notes = new int[]{0,1,0};
		trackLength = music.length;
	}

	public double[] GetTimes(){
		return times;
	}

	public int[] GetNotes(){
		return notes;
	}

	public double GetLength(){
		return trackLength;
	}
}
