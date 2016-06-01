using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AIRoamingScript))]
[RequireComponent(typeof(AIManagerScript))]
[RequireComponent(typeof(AudioSource))]
public class AIFollowScript : MonoBehaviour
{
  private NavMeshAgent agent;
  private Animator animator;
  private GameObject player;
  private Transform followSlot;
	private AudioSourceMetro metro;
	private AudioSource source;

  void Start(){
	player = Static.GetPlayer ();
	agent = GetComponent<NavMeshAgent>();
	source = GetComponent<AudioSource> ();
    animator = GetComponent<Animator>();
	metro = Static.GetMetronome ();
  }

	void Update(){
		//source.loop = true;
	    if(followSlot != null){
	      agent.SetDestination(followSlot.position);
		  animator.SetBool ("isWalking",true);
	    }
		if (!source.isPlaying)
			source.Play ();
  }

  public void setFollowSlot(Transform slot){
    followSlot = slot;
  }

  public Transform getFollowSlot(){
    return followSlot;
  }
}
