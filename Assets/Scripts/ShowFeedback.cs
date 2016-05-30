using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class ShowFeedback : MonoBehaviour {
	public float pauseFor = 3f;

	public GameObject player;

	public GameObject bad;
	public GameObject good;
	public GameObject great;
	public GameObject fireflies;
	public AudioSource hit;
	public AudioClip goodSound;
	public AudioClip badSound;
	public AudioClip winSound;
	public AudioClip lossSound;
	public AudioClip missSound;
	public AudioClip battleStartSound;
	private AudioClip queued;
	private AudioSource metro;
	private GameObject active;
	private float expiresAt;
	private Canvas me;
	private GameObject tether;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Kit Prefab");
		tether = GameObject.Find ("Feedback Canvas/Tether");
		AIBattleScript.BattleStart += BattleStart;
		AIBattleScript.BattleEnd += BattleEnd;
		AIBattleScript.GoodHit += GoodHit;
		AIBattleScript.BadHit += BadHit;
		AIBattleScript.GreatHit += GreatHit;
		AIBattleScript.Loss += Lost;
		AIBattleScript.Win += Won;

		me = GetComponent<Canvas> ();
	}

	private void BattleStart() {
		hit.PlayOneShot (battleStartSound);
	}

	private void BattleEnd() {
	}

	public void BadHit() {
		Set (bad);
		hit.PlayOneShot (badSound);
	}

	public void GoodHit ()	{
		Set (good);
		hit.PlayOneShot (goodSound);
	}

	public void GreatHit() {
		Set (great);
		hit.PlayOneShot (goodSound);
	}

	public void Miss() {
		Unset ();
		hit.PlayOneShot (missSound);
		expiresAt += 5f;
	}

	public void Won() {
		Unset ();
		Set (fireflies);
		hit.Stop ();
		hit.PlayOneShot (winSound);
		expiresAt += 5f;
	}


	public void Lost() {
		Unset ();
		hit.Stop ();
		hit.PlayOneShot (lossSound);
	}

	void Set(GameObject g) {
		expiresAt = Time.time + pauseFor;
		if (active != null)
			active.SetActive (false);
		active = g;
		active.SetActive (true);
	}

	void Unset() {
		active.SetActive (false);
		active = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > expiresAt && active != null) {
			Unset ();
		}
	}
}
