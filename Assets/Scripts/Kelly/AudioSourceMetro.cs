using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void Beat();
public class AudioSourceMetro : MonoBehaviour {
	public static event Beat OnBeat;
	public readonly double BEAT_TIME = 0.5455;

	public AudioSource sound;
	private int beat;
	private int lastBeat;
	private double beatStartTime;


	// Use this for initialization
	void Start () {
		beat = (int)(sound.time / BEAT_TIME);
		lastBeat = beat;
	}
	
	// Update is called once per frame
	void Update () {
		double time = sound.time;
		beat = (int)(time / BEAT_TIME);
		beatStartTime = Time.time - time + (beat * BEAT_TIME);

		if (beat != lastBeat) {
			lastBeat = beat;
			OnBeat ();
		}
	}

	public int GetBeat(){
		return beat;
	}

	public double GetBeatStartTime(){
		return beatStartTime;
	}
}
