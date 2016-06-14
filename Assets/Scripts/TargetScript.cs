using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AIBattleScript.BattleStart += Begin;
		AIBattleScript.BattleEnd += End;
	}

	private void End(bool win) {
		GetComponent<MeshRenderer> ().enabled = false;
	}

	private void Begin() {
		GetComponent<MeshRenderer> ().enabled = true;
	}
}
