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
  public float walkRadius = 150;
  public float maxIdleTime = 15;

  void Start(){
	  player = Static.GetPlayer ();
    agent = GetComponent<NavMeshAgent>();
    animator = GetComponent<Animator>();
    originLocation = transform.position;
    assignNewGoal();
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
        if(idleStartTime ==0){
          idleStartTime = Time.time;
          idleTime = Random.Range(0, maxIdleTime);
          animator.SetBool("isWalking", false);
        }
        //if timer has ended set next goal and move again
        if(Time.time - idleStartTime >= idleTime){
          animator.SetBool("isWalking", true);
          assignNewGoal();
          //set wait time to zero
          idleStartTime = 0;
			    GetComponentInChildren<Animator> ().SetBool ("isWalking", true);
         }
      }
  }

  void assignNewGoal(){
    currentGoal = randomPosition();
    NavMeshPath path = new NavMeshPath();
    agent.CalculatePath(currentGoal, path);
    while(path.status == NavMeshPathStatus.PathPartial) {
      currentGoal = randomPosition();
      agent.CalculatePath(currentGoal, path);
    }
    agent.SetDestination(currentGoal);
  }

  Vector3 randomPosition(){
    Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
    randomDirection += transform.position;
    NavMeshHit hit;
    NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
    return hit.position;
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
