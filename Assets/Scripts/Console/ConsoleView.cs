/// <summary>
/// Marshals events and data between ConsoleController and uGUI.
/// Copyright (c) 2014-2015 Eliot Lash
/// </summary>
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ConsoleView : MonoBehaviour {
	ConsoleController console = new ConsoleController();

	public GameObject viewContainer; //Container for console view, should be a child of this GameObject
	public Text logTextArea;
	public InputField inputField;
	public Camera gameCam;

	public AudioClip[] clickSound;
	public AudioClip[] feedSound;
	public AudioClip[] errorSound;
	public AudioClip[] warningSound;

	private AudioSource audioSource;

	void Start() {
		if (console != null) {
			console.logChanged += onLogChanged;
		}
		updateLogStr(console.log);
        inputField.caretWidth = 10;
		console.AssignImageEffect(gameCam.GetComponent<ImageEffect> ());
		audioSource = GetComponent<AudioSource> ();
	}

	~ConsoleView() {
		console.logChanged -= onLogChanged;
	}

	void Update() {
        //activate input field
        inputField.ActivateInputField();
        console.CheckTask();
		if (Input.GetButtonDown("Submit")) {
			runCommand ();
		}
    }

	void onLogChanged(string[] newLog) {
		updateLogStr(newLog);
	}

	void updateLogStr(string[] newLog) {
		if (newLog == null) {
			logTextArea.text = "";
		}  else {
			logTextArea.text = string.Join("\n", newLog);
		}
	}

	/// <summary>
	/// Event that should be called by anything wanting to submit the current input to the console.
	/// </summary>
	public void runCommand() {
		console.runCommandString (inputField.text);
		inputField.text = "";
		PlayFeed ();
	}

	public void PlayClick(){
		PlaySound (clickSound);
	}

	public void PlayFeed(){
		PlaySound (feedSound);
	}
	public void PlayWarning(){
		PlaySound (warningSound);
	}
	public void PlayError(){
		PlaySound (errorSound);
	}

	private void PlaySound(AudioClip[] soundArray){
		audioSource.Stop ();
		audioSource.clip = soundArray [Random.Range (0, soundArray.Length)];
		audioSource.loop = false;
		audioSource.Play ();
	}

}