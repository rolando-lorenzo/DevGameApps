/* Written by Gustavo Garcia */
/* InfiniteScroll.cs ver. 1.0.0 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PankioAssets {
	[Serializable]
	public class ScrollItem {
		public GameObject gameObject;
		public Transform transform;
		public RectTransform rectTransform;
		public CanvasGroup canvasGroup;

		public ScrollItem (GameObject gameObject, Transform transform, RectTransform rectTransform, CanvasGroup canvasGroup) {
			this.gameObject = gameObject;
			this.transform = transform;
			this.rectTransform = rectTransform;
			this.canvasGroup = canvasGroup;
		}
	}

	[RequireComponent (typeof (ScrollRect))]
	[ExecuteInEditMode]
	public class InfiniteScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

		#region Variables
		public enum _State { Working, MissingObject, MissingItem }
		/// <summary>
		/// List of Infinite Scroll states (Editor Only).
		/// </summary>
		public _State State { get; private set; }

		public enum _Orientation { Horizontal, Vertical }
		/// <summary>
		/// Orientation of content.
		/// </summary>
		public _Orientation Orientation;
		private bool m_Horizontal { get { return Orientation == _Orientation.Horizontal; } }

		public enum _Alignment { TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight }
		/// <summary>
		/// Child alignment
		/// </summary>
		public _Alignment Aligment;

		public enum _HorizontalAlignment { Top, Middle, Bottom }
		/// <summary>
		/// Horizontal child alignment
		/// </summary>
		public _HorizontalAlignment HorizontalAlignment;

		public enum _VerticalAlignment { Left, Middle, Right }
		/// <summary>
		/// Vertical child alignment
		/// </summary>
		public _VerticalAlignment VerticalAlignment;

		[SerializeField, HideInInspector]
		private ScrollRect m_Root;
		private float m_LastScrollVelocity;

		/// <summary>
		/// The rate at which movement slows down.
		/// </summary>
		public float DecelerationRate;
		private float m_DecelerationRate { get { return DecelerationRate = Mathf.Clamp (DecelerationRate, 0, float.MaxValue); } }

		/// <summary>
		/// Speed at which content moves to its destination.
		/// </summary>
		public float SnapSpeed;
		private float m_SnapSpeed { get { return SnapSpeed = Mathf.Clamp (SnapSpeed, 0, float.MaxValue); } }

		public enum _SnapTransition { Default, None }
		/// <summary>
		/// List of the trasition types.
		/// </summary>
		public _SnapTransition SnapTranstion;

		private float m_DragDirection;

		/// <summary>
		/// Can the item also change with buttons?
		/// </summary>
		public bool UseButtons;

		/// <summary>
		/// Button to go to the previous item.
		/// </summary>
		public Button PreviousButton;

		/// <summary>
		/// Button to go to the next item.
		/// </summary>
		public Button NextButton;

		public delegate void ItemChangedAction (ScrollItem previousItem, ScrollItem currentItem, int itemIndex);
		/// <summary>
		/// Called when current item is changed
		/// </summary>
		public event ItemChangedAction OnItemChanged;

		/// <summary>
		/// List of items.
		/// </summary>
		public List<ScrollItem> Items { get { return m_Items; } }
		[SerializeField]
		private List<ScrollItem> m_Items = new List<ScrollItem> ();

		private bool m_AutoDetectItem;
		private int m_CurrentItem;

		/// <summary>
		/// Returns current item.
		/// Set the target item.
		/// </summary>
		public int CurrentItem {
			get { return m_CurrentItem; }
			set {
				m_AutoDetectItem = true;
				int prevItem = m_CurrentItem;
				m_CurrentItem = value;

				if (value > m_Items.Count - 1)
					m_CurrentItem = 0;
				else if (value < 0)
					m_CurrentItem = m_Items.Count - 1;

				ChangeItem (prevItem, m_CurrentItem);
				SnapItem = true;
			}
		}

		[SerializeField, HideInInspector]
		private RectTransform m_Content;
		[SerializeField, HideInInspector]
		private InfiniteScrollLayout ItemsLayout;

		/// <summary>
		/// Size of item.
		/// </summary>
		public Vector2 ItemSize;
		private Vector2 m_ItemSize { get { return ItemSize = new Vector2 (Mathf.Clamp (ItemSize.x, 0, Mathf.Infinity), Mathf.Clamp (ItemSize.y, 0, Mathf.Infinity)); } }

		/// <summary>
		/// Space between items.
		/// </summary>
		public float ItemSpacing;
		private float m_ItemSpacing { get { return ItemSpacing = Mathf.Clamp (ItemSpacing, 0, Mathf.Infinity); } }

		private bool SnapItem;

		[SerializeField, HideInInspector]
		private float ItemOffset;
		[SerializeField, HideInInspector]
		private float MaxItemDist;

		[SerializeField, HideInInspector]
		private List<Image> Indicators = new List<Image> ();
		[SerializeField, HideInInspector]
		private RectTransform m_RootIndicators;
		[SerializeField, HideInInspector]
		private InfiniteScrollLayout IndicatorsLayout;

		/// <summary>
		/// Show items indicators?
		/// </summary>
		public bool ShowIndicators;

		/// <summary>
		/// Position of the indicators with regard to their pivot.
		/// </summary>
		public float RootIndicatorsPosition;
		private float m_RootIndicatorsPosition { get { return RootIndicatorsPosition = Mathf.Clamp (RootIndicatorsPosition, 0, Mathf.Infinity); } }

		public enum _Anchor { Left, Right }

		/// <summary>
		/// Anchor of indicators, only works if the orientation is vertical.
		/// </summary>
		public _Anchor RootIndicatorsAnchor;

		/// <summary>
		/// Graphic sprite of indicator.
		/// </summary>
		public Sprite IndicatorSprite;

		/// <summary>
		/// Size of indicator.
		/// </summary>
		public float IndicatorSize;
		private float m_IndicatorSize { get { return IndicatorSize = Mathf.Clamp (IndicatorSize, 0, Mathf.Infinity); } }

		/// <summary>
		/// Space between indicators.
		/// </summary>
		public float IndicatorSpacing;
		private float m_IndicatorSpacing { get { return IndicatorSpacing = Mathf.Clamp (IndicatorSpacing, 0, Mathf.Infinity); } }

		/// <summary>
		/// Indicator color when it does not belong to the current item.
		/// </summary>
		public Color NormalIndicatorColor;

		/// <summary>
		/// Indicator color when it belongs to the current article.
		/// </summary>
		public Color ActiveIndicatorColor;

		[SerializeField, HideInInspector]
		private RectTransform m_Viewport;

		/// <summary>
		/// Create a mask to display only a few items?
		/// </summary>
		public bool HideItems;

		/// <summary>
		/// Show mask graphic?
		/// </summary>
		public bool ShowMaskGraphic;

		/// <summary>
		/// Number of visible items, only works if the HideItems it's true.
		/// </summary>
		public int VisibleItems;
		private int m_VisibleItems {
			get {
				VisibleItems = Mathf.Clamp (VisibleItems, 1, MaxVisibleItems);
				VisibleItems = VisibleItems % 2 == 0 ? VisibleItems + 1 : VisibleItems;

				return VisibleItems;
			}
		}

		/// <summary>
		/// Get maximun visible number of items (Editor only).
		/// </summary>
		[HideInInspector]
		public int MaxVisibleItems { get { return m_Items.Count % 2 == 0 ? m_Items.Count - 1 : m_Items.Count; } }

		/// <summary>
		/// Are items scaled according to their position?
		/// </summary>
		public bool ScaleEffect;

		/// <summary>
		/// Percent of scale, only works if ScaleEffect it's true.
		/// </summary>
		public float ScalePercent;
		private float m_ScalePercent { get { return ScalePercent = Mathf.Clamp (ScalePercent, -5, 1); } }

		/// <summary>
		/// Are articles opaque according to their position?
		/// </summary>
		public bool AlphaEffect;

		/// <summary>
		/// Percent of opaque, only works if AlphaEffect it's true.
		/// </summary>
		public float AlphaPercent;
		private float m_AlphaPercent { get { return AlphaPercent = Mathf.Clamp (AlphaPercent, -5, 1); } }
		#endregion

#if UNITY_EDITOR
		[MenuItem ("GameObject/UI/Extensions/Infinite Scroll", false, 1000)]
		static void CreateInfiniteScroll () {
			// Root
			Transform root = new GameObject ("InfiniteScroll", typeof (RectTransform), typeof (ScrollRect), typeof (InfiniteScroll)).transform;
			root.GetComponent<ScrollRect> ().hideFlags = HideFlags.HideInInspector;

			// Canvas
			Canvas canvas = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<Canvas> () : null;
			if (canvas == null) {
				if (FindObjectOfType<Canvas> () != null)
					canvas = FindObjectOfType<Canvas> ();
				else {
					canvas = new GameObject ("Canvas", typeof (RectTransform), typeof (Canvas), typeof (CanvasScaler), typeof (GraphicRaycaster)).GetComponent<Canvas> ();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					canvas.gameObject.layer = 5;
				}
			}

			root.SetParent (canvas.transform, false);

			// Viewport
			Transform viewport = new GameObject ("Viewport", typeof (RectTransform), typeof (Image), typeof (Mask), typeof (InfiniteScrollObject)).transform;
			viewport.SetParent (root, false);
			viewport.gameObject.layer = 5;

			Image viewportImage = viewport.GetComponent<Image> ();
#if UNITY_EDITOR
			viewportImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite> ("UI/Skin/UIMask.psd");
#endif
			viewportImage.color = Color.white;
			viewportImage.type = Image.Type.Sliced;
			viewport.GetComponent<Mask> ().hideFlags = HideFlags.HideInInspector;
			viewport.GetComponent<InfiniteScrollObject> ().hideFlags = HideFlags.HideInInspector;

			// Content
			Transform content = new GameObject ("Content", typeof (RectTransform), typeof (InfiniteScrollLayout), typeof (InfiniteScrollObject)).transform;
			content.SetParent (viewport, false);
			content.gameObject.layer = 5;

			content.GetComponent<InfiniteScrollLayout> ().hideFlags = HideFlags.HideInInspector;
			content.GetComponent<InfiniteScrollObject> ().hideFlags = HideFlags.HideInInspector;

			// Root Indicators
			Transform rootIndicators = new GameObject ("Indicators", typeof (RectTransform), typeof (InfiniteScrollLayout), typeof (InfiniteScrollObject)).transform;
			rootIndicators.SetParent (root, false);
			rootIndicators.gameObject.layer = 5;

			rootIndicators.hideFlags = HideFlags.HideInHierarchy;
			rootIndicators.GetComponent<InfiniteScrollLayout> ().hideFlags = HideFlags.HideInInspector;
			rootIndicators.GetComponent<InfiniteScrollObject> ().hideFlags = HideFlags.HideInInspector;

			root.GetComponent<InfiniteScroll> ().UpdateUI ();
			for (int i = 0; i < 5; i++)
				root.GetComponent<InfiniteScroll> ().AddItem ();

			Selection.activeGameObject = root.gameObject;
		}
#endif
#if UNITY_EDITOR
		private void Reset () {
			SnapSpeed = 5;
			DecelerationRate = 0.135f;

			HorizontalAlignment = _HorizontalAlignment.Middle;
			VerticalAlignment = _VerticalAlignment.Middle;
			Aligment = _Alignment.MiddleCenter;

			ScaleEffect = true;
			AlphaEffect = true;

			ItemSize = Vector2.one * 150;
			ItemSpacing = 25;

			ShowIndicators = true;
			IndicatorSprite = AssetDatabase.GetBuiltinExtraResource<Sprite> ("UI/Skin/Knob.psd");
			NormalIndicatorColor = Color.white;
			ActiveIndicatorColor = new Color (1, 1, 1, 0.5f);
			IndicatorSize = 15;
			IndicatorSpacing = 15;
			RootIndicatorsPosition = 10;
		}
#endif
		private void Start () {
			if (!Application.isPlaying)
				return;

			if (UseButtons && PreviousButton != null && NextButton != null) {
				PreviousButton.onClick.AddListener (() => CurrentItem--);
				NextButton.onClick.AddListener (() => CurrentItem++);
			}

			m_Root.onValueChanged.AddListener (OnScroll);
			OnScroll (Vector2.zero);
		}

		private void Update () {
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				UpdateUI ();
				return;
			}
#endif

			if (SnapItem) {
				float scrollVelocity = Math.Abs (m_Horizontal ? m_Root.velocity.x : m_Root.velocity.y);
				float velocity = scrollVelocity - m_LastScrollVelocity;
				m_LastScrollVelocity = scrollVelocity;

				if (Mathf.Abs (velocity) < 10) {
					m_Root.velocity = Vector2.zero;
					Vector3 targetPosition = -m_Items[CurrentItem].rectTransform.anchoredPosition + ItemsLayout.Offset;

					if (SnapTranstion == _SnapTransition.Default)
						m_Content.anchoredPosition = Vector3.Lerp (m_Content.anchoredPosition, targetPosition, m_SnapSpeed * Time.deltaTime);
					else
						m_Content.localPosition = targetPosition;
				}
			}
		}

		/// <summary>
		/// Called when the position of the item chagues.
		/// See IScrollHandler
		/// </summary>
		/// <param name="value"></param>
		public void OnScroll (Vector2 value) {
			for (int i = 0; i < m_Items.Count; i++) {
				Vector2 scrollDirection = m_Horizontal ? Vector3.right : Vector3.down;
				Vector3 vectorDist = m_Root.transform.InverseTransformPoint (m_Items[i].rectTransform.position) - (Vector3) ItemsLayout.Offset;
				float itemDist = m_Horizontal ? vectorDist.x : vectorDist.y;

				if (!m_AutoDetectItem) {
					if (Mathf.Abs (itemDist) < Math.Abs (ItemOffset) / 2) {
						if (m_CurrentItem != i)
							ChangeItem (m_CurrentItem, i);
					}
				}

				if (CurrentItem == i) {
					if (Indicators[i].color != ActiveIndicatorColor)
						Indicators[i].color = ActiveIndicatorColor;
				}
				else if (Indicators[i].color != NormalIndicatorColor)
					Indicators[i].color = NormalIndicatorColor;

				if (itemDist > MaxItemDist)
					m_Items[i].rectTransform.anchoredPosition -= scrollDirection * ItemOffset * m_Items.Count;
				else if (itemDist < -MaxItemDist)
					m_Items[i].rectTransform.anchoredPosition += scrollDirection * ItemOffset * m_Items.Count;

				if (ScaleEffect || AlphaEffect) {
					float distPerc = GetPercent (0, MaxItemDist, itemDist);

					if (ScaleEffect) {
						float scalePerc = distPerc + ((1 - distPerc) * m_ScalePercent);
						m_Items[i].rectTransform.localScale = Vector3.one * Mathf.Clamp01 (scalePerc);
					}

					if (AlphaEffect) {
						float alphaPerc = distPerc + ((1 - distPerc) * m_AlphaPercent);
						m_Items[i].canvasGroup.alpha = Mathf.Clamp01 (alphaPerc);
					}
				}

				if (!ScaleEffect) {
					if (m_Items[i].rectTransform.localScale != Vector3.one)
						m_Items[i].rectTransform.localScale = Vector3.one;
				}
				if (!AlphaEffect) {
					if (m_Items[i].canvasGroup.alpha != 1)
						m_Items[i].canvasGroup.alpha = 1;
				}
			}

		}

		private void ChangeItem (int previousIndex, int currentIndex) {
			m_CurrentItem = currentIndex;
			Indicators[previousIndex].color = NormalIndicatorColor;
			Indicators[currentIndex].color = ActiveIndicatorColor;

			if (OnItemChanged != null)
				OnItemChanged (m_Items[previousIndex], m_Items[currentIndex], currentIndex);
		}

		private _State Setup () {
			ScrollRect root = GetComponent<ScrollRect> ();

			if (root != null)
				m_Root = root;
			else
				return _State.MissingObject;

			int foundObjects = 0;

			for (int i = 0; i < m_Root.transform.childCount; i++) {
				Transform child = m_Root.transform.GetChild (i);

				if (child.GetComponent<InfiniteScrollObject> () != null) {
					if (child.GetComponent<Mask> () != null) {
						m_Viewport = child.GetComponent<RectTransform> ();
						foundObjects++;
					}
					else if (child.GetComponent<InfiniteScrollLayout> () != null) {
						m_RootIndicators = child.GetComponent<RectTransform> ();
						foundObjects++;
					}
				}
			}

			if (foundObjects != 2)
				return _State.MissingObject;

			foundObjects = 0;

			for (int i = 0; i < m_Viewport.childCount; i++) {
				Transform child = m_Viewport.GetChild (i);

				if (child.GetComponent<InfiniteScrollObject> () != null) {
					if (child.GetComponent<InfiniteScrollLayout> () != null) {
						m_Content = child.GetComponent<RectTransform> ();
						foundObjects++;
					}
				}
			}

			if (foundObjects != 1)
				return _State.MissingObject;

			ItemsLayout = m_Content.GetComponent<InfiniteScrollLayout> ();
			IndicatorsLayout = m_RootIndicators.GetComponent<InfiniteScrollLayout> ();

			m_Items.Clear ();
			for (int i = 0; i < m_Content.childCount; i++) {
				GameObject item = m_Content.GetChild (i).gameObject;
				m_Items.Add (new ScrollItem (item, item.transform, item.GetComponent<RectTransform> (), item.GetComponent<CanvasGroup> ()));
			}

			while (m_RootIndicators.childCount > m_Items.Count) {
				DestroyImmediate (m_RootIndicators.GetChild (m_RootIndicators.childCount - 1).gameObject);
			}

			while (m_RootIndicators.childCount < m_Items.Count) {
				GameObject indicator = new GameObject ("Indicator" + m_RootIndicators.childCount, typeof (Image));
				indicator.transform.SetParent (m_RootIndicators, false);
				indicator.gameObject.layer = 5;
			}

			Indicators.Clear ();
			for (int i = 0; i < m_RootIndicators.childCount; i++)
				Indicators.Add (m_RootIndicators.GetChild (i).GetComponent<Image> ());

			if (m_Content.childCount < 2)
				return _State.MissingItem;
			else
				return _State.Working;
		}

		/// <summary>
		/// Update the Infinite Scroll, only works in editor mode.
		/// </summary>
		public void UpdateUI () {
			if (!Application.isPlaying) {
				State = Setup ();
				if (State != _State.Working) return;
				UpdateRoot ();
			}

			m_Root.decelerationRate = m_DecelerationRate;
			SnapSpeed = m_SnapSpeed;

			if (!Application.isPlaying) {
				UpdateContent ();
				UpdateIndicators ();
				UpdateViewport ();

				Vector2 distance = m_Items[1].rectTransform.anchoredPosition - m_Items[0].rectTransform.anchoredPosition;
				ItemOffset = m_Horizontal ? distance.x : distance.y;
				MaxItemDist = Mathf.Abs (m_Items.Count * ItemOffset / 2);
			}
		}

		private void UpdateRoot () {
			if (SnapTranstion == _SnapTransition.Default) {
				m_Root.horizontal = m_Horizontal;
				m_Root.vertical = !m_Horizontal;
			}
			else {
				m_Root.horizontal = false;
				m_Root.vertical = false;
			}

			m_Root.content = m_Content;
			m_Root.viewport = m_Viewport;

			m_Root.movementType = ScrollRect.MovementType.Unrestricted;
			m_Root.inertia = true;

			m_Root.horizontalScrollbar = null;
			m_Root.verticalScrollbar = null;
		}

		private void UpdateContent () {
			ResetTransform (ref m_Content);
			m_Content.anchoredPosition = Vector2.zero;
			m_Content.sizeDelta = m_ItemSize;

			m_Content.anchorMin = Vector2.one * 0.5f;
			m_Content.anchorMax = Vector2.one * 0.5f;
			m_Content.pivot = Vector2.one * 0.5f;

			ItemsLayout.UpdateUI (m_ItemSize, m_ItemSpacing, Orientation, Vector2.one / 2, GetAlignment ());
		}

		private _Alignment GetAlignment () {
			if (Orientation == _Orientation.Horizontal) {
				switch (HorizontalAlignment) {
					case _HorizontalAlignment.Top:
						return _Alignment.TopCenter;
					case _HorizontalAlignment.Bottom:
						return _Alignment.BottomCenter;
				}
			}
			else {
				switch (VerticalAlignment) {
					case _VerticalAlignment.Left:
						return _Alignment.MiddleLeft;
					case _VerticalAlignment.Right:
						return _Alignment.MiddleRight;
				}
			}
			return _Alignment.MiddleCenter;
		}

		private void UpdateIndicators () {
			ResetTransform (ref m_RootIndicators);
			m_RootIndicators.gameObject.SetActive (ShowIndicators);

			IndicatorsLayout.UpdateUI (Vector2.one * IndicatorSize, m_IndicatorSpacing, Orientation, new Vector2 (0, 1), _Alignment.TopLeft);

			if (m_Horizontal) {
				m_RootIndicators.anchoredPosition = Vector2.down * (m_ItemSize.y / 2 + m_IndicatorSize / 2 + m_RootIndicatorsPosition);
				m_RootIndicators.sizeDelta = new Vector2 (m_IndicatorSize * Indicators.Count + m_IndicatorSpacing * (Indicators.Count - 1), m_IndicatorSize);
			}
			else {
				Vector2 anchor = RootIndicatorsAnchor == _Anchor.Left ? Vector2.left : Vector2.right;
				m_RootIndicators.anchoredPosition = anchor * (m_ItemSize.x / 2 + m_IndicatorSize / 2 + m_RootIndicatorsPosition);
				m_RootIndicators.sizeDelta = new Vector2 (m_IndicatorSize, m_IndicatorSize * Indicators.Count + m_IndicatorSpacing * (Indicators.Count - 1));
			}

			m_RootIndicators.anchorMin = Vector2.one * 0.5f;
			m_RootIndicators.anchorMax = Vector2.one * 0.5f;
			m_RootIndicators.pivot = Vector2.one * 0.5f;

			for (int i = 0; i < Indicators.Count; i++) {
				Indicators[i].color = i == 0 ? ActiveIndicatorColor : NormalIndicatorColor;
				Indicators[i].sprite = IndicatorSprite;
			}
		}

		private void UpdateViewport () {
			ResetTransform (ref m_Viewport);
			m_Viewport.anchoredPosition = Vector2.zero;

			int visibleItems = !HideItems ? m_Items.Count : m_VisibleItems;

			if (m_Horizontal)
				m_Viewport.sizeDelta = new Vector2 (m_ItemSize.x * visibleItems + m_ItemSpacing * (visibleItems - 1), m_ItemSize.y);
			else
				m_Viewport.sizeDelta = new Vector2 (m_ItemSize.x, m_ItemSize.y * visibleItems + m_ItemSpacing * (visibleItems - 1));

			m_Viewport.anchorMin = Vector2.one * 0.5f;
			m_Viewport.anchorMax = Vector2.one * 0.5f;
			m_Viewport.pivot = Vector2.one * 0.5f;

			Mask viewportMask = m_Viewport.GetComponent<Mask> ();
			viewportMask.enabled = HideItems;
			Color viewportColor = viewportMask.GetComponent<Image> ().color;

#if UNITY_EDITOR
			if (viewportMask.enabled) {
				viewportMask.showMaskGraphic = ShowMaskGraphic;
				viewportColor.a = EditorPrefs.GetFloat ("ISViewportAlpha");
			}
			else {
				if (viewportColor.a != 0)
					EditorPrefs.SetFloat ("ISViewportAlpha", viewportColor.a);
				viewportColor.a = ShowMaskGraphic ? EditorPrefs.GetFloat ("ISViewportAlpha") : 0;
			}

			m_Viewport.GetComponent<Image> ().color = viewportColor;
#endif
		}

		private void ResetTransform (ref RectTransform rectTransform) {
			rectTransform.localPosition = new Vector3 (rectTransform.localPosition.x, rectTransform.localPosition.y, 0);
			rectTransform.localRotation = Quaternion.Euler (Vector3.zero);
			rectTransform.localScale = Vector3.one;
		}

		public void FixParentObjects () {
			m_Viewport.SetParent (m_Root.transform, false);
			m_Content.SetParent (m_Viewport, false);
			m_RootIndicators.SetParent (m_Root.transform, false);
		}

		public void AddItem () {
			GameObject item = new GameObject ("Item" + m_Content.childCount, typeof (Image), typeof (CanvasGroup));
			item.transform.SetParent (m_Content, false);
			item.gameObject.layer = 5;

			item.GetComponent<Image> ().type = Image.Type.Sliced;
#if UNITY_EDITOR
			item.GetComponent<Image> ().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite> ("UI/Skin/UISprite.psd");
#endif
			item.GetComponent<CanvasGroup> ().hideFlags = HideFlags.HideInInspector;
		}

		public void RemoveItem () {
			if (m_Items.Count > 2)
				DestroyImmediate (m_Items[m_Items.Count - 1].gameObject);
		}

		private float GetPercent (float from, float to, float value) {
			float percent = from - to == 0 ? 1 : (from - value) / (from - to);
			return Mathf.Abs (1 - Mathf.Abs (percent));
		}

		public void OnBeginDrag (PointerEventData eventData) {
			SnapItem = false;
		}

		public void OnDrag (PointerEventData eventData) {
			m_DragDirection = m_Horizontal ? eventData.delta.x : eventData.delta.y;
		}

		public void OnEndDrag (PointerEventData eventData) {
			m_AutoDetectItem = false;

			if (SnapTranstion == _SnapTransition.None) {
				if (m_DragDirection < 0) {
					if (m_Horizontal) CurrentItem++;
					else CurrentItem--;
				}
				else if (m_DragDirection > 0) {
					if (m_Horizontal) CurrentItem--;
					else CurrentItem++;
				}
			}

			SnapItem = true;
		}
	}
}