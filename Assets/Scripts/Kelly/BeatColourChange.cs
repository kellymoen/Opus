using UnityEngine;
using System.Collections;

public class BeatColourChange : MonoBehaviour {

	public AudioSourceMetro metronome;
	public Color[] colors;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		colourise ();
	}

	private void colourise() {
		Material mat = GetComponentInChildren<MeshRenderer> ().material;
		int beat = metronome.GetBeat();
		mat.color = colors [beat];
	}
}
