using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.RawImage))]
public class MarkerScript : MonoBehaviour {
	GameObject target;
	Camera cam;
	Quaternion rotation;

	void Start() {
		target = Static.GetTarget ();
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		GetComponent<UnityEngine.UI.RawImage> ().enabled = false;
		//gameObject.GetComponentInParent<Canvas> ().enabled = false;
		AIBattleScript.BattleStart += BattleStart;
		AIBattleScript.BattleEnd += BattleEnd;
	}

	public void BattleStart () 	{
		GetComponent<UnityEngine.UI.RawImage> ().enabled = true;
		//gameObject.GetComponentInParent<Canvas> ().enabled = true;
		transform.position = target.transform.position;
		transform.LookAt (cam.transform);
		rotation = transform.rotation;
	}

	public void BattleEnd(bool isWon) {
		GetComponent<UnityEngine.UI.RawImage> ().enabled = false;
		//gameObject.GetComponentInParent<Canvas> ().enabled = false;
	}

	void Update() {
		transform.position = target.transform.position;
		transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);;
	}
}
