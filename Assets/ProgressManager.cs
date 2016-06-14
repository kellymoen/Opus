using UnityEngine;
using System.Collections;

public delegate void OpusEvent(bool enabled);

public class ProgressManager : MonoBehaviour {
	public static event OpusEvent EnableCheats;
	public static event OpusEvent EndForest;

	private bool enabledCheats;
	private ArrayList wildCritters;
	private PlayerCritterManager manager;
	private GameObject player;
	private int cheatEnter;
	private bool inBattle;

	// Use this for initialization
	void Start () {
		wildCritters = new ArrayList ();
		GameObject[] critters = GameObject.FindGameObjectsWithTag ("Critter");
		for (int i = 0; i < critters.Length; i++) {
			wildCritters.Add (critters [i]);
		}
		player = Static.GetPlayer ();
		manager = player.GetComponent<PlayerCritterManager> ();
		manager.AddCritterEvent += NewCritter;
		AIBattleScript.BattleStart += BattleStart;
		AIBattleScript.BattleEnd += BattleEnd;
	}

	private void NewCritter(GameObject o) {
		wildCritters.Remove (o);
		if (wildCritters.Count == 0) {
			EndGame ();
		}
	}

	void BattleStart () {
		inBattle = true;
	}

	void BattleEnd (bool win) {
		inBattle = false;
	}

	/** Cheat mode = A, Left trigger, Right trigger when not in battle.
		Enables automatic catch. */
	void Update() {
		if (!inBattle) {
			if (Input.GetButtonDown ("A Button")) {
				cheatEnter = 1;
				Debug.Log ("a pressed");
			}

			if (cheatEnter == 1 && Input.GetButtonDown ("L Button")) {
				cheatEnter = 2;
			} else if (Input.GetButtonDown ("L Button")) {
				cheatEnter = 0;
				Debug.Log ("reset");
			}
			
			if (cheatEnter == 2 && Input.GetButtonDown ("R Button")) {
				cheatEnter = 0;
				enabledCheats = !enabledCheats;
				Debug.Log ("Dev hacks " + (enabledCheats ? "enabled" : "disabled") + "!");
				if (EnableCheats != null)
					EnableCheats (enabledCheats);
			} else if (Input.GetButtonDown ("R Button")) {
				cheatEnter = 0;
				Debug.Log ("missed on the last hurdle");
			}
		}
	}

	private void EndGame() {
		Debug.LogError ("End Game");
		if (EndForest != null)
			EndForest (enabledCheats);
	}

}
