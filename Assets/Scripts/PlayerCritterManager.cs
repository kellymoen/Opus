using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerCritterManager : MonoBehaviour {
  private Dictionary<Transform, GameObject> activeCritters;
  //all available critters
  private List<GameObject> allCritters;

  void Start(){
    activeCritters = new Dictionary<Transform, GameObject>();
    allCritters = new List<GameObject>();
    foreach (Transform child in GameObject.Find("CritterSlotContainer").transform)
    {
      activeCritters.Add(child, null);
    }
  }

  void Update(){

  }

  public void addCritter(GameObject critter){
    addCritterToFirstSpaceAvailable(critter);
    //add to List of all critters
    allCritters.Add(critter);
  }

  private bool addCritterToFirstSpaceAvailable(GameObject critter){

    foreach(KeyValuePair<Transform, GameObject> pair in activeCritters){
      //TODO deal with what ever bullshit this foreach is throwing
      if(pair.Value == null){
        activeCritters[pair.Key] = critter;
        critter.GetComponent<AIManagerScript>().startFollowing(pair.Key);
        return true;
      }
    }
    return false;
  }
}
