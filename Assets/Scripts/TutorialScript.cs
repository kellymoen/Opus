using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {
	private GameObject kit;
	private GameObject iconPanel;
	private GameObject introPanel;
	private AIManagerScript manager;
	private AIBattleScript battle;

	void Start() {
		Debug.Log (manager);
		battle = GetComponent<AIBattleScript> ();
		kit = Static.GetPlayer ();
		iconPanel = GameObject.Find ("IconPanel");
		introPanel = GameObject.Find ("IntroPanel");
		AIBattleScript.BattleStart += BattleStart;
		AIBattleScript.BattleEnd += BattleEnd;
	}
	
	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance (kit.transform.position, gameObject.transform.position);
		Debug.Log (kit.transform.position +" and "+ gameObject.transform.position +" = "+ distance);
		if (distance > (20f + 5f)) {
			iconPanel.SetActive (false);
			introPanel.SetActive (true);
		} else {
			iconPanel.SetActive (true);
			introPanel.SetActive (false);
		}
	}

	void BattleStart() {

	}

	void BattleEnd(bool win) {
		if (win) {
			this.enabled = false;
		}
	}
}
