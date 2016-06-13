using UnityEngine;
using System.Collections;

public class PlayOnSchedule : MonoBehaviour {

	private AudioSource[] audio;

	// Use this for initialization
	void Awake () {
		audio = GameObject.Find ("Sprites").GetComponentsInChildren<AudioSource> ();
		AudioSource metro = GameObject.Find ("Metronome").GetComponentInChildren<AudioSource> ();
		metro.PlayScheduled (AudioSettings.dspTime + 2.182f);
		metro.volume = 0;
		metro.loop = true;
		for (int i = 0; i < audio.Length; i++) {
			audio[i].PlayScheduled (AudioSettings.dspTime + 2.182f);
			audio [i].volume = 0;
			audio[i].loop = true;
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
