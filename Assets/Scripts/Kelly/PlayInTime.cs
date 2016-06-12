using UnityEngine;
using System.Collections;

public class PlayInTime : MonoBehaviour {

	public AudioClip myAudioClip;
	[Range(0f,1f)]
	public float gain;

	private AudioSource myAudioSource;
	private int clipSize;
	private float[] buffer;

	// these allow me to create rhythms
	private int j = 0;
	private float onOff = 0;

	int sample;

	void Start(){

		myAudioSource = GetComponent<AudioSource> ();

		clipSize = myAudioClip.samples;

		//this creates buffer large enough to store whole clip
		buffer = new float[clipSize];


		//this will fill whole buffer with audio.
		myAudioClip.GetData(buffer, 0);

	}

	void OnAudioFilterRead(float[] data, int channels){

		//this loops sample
		//for some reason I need to add 370 samples on the end or it breaks...
		sample %= clipSize;

		int end = Mathf.Min (data.Length, clipSize - sample);

		for (int i = 0; i < data.Length; i++) {
			data [i] = (buffer [(i + sample)%clipSize] * gain);
		}

		sample += 2048;
		//here's the important thing. adds the length of the buffer written each time


	}
}
