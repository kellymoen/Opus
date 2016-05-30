using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerFoley : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!GetComponent<Animator> ().GetBool ("isWalking"))
			GetComponent<AudioSource> ().Stop ();
		else {
			if (!GetComponent<AudioSource> ().isPlaying) {
				GetComponent<AudioSource> ().Play ();
			}
		}
	}
}
