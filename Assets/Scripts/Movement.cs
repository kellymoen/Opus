using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour {

	public float speed = 3.0F;
	public float jumpSpeed = 2f;
	public float lookSpeed = 2f;
	private CharacterController controller;
	private Animator animator;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 curLoc;
	private Vector3 prevLoc;
	private bool tethered = false;

	void Start(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
	}

	void Update() {
		if(!tethered){
				UpdateInput();
				if(CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0){
					animator.SetBool("isWalking", true);
					RotateRelativeToCamera(moveDirection, Camera.main);
					controller.SimpleMove(transform.TransformDirection(Vector3.forward) * speed);
				}
				else{
					animator.SetBool("isWalking", false);
				}
			}
			if (Input.GetButton ("Jump")) {
				animator.SetTrigger ("jump");
				moveDirection.y = jumpSpeed;
			}
		}


	private void UpdateInput()
	{
		moveDirection = Vector3.zero;
		if(CrossPlatformInputManager.GetAxis("Horizontal") > 0){
			moveDirection += Vector3.right;
		}
		if(CrossPlatformInputManager.GetAxis("Horizontal") < 0){
			moveDirection += Vector3.left;
		}
		if(CrossPlatformInputManager.GetAxis("Vertical") > 0){
			moveDirection += Vector3.forward;
		}
		if(CrossPlatformInputManager.GetAxis("Vertical") < 0){
			moveDirection += Vector3.back;
		}
	}

	private void RotateRelativeToCamera(Vector3 direction, Camera cam) {
     // rotate given direction by the camera's rotation
     Vector3 camDir = cam.transform.rotation * direction;
     // add result to object's location to get relative direction
     Vector3 objectDir = transform.position + camDir;
     // create quaternion facing direction
     Quaternion targetRotation = Quaternion.LookRotation(objectDir - transform.position);
     // constrain rotation to the Y axis
     Quaternion constrained = Quaternion.Euler(0.0f, targetRotation.eulerAngles.y, 0.0f);
     // slerp rotation
     transform.rotation = Quaternion.Slerp(transform.rotation, constrained, Time.deltaTime * lookSpeed);
 }

	public void Tether(GameObject sprite){
		animator = GetComponent<Animator>();
		tethered = true;
		gameObject.transform.LookAt(sprite.transform.position);
		animator.SetBool("isWalking", false);
	}
	public void Untether(GameObject sprite){
		tethered = false;
	}

	public void Untether() {
		tethered = false;
	}
}
