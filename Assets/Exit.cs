using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Exit : MonoBehaviour {
	float delay;

	// Use this for initialization
	void Start () {
		gameObject.SetActive (false);
		GetComponent<Image>().color = new Color(255f,255f,255f,0f);
		ProgressManager.EndForest += ProgressManager_EndForest;
	}

	void ProgressManager_EndForest (bool enabled)
	{
		delay = Time.time + 4f;
		gameObject.SetActive (true);
	}

	void OnEnable() {
		gameObject.GetComponent<Image>().CrossFadeColor (Color.white, 0f, false, true);
	}

	void Update() {
		if (delay < Time.time)
			gameObject.SetActive (false);
	}
}
