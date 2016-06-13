using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.RawImage))]
public class MarkerScript : MonoBehaviour {
	GameObject target;
	Camera cam;

	void Start() {
		target = Static.GetTarget ();
		cam = GameObject.FindGameObjectWithTag ("BattleCamera").GetComponent<Camera>();
		GetComponent<UnityEngine.UI.RawImage> ().enabled = false;
		AIBattleScript.BattleStart += BattleStart;
		AIBattleScript.BattleEnd += BattleEnd;
	}

	public void BattleStart () 	{
		GetComponent<UnityEngine.UI.RawImage> ().enabled = true;
		transform.position = target.transform.position;
		transform.LookAt (cam.transform);
	}

	public void BattleEnd(bool isWon) {
		GetComponent<UnityEngine.UI.RawImage> ().enabled = false;
	}

	void Update() {
		transform.position = target.transform.position;
		transform.LookAt (cam.transform);
	}
}
