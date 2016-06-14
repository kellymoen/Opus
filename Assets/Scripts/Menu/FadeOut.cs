using UnityEngine;
using System.Collections;

/** from unity3d.com/learn/tutorials/topics/graphics/fading-between-scenes */
public class FadeOut : MonoBehaviour {
	public Texture2D fadeoutTexture;
	public float fadeSpeed;

	private int drawDepth = -1000;
	private float alpha = 1f;
	private int fadeDir = -1;

	// Use this for initialization
	void OnGUI() {
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01 (alpha);

		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeoutTexture);

	}

	//
	public float BeginFade(int direction) {
		fadeDir = direction;
		return fadeSpeed;
	}

	void OnLevelWasLoaded() {
		BeginFade (-1);
	}
}
