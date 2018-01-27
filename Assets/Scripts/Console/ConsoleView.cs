/// <summary>
/// Marshals events and data between ConsoleController and uGUI.
/// Copyright (c) 2014-2015 Eliot Lash
/// </summary>
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class ConsoleView : MonoBehaviour {
	ConsoleController console = new ConsoleController();

	bool didShow = false;

	public GameObject viewContainer; //Container for console view, should be a child of this GameObject
	public Text logTextArea;
	public InputField inputField;

	void Start() {
		if (console != null) {
			console.logChanged += onLogChanged;
		}
		updateLogStr(console.log);

        
	}

	~ConsoleView() {
		console.logChanged -= onLogChanged;
	}

	void Update() {
        //activate input field
        inputField.ActivateInputField();
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

}