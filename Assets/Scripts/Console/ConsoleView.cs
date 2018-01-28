/// <summary>
/// Marshals events and data between ConsoleController and uGUI.
/// Copyright (c) 2014-2015 Eliot Lash
/// </summary>
using System;
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
    private AudioSource siren;

	void Start() {
		if (console != null) {
			console.logChanged += onLogChanged;
		}
		updateLogStr(console.log);
        inputField.caretWidth = 10;
		console.AssignImageEffect(gameCam.GetComponent<ImageEffect> ());
        console.AssignConsoleView(this);
		audioSource = GetComponent<AudioSource> ();
        siren = transform.Find("Alarm").GetComponent<AudioSource>();
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
        if (gameCam.GetComponent<ImageEffect>().intensity >= 0.99 && console.alive) {
            console.Die(TimeSpan.FromSeconds(GetComponentInChildren<Timer>().seconds));
        }
        console.UpdateLog();
        float intensity = Camera.main.GetComponent<ImageEffect>().intensity;
        float volume;
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
            console.runCommandString(inputField.text);
            inputField.text = "";
	}

	public void PlayClick(){
		PlaySound (audioSource, clickSound);
	}

	public void PlayFeed(){
		PlaySound (audioSource, feedSound);
	}
	public void PlayWarning(){
		PlaySound (siren, warningSound);
	}
    public void StopWarning() {
        siren.Stop();
    }
	public void PlayError(){
		PlaySound (audioSource, errorSound);
	}

	private void PlaySound( AudioSource source, AudioClip[] soundArray){
        print("feeding");
        source.Stop ();
        source.clip = soundArray [UnityEngine.Random.Range(0, soundArray.Length)];
        source.Play ();
	}

}