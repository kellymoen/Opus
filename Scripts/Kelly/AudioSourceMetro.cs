using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioSourceMetro : MonoBehaviour {
	public AudioSource sound;
	private int beat;
	private double beatStartTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		double time = sound.time;
		beat = (int)(time / 0.5455);
		beatStartTime = Time.time - time + (beat * 0.5455);
	}

	public int GetBeat(){
		return beat;
	}

	public double GetBeatStartTime(){
		return beatStartTime;
	}
}
