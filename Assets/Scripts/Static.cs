using UnityEngine;
using System.Collections;

public class Static : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public static GameObject GetPlayer() {
		return GameObject.Find ("Kit Container");
	}

	public static AudioSourceMetro GetMetronome() {
		return GameObject.FindGameObjectWithTag ("Metronome").GetComponent<AudioSourceMetro>();
	}

	public static GameObject GetTarget() {
		return GameObject.FindGameObjectWithTag ("Target");
	}
}
