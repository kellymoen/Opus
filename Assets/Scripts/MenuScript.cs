using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	//Name of the main scene to load on start
	public string gameScene;
	public string composeScene;
	public string menu;

	//Fields to store the UI text labels
	public Text btnStart;
	public Text btnCompose;
	public Text btnOptions;
	public Text btnExit;

	public Color colorSelectedText;
	public Color colorText;

	public GameObject menuPanel;
	public GameObject optionsPanel;

	//Option selected 0-2
	private int selectedOption;
	//Timeout counter to slow down selection changing
	private float timeout;
	[Range(0,30)]
	public const float maxTimeout = 10;

	void Start () {
		timeout = 0;
		// Make -1 so nothing selected initially
		selectedOption = -1;
		btnStart.color = colorText;
		btnOptions.color = colorText;
		btnExit.color = colorText;
		optionsPanel = GameObject.Find ("Canvas/OptionsPanel");
		menuPanel = GameObject.Find ("Canvas/Buttons");
		optionsPanel.SetActive (false);
	}

	void FixedUpdate(){
		if (Input.GetButtonDown ("Fire1") || Input.GetKeyDown(KeyCode.Return)) {
			
			switch (selectedOption) {
			case 0:
				StartGame ();
				return;
			case 1:
				Composition ();
				return;

			case 2:
				MenuToOptions ();
				return;
			case 3:
				Application.Quit ();
				return;
			}
		}
		//If option hasn't changed in last 10 ticks
		if (timeout == 0) {
			//Check if up or down movement
			if (Input.GetAxisRaw ("Vertical") > 0) {
				selectedOption--;
				if (selectedOption < 0) {
					selectedOption = 3;
				}
				timeout = maxTimeout;
			} else if (Input.GetAxisRaw ("Vertical") < 0) {
				selectedOption = (selectedOption + 1) % 4;
				timeout = maxTimeout;
			}
		} else if (timeout > 0) {
			timeout--;
		}
	}

	void Update(){
		// Prevent some unnecessary color changes
		// Can't guaruntee that update will be run before FixedUpdate runs again so don't use timeout == maxTimeout
		if (timeout > 0) {
			// Reset colours on the text
			btnStart.color = colorText;
			btnCompose.color = colorText;
			btnOptions.color = colorText;
			btnExit.color = colorText;
			// Update selected option.
			switch (selectedOption) {
			case 0:
				btnStart.color = colorSelectedText;
				break;
			case 1:
				btnCompose.color = colorSelectedText;
				break;
			case 2:
				btnOptions.color = colorSelectedText;
				break;
			case 3:
				btnExit.color = colorSelectedText;
				break;
			}
		}
	}

	public void StartGame(){
		// Load the game world scene
		// Scene must be added to build settings or else this does not work
		SceneManager.LoadScene(gameScene);
	}

	public void Composition(){

		SceneManager.LoadScene (composeScene);
	}

	public void Menu(){
		SceneManager.LoadScene (menu);
	}

	public void MenuToOptions(){
		menuPanel.SetActive (false);
		optionsPanel.SetActive (true);
	}

	public void OptionsToMenu(){
		menuPanel.SetActive (true);
		optionsPanel.SetActive (false);
	}

	public void Quit() {
		Debug.Log ("Quitting.");
		Application.Quit ();
	}
}
