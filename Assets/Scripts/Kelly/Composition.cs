using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Composition : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip sound;
	public AudioSourceMetro metro;

	public int[] bars = {1,0,1,0};
	public int numBars = 2;
	float currentBar;
	double wait;
	public bool mute = false;

	public void Start ()
	{
		audioSource=this.GetComponent<AudioSource>();
		wait = metro.GetNextBarStartTime();
	}

	void Update(){
		int barIndex = (metro.GetBarCounter () / numBars) % bars.Length;
		if (bars[barIndex] == 1) {
			mute = true;
		} else {
			mute = false;
		}
		if (mute) {
			audioSource.volume = 0;
		} else {
			audioSource.volume = 1;
		}
	}
}


