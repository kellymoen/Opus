using UnityEngine;
using System.Collections;

public class BillboardScript : MonoBehaviour {
	public GameObject tutorialSprite;
	public float height;
	private GameObject kit;
	private AIBattleScript battle;

	void Start() {
		battle = tutorialSprite.GetComponent<AIBattleScript> ();
		kit = Static.GetPlayer ();
		AIBattleScript.BattleStart += BattleStart;
		AIBattleScript.BattleEnd += BattleEnd;
	}

	void Update() {
		if (battle.enabled) {
			transform.position = new Vector3 (tutorialSprite.transform.position.x, 
				tutorialSprite.transform.position.y + height, tutorialSprite.transform.position.z);
			transform.LookAt (kit.transform);
		}
	}

	void BattleStart() {

	}

	void BattleEnd(bool win) {
		if (win) {
			gameObject.SetActive (false);
		}
	}
}
