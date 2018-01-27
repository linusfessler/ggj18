using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public static class Vibration {

	public static IEnumerator Vibrate(float duration, float intensity) {
		GamePad.SetVibration(0, intensity, intensity);
		yield return new WaitForSeconds(duration);
		GamePad.SetVibration(0, 0, 0);
	}
}
