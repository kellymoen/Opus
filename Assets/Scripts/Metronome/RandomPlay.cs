using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RandomPlay : MonoBehaviour {
	[Range(5,100)]
	public int play = 50;
	private float lastPlayed;
	private int minWait = 5;

	void Update() {
		if (lastPlayed + minWait > Time.time)
			return;
		if (Random.value * play > 50) {
			GetComponent<AudioSource> ().Play ();
			lastPlayed = Time.time;
		}
	}
}
