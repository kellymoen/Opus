using UnityEngine;
using System.Collections;

public class NewCamera : MonoBehaviour {
	private float x;
	private float y;
	enum  CameraState{Explore, Battle, Compose};
	public float xSpeed;
	public float ySpeed;
	public float yMinLimit;
	public float yMaxLimit;
	public float height, FollowDistance;
	public Transform target;
	public GameObject composePos;
	public GameObject battlePos;
	private CameraState camType = CameraState.Explore;
	private float composeFOV;
	private float battleFOV;
	private Camera cam;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		composeFOV = composePos.GetComponent<Camera> ().fieldOfView;
		battleFOV = battlePos.GetComponent<Camera> ().fieldOfView;
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		x -= Input.GetAxis("Mouse X") * xSpeed * 0.02f;
		y += Input.GetAxis("Mouse Y") * ySpeed * 0.02f;


		y += Input.GetAxis ("RVertical") * ySpeed * 0.02f;
		x -= Input.GetAxis ("RHorizontal") * xSpeed * 0.02f;

		y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
		if (camType == CameraState.Explore) {
			Vector3 position = getPos ();
			transform.position = Vector3.Lerp (transform.position, position, 4.0f * Time.deltaTime);
			cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, composeFOV, 4.0f * Time.deltaTime);
			transform.LookAt (target.position + new Vector3 (0, height, 0));
		} else if (camType == CameraState.Battle) {
			transform.position = Vector3.Lerp (transform.position, battlePos.transform.position, 2.0f * Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, battlePos.transform.rotation, 2.0f * Time.deltaTime);
			cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, battleFOV, 2.0f * Time.deltaTime);
		} else {
			transform.position = Vector3.Lerp (transform.position, composePos.transform.position, 2.0f * Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, composePos.transform.rotation, 2.0f * Time.deltaTime);
			cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, composeFOV, 2.0f * Time.deltaTime);
		}
	}

	Vector3 getPos(){
		Quaternion rotation = Quaternion.Euler(-y, x, 0f);
		transform.rotation = rotation;
		Vector3 position = rotation * new Vector3(0f, height, FollowDistance) + target.position;
		Vector3 direction = Vector3.Normalize (target.position - position);
		Vector3 pos = new Vector3 ();
		for(float i = 0f; i < 1f; i += .1f){
			pos = Vector3.Lerp(position, target.position + new Vector3(0, height, 0), i);
			if(CanSeePlayer(pos)){
				return new Vector3 (pos.x, Mathf.Max (pos.y, position.y), pos.z);
			}
		}
		return new Vector3 (pos.x, Mathf.Max (pos.y, position.y), pos.z);
	}

	/*Vector3 CanSeePlayer(Vector3 viewPoint){
		RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (viewPoint, target.position - viewPoint, out hit, 1000)) {
			Vector3 direction = Vector3.Normalize (target.position - viewPoint);
			while (hit.transform.position != target.position) {
				if (Physics.Raycast (viewPoint, target.position - viewPoint, out hit, 1000)) {
					viewPoint.x += direction.x;
					viewPoint.z += direction.z;
				}
			}
		}
		return viewPoint;
	}*/

	bool CanSeePlayer(Vector3 viewPoint){
		RaycastHit hit;
		if(Physics.Raycast(viewPoint, target.position - viewPoint, out hit, (target.position-viewPoint).magnitude + 10f)){
			if(hit.transform != target){
				return false;
			}
		}
		return true;
	}

	public void EnableComposeCam(){
		camType = CameraState.Compose;
	}

	public void EnableExploreCam(){
		camType = CameraState.Explore;
	}

	public void EnableBattleCam(){
		camType = CameraState.Battle;
	}
}
