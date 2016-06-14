using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour {
	GameObject fade;
	public string desertScene;

	// Use this for initialization
	void Start () {
		fade = Static.GetProgressManager();
		if (desertScene == "")
			desertScene = "desert";
		ProgressManager.EndForest += ProgressManager_EndForest;
	}

	void ProgressManager_EndForest (bool enabled)
	{
		Debug.Log ("hi");
		StartCoroutine (ChangeLevel());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator ChangeLevel() {
		float fadeTime = fade.GetComponent<FadeOut> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene (desertScene);
	}
}
