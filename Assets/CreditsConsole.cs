using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsConsole : MonoBehaviour {
    public InputField inputField;
    public Text textField;
    string text;
    int printindex = 0;

    private void Start()
    {
        text = textField.text;
        textField.text = "";
        inputField.caretWidth = 12;
    }
    private void Update()
    {
        if (printindex < text.Length) {
            textField.text += text[printindex];
            printindex++;
        }
        inputField.ActivateInputField();
    }

    public void Submit() {
        string input = inputField.text;

        if (input == "exit") {
            SceneManager.LoadScene(0);
            inputField.text = "";
        }
    }
}
