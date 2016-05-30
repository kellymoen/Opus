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

	private int beatCounter;

	private double lastTime;

	private double beatStartTime;
	private double nextBarStartTime;


	// Use this for initialization
	void Start () {
		beat = (int)(sound.time / BEAT_TIME);
		lastBeat = beat;
		lastTime = sound.time;
	}
	
	// Update is called once per frame
	void Update () {
		double time = sound.time;
		beat = (int)(time / BEAT_TIME);
		beatStartTime = Time.time - time + (beat * BEAT_TIME);
		nextBarStartTime = Time.time - sound.time + (BEAT_TIME * 4);

		if (beat != lastBeat) {
			lastBeat = beat;
			beatCounter++;
//			OnBeat ();
		}
		lastTime = time;
	}

	public int GetBeat(){
		return beat;
	}

	public int GetBeatCounter(){
		return beatCounter;
	}

	public int GetBarCounter(){
		return (int)(beatCounter/4);
	}

	public double GetBeatStartTime(){
		return beatStartTime;
	}

	public double GetNextBarStartTime(){
		return nextBarStartTime;
	}

	public double GetMetroTime(){
		return sound.time;
	}

	public double GetDeltaTime(){
		if (lastTime > sound.time) {
			lastTime -= 2.182;
		}
			return sound.time - lastTime;
	}
}
