/// <summary>
/// Handles parsing and execution of console commands, as well as collecting log output.
/// Copyright (c) 2014-2015 Eliot Lash
/// </summary>
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

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
	const int scrollbackSize = 100;

	Queue<string> scrollback = new Queue<string>(scrollbackSize);
    Queue<string> notYetPrintedStrings = new Queue<string>(500);
    string currentPrintingString = "";
    Queue<Char> notYetPrintedChars = new Queue<char>(200);
	List<string> commandHistory = new List<string>();
	Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

    public float firmwareVersion = 1.3f;
    public string instableAxis = "Z";
    public int coreTemp = 90;
    public int memorynumber;
    public string memoryHash;
    public bool alive = true;

    private bool taskCompleted = true;
    private string activeTask = "";
    private float nextTaskTime = 0f;

    private ConsoleView consoleView;
	private ImageEffect glitchEffect;
	private float glitchRate = 0.02f;
    

	public string[] log { get; private set; } //Copy of scrollback as an array for easier use by ConsoleView

	const string repeatCmdName = "!!"; //Name of the repeat command, constant since it needs to skip these if they are in the command history

	public ConsoleController() {
        //When adding commands, you must add a call below to registerCommand() with its name, implementation method, and help text.
		registerCommand("help", help, "\ntype 'help' for command list.\n");
		registerCommand("update", updateFirmware, "\ntype 'update [version number]' \nto update current  firmware.\n");
		registerCommand("calibrate", calibrateSender, "\ntype 'calibrate [axis]'\nto recalibrate rotation of transmission satellite.\n");
		registerCommand("energy", adjustEnergy,"\ntype 'energy [operation] [amount]'\nto adjust energy temperature.\n");
		registerCommand("allocate", allocate, "\ntype 'allocate [number]'\nallocates number from fragmented memory.\n");
		registerCommand("exit", exit, "go to main menu");
		/*registerCommand("reload", reload, "Reload game.");
		registerCommand("resetprefs", resetPrefs, "Reset & saves PlayerPrefs.");*/
	}

	void registerCommand(string command, CommandHandler handler, string help) {
		commands.Add(command, new CommandRegistration(command, handler, help));
	}

	public void appendLogLine(string line) {
		if (scrollback.Count >= ConsoleController.scrollbackSize) {
			scrollback.Dequeue();
		}
		notYetPrintedStrings.Enqueue(line);
	}

    public void UpdateLog() {
        for (int i = 0; i < 2; i++)
        {
            if (notYetPrintedStrings.Count != 0 || notYetPrintedChars.Count != 0)
            {
                if (notYetPrintedChars.Count > 0)
                {
                    //IF THERE ARE STILL CHARACTES TO BE PRINTED, APPEDN TEHM TO CURREN STRING
                    currentPrintingString += notYetPrintedChars.Dequeue();
                }
                else
                {
                    //GET NEXT STRING FROM QUEUE
                    scrollback.Enqueue(currentPrintingString);

                    currentPrintingString = "";
                    string nextString = notYetPrintedStrings.Dequeue();
                    foreach (char character in nextString)
                    {
                        if (character == ' ' || character == '\n')
                            consoleView.PlayFeed();
                        notYetPrintedChars.Enqueue(character);
                    }
                }
            }
            log = new string[scrollback.ToArray().Length + 1];
            scrollback.CopyTo(log, 0);
            log[log.Length - 1] = currentPrintingString;
            if (logChanged != null)
            {
                logChanged(log);
            }
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

    #region task generation

    public void CheckTask() {
        if (taskCompleted && Time.time > nextTaskTime && alive) {
            CreatenewTask();
        }
    }

    private void CreatenewTask() {
		glitchEffect.DecreaseConnection (glitchRate);
		glitchRate += 0.005f;
        int taskid = UnityEngine.Random.Range(0, 4);
        switch (taskid) {
            case 0:
                CreateUpdateTask();
                taskCompleted = false;
                break;
            case 1:
                CreateCalibrateTask();
                taskCompleted = false;
                break;
            case 2:
                CreateAdjustTask();
                taskCompleted = false;
                break;
		case 3:
			CreateAllocateTask ();
			taskCompleted = false;
			break;

        }
    }

    private void CompleteTask()
    {
        taskCompleted = true;
        nextTaskTime = Time.time + 2f;
		glitchEffect.StabilizeConnection ();
		glitchEffect.IncreaseConnectionBurst (0.35f);
    }

    #region individual tasks

    private void CreateUpdateTask() {
        appendLogLine("\n...\n\n<color=red>transmission protocol outdated!</color>\ncurrent firmware: " + firmwareVersion.ToString());
        activeTask = "update";
    }
    private void CreateCalibrateTask() {
        coreTemp = 1000 + UnityEngine.Random.Range(-100, 100) * 10;
        coreTemp = coreTemp == 1000 ? 770 : coreTemp;
        appendLogLine("\n...\n\n<color=red>energy core temperature critical!</color>\ncurrent core temperature: " + coreTemp.ToString() + "°C\nrecommended temperature: 1000°C");
        activeTask = "energy";
    }
    private void CreateAdjustTask()
    {
        int x = UnityEngine.Random.Range(50, 100);
        int y = UnityEngine.Random.Range(50, 100);
        int z = UnityEngine.Random.Range(50, 100);

        if (x < y && x < z)
        {
            //itsx
            instableAxis = "x";
        }
        else if (y < z)
        {
            //its y
            instableAxis = "y";
        }
        else {
            //its z
            instableAxis = "z";
        }
        appendLogLine("\n...\n\n<color=red>connection instable!</color>\ndrone is leaving sending range of satellite.\n" +
            "signal strength x: " + x.ToString() +"%\n" +
            "signal strength y: " + y.ToString() + "%\n" +
            "signal strength z: " + z.ToString() + "%");
        activeTask = "calibrate";
    }
    private void CreateAllocateTask()
    {
        appendLogLine("\n...\n\n<color=red>memory data corrupted!</color>\nallocate number from hash");
        //string sonderzeichen = "!#$%&()?@[]^_{}~";
        string sonderzeichen = ",:></='¨}´+-*_°!#$%&()?@[]^_{}~";
        string memory = "";
        int number = UnityEngine.Random.Range(10000, 100000);
        for (int y = 0; y< 5; y++) {
            memory += "\n";
            int pos = UnityEngine.Random.Range(0, 9);
            for (int x = 0; x < 10; x++)
            {
                memory += x == pos ? number.ToString()[y] : sonderzeichen[UnityEngine.Random.Range(0, sonderzeichen.Length)];
            }
        }
        appendLogLine(memory);
        activeTask = "allocate";
        memorynumber = number;
        memoryHash = memory;
    }
    #endregion


    private bool IsTaskActive(string task) {
        return (task == activeTask);
    }

    #endregion

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

        float firmwareInput = 0;
        if (float.TryParse(args[0], out firmwareInput))
        {
            if (!alive)
            {
                appendLogLine("<color=red>Error #B2x002783</color>\nConnection Timed Out");
                return;
            }
            if (IsTaskActive("update"))
            {
                if (firmwareInput > firmwareVersion)
                {
                    firmwareVersion = firmwareInput;
                    appendLogLine("firmware update to version " + firmwareInput.ToString() + " successful.");
                    CompleteTask();
                }
                else
                {
                    appendLogLine("version " + firmwareInput.ToString() + " outdated. cannot downgrade.");
                }
            }
            else {
                appendLogLine("firmware is up to date.");
            }
            
        }
        else
        {
            consoleView.PlayError();
            appendLogLine(args[0]  + " is not a valid firmware.");
        }
	}

	void calibrateSender(string[] args) {
        if (args.Length < 1)
        {
            appendLogLine("Expected 1 argument.\ncalibrate [axis] (x, y, z)");
            return;
        }
        string inputaxis = args[0].ToLower();
        if ((inputaxis == "x")|| (inputaxis == "y")|| (inputaxis == "z"))
        {
            if (IsTaskActive("calibrate"))
            {
                if (!alive)
                {
                    appendLogLine("<color=red>Error #B2x002783</color>\nConnection Timed Out");
                    return;
                }
                if (inputaxis.ToUpper() == instableAxis.ToUpper())
                {
                    appendLogLine(inputaxis + " axis sucessfully recalibrated.");
                    CompleteTask();
                }
                else
                {
                    appendLogLine(inputaxis + " axis does not need to be calibrated.");
                }
            }
            else {
                appendLogLine("signal is already stable.");
            }

        }
        else
        {
            consoleView.PlayError();
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
                if (!alive)
                {
                    appendLogLine("<color=red>Error #B2x002783</color>\nConnection Timed Out");
                    return;
                }
                if (IsTaskActive("energy"))
                {
                    int i = operation == "dec" ? -1 : 1;

                    if (coreTemp + amount * i == 1000)
                    {
                        appendLogLine("energy successfully adjusted.");
                        CompleteTask();
                    }
                    else
                    {
                        LogTemp();
                        appendLogLine("operation not recommended!");
                    }
                }
                else {
                    appendLogLine("core temperature is stable.");
                }
            }
            else
            {
                consoleView.PlayError();
                LogTemp();
                appendLogLine(amount.ToString() + " is not a valid amount.");
            }

        }
        else
        {
            consoleView.PlayError();
            LogTemp();
            appendLogLine(args[0] + " is not a valid command");
        }
    }

    void allocate(string[] args)
    {
        if (args.Length < 1)
        {
            appendLogLine("Expected 1 argument.\nallocate [number]");
            return;
        }

        float numberInput = 0;
        if (float.TryParse(args[0], out numberInput))
        {
            if (!alive)
            {
                appendLogLine("<color=red>Error #B2x002783</color>\nConnection Timed Out");
                return;
            }
            if (IsTaskActive("allocate"))
            {
                if (numberInput == memorynumber)
                {
                    appendLogLine("hashing complete!\n" + numberInput.ToString() + " successfully allocated!");
                    CompleteTask();
                }
                else
                {
                    appendLogLine(numberInput.ToString() + " not found. cannot allocate." + memoryHash);
                }
            }
            else
            {
                appendLogLine("memory is not corrupted.");
            }

        }
        else
        {
            consoleView.PlayError();
            appendLogLine(args[0] + " is not numeric.");
        }
    }

    void exit(string[] args)
    {
        SceneManager.LoadScene(0);
    }

    void LogTemp() {
        appendLogLine("energy core temperature: " + coreTemp.ToString() + "\nrecommended temperature: 1000");
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
    #endregion

	public void AssignImageEffect(ImageEffect effect){
		glitchEffect = effect;
	}

    public void AssignConsoleView(ConsoleView cv) {
        consoleView = cv;
    }

	public void Die(Timer timer) {
		timer.Stop();
		Highscores.Add("Linus", timer.seconds);
		Debug.Log(Highscores.AsString());
        alive = false;
		appendLogLine("\n<color=red>Connection timed out!</color>\nConnection time: " + timer.asString());
    }
}