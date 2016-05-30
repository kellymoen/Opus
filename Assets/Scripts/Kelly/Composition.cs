using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Composition : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip sound;
	public AudioSourceMetro metro;
	public Circle circle;
	public GameObject noteSprite;

	public int[] bars;
	public int barsLength = 1;
	public int numBars = 2;
	float currentBar;
	double wait;
	double currentTime;
	double totalTime;
	public bool mute = false;
	public bool selected = false;
	public int selectedSegment = 0;

	public void Start ()
	{
		bars = new int[16];
		currentTime = 0;
	}

	void Update(){
		int barIndex = (metro.GetBarCounter() / numBars) % barsLength;
		totalTime = numBars * 4 * metro.BEAT_TIME * barsLength;
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
		currentTime += metro.GetDeltaTime();


		noteSprite.transform.position = circle.PositionOnCircle(currentTime/totalTime);
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
