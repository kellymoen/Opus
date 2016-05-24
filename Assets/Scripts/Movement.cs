using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour {

	public float speed = 6.0F;
	public float jumpSpeed = 2f;
	public float lookSpeed = 2f;
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
