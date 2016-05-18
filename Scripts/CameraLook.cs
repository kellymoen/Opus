using UnityEngine;
using System.Collections;

public class CameraLook : MonoBehaviour {
	private GameObject target;
	Vector3 offset;
	public float damping = 1;

	void Start(){
		target = GameObject.FindWithTag ("Player");
		offset = gameObject.transform.position - target.transform.position;

	}

	void LateUpdate() {
		Vector3 desiredPosition = target.transform.position + offset;
		Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
		transform.position = position;
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		transform.LookAt(target.transform.position);
	}

}

