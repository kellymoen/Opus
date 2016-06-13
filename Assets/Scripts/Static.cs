using UnityEngine;
using System.Collections;

public static class Static {
	static GameObject metronome;
	static GameObject player;
	static GameObject target;
	
	public static GameObject GetPlayer() {
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player");
		return player;
	}

	public static AudioSourceMetro GetMetronome() {
		if (metronome == null) {
			metronome = GameObject.FindGameObjectWithTag ("Metronome");
		}
		if (metronome == null)
			Debug.LogError ("Metronome is null!");
		Debug.Log(metronome);
		return metronome.GetComponent<AudioSourceMetro>();
	}

	public static GameObject GetTarget() {
		if (target == null)
			target = GameObject.FindGameObjectWithTag ("Target");
		if (target == null)
			Debug.LogError ("Target is null!");
		return GameObject.FindGameObjectWithTag ("Target");
	}
}
