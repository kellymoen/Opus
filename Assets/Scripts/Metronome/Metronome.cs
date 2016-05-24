using UnityEngine;
using System.Collections;

public delegate void MetronomeEvent (int beat); // returns void, no parameters

/** Base code courtesy of the amazing Tuxic @ cubeslam 
 * (I would paste a URL but MonoDevelop has forgotten how to do that :C) */
public class Metronome : MonoBehaviour {
	public AudioClip meep;
	[Range(1,8)]
	public int Base = 4; // beats
	[Range(1,8)]
	public int Step; // number of beats per measure
	public float BPM;
	public int CurrentStep = 1;
	public int CurrentMeasure;
	private AudioSource source;

	private float interval; // absolute # seconds between 2 beats
	private float nextTime; // next beat time, relative

	public static event MetronomeEvent OnBeat; // our event is the type of the delegate
	public event MetronomeEvent OnNewMeasure;

	/** Method for starting the metronome. 
	 * -- Is it better to have many metronomes which count in different times, or one
	 * 		big metronome which reports all of them?
	 */
	public void StartMetronome() { // expensive method?
		StopCoroutine ("DoTick"); // stop any existing coroutines
		CurrentStep = 1; // start in our first step!
		float multiplier = Base / Step; // base time division
		float tmpInterval = 60f / BPM; // 60 BPM: ((60 * 1) / x) seconds/beat
		interval = tmpInterval / multiplier; // calculations!
		nextTime = Time.time;
		StartCoroutine("DoTick");
	}

	public void Start() {
		StartMetronome ();
		source = GetComponent<AudioSource> ();
	}

	/** An infinite coroutine loop which, simply, increments currentstep and currentmeasure. */
	IEnumerator DoTick() {
		for (; ;) {
			nextTime += interval; 

			yield return new WaitForSeconds (interval);

			//EventManager.TriggerEvent ("OnBeat");

			OnBeat (CurrentStep); // if you are a n00b to delegates, as I am, let me explain
					   // how this works:
					   //  ... it's black magic. 
					   // (everything with Metronome.OnBeat += (some method) will have that method called
					   // this is some devil-related witchcraft and I LOVE IT. 

			if (source != null) {
				if (source.isPlaying)
					source.Stop ();
				source.Play (); // if you want your metronome to tick, give it an audiosource
			}

			CurrentStep++;

			if (CurrentStep > Step) {
				CurrentStep = 1;
				CurrentMeasure++;
			}
		}
	}



}