using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionScript : MonoBehaviour {

	public Image instructions;

	[Range(120,600)]
	public int displayFrames = 360;
	[Range(0,120)]
	public int fadeOutFrames = 60;

	private int curFrame = 0;

	// Use this for initialization
	void Start () {
	}

	void Update () {
		if (curFrame < displayFrames) {
			curFrame++;
		} else if (curFrame == displayFrames) {
			instructions.CrossFadeAlpha (0, fadeOutFrames * Time.deltaTime, false);
		}
	}

	void fadeOut(float alpha){
	}
}
