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

	public int[] bars = {1,0,1,0};
	public int numBars = 2;
	float currentBar;
	double wait;
	double currentTime;
	double totalTime;
	public bool mute = false;

	public void Start ()
	{
		currentTime = 0;
		totalTime = numBars * 4 * metro.BEAT_TIME * bars.Length;
	}

	void Update(){
		int barIndex = (metro.GetBarCounter() / numBars) % bars.Length;
		if (bars[barIndex] == 1) {
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
}
