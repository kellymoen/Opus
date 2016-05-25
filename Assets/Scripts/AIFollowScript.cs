using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AIRoamingScript))]
public class AIFollowScript : MonoBehaviour
{
  private NavMeshAgent agent;
  private Animator animator;
  private GameObject player;
  private Transform followSlot;

  void Start(){
		player = GameObject.FindWithTag ("Player");
    agent = GetComponent<NavMeshAgent>();
    //animator = GetComponent<Animator>();
  }

  void Update(){
    if(followSlot != null){
      agent.SetDestination(followSlot.position);
    }
  }

  public void setFollowSlot(Transform slot){
    followSlot = slot;
  }

  public Transform getFollowSlot(){
    return followSlot;
  }
}
