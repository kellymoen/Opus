using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Movement))]
public class PlayerManagerScript : MonoBehaviour {
  private GameObject tether;
  private Movement moveScript;
  private PlayerCritterManager critterManager;
  private GameObject mainCamera;
  private GameObject battleCamera;
  private GameObject critterSelectionCamera;
  private GameObject currentCritterBattle;
  private bool inBattle = false;

  public GameObject tetherPrefab;

  void Start(){
    moveScript = gameObject.GetComponent<Movement>();
    critterManager = gameObject.GetComponent<PlayerCritterManager>();
    mainCamera = GameObject.Find("MainCamera");
    battleCamera = GameObject.Find("BattleCamera");
    critterSelectionCamera = GameObject.Find("CritterSelectionCamera");
  }

  void Update(){
    if(CrossPlatformInputManager.GetButton ("Fire3") && inBattle){
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
      inBattle = true;
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
    inBattle = false;
    switchToExploreCamera();
    moveScript.setMovementLock(false);
    Destroy(tether);
    //tell ai if it has been captured or if it should run away
    if(success){
      Debug.Log("Before addCritter");
      critterManager.addCritter(currentCritterBattle);
      Debug.Log(currentCritterBattle);
      //currentCritterBattle.GetComponent<AIManagerScript>().capture();
      currentCritterBattle = null;
    }
    else{
      currentCritterBattle.GetComponent<AIManagerScript>().escape();
      currentCritterBattle = null;
    }
  }

  void switchToCritterManagerCamera(){
    //enable selection camera & audio listener
    critterSelectionCamera.GetComponent<Camera>().enabled = true;
    critterSelectionCamera.GetComponent<AudioListener>().enabled = true;
    //disable all other cameras
    mainCamera.GetComponent<Camera>().enabled = false;
    mainCamera.GetComponent<AudioListener>().enabled = false;
    battleCamera.GetComponent<Camera>().enabled = false;
    battleCamera.GetComponent<AudioListener>().enabled = false;
  }

  void switchToExploreCamera(){
    //enable main camera & audio listener
    mainCamera.GetComponent<Camera>().enabled = true;
    mainCamera.GetComponent<AudioListener>().enabled = true;
    //disable all other cameras
    critterSelectionCamera.GetComponent<Camera>().enabled = false;
    critterSelectionCamera.GetComponent<AudioListener>().enabled = false;
    battleCamera.GetComponent<Camera>().enabled = false;
    battleCamera.GetComponent<AudioListener>().enabled = false;
  }

  void switchToBattleCamera(){
    //enable battle camera & audio listener
    battleCamera.GetComponent<AudioListener>().enabled = true;
    battleCamera.GetComponent<Camera>().enabled = true;
    //disable all other cameras
    mainCamera.GetComponent<Camera>().enabled = false;
    mainCamera.GetComponent<AudioListener>().enabled = false;
    critterSelectionCamera.GetComponent<Camera>().enabled = false;
    critterSelectionCamera.GetComponent<AudioListener>().enabled = false;

  }

}
