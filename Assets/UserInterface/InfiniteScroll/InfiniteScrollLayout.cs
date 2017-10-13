/* Written by Gustavo Garcia */
/* InfiniteScroll.cs ver. 1.0.0 */

using UnityEngine;

namespace PankioAssets {
	[ExecuteInEditMode]
	public class InfiniteScrollLayout : MonoBehaviour {

		[SerializeField]
		public Vector2 Offset;

		private RectTransform m_Root;
		private RectTransform[] m_Childs = new RectTransform[0];
		private InfiniteScroll._Orientation m_Orientation;

		private Vector2 m_CellSize;
		private float m_Spacing;

		private Vector2 m_PivotPosition;
		private Vector2 m_AnchorPosition;

#if UNITY_EDITOR
		private void Update () {
			if (Application.isPlaying)
				return;

			if (m_Root == null)
				m_Root = GetComponent<RectTransform> ();

			if (m_Root != null && m_Childs.Length != m_Root.childCount) {
				m_Childs = new RectTransform[m_Root.childCount];
				for (int i = 0; i < m_Root.childCount; i++)
					m_Childs[i] = m_Root.GetChild (i) as RectTransform;
			}

			for (int i = 0; i < m_Childs.Length; i++)
				UpdateChild (i);
		}
#endif

		public void UpdateUI (Vector2 cellSize, float spacing, InfiniteScroll._Orientation orientation, Vector2 anchor, InfiniteScroll._Alignment alignment) {
			m_CellSize = cellSize;
			m_Spacing = spacing;
			m_Orientation = orientation;
			m_AnchorPosition = anchor;

			switch (alignment) {
				case InfiniteScroll._Alignment.TopLeft:
					m_PivotPosition = new Vector2 (0, 1);
					break;
				case InfiniteScroll._Alignment.TopCenter:
					m_PivotPosition = new Vector2 (0.5f, 1);
					break;
				case InfiniteScroll._Alignment.TopRight:
					m_PivotPosition = new Vector2 (1, 1);
					break;
				case InfiniteScroll._Alignment.MiddleLeft:
					m_PivotPosition = new Vector2 (0, 0.5f);
					break;
				case InfiniteScroll._Alignment.MiddleCenter:
					m_PivotPosition = new Vector2 (0.5f, 0.5f);
					break;
				case InfiniteScroll._Alignment.MiddleRight:
					m_PivotPosition = new Vector2 (1, 0.5f);
					break;
				case InfiniteScroll._Alignment.BottomLeft:
					m_PivotPosition = new Vector2 (0, 0);
					break;
				case InfiniteScroll._Alignment.BottomCenter:
					m_PivotPosition = new Vector2 (0.5f, 0);
					break;
				case InfiniteScroll._Alignment.BottomRight:
					m_PivotPosition = new Vector2 (1, 0);
					break;
				default:
					break;
			}
		}

		private void UpdateChild (int childIndex) {
			RectTransform child = m_Childs[childIndex];

			child.sizeDelta = m_CellSize;
			child.anchorMin = m_AnchorPosition;
			child.anchorMax = m_AnchorPosition;
			child.pivot = m_PivotPosition;

			float xPos = (m_PivotPosition.x - m_AnchorPosition.x) * m_CellSize.x;
			float yPos = (m_PivotPosition.y - m_AnchorPosition.y) * m_CellSize.y;
			Offset = new Vector2 (xPos, yPos);

			if (m_Orientation == InfiniteScroll._Orientation.Horizontal)
				child.anchoredPosition = new Vector2 (xPos + (m_CellSize.x + m_Spacing) * childIndex, yPos);
			else
				child.anchoredPosition = new Vector2 (xPos, yPos - (m_CellSize.y + m_Spacing) * childIndex);

		}
	}
}