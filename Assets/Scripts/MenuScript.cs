using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuScript : MonoBehaviour {


	public AudioMixer mainAudioMixer;
	//Name of the main scene to load on start
	public string gameScene;
	public string composeScene;
	public string menu;

	//Fields to store the UI text labels
	public Text btnStart;
	public Text btnCompose;
	public Text btnOptions;
	public Text btnExit;
	public Text lblMusic;
	public Text lblSfx;
	public Text lblMenu;
	public Text lblBack;
	//Store the volume sliders
	public Slider slMusic;
	public Slider slSfx;
	public Slider slMenu;

	public Color colorSelectedText;
	public Color colorText;

	public GameObject menuPanel;
	public GameObject optionsPanel;

	private bool inOptions = false;
	//Option selected 0-2
	private int selectedOption;

	private float curSliderValue;

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
				if (!inOptions) {
					StartGame ();
				}
				return;
			case 1:
				if (!inOptions) {
					Composition ();
				}
				return;
			case 2:
				if (!inOptions) {
					MenuToOptions ();
				}
				return;
			case 3:
				if (!inOptions) {
					Application.Quit ();
				} else {
					OptionsToMenu ();
				}
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
			if (inOptions) {
				curSliderValue = GetCurSlValue ();
				//Check if horizontal movement
				if (Input.GetAxisRaw ("Horizontal") > 0) {
					curSliderValue = (curSliderValue + 5);
				} else if (Input.GetAxisRaw ("Horizontal") < 0) {
					curSliderValue = (curSliderValue - 5);
				}
				switch (selectedOption) {
				case 0:
					slMusic.value = curSliderValue;
					break;
				case 1:
					slSfx.value = curSliderValue;
					break;
				case 2:
					slMenu.value = curSliderValue;
					break;
				}
			}
		} else if (timeout > 0) {
			timeout--;
		}
	}

	void Update(){
		// Prevent some unnecessary color changes
		// Can't guarantee that update will be run before FixedUpdate runs again so don't use timeout == maxTimeout
		if (timeout > 0) {
			// Reset colours on the text
			btnStart.color = colorText;
			btnCompose.color = colorText;
			btnOptions.color = colorText;
			btnExit.color = colorText;
			lblMusic.color = colorText;
			lblSfx.color = colorText;
			lblMenu.color = colorText;
			lblBack.color = colorText;
			// Update selected option.
			switch (selectedOption) {
			case 0:
				btnStart.color = colorSelectedText;
				lblMusic.color = colorSelectedText;
				break;
			case 1:
				btnCompose.color = colorSelectedText;
				lblSfx.color = colorSelectedText;
				break;
			case 2:
				btnOptions.color = colorSelectedText;
				lblMenu.color = colorSelectedText;
				break;
			case 3:
				btnExit.color = colorSelectedText;
				lblBack.color = colorSelectedText;
				break;
			}
		}
	}

	private float GetCurSlValue(){
		switch (selectedOption) {
		case 0:
			return slMusic.value;
		case 1:
			return slSfx.value;
		case 2:
			return slMenu.value;
		}
		return 0.0f;
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
		inOptions = true;
		menuPanel.SetActive (false);
		optionsPanel.SetActive (true);
	}

	public void OptionsToMenu(){
		inOptions = false;
		menuPanel.SetActive (true);
		optionsPanel.SetActive (false);
	}

	public void Quit() {
		Debug.Log ("Quitting.");
		Application.Quit ();
	}

	public void OnSlider(float value){
		//do stuff
		switch (selectedOption) {
		case 0:
			//Modify music volume
			mainAudioMixer.SetFloat ("MusicVol",value);
			break;
		case 1:
			//Modify sfx volume
			mainAudioMixer.SetFloat ("FoleyVol",value);
			break;
		case 2:
			//Modify menu volume
			mainAudioMixer.SetFloat ("MenuVol",value);
			break;
		}
	}
}
