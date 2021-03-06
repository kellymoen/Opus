﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuScript : MonoBehaviour {

	//Name of the main scene to load on start
	public string gameScene;
	public AudioMixer mainAudioMixer;

	public GameObject menuPanel;
	public GameObject optionsPanel;
	public GameObject helpPanel;

	//Fields to store the UI selections
	public GameObject btnStart;
	public GameObject btnHelp;
	public GameObject btnOptions;
	public GameObject btnExit;
	public Text lblMusic;
	public Text lblSfx;
	public Text lblMenu;
	public GameObject btnBack;

	//Store the volume sliders
	public Slider slMusic;
	public Slider slSfx;
	public Slider slMenu;

	public Color colorSelectedText;
	public Color colorText;

	private enum MenuScreen { Main, Options, Help };
	private MenuScreen curScreen;
	//Option selected 0-2
	private int selectedOption;
	private float curSliderValue;

	//Timeout counter to slow down selection changing
	private float timeout;
	[Range(0,30)]
	public float maxTimeout = 10;


	void Start () {
		timeout = 0;
		// Make -1 so nothing selected initially
		selectedOption = -1;
		curScreen = MenuScreen.Main;
		optionsPanel.SetActive (false);
		helpPanel.SetActive (false);
	}

	void FixedUpdate(){
		if (Input.GetButtonDown ("Fire1") || Input.GetKeyDown(KeyCode.Return)) {
			switch (selectedOption) {
			case 0:
				if (curScreen == MenuScreen.Main) {
					StartGame ();
				}
				return;
			case 1:
				if (curScreen == MenuScreen.Main) {
					ToHelp ();
				}
				return;
			case 2:
				if (curScreen == MenuScreen.Main) {
					ToOptions ();
				}
				return;
			case 3:
				if (curScreen == MenuScreen.Main) {
					Application.Quit ();
				} else {
					ToMenu ();
				}
				return;
			}
		}
		//If option hasn't changed in last 10 ticks
		if (timeout == 0) {
			float vertInput = Input.GetAxis ("Vertical");
			float horizInput = Input.GetAxis ("Horizontal");

			//* CODE FOR USING THE MENU WITHOUT A CONTROLLER *//
			// vertInput += (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
			// horizInput += (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
			Vector2 axis = new Vector2(horizInput, vertInput);
			//Check movement, No options in help screen
			if (curScreen == MenuScreen.Main && (axis.magnitude > 0.1)) {
				//Choose option that is in the specified direction from the center
				float axisAngle = (45 - (Mathf.Atan2 (axis.y, axis.x) * 180 / Mathf.PI)) % 360;
				if (axisAngle < 0)
					axisAngle += 360;
				int option = 3 - (int)((axisAngle / 360.0f) * 4);
				//int option = (Mathf.Abs (vertInput) >= Mathf.Abs (horizInput)) ? ((vertInput > 0) ? 0 : 2) : ((horizInput > 0) ? 3 : 1);
				Select (option);
				//MoveSelection (horizInput > 0 ? 1 : -1);
				timeout = maxTimeout;
			}else if (Mathf.Abs(vertInput) > 0.5 && curScreen == MenuScreen.Options) {
				MoveSelection (vertInput > 0 ? 1 : -1);
				timeout = maxTimeout;
			}

			//Update the current slider if we need to
			if (curScreen == MenuScreen.Options) {
				curSliderValue = GetCurSlValue ();
				//Check if horizontal movement
				if (horizInput > 0.5f) {
					curSliderValue += 1.0f;
				} else if (horizInput < -0.5f) {
					curSliderValue -= 1.0f;
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


	public void StartGame(){
		// Load the game world scene
		// Scene must be added to build settings or else this does not work
		SceneManager.LoadScene(gameScene);
	}

	public void Quit() {
		Debug.Log ("Quitting.");
		Application.Quit ();
	}

	//Switch menu to main
	public void ToMenu(){
		UnselectCurrent ();
		selectedOption = -1;

		curScreen = MenuScreen.Main;
		menuPanel.SetActive (true);
		optionsPanel.SetActive (false);
		helpPanel.SetActive (false);
	}

	//Switch menu to options
	public void ToOptions(){
		UnselectCurrent ();
		selectedOption = 3;
		SelectCurrent ();

		curScreen = MenuScreen.Options;
		menuPanel.SetActive (false);
		optionsPanel.SetActive (true);
		helpPanel.SetActive (false);
	}

	//Switch menu to help
	public void ToHelp(){
		UnselectCurrent ();
		selectedOption = 3;
		SelectCurrent ();

		curScreen = MenuScreen.Help;
		menuPanel.SetActive (false);
		optionsPanel.SetActive (false);
		helpPanel.SetActive (true);
	}

	public void OnSlider(float value){
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
			mainAudioMixer.SetFloat ("UIVol",value);
			break;
		}
	}

	private void Select(int val){
		UnselectCurrent ();
		selectedOption = val;
		SelectCurrent ();
	}


	private void MoveSelection(int dir){
		UnselectCurrent ();
		if (dir < 0) {
			selectedOption = (selectedOption + 1) % 4;
		} else {
			selectedOption--;
			if (selectedOption < 0) {
				selectedOption = 3;
			}
		}
		SelectCurrent ();
	}

	private void UnselectCurrent(){
		// Update selected option.
		switch (selectedOption) {
		case 0:
			btnStart.GetComponent<ImageSelect> ().setSelect (false);
			lblMusic.color = colorText;
			break;
		case 1:
			btnHelp.GetComponent<ImageSelect> ().setSelect (false);
			lblSfx.color = colorText;
			break;
		case 2:
			btnOptions.GetComponent<ImageSelect> ().setSelect (false);
			lblMenu.color = colorText;
			break;
		case 3:
			btnExit.GetComponent<ImageSelect> ().setSelect (false);
			btnBack.GetComponent<ImageSelect> ().setSelect (false);
			break;
		}
	}

	private void SelectCurrent(){
		// Update selected option.
		switch (selectedOption) {
		case 0:
			btnStart.GetComponent<ImageSelect> ().setSelect (true);
			lblMusic.color = colorSelectedText;
			break;
		case 1:
			btnHelp.GetComponent<ImageSelect> ().setSelect (true);
			lblSfx.color = colorSelectedText;
			break;
		case 2:
			btnOptions.GetComponent<ImageSelect> ().setSelect (true);
			lblMenu.color = colorSelectedText;
			break;
		case 3:
			btnExit.GetComponent<ImageSelect> ().setSelect (true);
			btnBack.GetComponent<ImageSelect> ().setSelect (true);
			break;
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
}
