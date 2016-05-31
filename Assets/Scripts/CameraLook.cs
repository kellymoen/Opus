using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraLook : MonoBehaviour {
	private GameObject target;
	Vector3 offset;
	public float turnSpeed = 2.0f;
	public float damping = 1f;
	//TODO Use damping
	private GameObject player;
	private float raycastDelay = 0.1f;
	private float lastCast;

	void Start(){
		player = gameObject.transform.parent;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		target = GameObject.FindWithTag ("Player");
		offset = gameObject.transform.position - target.transform.position;
		transform.LookAt(target.transform.position);
	}

	void LateUpdate(){
		offset = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
    transform.position = target.transform.position + offset;
    transform.LookAt(target.transform.position);
		checkEsc();
		if (lastCast + raycastDelay > Time.time) {
			lastCast = Time.time;
			Ray newRay = new RayHit(
		}
	}

	void checkEsc(){
		if(Input.GetButtonDown("Cancel")){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

}
