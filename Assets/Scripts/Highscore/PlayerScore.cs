using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : IComparable<PlayerScore> {

	public string name;
	public float score;

	public PlayerScore(string name, float score) {
		this.name = name;
		this.score = score;
	}

	public int CompareTo(PlayerScore other) {
		return score.CompareTo(other.score);
	}
}
