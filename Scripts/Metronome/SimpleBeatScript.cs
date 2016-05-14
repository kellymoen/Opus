using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/** This is an example script which plays a sound every onBeat Metronome beats. */
[RequireComponent(typeof(AudioSource))]
public class SimpleBeatScript : MonoBehaviour {
	[Range(1,4)]
	public int onBeat = 2; // every onBeat beats, play a note
	public bool isPlayingFully = false;
	[Range(0f,1f)]
	public float loopTransitionTime; // if a beat occurs this many seconds before the end
										// of the clip, don't wait for the clip to end--restart it
	int beatCount = 0; // how many beats have we listened to?
	int measureCount = 0;
	bool pause = false;
	AudioSource source;

	public Color[] colorLoop;

	public void Start() {
		if (colorLoop == null || colorLoop.Length == 0)
			colorLoop = new Color[] { Color.red, Color.yellow, 
			Color.green, Color.blue, Color.white };
		source = GetComponent<AudioSource> ();
	}

	void OnDisable() {
		Metronome.OnBeat -= Play; // unsubscribe to the event
	}

	void OnEnable() {
		Metronome.OnBeat += Play; // resubscribe to the event 
	}

	void Play(int beat) {
		if (isPlayingFully && source.isPlaying) {
			if (source.time > source.clip.length - loopTransitionTime)
				source.Play ();
		} else {
			if (beat % onBeat == 0) {
				if (source.isPlaying) { // we'll be a little more sophisticated than this, I promise.
					source.Stop ();
				}
				if (!pause) {
					source.Play ();
					colourise ();
				}
			}
			beat++;
		}
		beatCount++;
		if (beatCount > 4) {
			measureCount++;
		}
	}


	// just messing around with materials
	private void colourise() {
		Material mat = GetComponentInChildren<MeshRenderer> ().material;
		if (mat == null)
			return;
		mat.color = colorLoop [beatCount % colorLoop.Length];
	}
}