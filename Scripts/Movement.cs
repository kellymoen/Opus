using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speed = 6.0F;
	public float turningSpeed = 500;

	private CharacterController controller;
	private Animator animator;
	private Vector3 moveDirection = Vector3.zero;

	void Start(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
	}

	void Update() {
			if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0){
				animator.SetBool("isWalking", true);
			}
			else{
				animator.SetBool("isWalking", false);
			}
			float horizontal = Input.GetAxis("Horizontal") * turningSpeed * Time.deltaTime;
			transform.Rotate(0, horizontal, 0);
			float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
			transform.Translate(0, 0, vertical);
		}



}
