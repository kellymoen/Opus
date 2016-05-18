using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speed = 6.0F;

	private Animator animator;
	private Vector3 moveDirection = Vector3.zero;
	public float lookSpeed = 10;
	private Vector3 curLoc;
	private Vector3 prevLoc;

	void Start(){
		animator = GetComponent<Animator>();
	}

	void Update() {
			InputCheck();
			if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
				animator.SetBool("isWalking", true);
			}
			else{
				animator.SetBool("isWalking", false);
			}
			transform.rotation = Quaternion.Lerp (transform.rotation,  Quaternion.LookRotation(transform.position - prevLoc), Time.fixedDeltaTime * lookSpeed);
			
	
		}


	private void InputCheck()
	{
		prevLoc = curLoc;
		curLoc = transform.position;

		if(Input.GetAxis("Horizontal") != 0)
			curLoc.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		if(Input.GetAxis("Vertical") != 0)
			curLoc.z += Input.GetAxis("Vertical") * speed * Time.deltaTime;

		transform.position = curLoc;

	}

	public void Tether(GameObject sprite){
	}

}
