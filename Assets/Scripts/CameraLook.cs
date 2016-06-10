using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraLook : MonoBehaviour {
	private float minY = .1f;
	private float maxY = 60f;
	public float turnSpeed = 2.0f;
	public float damping = 1f;
	private GameObject target;
	private GameObject kitModel;
	private Vector3 offset;
	private float offsetMag;
	private GameObject player;
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
			float newY = (Quaternion.AngleAxis (Input.GetAxis("Mouse Y") * turnSpeed, Vector3.left) * offset).y;
			if(newY < minY){
				newY = minY;
			}
			else if(newY > maxY){
				newY = maxY;
			}
			offset = new Vector3(offset.x, newY, offset.z);
			Vector3 abovePos = target.transform.position;
			for(float i = 0f; i < 1f; i += .1f){
				Vector3 pos = Vector3.Lerp(target.transform.position + offset, abovePos, i);
				if(CanSeePlayer(pos)){
					return pos;
				}
			}
			return Vector3.Lerp(target.transform.position + offset, abovePos, 1f);//For the compilers sake
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
