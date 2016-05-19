using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraLook : MonoBehaviour {
	private GameObject target;
	Vector3 offset;
	public float XSensitivity = 2f;
	public float YSensitivity = 2f;
	private float xOffset = 0;
	private float yOffset = 0;

	public float mouseSensitivity = 100.0f;
	public float clampAngle = 80.0f;
	public float damping = 1;


	void Start(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		target = GameObject.FindWithTag ("Player");
		offset = gameObject.transform.position - target.transform.position;

	}

	void LateUpdate() {
		//follow player
		Vector3 desiredPosition = target.transform.position + offset;
		Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
		transform.position = position;
		//Rotate according to mouse position
		updateMouseRotation();
		Vector3 desiredRotPosition = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
		transform.RotateAround(target.transform.position, desiredRotPosition, mouseSensitivity * Time.deltaTime);
		transform.LookAt(target.transform.position);
		checkEsc();
	}

	void updateMouseRotation(){
		float mouseX = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
		float mouseY = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
		yOffset += mouseX *  Time.deltaTime;
		xOffset += mouseY *  Time.deltaTime;
	}

	void checkEsc(){
		if(Input.GetButtonDown("Cancel")){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

}
