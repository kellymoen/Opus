using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraLook : MonoBehaviour {
	private GameObject target;
	Vector3 offset;
	public float mouseSensitivity = 70f;
	private float xOffset = 0;
	private float yOffset = 0;

	public float MinimumY = 0F;
	public float MaximumY = 180F;
	public float damping = .5f;


	void Start(){

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		target = GameObject.FindWithTag ("Player");
		offset = gameObject.transform.position - target.transform.position;
		transform.LookAt(target.transform.position);
	}

	void LateUpdate() {
		//follow player
		Vector3 desiredPosition = target.transform.position + offset;
		Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
		transform.position = position;
		//Rotate according to mouse position
		updateMouseRotation();
		//float y = clampFloat(transform.position.y + yOffset, MinimumY, MaximumY);
		Vector3 desiredRotPosition = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
		transform.RotateAround(target.transform.position, desiredRotPosition, mouseSensitivity * Time.deltaTime);
		transform.LookAt(target.transform.position);
		checkEsc();
	}

	void updateMouseRotation(){
		float mouseX = CrossPlatformInputManager.GetAxis("Mouse X");
		float mouseY = CrossPlatformInputManager.GetAxis("Mouse Y");
		yOffset += mouseX *  Time.deltaTime;
		xOffset += mouseY *  Time.deltaTime;
	}

	void checkEsc(){
		if(Input.GetButtonDown("Cancel")){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	float clampFloat(float num, float min, float max){
		if(num < min){
			return min;
		}
		if(num > max){
			return max;
		}
		return num;
	}

}
