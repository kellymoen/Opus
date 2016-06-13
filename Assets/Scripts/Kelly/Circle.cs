using UnityEngine;
using System.Collections;

public class Circle : MonoBehaviour
{
	public float scale = 1;
	public int segments;
	public float xradius;
	public float yradius;
	public GameObject line;
	public Composition composition;
	GameObject[] renderers;
	private Transform player;
	private Vector3 playerPos;
	private bool displayCircle;

	void Start ()
	{
		if (GameObject.Find ("Kit Prefab") != null) {
			playerPos = GameObject.Find ("Kit Container").transform.position;
			player = GameObject.Find ("Kit Container").transform;
		} else {
			playerPos = new Vector3 (0, 0, 0);
		}
		int[] elems = composition.bars;
		renderers = new GameObject[16];
		for (int i = 0; i < 16; i++) {
			renderers [i] = Instantiate(line);
			renderers [i].GetComponent<LineRenderer>().enabled = false;
		}
		for (int i = 0; i < composition.barsLength; i++) {
			float arcDegrees = 360.0f / (float)composition.barsLength;
			int segments = 100 / composition.barsLength;
			LineRenderer renderer = renderers [i].GetComponent<LineRenderer> ();
			renderer.enabled = true;
			if (composition.selected) {
				if (composition.selectedSegment == i) {
					renderer.material.color = new Color (1.0f, 0.0f, 0.0f);
				} else {
					renderer.material.color = new Color (1.0f, 1.0f, 1.0f);
				}
			}
			if (elems [i] == 0) {
				renderer.SetWidth (0.1f, 0.1f);
			} else {
				renderer.SetWidth (0.5f, 0.5f);
			}
			renderer.SetVertexCount (segments + 1);
			renderer.useWorldSpace = false;
			CreatePoints (renderer, arcDegrees*i, arcDegrees, segments);
		}
	}

	void Update(){
		if (GameObject.Find ("Kit Container") != null) {
			playerPos = GameObject.Find ("Kit Container").transform.position;
			player = GameObject.Find ("Kit Container").transform;
		} else {
			playerPos = new Vector3 (0, 0, 0);
		}
		if (displayCircle) {
			int[] elems = composition.bars;
			for (int i = 0; i < composition.barsLength; i++) {
				float arcDegrees = 360.0f / (float)composition.barsLength;
				int segments = 100 / composition.barsLength;
				LineRenderer renderer = renderers [i].GetComponent<LineRenderer> ();
				renderer.enabled = true;
				if (composition.selected) {
					if (composition.selectedSegment == i) {
						renderer.material.color = new Color (1.0f, 0.0f, 0.0f, 0.6f);
					} else {
						renderer.material.color = new Color (1.0f, 1.0f, 1.0f, 0.6f);
					}
				} else
					renderer.material.color = HexToColor ("3A00F380");
				;
				if (elems [i] == 0) {
					renderer.SetWidth (0.1f * scale, 0.1f * scale);
				} else {
					renderer.SetWidth (0.2f * scale, 0.2f * scale);
				}
				renderer.SetVertexCount (segments + 1);
				renderer.useWorldSpace = false;
				CreatePoints (renderer, arcDegrees * i, arcDegrees, segments);
			}

			for (int i = composition.barsLength; i < 16; i++) {
				LineRenderer renderer = renderers [i].GetComponent<LineRenderer> ();
				renderer.enabled = false;
			}
		} else {
			for (int i = 0; i < 16; i++){ 
				LineRenderer renderer = renderers [i].GetComponent<LineRenderer> ();
				renderer.enabled = false;
			}
		}
	}


	void CreatePoints (LineRenderer line, float startAngle, float degrees, int segments)
	{
		float x;
		float y = 10.001f;
		float z = 0f;

		float angle = startAngle;

		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
			z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;
			Vector3 position = new Vector3 (x,y,z);
			line.SetPosition (i, player.TransformDirection(position) + playerPos );

			angle += (degrees / segments);
		}
	}


	public Vector3 PositionOnCircle(float t){
		float angle = 360.0f * (float)t;
		float x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
		float z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;
		Vector3 position = player.TransformDirection(new Vector3 (x, 10.001f, z)) + playerPos;
		return position;
	}

	//Author: mvi
	// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
	string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		byte a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32 (r, g, b, a);
	}


	public void ShowCircle(){
		this.displayCircle = true;
	}

	public void HideCircle(){
		this.displayCircle = false;
	}
}