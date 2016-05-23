using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour {

	public float speed = 6.0F;
	public float jumpSpeed = 6;
	public float lookSpeed = 10;

	private Animator animator;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 curLoc;
	private Vector3 prevLoc;
	private bool tethered = false;

	void Start(){
		animator = GetComponent<Animator>();
	}

	void Update() {
		if(!tethered){
				InputCheck();
				if(CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0){
					animator.SetBool("isWalking", true);
					transform.position = curLoc;
					transform.rotation = Quaternion.Lerp (transform.rotation,  Quaternion.LookRotation(transform.position - prevLoc), Time.fixedDeltaTime * lookSpeed);
				}
				else{
					animator.SetBool("isWalking", false);
				}

			}
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton ("Jump")) {
				animator.SetTrigger ("jump");
				moveDirection.y = jumpSpeed;
			}
		}


	private void InputCheck()
	{
		prevLoc = curLoc;
		curLoc = transform.position;

		if(CrossPlatformInputManager.GetAxis("Horizontal") != 0)
			curLoc.x += CrossPlatformInputManager.GetAxis("Horizontal") * speed * Time.deltaTime;
		if(CrossPlatformInputManager.GetAxis("Vertical") != 0)
			curLoc.z += CrossPlatformInputManager.GetAxis("Vertical") * speed * Time.deltaTime;
	}

	public void Tether(GameObject sprite){
		tethered = true;
		if (animator == null) { // this shouldn't happen, but it's been throwing nulls
			animator = GetComponent<Animator>(); 
		}
		animator.SetBool ("isWalking", false);
	}

	public void Untether() {
		tethered = false;
	}
}
