using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoardConsole : MonoBehaviour {
    public InputField inputField;
    public Text textField;
    string startext;
    string scorebordText;
    string text;
    int printindex = 0;
    string exitText = "\n\ntype 'exit' to return to menu";

    private void Start()
    {
        startext = textField.text;
        textField.text = "";
        inputField.caretWidth = 12;

        //get scorebord text
        scorebordText = Highscores.AsString();

        //combine texts

        text = startext + "\n\n"  + scorebordText + exitText;
    }
    private void Update()
    {
        if (printindex < text.Length)
        {
            textField.text += text[printindex];
            printindex++;
        }
        inputField.ActivateInputField();
    }

    public void Submit()
    {
        string input = inputField.text;

        if (input == "exit")
        {
            SceneManager.LoadScene(0);
            inputField.text = "";
        }
    }
}
