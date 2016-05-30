using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerCritterManager : MonoBehaviour {
  private Dictionary<Transform, GameObject> activeCritters;
  //all available critters
  private List<GameObject> allCritters;
  private bool open = false;

  void Start(){
    activeCritters = new Dictionary<Transform, GameObject>();
    allCritters = new List<GameObject>();
    foreach (Transform child in GameObject.Find("CritterSlotContainer").transform)
    {
      activeCritters.Add(child, null);
    }
  }

  void Update(){
    if(open){

    }
  }

  public void addCritter(GameObject critter){
    //if any spaces available add to current following
    addCritterToFirstSpaceAvailable(critter);
    //add to List of all critters
    allCritters.Add(critter);
  }

  private bool addCritterToFirstSpaceAvailable(GameObject critter){
    foreach(KeyValuePair<Transform, GameObject> pair in activeCritters){
      if(pair.Value == null){
        activeCritters[pair.Key] = critter;
        critter.GetComponent<AIManagerScript>().startFollowing(pair.Key);
        return true;
      }
    }
    return false;
  }

  private void displayAllCritters(){
    foreach(GameObject critter in allCritters){
      //TODO display
    }
  }

  public void openSelection(){
    open = true;
    displayAllCritters();
  }

  public void closeSelection(){
    open = false;
  }

	public int CritterLength() {
		return allCritters.Count;
	}
}
