using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CompositionLoader : MonoBehaviour {
	private GameObject player;
	public PlayerCritterManager manager;
	private CompositionController cc;
	private bool canLoad = false;
	public string compositionKey;

	// Use this for initialization
	void Start () {
		if (compositionKey.Length == 0)
			compositionKey = "F";
		cc = GetComponentInChildren<CompositionController> ();
		cc.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!canLoad && this.enabled && manager.CritterLength() > 0) {
			canLoad = true;
		}

		if (canLoad && CrossPlatformInputManager.GetButtonDown (compositionKey)) {
			Debug.Log ("Loading composition.");
			cc.ShowComposition ();
		}
	}
}
