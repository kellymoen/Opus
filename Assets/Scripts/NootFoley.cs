using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class NootFoley : MonoBehaviour {
	public AudioClip[] sounds;
	public float averagePlaysEveryMinute;
	private AudioSource me;
	private float lastCheck;

	// Use this for initialization
	void Start () {
		me = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time < lastCheck + 1f)
			return;
		
		if (!me.isPlaying && Random.Range(0,60) < averagePlaysEveryMinute) {
			me.PlayOneShot (sounds [(int)(Random.Range(0,sounds.Length-1))]);
		}
	}
}
