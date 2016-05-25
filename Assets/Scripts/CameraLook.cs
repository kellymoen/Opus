using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraLook : MonoBehaviour {
	private GameObject target;
	Vector3 offset;
	public float turnSpeed = 2.0f;
	public float damping = 1f;


	void Start(){
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
	}

	void checkEsc(){
		if(Input.GetButtonDown("Cancel")){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

}
