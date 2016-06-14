using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AIFollowScript))]
[RequireComponent(typeof(AIManagerScript))]
public class AIRoamingScript : MonoBehaviour
{
  private Vector3 currentGoal;
  private Vector3 originLocation;
  private NavMeshAgent agent;
  private Animator animator;
  private float idleStartTime = 0;
  private float idleTime;
	private GameObject player;
  private bool movementLocked = false;

  public float minDistance = 3;
  public float radius = 20;
  public float maxIdleTime = 15;

  void Start(){
	player = Static.GetPlayer ();
    agent = GetComponent<NavMeshAgent>();
    originLocation = transform.position;
    assignNewGoal();
	animator = GetComponentInChildren<Animator> ();
  }

  void Update(){
    if(!movementLocked){
      updateMovement();
    }
  }

  void updateMovement(){
    //If at the next patrol point
      if(Vector3.Distance(transform.position, currentGoal) < minDistance){
        //If just arrived, set timer
        if(idleStartTime == 0){
          idleStartTime = Time.time;
          idleTime = Random.Range(0, maxIdleTime);
			animator.SetBool ("isWalking", false);
          //animator.SetBool("isWalking", false);
        }
        //if timer has ended set next goal and move again
        if(Time.time - idleStartTime >= idleTime){
          //animator.SetBool("isWalking", true);
          assignNewGoal();
          //set wait time to zero
           idleStartTime = 0;
			animator.SetBool ("isWalking", true);
         }
      }
	}

	public void Escape() {
		int count = 0;
		while (Vector3.Distance (currentGoal, player.transform.position) < 10 && count < 10) {
			assignNewGoal ();
			count++;
		}
	}

  void assignNewGoal(){
    currentGoal = randomPosition();
    bool successfullyAssigned = agent.SetDestination(currentGoal);
    int count = 0;
    while(!successfullyAssigned && count < 10000){
      successfullyAssigned = agent.SetDestination(currentGoal);
      count++;
    }
  }

  Vector3 randomPosition(){
    float x = Random.Range(originLocation.x - radius, originLocation.x + radius);
    float z = Random.Range(originLocation.z - radius, originLocation.z + radius);
    return new Vector3(x, originLocation.y, z);
  }

  public void setMovementLock(bool locked){
    movementLocked = locked;
    if(locked){
      agent.SetDestination(transform.position);
      gameObject.transform.LookAt(player.transform.position);
		animator.SetBool("isWalking", false);
    }
  }

}
