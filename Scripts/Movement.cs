using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour {

	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	public Transform spawnPoint;

	private CharacterController controller;
	private Animator animator;
	private Vector3 moveDirection = Vector3.zero;
	private Camera camera;

	void Start(){
		controller = GetComponent<CharacterController>();
		  camera = Camera.main;
		animator = GetComponent<Animator>();
	}

	void Update() {
		if (controller.isGrounded) {
			if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0){
				animator.SetBool("isWalking", true);
			}
			else{
				animator.SetBool("isWalking", false);
			}
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton ("Jump")) {
				animator.SetTrigger ("jump");
				moveDirection.y = jumpSpeed;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}

	public void respawn(){
		transform.position = spawnPoint.position;
	}


}
