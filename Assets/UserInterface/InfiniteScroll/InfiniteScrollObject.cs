/* Written by Gustavo Garcia */
/* InfiniteScroll.cs ver. 1.0.0 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PankioAssets {
	[ExecuteInEditMode]
	public class InfiniteScrollObject : MonoBehaviour {

#if UNITY_EDITOR
		private void OnDestroy () {
			if (!Application.isPlaying && Selection.activeObject == gameObject) {
				GameObject copy = Instantiate (gameObject, transform.parent);
				copy.name = gameObject.name;
				copy.SetActive (true);
			}
		}
#endif
	}
}