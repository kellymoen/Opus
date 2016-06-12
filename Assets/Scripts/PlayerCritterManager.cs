using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerCritterManager : MonoBehaviour {
  private Dictionary<Transform, GameObject> activeCritters;
  //all available critters
  private List<GameObject> allCritters;
  private float heightOffGround = 0.5f;//For moving critters around, to avoid getting stuck add extra height

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
    //if any spaces available add to current following
    if(!addCritterToFirstSpaceAvailable(critter)){
      //if no spaces available disable critter
      critter.SetActive(false);
    }
    //add to List of all critters
    allCritters.Add(critter);
  }

  public bool addCritterToFirstSpaceAvailable(GameObject critter){
    foreach(KeyValuePair<Transform, GameObject> pair in activeCritters){
      if(pair.Value == null){
        activeCritters[pair.Key] = critter;
        critter.SetActive(true);
        critter.GetComponent<AIManagerScript>().startFollowing(pair.Key);
        return true;
      }
    }
    return false;
  }

  public bool stopCritterFollowing(GameObject critter){
    if(critter == null){
      return false;
    }
    foreach(KeyValuePair<Transform, GameObject> pair in activeCritters){
      if(pair.Value == critter){
        activeCritters[pair.Key] = null;
        critter.SetActive(false);
        return true;
      }
    }
    return false;
  }

  public List<GameObject> getAllCritters(){
    return allCritters;
  }
	public int CritterLength() {
		return allCritters.Count;
	}
}
