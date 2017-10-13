using System;
using System.Collections.Generic;
using UnityEngine;

public enum TweenState { Running, Paused, Stopped }

public enum TweenStopState {
	DoNotModify, // Does not change the current value.
	Complete // Causes the tween to progress to the end value immediately.
}

public class Tween : MonoBehaviour {

	private static GameObject m_Root; //Root object, contains Tween script
	private static readonly List<ITween> m_Tweens = new List<ITween> ();

	private void Awake () {
		m_Tweens.Clear ();
	}

	private void Update () {
		ITween tween;

		for (int i = m_Tweens.Count - 1; i >= 0; i--) {
			tween = m_Tweens[i];

			if (tween.Reference == null || !tween.Reference.activeSelf) {
				m_Tweens.RemoveAt (i);
				continue;
			}

			if (tween.Update (Time.deltaTime) && i < m_Tweens.Count && m_Tweens[i] == tween)
				m_Tweens.RemoveAt (i);
		}
	}

	private static void CreateRoot () {
		if (m_Root == null)
			m_Root = new GameObject ("Tween", typeof (Tween));
	}

	public static FloatTween FloatTween (GameObject reference, string key, float start, float end, float duration, Action<ITween> progress) {
		return InitFloatTween (reference, key, start, end, duration, TweenFunctions.Linear, progress, null);
	}

	public static FloatTween FloatTween (GameObject reference, string key, float start, float end, float duration, Action<ITween> progress, Action<ITween> finish) {
		return InitFloatTween (reference, key, start, end, duration, TweenFunctions.Linear, progress, finish);
	}

	public static FloatTween FloatTween (GameObject reference, string key, float start, float end, float duration, Func<float, float> scaleFunction, Action<ITween> progress) {
		return InitFloatTween (reference, key, start, end, duration, scaleFunction, progress, null);
	}

	public static FloatTween FloatTween (GameObject reference, string key, float start, float end, float duration, Func<float, float> scaleFunction, Action<ITween> progress, Action<ITween> finish) {
		return InitFloatTween (reference, key, start, end, duration, scaleFunction, progress, finish);
	}

	protected static FloatTween InitFloatTween (GameObject reference, string key, float start, float end, float duration, Func<float, float> scaleFunction, Action<ITween> progress, Action<ITween> finish) {
		FloatTween floatTween = new FloatTween ();

		floatTween.Key = reference.GetInstanceID () + key;
		floatTween.Start (reference, start, end, duration, scaleFunction, progress, finish);

		AddTween (floatTween);
		return floatTween;
	}

	protected static void AddTween (ITween tween) {
		CreateRoot ();
		if (tween.Key != null)
			RemoveTween (tween.Key, TweenStopState.DoNotModify);

		m_Tweens.Add (tween);
	}

	protected static bool RemoveTween (ITween tween, TweenStopState stopBehavior) {
		tween.Stop (stopBehavior);
		return m_Tweens.Remove (tween);
	}

	protected static bool RemoveTween (string key, TweenStopState stopBehavior) {
		bool foundOne = false;
		for (int i = m_Tweens.Count - 1; i >= 0; i--) {
			ITween tween = m_Tweens[i];

			if (key.Equals (tween.Key)) {
				tween.Stop (stopBehavior);
				m_Tweens.RemoveAt (i);
				foundOne = true;
			}
		}
		return foundOne;
	}
}

public interface ITween {

	string Key { get; }
	GameObject Reference { get; }

	TweenState State { get; }

	float Value { get; }
	float Progress { get; }

	void Start (GameObject reference, float start, float end, float duration, Func<float, float> scaleFunction, Action<ITween> progress, Action<ITween> finish);

	void Pause ();
	void Resume ();

	void Stop (TweenStopState stopBehavior);

	bool Update (float elapsedTime);
}

public class FloatTween : ITween {

	private Func<float, float> m_ScaleFunction;

	public string Key { get; set; }
	public GameObject Reference { get; set; }

	public TweenState State { get; private set; }

	public float Duration { get; private set; }

	public float StartValue { get; private set; }
	public float EndValue { get; private set; }
	public float CurrentTime { get; private set; }
	public float Value { get; private set; }

	public float Progress { get; private set; }

	private Action<ITween> m_ProgressCallback;
	private Action<ITween> m_FinishCallback;

	public void Start (GameObject reference, float start, float end, float duration, Func<float, float> scaleFunction, Action<ITween> progress, Action<ITween> finish) {
		if (reference == null)
			return;

		Reference = reference;

		m_ScaleFunction = scaleFunction;
		State = TweenState.Running;

		Duration = duration;
		StartValue = start;
		EndValue = end;

		m_ProgressCallback = progress;
		m_FinishCallback = finish;

		CurrentTime = 0;
		UpdateValue ();
	}

	public void Pause () {
		if (State == TweenState.Running)
			State = TweenState.Paused;
	}

	public void Resume () {
		if (State == TweenState.Paused)
			State = TweenState.Running;
	}

	public void Stop (TweenStopState stopBehavior) {
		if (State != TweenState.Stopped) {
			State = TweenState.Stopped;

			if (stopBehavior == TweenStopState.Complete) {
				CurrentTime = Duration;
				UpdateValue ();

				if (m_FinishCallback != null) {
					m_FinishCallback.Invoke (this);
					m_FinishCallback = null;
				}
			}
		}
	}

