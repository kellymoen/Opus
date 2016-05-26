using UnityEngine;

[RequireComponent(typeof(AIRoamingScript))]
[RequireComponent(typeof(AIFollowScript))]
public class AIManagerScript : MonoBehaviour
{

  enum State {Roam, Follow, Battle};
  private GameObject player;
  private GameObject critter;
  private AIRoamingScript roamingScript;
  private AIFollowScript followScript;
  private State currentState;


  public float maxPlayerDetectDistance = 0.5f;

  void Start(){
    roamingScript = gameObject.GetComponent<AIRoamingScript>();
    followScript = gameObject.GetComponent<AIFollowScript>();
    player = GameObject.FindWithTag ("Player");
  }

  void Update(){
    //if roaming check for player
    if(currentState == State.Roam && Vector3.Distance(player.transform.position, transform.position) < maxPlayerDetectDistance){
      readyBattle();
    }
  }

  public void startRoaming(){
    currentState = State.Roam;
    followScript.enabled = false;
    roamingScript.enabled = true;
  }

  public void startFollowing(Transform slot){
    currentState = State.Follow;
    roamingScript.enabled = false;
    followScript.enabled = true;
    followScript.setFollowSlot(slot);
  }

  bool isPlayerNearby(){
    return Vector3.Distance (transform.position, player.transform.position) < maxPlayerDetectDistance;
  }

  void readyBattle(){
    roamingScript.setMovementLock(true);
    player.GetComponent<PlayerManagerScript>().startBattle(gameObject);
    currentState = State.Battle;
    //TODO enable NoteBattleScript on critter
  }

  public void capture(){
    //TODO create playerCritterManager script

  }

  public void escape(){
    //Destroy(gameObject);
    //TODO respawn critter
  }
}
