using UnityEngine;
using System.Collections;

/** source: http://answers.unity3d.com/questions/52656/how-i-can-create-an-sprite-that-always-look-at-the.html */
public class Billboard : MonoBehaviour {


	void LateUpdate() {
		transform.LookAt(Camera.main.transform.position, -Vector3.up);
	}
}