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
  private bool following = false;
	private GameObject player;
  private Rigidbody rBody;
  private GameObject tether;

  public GameObject tetherPrefab;
  public float height = 1f;
  public float maxPlayerDetectDistance = 0.5f;
  public float minDistance = 3;
  public float radius = 20;
  public float maxIdleTime = 15;

  void Start(){
		player = GameObject.FindWithTag ("Player");
    agent = GetComponent<NavMeshAgent>();
    rBody = GetComponent<Rigidbody> ();
    //animator = GetComponent<Animator>();
    originLocation = transform.position;
    currentGoal = randomPosition();
    bool successfullyAssigned = agent.SetDestination(currentGoal);
    int count = 0;
    while(!successfullyAssigned && count < 100){
      agent.SetDestination(currentGoal);
      count++;
    }
  }

  void Update(){
    makeFloat();
    if(!roaming && Input.GetButtonDown("Fire3")){
      endBattle();
    }
		if(roaming && playerNearby()){
			startBattle();
		}
    if(roaming){
      updateMovement();
    }
    if(following){
      agent.SetDestination(player.transform.position);
    }

  }

  Vector3 randomPosition(){
    float x = Random.Range(originLocation.x - radius, originLocation.x + radius);
    float z = Random.Range(originLocation.z - radius, originLocation.z + radius);
    return new Vector3(x, originLocation.y, z);
  }

  void makeFloat(){
    //Make the object hover above the ground
    Ray ray = new Ray (transform.position, -transform.up);
    RaycastHit downCast;

    if (Physics.Raycast (ray, out downCast, height)) {
      if (downCast.distance < height) {
        rBody.useGravity = false;
        rBody.AddForce(0, (height - downCast.distance)/downCast.distance, 0);
      }
    } else {
      rBody.useGravity = true;
    }
  }

  private void startBattle(){
    setRoaming (false);
    agent.SetDestination(transform.position);
    gameObject.transform.LookAt(player.transform.position);
    player.GetComponent<Movement>().Tether(gameObject);
    //camera switch
    switchToBattleCamera();
    //create tether
    tether = (GameObject) Instantiate(tetherPrefab, gameObject.transform.position, gameObject.transform.rotation);
    tether.transform.LookAt(player.transform.position);
    float scale = Vector3.Distance (transform.position, player.transform.position);
    tether.transform.localScale = new Vector3(tether.transform.localScale.x, tether.transform.localScale.y, scale);
  }

  private void endBattle(){
    setFollowing(true);
    player.GetComponent<Movement>().Untether(gameObject);
    switchToExploreCamera();
    Destroy(tether);
    transform.parent = player.transform.parent;
    Destroy(gameObject);
    //agent.SetDestination(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z-.05f));
    //transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.05f, player.transform.position.z);
  }

  public void setRoaming(bool isRoaming){
    roaming = isRoaming;
    //if stopped reset wait time
    if(!roaming){
      idleStartTime = 0;
    }
  }

  public void setFollowing(bool follow){
    following = follow;
    //must be roaming xor following
    roaming = !follow;
  }


  void switchToExploreCamera(){
    GameObject.Find("MainCamera").GetComponent<Camera>().enabled = true;
    GameObject.Find("BattleCamera").GetComponent<Camera>().enabled = false;
    GameObject.Find("MainCamera").GetComponent<AudioListener>().enabled = true;
    GameObject.Find("BattleCamera").GetComponent<AudioListener>().enabled = false;
  }

  void switchToBattleCamera(){
    GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;
    GameObject.Find("BattleCamera").GetComponent<Camera>().enabled = true;
    GameObject.Find("MainCamera").GetComponent<AudioListener>().enabled = false;
    GameObject.Find("BattleCamera").GetComponent<AudioListener>().enabled = true;
  }

  void updateMovement(){
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
       }
  }
	bool playerNearby(){
		return Vector3.Distance (transform.position, player.transform.position) < maxPlayerDetectDistance;
	}
}
