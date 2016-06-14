using UnityEngine;
using System.Collections;

public class NewMovement : MonoBehaviour {
	private float x;
	private float y;
	public float speed;
	public Transform camera;
	private Animator animator;
	private CharacterController controller;
	public float gravity;

	private bool cheats;

	private bool movementLocked = false;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		ProgressManager.EnableCheats += ProgressManager_EnableCheats;
	}


	void ProgressManager_EnableCheats (bool enabled) {
		cheats = enabled;
		if (enabled) {
			animator.speed *= 4;
			speed *= 4;
		} else {
			animator.speed /= 4;
			speed /= 4;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 direction = transform.position - camera.position;
		float angle = 90.0f-(Mathf.Atan2 (direction.z, direction.x) * 180 / Mathf.PI);
		x = Input.GetAxis("Horizontal") * speed;
		y = Input.GetAxis("Vertical") * speed;
		float directionAngle = 90.0f - (Mathf.Atan2 (y, x) * 180 / Mathf.PI);
		//Debug.Log (directionAngle);
		Vector3 movement = new Vector3 (x, 0, y);
		//Debug.Log (movement.magnitude);
		if (movement.magnitude > 5 && !movementLocked) {
			animator.SetBool ("isWalking", true);
			Quaternion rotation = Quaternion.Euler (0, angle, 0);
			movement = rotation * movement;
			//movement = camera.TransformDirection (movement);
			movement.y -= gravity;
			transform.rotation = Quaternion.AngleAxis (directionAngle, Vector3.up) * Quaternion.Euler (0, angle, 0);
			controller.Move(movement * Time.fixedDeltaTime);
		} else {
			animator.SetBool ("isWalking", false);
		}
	}

	public void setMovementLock(bool locked){
		movementLocked = locked;
		if(locked){
			animator.SetBool("isWalking", false);
		}
	}
}
