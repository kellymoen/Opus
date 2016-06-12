using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ButtonSFX : MonoBehaviour {
	public AudioClip confirm;
	public AudioClip back;
	public AudioClip swap; 

	public void Confirm() {
		GetComponent<AudioSource>().PlayOneShot(confirm);
	}

	public void Back() {
		GetComponent<AudioSource>().PlayOneShot(back);
	}

	public void Switch() {
		GetComponent<AudioSource>().PlayOneShot(swap);
	}
}
