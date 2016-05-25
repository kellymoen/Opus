using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Movement))]
public class PlayerManagerScript : MonoBehaviour {
  private GameObject tether;
  private Movement moveScript;
  private GameObject mainCamera;
  private GameObject battleCamera;
  private GameObject critterSelectionCamera;
  private GameObject currentCritterBattle;


  public GameObject tetherPrefab;
  void Start(){
    moveScript = gameObject.GetComponent<Movement>();
    mainCamera = GameObject.Find("MainCamera");
    battleCamera = GameObject.Find("BattleCamera");
    //critterSelectionCamera = GameObject.Find("critterSelectionCamera");
  }

  void Update(){
    if(CrossPlatformInputManager.GetButton ("Fire3")){
      endBattle(true);
    }
  }

  private void createTether(){
    tether = (GameObject) Instantiate(tetherPrefab, gameObject.transform.position, gameObject.transform.rotation);
    tether.transform.LookAt(currentCritterBattle.transform.position);
    float scale = Vector3.Distance (transform.position, currentCritterBattle.transform.position);
    tether.transform.localScale = new Vector3(tether.transform.localScale.x, tether.transform.localScale.y, scale);
  }

  public void startBattle(GameObject critter){
    //TODO make sure only one battle at a time is active
    if(critter != null){
      currentCritterBattle = critter;
      gameObject.transform.LookAt(critter.transform.position);
      moveScript.setMovementLock(true);
      //camera switch
      switchToBattleCamera();
      createTether();
    }
    else{
      Debug.Log("Can't start battle with critter value Null");
    }
  }

  public void endBattle(bool success){
    switchToExploreCamera();
    moveScript.setMovementLock(false);
    Destroy(tether);
    //tell ai if it has been captured or if it should run away
    if(success){
      currentCritterBattle.GetComponent<AIManagerScript>().capture();
      currentCritterBattle = null;
    }
    else{
      currentCritterBattle.GetComponent<AIManagerScript>().escape();
      currentCritterBattle = null;
    }
  }

  void switchToExploreCamera(){
    mainCamera.GetComponent<Camera>().enabled = true;
    battleCamera.GetComponent<Camera>().enabled = false;
    mainCamera.GetComponent<AudioListener>().enabled = true;
    battleCamera.GetComponent<AudioListener>().enabled = false;
  }

  void switchToBattleCamera(){
    mainCamera.GetComponent<Camera>().enabled = false;
    battleCamera.GetComponent<Camera>().enabled = true;
    mainCamera.GetComponent<AudioListener>().enabled = false;
    battleCamera.GetComponent<AudioListener>().enabled = true;
  }

}
