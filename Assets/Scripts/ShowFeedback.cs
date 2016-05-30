using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class ShowFeedback : MonoBehaviour {
	public float pauseFor = 3f;

	public GameObject player;

	private GameObject active;
	private float expiresAt;
	public GameObject bad;
	public GameObject good;
	public GameObject great;
	public GameObject fireflies;
	private Canvas me;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Kit Prefab");

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
	}

	private void BattleEnd() {
	}

	public void BadHit() {
		Debug.Log ("Feedback is bad.");
		Set (bad);
	}

	public void GoodHit ()	{
		Debug.Log ("Feedback is good.");
		Set (good);
	}

	public void GreatHit() {
		Debug.Log ("Feedback is great!");
		Set (great);
	}

	public void Won() {
		Unset ();
		Set (fireflies);
		expiresAt += 5f;
	}

	public void Lost() {
		Unset ();
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
