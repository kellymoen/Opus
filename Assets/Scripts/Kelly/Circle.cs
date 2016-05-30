using UnityEngine;
using System.Collections;

public class Circle : MonoBehaviour
{
	public int segments;
	public float xradius;
	public float yradius;
	public GameObject line;
	public Composition composition;
	GameObject[] renderers;

	void Start ()
	{
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
		int[] elems = composition.bars;
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
			else
				renderer.material.color = HexToColor ("3A00F3FF");;
			if (elems [i] == 0) {
				renderer.SetWidth (0.1f, 0.1f);
			} else {
				renderer.SetWidth (0.5f, 0.5f);
			}
			renderer.SetVertexCount (segments + 1);
			renderer.useWorldSpace = false;
			CreatePoints (renderer, arcDegrees*i, arcDegrees, segments);
		}

		for (int i = composition.barsLength; i < 16; i++) {
			LineRenderer renderer = renderers [i].GetComponent<LineRenderer> ();
			renderer.enabled = false;
		}
	}


	void CreatePoints (LineRenderer line, float startAngle, float degrees, int segments)
	{
		float x;
		float y = 0.001f;
		float z = 0f;

		float angle = startAngle;

		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
			z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;

			line.SetPosition (i,new Vector3(x,y,z) );

			angle += (degrees / segments);
		}
	}


	public Vector3 PositionOnCircle(double t){
		float angle = 360f * (float)t;
		float x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
		float z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;
		return new Vector3 (x, 0.001f, z);
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
		return new Color32(r,g,b, 255);
	}
}