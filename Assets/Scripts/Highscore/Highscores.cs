using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Highscores {

	private const string nameKey = "name";
	private const string scoreKey = "score";
	private const int max = 10;

	private static List<PlayerScore> list;
	private static List<PlayerScore> List {
		get {
			if (list == null) {
				list = new List<PlayerScore>();
				for (int i = 0; i < max; i++) {
					string name = PlayerPrefs.GetString(nameKey + i, "");
					float score = PlayerPrefs.GetFloat(scoreKey + i, 0);
					if (name.Length > 0 && score != 0) {
						list.Add(new PlayerScore(name, score));
					}
				}
			}
			return list;
		}
	}

	public static void Add(string name, float score) {
		List.Add(new PlayerScore(name, score));
		List.Sort();
		if (List.Count > max) {
			List.RemoveAt(List.Count - 1);
		}
		Save ();
	}

	private static void Save() {
		for (int i = 0; i < List.Count; i++) {
			PlayerPrefs.SetString(nameKey + i, List[i].name);
			PlayerPrefs.SetFloat(scoreKey + i, List[i].score);
		}
	}

	public static string AsString() {
		string result = "";
		for (int i = 0; i < List.Count; i++) {
			result += (i+1) + " " + List[i].name + " " + Format(List[i].score) + "\n";
		}
		return result;
	}

    public static string Format(float score) {
        TimeSpan t = TimeSpan.FromSeconds(score);
        return string.Format("<color=red>{0:D2}:{1:D2}.{2:D3}</color>",
            t.Minutes,
            t.Seconds,
            t.Milliseconds);
    }
}
