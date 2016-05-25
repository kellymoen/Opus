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
		renderers = new GameObject[elems.Length];
		for (int i = 0; i < elems.Length; i++) {
			int arcDegrees = 360 / elems.Length;
			int segments = 100 / elems.Length;
			renderers [i] = Instantiate(line);
			LineRenderer renderer = renderers [i].GetComponent<LineRenderer> ();
			if (elems [i] == 1) {
				renderer.SetWidth (0.1f, 0.1f);
			} else {
				renderer.SetWidth (0.5f, 0.5f);
			}
			renderer.SetVertexCount (segments + 1);
			renderer.useWorldSpace = false;
			CreatePoints (renderer, arcDegrees*i, arcDegrees, segments);
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
}