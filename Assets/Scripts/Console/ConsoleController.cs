/// <summary>
/// Handles parsing and execution of console commands, as well as collecting log output.
/// Copyright (c) 2014-2015 Eliot Lash
/// </summary>
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

public delegate void CommandHandler(string[] args);

public class ConsoleController{

	#region Event declarations
	// Used to communicate with ConsoleView
	public delegate void LogChangedHandler(string[] log);
	public event LogChangedHandler logChanged;

	public delegate void VisibilityChangedHandler(bool visible);
	public event VisibilityChangedHandler visibilityChanged;
	#endregion

	/// <summary>
	/// Object to hold information about each command
	/// </summary>
	class CommandRegistration {
		public string command { get; private set; }
		public CommandHandler handler { get; private set; }
		public string help { get; private set; }

		public CommandRegistration(string command, CommandHandler handler, string help) {
			this.command = command;
			this.handler = handler;
			this.help = help;
		}
	}

	/// <summary>
	/// How many log lines should be retained?
	/// Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
	/// </summary>
	const int scrollbackSize = 20;

	Queue<string> scrollback = new Queue<string>(scrollbackSize);
	List<string> commandHistory = new List<string>();
	Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

    public float firmwareVersion = 1.3f;
    public string instableAxis = "Z";
    public int coreTemp = 90;
	public bool successful = false;

	public string[] log { get; private set; } //Copy of scrollback as an array for easier use by ConsoleView

	const string repeatCmdName = "!!"; //Name of the repeat command, constant since it needs to skip these if they are in the command history

	public ConsoleController() {
        //When adding commands, you must add a call below to registerCommand() with its name, implementation method, and help text.
		registerCommand("help", help, "\ntype 'help' for command list.\n");
		registerCommand("update", updateFirmware, "\ntype 'update [version number]' \nto update current  firmware.\n");
		registerCommand("calibrate", calibrateSender, "\ntype 'calibrate [axis]'\nto recalibrate rotation of transmission satellite.\n");
		registerCommand("energy", adjustEnergy,"\ntype 'energy [operation] [amount]'\nto adjust energy temperature.\n");
		/*registerCommand("hide", hide, "Hide the console.");
		registerCommand(repeatCmdName, repeatCommand, "Repeat last command.");
		registerCommand("reload", reload, "Reload game.");
		registerCommand("resetprefs", resetPrefs, "Reset & saves PlayerPrefs.");*/
	}

	void registerCommand(string command, CommandHandler handler, string help) {
		commands.Add(command, new CommandRegistration(command, handler, help));
	}

	public void appendLogLine(string line) {
		Debug.Log(line);

		if (scrollback.Count >= ConsoleController.scrollbackSize) {
			scrollback.Dequeue();
		}
		scrollback.Enqueue(line);

		log = scrollback.ToArray();
		if (logChanged != null) {
			logChanged(log);
		}
	}

	public void runCommandString(string commandString) {
        appendLogLine("\n...\n");
		appendLogLine("$ " + commandString);

		string[] commandSplit = parseArguments(commandString);
		string[] args = new string[0];
		if (commandSplit.Length < 1) {
			appendLogLine(string.Format("Unable to process command '{0}'", commandString));
			return;

		}  else if (commandSplit.Length >= 2) {
			int numArgs = commandSplit.Length - 1;
			args = new string[numArgs];
			Array.Copy(commandSplit, 1, args, 0, numArgs);
		}
		runCommand(commandSplit[0].ToLower(), args);
		commandHistory.Add(commandString);
	}

	public void runCommand(string command, string[] args) {
		CommandRegistration reg = null;
		if (!commands.TryGetValue(command, out reg)) {
			appendLogLine(string.Format("Unknown command '{0}'.\ntype 'help' for list.", command));
		}  else {
			if (reg.handler == null) {
				appendLogLine(string.Format("Unable to process command '{0}', handler was null.", command));
			}  else {
				reg.handler(args);
			}
		}
	}

