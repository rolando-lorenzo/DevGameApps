using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Test : MonoBehaviour {

	#region Class members
	public Transform[] points;
	public Transform trans;
	//public Transform[] corners;
	public Vector3[] middles;
	public Vector2 size;
	[Range (0, 360)]
	public float angle;
	#endregion

	#region Class accesors
	#endregion

	#region MonoBehaviour overrides
	private void Update () {
		Vector3 center = GetCenter (points).center;
		Vector3 size = GetCenter (points).size;

		Vector3 rightNorm = -Vector3.SlerpUnclamped (Vector3.left, Vector3.down, angle * (4 / 360f));
		Vector3 upNorm = new Vector3 (-rightNorm.y, rightNorm.x);
		Vector3 left = center - (rightNorm * size.x / 2f);
		Vector3 right = center + (rightNorm * size.x / 2f);
		Vector3 down = center - (upNorm * size.y / 2f);
		Vector3 up = center + (upNorm * size.y / 2f);

		Vector3[] corners = new Vector3[4];
		corners[0] = center + (up - right);
		corners[1] = center + (down - right);
		corners[2] = center + (up - left);
		corners[3] = center + (down - left);

		middles[0] = left;
		middles[1] = right;
		middles[2] = down;
		middles[3] = up;

		//corners[0].position = up - right;
		//corners[1].position = -up - right;
		//corners[2].position = up + right;
		//corners[3].position = -up + right;

		//for (int i = 0; i < points.Length; i++) {
		//	points[i].position = middles[i];
		//}
	}

	private void OnDrawGizmos () {
		for (int i = 0; i < middles.Length; i++) {
			Gizmos.DrawWireSphere (middles[i], 0.5f);
		}
	}

	private void LateUpdate () {
		//Vector2 pos = Vector2.zero;
		//for (int i = 0; i < bounds.Length; i++) {
		//	pos += (Vector2) bounds[i].position;
		//}
		//point.position = pos / bounds.Length;

		//Bounds bounds = GetCenter (points);
		//point.position = bounds.center;
		//point.localScale = bounds.size;
	}
	#endregion

	#region Super class overrides
	#endregion

	#region Class implementation
	private Bounds GetCenter (Transform[] points) {
		Vector2 minPoint = points[0].position;
		Vector2 maxPoint = points[0].position;

		for (int i = 1; i < points.Length; i++) {
			Vector2 pos = points[i].position;

			if (pos.x < minPoint.x)
				minPoint.x = pos.x;
			if (pos.x > maxPoint.x)
				maxPoint.x = pos.x;
			if (pos.y < minPoint.y)
				minPoint.y = pos.y;
			if (pos.y > maxPoint.y)
				maxPoint.y = pos.y;
		}

		Vector2 center = (maxPoint - minPoint) * 0.5f + minPoint;
		Vector2 size = maxPoint - minPoint;
		return new Bounds (center, size);
	}

	#endregion

	#region Interface implementation
	#endregion
}