using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods {

	#region Class members
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	public static void DrawCapsule2D (Vector3 center, Vector2 size, float angle, CapsuleDirection2D direction) {
		if (direction == CapsuleDirection2D.Horizontal)
			size.x = Mathf.Clamp (size.x, size.y, float.MaxValue);
		else
			size.y = Mathf.Clamp (size.y, size.x, float.MaxValue);

		Vector3 rectSize = (direction == CapsuleDirection2D.Horizontal) ? new Vector2 (size.x - size.y, size.y) : new Vector2 (size.x, size.y - size.x);
		float radius = Mathf.Min (size.x, size.y) / 2f;

		Vector3 rightNorm = -Vector3.SlerpUnclamped (Vector3.left, Vector3.down, angle * (4 / 360f));
		Vector3 upNorm = new Vector3 (-rightNorm.y, rightNorm.x);
		Vector3 left = center - (rightNorm * rectSize.x / 2f);
		Vector3 right = center + (rightNorm * rectSize.x / 2f);
		Vector3 down = center - (upNorm * rectSize.y / 2f);
		Vector3 up = center + (upNorm * rectSize.y / 2f);

		Vector3[] corners = new Vector3[4];
		corners[0] = center + (up - right);
		corners[1] = center + (down - right);
		corners[2] = center + (up - left);
		corners[3] = center + (down - left);

		#region Debug
		//Color[] colors = new Color[4] { Color.blue, Color.green, Color.yellow, Color.cyan };

		//Gizmos.color = colors[0];
		//Gizmos.DrawSphere (left, 0.125f);
		//Gizmos.color = colors[1];
		//Gizmos.DrawSphere (right, 0.125f);
		//Gizmos.color = colors[2];
		//Gizmos.DrawSphere (down, 0.125f);
		//Gizmos.color = colors[3];
		//Gizmos.DrawSphere (up, 0.125f);

		//for (int i = 0; i < corners.Length; i++) {
		//	Gizmos.color = colors[i];
		//	Gizmos.DrawSphere (corners[i], 0.125f);
		//} 
		#endregion

		if (direction == CapsuleDirection2D.Horizontal) {
			Gizmos.DrawLine (corners[0], corners[2]);
			Gizmos.DrawLine (corners[1], corners[3]);
		}
		else {
			Gizmos.DrawLine (corners[0], corners[1]);
			Gizmos.DrawLine (corners[2], corners[3]);
		}

		rightNorm *= radius;
		upNorm *= radius;

		float arcSteps = 25;
		for (int i = 0; i < arcSteps * 2; i++) {
			if (direction == CapsuleDirection2D.Horizontal) {
				Gizmos.DrawLine (Vector3.SlerpUnclamped (-upNorm, -rightNorm, i / arcSteps) + left, Vector3.SlerpUnclamped (-upNorm, -rightNorm, (i + 1) / arcSteps) + left);
				Gizmos.DrawLine (Vector3.SlerpUnclamped (-upNorm, rightNorm, i / arcSteps) + right, Vector3.SlerpUnclamped (-upNorm, rightNorm, (i + 1) / arcSteps) + right);
			}
			else {
				Gizmos.DrawLine (Vector3.SlerpUnclamped (-rightNorm, upNorm, i / arcSteps) + up, Vector3.SlerpUnclamped (-rightNorm, upNorm, (i + 1) / arcSteps) + up);
				Gizmos.DrawLine (Vector3.SlerpUnclamped (-rightNorm, -upNorm, i / arcSteps) + down, Vector3.SlerpUnclamped (-rightNorm, -upNorm, (i + 1) / arcSteps) + down);
			}
		}
	}

	public static void SetBool (string key, bool value) {
		PlayerPrefs.SetInt (key, value ? 1 : 0);
	}

	public static bool GetBool (string key, bool defaultValue) {
		return PlayerPrefs.GetInt (key, defaultValue ? 1 : 0) == 1 ? true : false;
	}

	public static bool GetBool (string key) {
		return PlayerPrefs.GetInt (key) == 1 ? true : false;
	}
	#endregion

	#region Interface implementation
	#endregion
}