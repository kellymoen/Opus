using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraLook : MonoBehaviour {

	public float turnSpeed = 2.0f;
	public float damping = 1f;
	private GameObject player;
	private GameObject target;
	private GameObject kitModel;
	private Vector3 offset;
	private float offsetMag;
	private float raycastDelay = 0.1f;
	private float lastCast;

	void Start(){
		player = gameObject.transform.parent.gameObject;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		kitModel = GameObject.FindWithTag ("Player");
		target = GameObject.FindWithTag ("CameraFocus");
		offset = gameObject.transform.position - target.transform.position;
		offsetMag = offset.magnitude - 0.5f;
		transform.LookAt(target.transform.position);
	}

	void LateUpdate(){
		Vector3 newPos = GetNewCameraPos();
		// Lerp the camera's position between it's current position and it's new position.
		transform.position = Vector3.Lerp(transform.position, newPos, damping * Time.deltaTime);
		transform.LookAt(target.transform.position);

		checkEsc();
		if (lastCast + raycastDelay > Time.time) {
			lastCast = Time.time;
		}
	}

	void checkEsc(){
		if(Input.GetButtonDown("Cancel")){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	Vector3 GetNewCameraPos(){
		offset = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
		Vector3 abovePos = target.transform.position;
		Vector3[] checkPoints = new Vector3[5];
		checkPoints[0] = Vector3.Lerp(target.transform.position + offset, abovePos, 0f);
		checkPoints[1] = Vector3.Lerp(target.transform.position + offset, abovePos, 0.25f);
		checkPoints[2] = Vector3.Lerp(target.transform.position + offset, abovePos, 0.5f);
		checkPoints[3] = Vector3.Lerp(target.transform.position + offset, abovePos, 0.75f);
		checkPoints[4] = Vector3.Lerp(target.transform.position + offset, abovePos, 0.9f);
		for(int i = 0; i < checkPoints.Length; i++){
			if(CanSeePlayer(checkPoints[i])){
				return checkPoints[i];
			}
		}
		return checkPoints[4];//For the compilers sake
	}

	bool CanSeePlayer(Vector3 viewPoint){
		RaycastHit hit;
		if(Physics.Raycast(viewPoint, kitModel.transform.position - viewPoint, out hit, offsetMag)){
		  if(hit.transform != kitModel.transform){
		    return false;
			}
		}
		return true;
	}
}
