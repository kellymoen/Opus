using UnityEngine;
using System.Collections;

public static class Static {
	static GameObject metronome;
	static GameObject player;
	static GameObject target;
	static GameObject manager;
	static CompositionController cc;
	public static string LB = "L Button";
	public static string RB = "R Button";
	public static string both = "both";
	
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

	public static GameObject GetProgressManager() {
		if (manager == null)
			manager = GameObject.Find ("ProgressManager");
		if (manager == null)
			Debug.LogError ("Scene needs a ProgressManager object!");
		return manager;
	}

	public static CompositionController GetCompositionController() {
		if (cc == null)
			cc = GameObject.Find ("CompositionController").GetComponent<CompositionController>();
		if (cc == null)
			Debug.LogError ("Scene needs a CompositionController object!");
		return cc;
	}
}