	static string[] parseArguments(string commandString)
	{
		LinkedList<char> parmChars = new LinkedList<char>(commandString.ToCharArray());
		bool inQuote = false;
		var node = parmChars.First;
		while (node != null)
		{
			var next = node.Next;
			if (node.Value == '"') {
				inQuote = !inQuote;
				parmChars.Remove(node);
			}
			if (!inQuote && node.Value == ' ') {
				node.Value = '\n';
			}
			node = next;
		}
		char[] parmCharsArr = new char[parmChars.Count];
		parmChars.CopyTo(parmCharsArr, 0);
		return (new string(parmCharsArr)).Split(new char[] {'\n'} , StringSplitOptions.RemoveEmptyEntries);
	}

    #region Command handlers
    //Implement new commands in this region of the file.

    /// <summary>
    /// A test command to demonstrate argument checking/parsing.
    /// Will repeat the given word a specified number of times.
    /// </summary>


    void help(string[] args)
    {
        foreach (CommandRegistration reg in commands.Values)
        {
            appendLogLine(string.Format("{0}: {1}", reg.command, reg.help));
        }
    }
    void updateFirmware(string[] args) {
		if (args.Length < 1) {
            appendLogLine("Expected 1 argument.\nupdate [version number]\ncurrent version: " + firmwareVersion.ToString());
			return;
		}

        double firmwareInput = 0;
        if (double.TryParse(args[0], out firmwareInput))
        {
            if (firmwareInput > firmwareVersion)
            {
				appendLogLine ("firmware update to version " + firmwareInput.ToString () + " successful.");
				successful = true;
            }
            else {
                appendLogLine("version " + firmwareInput.ToString() + " outdated. cannot downgrade.");
            }
            
        }
        else
        {
            appendLogLine(args[0]  + " is not a valid firmware.");
        }
	}

	void calibrateSender(string[] args) {
        if (args.Length < 1)
        {
            appendLogLine("Expected 1 argument.\ncalibrate [axis] (x, y, z)");
            return;
        }
        string inputaxis = args[0];
        if ((inputaxis == "x")|| (inputaxis == "y")|| (inputaxis == "z"))
        {
            if (inputaxis.ToUpper() == instableAxis.ToUpper())
            {
                appendLogLine(inputaxis + " axis sucessfully recalibrated.");
            }
            else
            {
                appendLogLine(inputaxis + " axis does not need to be calibrated");
            }

        }
        else
        {
            appendLogLine(args[0] + " is not a valid axis");
        }
    }

	void adjustEnergy(string[] args) {
        if (args.Length < 2)
        {
            LogTemp();
            appendLogLine("Expected 2 arguments.\nenergy [operation] (dec, inc) [amount].");
            return;
        }
        string operation = args[0];
        if ((operation == "dec") || (operation == "inc"))
        {
            int amount;
            if (int.TryParse(args[1], out amount))
            {
                int i = operation == "dec" ? -1 : 1;

                if (coreTemp + amount * i == 100)
                {
                    appendLogLine("energy successfully adjusted.");
                }
                else {
                    LogTemp();
                    appendLogLine("operation not recommended");
                }
            }
            else
            {
                LogTemp();
                appendLogLine(amount.ToString() + " is not a valid amount.");
            }

        }
        else
        {
            LogTemp();
            appendLogLine(args[0] + " is not a valid command");
        }
    }

    void LogTemp() {
        appendLogLine("energy core temperature: " + coreTemp.ToString() + "\nrecommended temperature: 100");
    }

	void hide(string[] args) {
		if (visibilityChanged != null) {
			visibilityChanged(false);
		}
	}

	void repeatCommand(string[] args) {
		for (int cmdIdx = commandHistory.Count - 1; cmdIdx >= 0; --cmdIdx) {
			string cmd = commandHistory[cmdIdx];
			if (String.Equals(repeatCmdName, cmd)) {
				continue;
			}
			runCommandString(cmd);
			break;
		}
	}
	public void sendTask (){
		if (successful){
			//random task
			successful = false;
		}

	}

	void task1 (){
		appendLogLine("Task 1");
		appendLogLine ("ajskdhladhhlaksd");
	}
	void task2 (){
		appendLogLine("Task 2");
		appendLogLine ("ajskdhladhhlaksd");
	}
	void task3 (){
		appendLogLine("Task 3");
		appendLogLine ("ajskdhladhhlaksd");
	}
	/*void reload(string[] args) {
		SceneManager.LoadLevel(Application.loadedLevel);
	}*/

	void resetPrefs(string[] args) {
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
    #endregion
}