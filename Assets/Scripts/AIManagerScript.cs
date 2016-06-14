using UnityEngine;

[RequireComponent(typeof(AIRoamingScript))]
[RequireComponent(typeof(AIFollowScript))]
[RequireComponent(typeof(AIBattleScript))]
public class AIManagerScript : MonoBehaviour
{

	enum State {Roam, Follow, Battle};
	public float escapeLength = 15f;
  private GameObject player;
  private GameObject critter;
  private AIRoamingScript roamingScript;
  private AIFollowScript followScript;
  private AIBattleScript battleScript;
  private State currentState;
	private float delayRestart;


  public float maxPlayerDetectDistance = 20f;


  void Start(){
    roamingScript = gameObject.GetComponent<AIRoamingScript>();
	followScript = gameObject.GetComponent<AIFollowScript>();
    battleScript = gameObject.GetComponent<AIBattleScript>();
	player = Static.GetPlayer ();
  }  

  void Update(){
    //if roaming check for player
		if (currentState == State.Roam && Vector3.Distance (player.transform.position, transform.position) < maxPlayerDetectDistance
			&& delayRestart < Time.time) {
			readyBattle ();
		}
		if (delayRestart >= Time.time) {
			roamingScript.Escape ();
		}
  }

  public void startRoaming(){
    currentState = State.Roam;
    followScript.enabled = false;
    roamingScript.enabled = true;
	battleScript.enabled = false;
  }

  public void startFollowing(Transform slot){
    currentState = State.Follow;
    roamingScript.enabled = false;
    followScript.enabled = true;
    followScript.setFollowSlot(slot);
	battleScript.enabled = false;
  }

  bool isPlayerNearby(){
    return Vector3.Distance (transform.position, player.transform.position) < maxPlayerDetectDistance;
  }

  void readyBattle(){
    if(player.GetComponent<PlayerManagerScript>().startBattle(gameObject)){
      roamingScript.setMovementLock(true);
      currentState = State.Battle;
	  battleScript.enabled = true;
	  battleScript.Begin (transform, Static.GetTarget().transform);
    }
  }

  public void capture(){
    //TODO create playerCritterManager script
	
  }

	public void escape(){
		delayRestart += escapeLength;
		startRoaming ();
  }
}
