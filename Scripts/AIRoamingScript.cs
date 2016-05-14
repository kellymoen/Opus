using UnityEngine;

public class AIRoamingScript : MonoBehaviour
{
  private Vector3 currentGoal;
  private Vector3 originLocation;
  private NavMeshAgent agent;
  private Animator animator;
  private float idleStartTime = 0;
  private float idleTime;
  private bool roaming = true;

  public float minDistance = 3;
  public float radius = 20;
  public float maxIdleTime = 15;

  void Start(){
    agent = GetComponent<NavMeshAgent>();
    //animator = GetComponent<Animator>();
    originLocation = transform.position;
    currentGoal = randomPosition();
    bool successfullyAssigned = agent.SetDestination(currentGoal);
    while(!successfullyAssigned){
      agent.SetDestination(currentGoal);
    }
  }

  void Update(){
    if(roaming){
      //Debug.Log("Current position: " + transform.position);
      //Debug.Log("Distance " + Vector3.Distance(transform.position, currentGoal));
      //Debug.Log("Current: " + transform.position + " Goal: " + currentGoal);

      //If at the next patrol point
        if(Vector3.Distance(transform.position, currentGoal) < minDistance){
          //If just arrived, set timer
          if(idleStartTime ==0){
            idleStartTime = Time.time;
            idleTime = Random.Range(0, maxIdleTime);
            //animator.SetBool("isWalking", false);
          }
          //if timer has ended set next goal and move again
          if(Time.time - idleStartTime >= idleTime){
            //animator.SetBool("isWalking", true);
            currentGoal = randomPosition();
            bool successfullyAssigned = agent.SetDestination(currentGoal);
            while(!successfullyAssigned){
              agent.SetDestination(currentGoal);
            }
             //set wait time to zero
             idleStartTime = 0;
           }
           else{
             //Debug.Log("Waiting:");
             //Debug.Log("Wait time: " + (Time.time - idleStartTime));
             //Debug.Log("Target time: " + idleTime);
           }
         }
       }
  }

  Vector3 randomPosition(){
    float x = Random.Range(originLocation.x - radius, originLocation.x + radius);
    float z = Random.Range(originLocation.z - radius, originLocation.z + radius);
    return new Vector3(x, originLocation.y, z);
  }
  
  public void setRoaming(bool isRoaming){
    roaming = isRoaming;
    //if stopped reset wait time
    if(!roaming){
      idleStartTime = 0;
    }
  }
}