	public bool Update (float elapsedTime) {
		if (Reference == null) {
			Stop (TweenStopState.DoNotModify);
			return false;
		}

		if (State == TweenState.Running) {
			CurrentTime += elapsedTime;

			if (CurrentTime >= Duration) {
				Stop (TweenStopState.Complete);
				return true;
			}
			else {
				UpdateValue ();
				return false;
			}
		}
		return (State == TweenState.Stopped);
	}

	private void UpdateValue () {
		if (Reference == null) {
			Stop (TweenStopState.DoNotModify);
			return;
		}

		Progress = m_ScaleFunction (CurrentTime / Duration);
		Value = Mathf.Lerp (StartValue, EndValue, Progress);

		if (m_ProgressCallback != null)
			m_ProgressCallback.Invoke (this);
	}
}

public static class TweenFunctions {
	private const float HalfPi = Mathf.PI * 0.5f;

	// A linear progress scale function.
	public static readonly Func<float, float> Linear = m_Linear;
	private static float m_Linear (float progress) { return progress; }

	// A quadratic (x^2) progress scale function that eases in.
	public static readonly Func<float, float> QuadraticEaseIn = m_QuadraticEaseIn;
	private static float m_QuadraticEaseIn (float progress) { return EaseInPower (progress, 2); }

	// A quadratic (x^2) progress scale function that eases out.
	public static readonly Func<float, float> QuadraticEaseOut = m_QuadraticEaseOut;
	private static float m_QuadraticEaseOut (float progress) { return EaseOutPower (progress, 2); }

	// A quadratic (x^2) progress scale function that eases in and out.
	public static readonly Func<float, float> QuadraticEaseInOut = m_QuadraticEaseInOut;
	private static float m_QuadraticEaseInOut (float progress) { return EaseInOutPower (progress, 2); }

	// A cubic (x^3) progress scale function that eases in.
	public static readonly Func<float, float> CubicEaseIn = m_CubicEaseIn;
	private static float m_CubicEaseIn (float progress) { return EaseInPower (progress, 3); }

	// A cubic (x^3) progress scale function that eases out.
	public static readonly Func<float, float> CubicEaseOut = m_CubicEaseOut;
	private static float m_CubicEaseOut (float progress) { return EaseOutPower (progress, 3); }

	// A cubic (x^3) progress scale function that eases in and out.
	public static readonly Func<float, float> CubicEaseInOut = m_CubicEaseInOut;
	private static float m_CubicEaseInOut (float progress) { return EaseInOutPower (progress, 3); }

	// A quartic (x^4) progress scale function that eases in.
	public static readonly Func<float, float> QuarticEaseIn = m_QuarticEaseIn;
	private static float m_QuarticEaseIn (float progress) { return EaseInPower (progress, 4); }

	// A quartic (x^4) progress scale function that eases out.
	public static readonly Func<float, float> QuarticEaseOut = m_QuarticEaseOut;
	private static float m_QuarticEaseOut (float progress) { return EaseOutPower (progress, 4); }

	// A quartic (x^4) progress scale function that eases in and out.
	public static readonly Func<float, float> QuarticEaseInOut = m_QuarticEaseInOut;
	private static float m_QuarticEaseInOut (float progress) { return EaseInOutPower (progress, 4); }

	// A quintic (x^5) progress scale function that eases in.
	public static readonly Func<float, float> QuinticEaseIn = m_QuinticEaseIn;
	private static float m_QuinticEaseIn (float progress) { return EaseInPower (progress, 5); }

	// A quintic (x^5) progress scale function that eases out.
	public static readonly Func<float, float> QuinticEaseOut = m_QuinticEaseOut;
	private static float m_QuinticEaseOut (float progress) { return EaseOutPower (progress, 5); }

	// A quintic (x^5) progress scale function that eases in and out.
	public static readonly Func<float, float> QuinticEaseInOut = m_QuinticEaseInOut;
	private static float m_QuinticEaseInOut (float progress) { return EaseInOutPower (progress, 5); }

	// A sine progress scale function that eases in.
	public static readonly Func<float, float> SineEaseIn = m_SineEaseIn;
	private static float m_SineEaseIn (float progress) { return Mathf.Sin (progress * HalfPi - HalfPi) + 1; }

	// A sine progress scale function that eases out.
	public static readonly Func<float, float> SineEaseOut = m_SineEaseOut;
	private static float m_SineEaseOut (float progress) { return Mathf.Sin (progress * HalfPi); }

	// A sine progress scale function that eases in and out.
	public static readonly Func<float, float> SineEaseInOut = m_SineEaseInOut;
	private static float m_SineEaseInOut (float progress) { return (Mathf.Sin (progress * Mathf.PI - HalfPi) + 1) / 2; }

	private static float EaseInPower (float progress, int power) {
		return Mathf.Pow (progress, power);
	}

	private static float EaseOutPower (float progress, int power) {
		int sign = power % 2 == 0 ? -1 : 1;
		return (sign * (Mathf.Pow (progress - 1, power) + sign));
	}

	private static float EaseInOutPower (float progress, int power) {
		progress *= 2.0f;
		if (progress < 1)
			return Mathf.Pow (progress, power) / 2.0f;
		else {
			int sign = power % 2 == 0 ? -1 : 1;
			return (sign / 2.0f * (Mathf.Pow (progress - 2, power) + sign * 2));
		}
	}
}