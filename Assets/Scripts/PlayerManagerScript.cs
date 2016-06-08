using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Movement))]
public class PlayerManagerScript : MonoBehaviour {
  enum State {Battle, Explore, Menu};
  private GameObject tether;
  private Movement moveScript;
  private PlayerCritterManager critterManager;
  private GameObject mainCamera;
  private GameObject battleCamera;
  private GameObject currentCritterBattle;
  private bool inBattle = false;
  private State currentState = State.Explore;
  private List<GameObject> hiddenObjects = new List<GameObject>();

  public GameObject tetherPrefab;

  void Start(){
    moveScript = gameObject.GetComponent<Movement>();
    critterManager = gameObject.GetComponent<PlayerCritterManager>();
    mainCamera = GameObject.Find("MainCamera");
    battleCamera = GameObject.Find("BattleCamera");
  }

  void Update(){
    if(CrossPlatformInputManager.GetButtonDown("Fire2")){
      if(currentState == State.Explore){
        //openSelectionMenu();
      }
      else if(currentState == State.Menu){
        closeSelectionMenu();
      }
    }

    //remove once we have an actual capture mechanic
    if(CrossPlatformInputManager.GetButtonDown("Fire3") && currentState == State.Battle){
      endBattle(true);
    }
  }

  private void createTether(){
    tether = (GameObject) Instantiate(tetherPrefab, gameObject.transform.position, gameObject.transform.rotation);
    tether.transform.LookAt(currentCritterBattle.transform.position);
    float scale = Vector3.Distance (transform.position, currentCritterBattle.transform.position);
    tether.transform.localScale = new Vector3(tether.transform.localScale.x, tether.transform.localScale.y, scale);
  }

  private void openSelectionMenu(){
    currentState = State.Menu;
    moveScript.setMovementLock(true);
  }

  private void closeSelectionMenu(){
    switchToExploreCamera();
    currentState = State.Explore;
    moveScript.setMovementLock(false);
  }

  public bool startBattle(GameObject critter){
    if(critter != null && currentState == State.Explore){
      currentState = State.Battle;
      currentCritterBattle = critter;
      gameObject.transform.LookAt(critter.transform.position);
      moveScript.setMovementLock(true);
      //camera switch
      switchToBattleCamera();
      removeBattleCameraViewObstructions(critter);
      createTether();
      return true;
    }
    else{
      return false;
    }
  }

  public void endBattle(bool success){
    currentState = State.Explore;
    switchToExploreCamera();
    moveScript.setMovementLock(false);
    Destroy(tether);

    //tell ai if it has been captured or if it should run away
    if(success){
      critterManager.addCritter(currentCritterBattle);
      Debug.Log(currentCritterBattle);
      //currentCritterBattle.GetComponent<AIManagerScript>().capture();
      currentCritterBattle = null;
    }
    else{
      currentCritterBattle.GetComponent<AIManagerScript>().escape();
      currentCritterBattle = null;
    }
    returnObstructionsToView();
  }



  void switchToExploreCamera(){
    //enable main camera & audio listener
    mainCamera.GetComponent<Camera>().enabled = true;
    //disable all other cameras
    battleCamera.GetComponent<Camera>().enabled = false;
  }

  void switchToBattleCamera(){
		Debug.Log ("battle camera enabled!");
    //enable battle camera & audio listener
    battleCamera.GetComponent<Camera>().enabled = true;
    //disable all other cameras
    mainCamera.GetComponent<Camera>().enabled = false;
  }

  void removeBattleCameraViewObstructions(GameObject critter){
    RaycastHit hit;
    if(Physics.Raycast(battleCamera.transform.position, transform.position - battleCamera.transform.position, out hit)){
      if(hit.transform != transform){
        hiddenObjects.Add(hit.transform.gameObject);
        hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
      }
    }
    if(Physics.Raycast(battleCamera.transform.position, critter.transform.position - battleCamera.transform.position, out hit)){
      if(hit.transform != critter.transform){
        hiddenObjects.Add(hit.transform.gameObject);
        hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
      }
    }
  }

  void returnObstructionsToView(){
    foreach(GameObject gObject in hiddenObjects){
      gObject.GetComponent<MeshRenderer>().enabled = true;
    }
    hiddenObjects.Clear();
  }

}
