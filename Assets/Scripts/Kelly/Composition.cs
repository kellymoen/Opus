using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Composition : MonoBehaviour
{
	public AudioSource audioSource;
	//public AudioClip sound;
	private AudioSourceMetro metro;
	public Circle circle;
	public GameObject noteSprite;
	private Track track;

	public int[] bars;
	public int barsLength = 1;
	public int numBars = 2;
	float currentBar;
	double wait;
	double currentTime;
	double totalTime;

	public bool mute = false;
	public bool selected = false;

	private bool beatSet = false;


	public int selectedSegment = 0;

	public void Start ()
	{
		bars = new int[16];
		currentTime = 0;
		audioSource = GetComponent<AudioSource> ();
		track = GetComponent<Track> ();

		totalTime = audioSource.clip.length;
	}

	public void SetBeats(){
		if (track != null && track.enabled == true) {
			double[] beats = track.GetTimes ();
			for (int i =0; i < beats.Length; i++){
				double beat = beats [i];
				totalTime = audioSource.clip.length;
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				cube.transform.position = circle.PositionOnCircle ((float)(beat / totalTime));
			}
		} else {
			Debug.Log ("NOT ACTIVE YET");
		}
	}

	void Update(){
		if (circle == null || noteSprite == null)
			return;
		while (metro == null) {
			metro = GameObject.FindGameObjectWithTag ("Metronome").GetComponent<AudioSourceMetro> ();
		}

		if (!beatSet) {
			SetBeats ();
			beatSet = true;
		}
		totalTime = audioSource.clip.length * barsLength;

		numBars = (int)(audioSource.clip.length / 2.181f);

			
		int barIndex = (metro.GetBarCounter() / numBars) % barsLength;
		if (bars[barIndex] == 0) {
			mute = true;
		} else {
			mute = false;
		}
		if (mute) {
			audioSource.volume = 0;
		} else {
			audioSource.volume = 1;
		}
		totalTime = audioSource.clip.length * barsLength;
		currentTime += metro.GetDeltaTime();
		noteSprite.transform.position = circle.PositionOnCircle((float)(currentTime/totalTime));
		noteSprite.transform.rotation = Quaternion.Euler (0, 90.0f + 360.0f * (float)(currentTime / totalTime),0);
	}

	public void moveLeft(){
			selectedSegment--;
		if (selectedSegment < 0) {
			selectedSegment = barsLength - 1;
		}
	}

	public void moveRight(){
			selectedSegment++;

		if (selectedSegment > barsLength - 1) {
			selectedSegment = 0;
		}
	}

	public void increaseBars(){
		if (barsLength < 16) {
			barsLength *= 2;
		}
	}

	public void decreaseBars(){
		if (barsLength > 1) {
			barsLength /= 2;
		}
	}

	public void toggle(){
		if (bars [selectedSegment] == 0)
			bars [selectedSegment] = 1;
		else 
			bars [selectedSegment] = 0;
	}
}
