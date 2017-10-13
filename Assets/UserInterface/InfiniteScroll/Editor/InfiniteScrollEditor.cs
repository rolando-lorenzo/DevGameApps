/* Written by Gustavo Garcia */
/* InfiniteScroll.cs ver. 1.0.0 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;

namespace PankioAssets {
	[CustomEditor (typeof (InfiniteScroll), true)]
	[CanEditMultipleObjects]
	public class InfiniteScrollEditor : Editor {

		private const string m_PrimaryStyle = "HelpBox";
		private const string m_SecondaryStyle = "TL SelectionButton";

		private InfiniteScroll m_Target;

		private AnimBool m_SnapTransitionDefaultAnim;
		private AnimBool m_ToggleButtonsAnim;
		private AnimBool m_HideItemsAnim;

		private void OnEnable () {
			m_Target = target as InfiniteScroll;
			m_SnapTransitionDefaultAnim = new AnimBool (m_Target.SnapTranstion == InfiniteScroll._SnapTransition.Default, Repaint);
			m_ToggleButtonsAnim = new AnimBool (m_Target.UseButtons, Repaint);
			m_HideItemsAnim = new AnimBool (m_Target.HideItems, Repaint);
		}

		public override void OnInspectorGUI () {
			BeginInspector ();

			DrawTitle ("Settings", FontStyle.Bold, m_PrimaryStyle);

			EditorGUI.BeginDisabledGroup (m_Target.State != InfiniteScroll._State.Working);
			DrawOrientationSettings ();
			DrawTransitionSettings ();
			DrawButtonSettings ();
			GUILayout.Space (2.5f);

			DrawItemsAndIndicatorsToolbar ();
			EditorGUI.EndDisabledGroup ();

			if (m_Target.State == InfiniteScroll._State.MissingObject) {
				EditorGUILayout.HelpBox ("One of the objects necessary for Infinite Scroll to work has been un-related, please press the fix button for re-teach that object.", MessageType.Error);

				if (GUILayout.Button ("Fix")) {
					Undo.RecordObject (target, "IS Fix Missing Object");
					m_Target.FixParentObjects ();
				}
			}

			if (m_Target.State == InfiniteScroll._State.MissingItem)
				EditorGUILayout.HelpBox ("Infinite Scroll only works with more than 2 items, please add the missing items.", MessageType.Error);

			EditorGUI.BeginDisabledGroup (m_Target.State == InfiniteScroll._State.MissingObject);
			DrawRemoveAndAddButtons ();
			EditorGUI.EndDisabledGroup ();

			EndInspector ();

			if (!Application.isPlaying)
				m_Target.UpdateUI ();

			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}

		private void DrawOrientationSettings () {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			EditorGUILayout.BeginVertical (m_PrimaryStyle);
			EditorGUI.BeginChangeCheck ();
			InfiniteScroll._Orientation orientation = (InfiniteScroll._Orientation) EditorGUILayout.EnumPopup (new GUIContent ("Orientation", "Orientation of content."), m_Target.Orientation);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (target, "IS Orientation");
				m_Target.Orientation = orientation;
			}

			DrawAlignmentSettings ();

			EditorGUILayout.EndVertical ();
			EditorGUI.EndDisabledGroup ();
		}

		private void DrawAlignmentSettings () {
			if (m_Target.Orientation == InfiniteScroll._Orientation.Horizontal) {
				EditorGUI.BeginChangeCheck ();
				InfiniteScroll._HorizontalAlignment horizontal = (InfiniteScroll._HorizontalAlignment) EditorGUILayout.EnumPopup (new GUIContent ("Alignment", "Alignment of childs."), m_Target.HorizontalAlignment);
				switch (horizontal) {
					case InfiniteScroll._HorizontalAlignment.Top:
						m_Target.Aligment = InfiniteScroll._Alignment.TopCenter;
						break;
					case InfiniteScroll._HorizontalAlignment.Middle:
						m_Target.Aligment = InfiniteScroll._Alignment.MiddleCenter;
						break;
					case InfiniteScroll._HorizontalAlignment.Bottom:
						m_Target.Aligment = InfiniteScroll._Alignment.BottomCenter;
						break;
				}
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Alignment");
					m_Target.HorizontalAlignment = horizontal;
				}
			}
			else {
				EditorGUI.BeginChangeCheck ();
				InfiniteScroll._VerticalAlignment vertical = (InfiniteScroll._VerticalAlignment) EditorGUILayout.EnumPopup (new GUIContent ("Alignment", "Alignment of childs."), m_Target.VerticalAlignment);
				switch (vertical) {
					case InfiniteScroll._VerticalAlignment.Left:
						m_Target.Aligment = InfiniteScroll._Alignment.MiddleLeft;
						break;
					case InfiniteScroll._VerticalAlignment.Middle:
						m_Target.Aligment = InfiniteScroll._Alignment.MiddleCenter;
						break;
					case InfiniteScroll._VerticalAlignment.Right:
						m_Target.Aligment = InfiniteScroll._Alignment.MiddleRight;
						break;
				}
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Alignment");
					m_Target.VerticalAlignment = vertical;
				}
			}
		}	

		private void DrawTransitionSettings () {
			EditorGUILayout.BeginVertical (m_PrimaryStyle);
			{
				EditorGUI.BeginDisabledGroup (Application.isPlaying);
				EditorGUI.BeginChangeCheck ();
				InfiniteScroll._SnapTransition snapTransition = (InfiniteScroll._SnapTransition) EditorGUILayout.EnumPopup (new GUIContent ("Snap Transition", "Transition type."), m_Target.SnapTranstion);
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Snap Transition");
					m_Target.SnapTranstion = snapTransition;
				}
				EditorGUI.EndDisabledGroup ();

				m_SnapTransitionDefaultAnim.target = m_Target.SnapTranstion == InfiniteScroll._SnapTransition.Default;
				if (EditorGUILayout.BeginFadeGroup (m_SnapTransitionDefaultAnim.faded)) {
					EditorGUILayout.BeginVertical (m_PrimaryStyle);
					{
						EditorGUI.BeginChangeCheck ();
						float snapSpeed = EditorGUILayout.FloatField (new GUIContent ("Snap Speed:", "Speed at which content moves to its destination."), m_Target.SnapSpeed);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (target, "IS Snap Speed");
							m_Target.SnapSpeed = snapSpeed;
						}

						EditorGUI.BeginChangeCheck ();
						float decelerationRate = EditorGUILayout.FloatField (new GUIContent ("Deceleration Rate:", "The rate at which movement slows down."), m_Target.DecelerationRate);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (target, "IS Deceleration Rate");
							m_Target.DecelerationRate = decelerationRate;
						}
					}
					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndFadeGroup ();

				EditorGUILayout.BeginVertical (m_PrimaryStyle);
				{
					EditorGUI.BeginChangeCheck ();
					DrawToggleSlider (new GUIContent ("Scale Effect", "Percent of scale."), 80, ref m_Target.ScalePercent, -5, 1, ref m_Target.ScaleEffect);
					DrawToggleSlider (new GUIContent ("Alpha Effect", "Percent of alpha."), 80, ref m_Target.AlphaPercent, -5, 1, ref m_Target.AlphaEffect);

					if (EditorGUI.EndChangeCheck () && Application.isPlaying)
						m_Target.OnScroll (Vector2.zero);
				}
				EditorGUILayout.EndVertical ();
			}
			EditorGUILayout.EndVertical ();
		}

		private void DrawButtonSettings () {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			EditorGUILayout.BeginVertical (m_PrimaryStyle);
			{
				EditorGUI.BeginChangeCheck ();
				bool useButtons = GUILayout.Toggle (m_Target.UseButtons, "Use Buttons", EditorStyles.miniButton);
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Use Buttons");
					m_Target.UseButtons = useButtons;
				}

				m_ToggleButtonsAnim.target = m_Target.UseButtons;

				if (EditorGUILayout.BeginFadeGroup (m_ToggleButtonsAnim.faded)) {
					EditorGUILayout.BeginVertical (m_PrimaryStyle);
					{
						EditorGUI.BeginChangeCheck ();
						Button previousButton = EditorGUILayout.ObjectField ("Previous", m_Target.PreviousButton, typeof (Button), true) as Button;
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (target, "IS Previous Button");
							m_Target.PreviousButton = previousButton;
						}

						EditorGUI.BeginChangeCheck ();
						Button nextButton = EditorGUILayout.ObjectField ("Next", m_Target.NextButton, typeof (Button), true) as Button;
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (target, "IS Next Button");
							m_Target.NextButton = nextButton;
						}
					}
					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndFadeGroup ();
			}
			EditorGUILayout.EndVertical ();
			EditorGUI.EndDisabledGroup ();
		}

		private void DrawItemsAndIndicatorsToolbar () {
			EditorGUILayout.BeginVertical (m_PrimaryStyle);
			{
				int toolbarIndex = EditorPrefs.GetInt ("IAIT");

				EditorGUI.BeginChangeCheck ();
				GUILayout.Toolbar (toolbarIndex, new string[] { "Item", "Indicator" });

				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Toolbar");
					if (GUI.changed) {
						if (toolbarIndex == 1)
							toolbarIndex = 0;
						else
							toolbarIndex = 1;

						EditorPrefs.SetInt ("IAIT", toolbarIndex);
					}
				}

				if (toolbarIndex == 0)
					DrawItemsSettings ();
				else
					DrawIndicatorsSettings ();
			}
			EditorGUILayout.EndVertical ();
		}

		private void DrawItemsSettings () {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			EditorGUILayout.BeginVertical (m_PrimaryStyle);
			{
				EditorGUI.BeginChangeCheck ();
				bool hideItems = GUILayout.Toggle (m_Target.HideItems, new GUIContent ("Hide Items", "Create a mask to display only a few items?"), EditorStyles.miniButton);
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Hide Items");
					m_Target.HideItems = hideItems;
				}

				m_HideItemsAnim.target = m_Target.HideItems;
				if (EditorGUILayout.BeginFadeGroup (m_HideItemsAnim.faded)) {
					EditorGUILayout.BeginVertical (m_PrimaryStyle);
					{
						EditorGUI.BeginChangeCheck ();
						int visibleItems = EditorGUILayout.IntSlider (new GUIContent ("Visible Items:", "Number of visible items"), m_Target.VisibleItems, 1, m_Target.MaxVisibleItems);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (target, "IS Visible Items");
							m_Target.VisibleItems = visibleItems;
						}
					}
					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndFadeGroup ();

				EditorGUI.BeginChangeCheck ();
				bool showMaskGraphic = GUILayout.Toggle (m_Target.ShowMaskGraphic, "Show Mask Graphic", EditorStyles.miniButton);
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Show Mask Graphic");
					m_Target.ShowMaskGraphic = showMaskGraphic;
				}

				EditorGUILayout.BeginVertical (m_PrimaryStyle);
				{
					EditorGUI.BeginChangeCheck ();
					float sizeX = EditorGUILayout.FloatField ("Size X:", m_Target.ItemSize.x);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Item Size X");
						m_Target.ItemSize = new Vector2 (sizeX, m_Target.ItemSize.y);
					}

					EditorGUI.BeginChangeCheck ();
					float sizeY = EditorGUILayout.FloatField ("Size Y:", m_Target.ItemSize.y);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Item Size Y");
						m_Target.ItemSize = new Vector2 (m_Target.ItemSize.x, sizeY);
					}
				}
				EditorGUILayout.EndVertical ();

				EditorGUILayout.BeginVertical (m_PrimaryStyle);
				EditorGUI.BeginChangeCheck ();
				float itemSpacing = EditorGUILayout.FloatField ("Spacing:", m_Target.ItemSpacing);
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (target, "IS Item Spacing");
					m_Target.ItemSpacing = itemSpacing;
				}
				EditorGUILayout.EndVertical ();
			}
			EditorGUILayout.EndVertical ();
			EditorGUI.EndDisabledGroup ();
		}

		private void DrawIndicatorsSettings () {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			EditorGUI.BeginChangeCheck ();
			bool showIndicators = GUILayout.Toggle (m_Target.ShowIndicators, "Show Indicators", EditorStyles.miniButton);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (target, "IS Show Indicators");
				m_Target.ShowIndicators = showIndicators;
			}
			EditorGUI.EndDisabledGroup ();

			EditorGUI.BeginDisabledGroup (!m_Target.ShowIndicators);
			{
				EditorGUILayout.BeginVertical (m_PrimaryStyle);
				{
					EditorGUI.BeginDisabledGroup (Application.isPlaying);
					EditorGUI.BeginChangeCheck ();
					Sprite indicatorSprite = EditorGUILayout.ObjectField (new GUIContent ("Sprite", "Graphic of indicator."), m_Target.IndicatorSprite, typeof (Sprite), false, GUILayout.Height (16)) as Sprite;
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Indicator Sprite");
						m_Target.IndicatorSprite = indicatorSprite;
					}
					EditorGUI.EndDisabledGroup ();

					EditorGUI.BeginChangeCheck ();
					EditorGUI.BeginChangeCheck ();
					Color normalColorIndicator = EditorGUILayout.ColorField (new GUIContent ("Normal Color", "Indicator color when it does not belong to the current item."), m_Target.NormalIndicatorColor);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Normal Color Indicator");
						m_Target.NormalIndicatorColor = normalColorIndicator;
					}

					EditorGUI.BeginChangeCheck ();
					Color activeIndicatorColor = EditorGUILayout.ColorField (new GUIContent ("Active Color", "Indicator color when it belongs to the current article."), m_Target.ActiveIndicatorColor);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Active Color Indicator");
						m_Target.ActiveIndicatorColor = activeIndicatorColor;
					}

					if (EditorGUI.EndChangeCheck () && Application.isPlaying)
						m_Target.OnScroll (Vector2.zero);
				}
				EditorGUILayout.EndVertical ();

				EditorGUI.BeginDisabledGroup (Application.isPlaying);
				EditorGUILayout.BeginVertical (m_PrimaryStyle);
				{
					EditorGUI.BeginChangeCheck ();
					float indicatorSize = EditorGUILayout.FloatField ("Size:", m_Target.IndicatorSize);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Indicator Size");
						m_Target.IndicatorSize = indicatorSize;
					}

					EditorGUI.BeginChangeCheck ();
					float indicatorSpacing = EditorGUILayout.FloatField ("Spacing:", m_Target.IndicatorSpacing);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Indicator Spacing");
						m_Target.IndicatorSpacing = indicatorSpacing;
					}
				}
				EditorGUILayout.EndVertical ();

				EditorGUILayout.BeginVertical (m_PrimaryStyle);
				{
					if (m_Target.Orientation == InfiniteScroll._Orientation.Vertical) {
						EditorGUI.BeginChangeCheck ();
						InfiniteScroll._Anchor rootIndicatorsAnchor = (InfiniteScroll._Anchor) EditorGUILayout.EnumPopup ("Anchor", m_Target.RootIndicatorsAnchor);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (target, "IS Root Indicators Anchor");
							m_Target.RootIndicatorsAnchor = rootIndicatorsAnchor;
						}
					}

					EditorGUI.BeginChangeCheck ();
					float rootIndicatorsPosition = EditorGUILayout.FloatField ("Position:", m_Target.RootIndicatorsPosition);
					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (target, "IS Root Indicators Position");
						m_Target.RootIndicatorsPosition = rootIndicatorsPosition;
					}
				}
				EditorGUILayout.EndVertical ();
				EditorGUI.EndDisabledGroup ();
			}
			EditorGUI.EndDisabledGroup ();
		}

		private void DrawRemoveAndAddButtons () {
			EditorGUI.BeginDisabledGroup (Application.isPlaying);
			EditorGUILayout.BeginHorizontal (m_PrimaryStyle);
			{
				string itemsText = m_Target.Items.Count + " Items";
				float labelWidth = GUI.skin.GetStyle ("HelpBox").CalcSize (new GUIContent (itemsText)).x;
				float buttonWidth = (Screen.width - 25) / 2 - ((labelWidth + 20) / 2);

				if (GUILayout.Button ("Remove", GUILayout.MaxWidth (buttonWidth))) {
					Undo.RecordObject (target, "IS Remove Item");
					m_Target.RemoveItem ();
				}

				GUILayout.FlexibleSpace ();
				EditorGUILayout.HelpBox (itemsText, MessageType.None);
				GUILayout.FlexibleSpace ();

				if (GUILayout.Button ("Add", GUILayout.MaxWidth (buttonWidth))) {
					Undo.RecordObject (target, "IS Add Item");
					m_Target.AddItem ();
				}
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUI.EndDisabledGroup ();
		}

		private void DrawToggleSlider (GUIContent label, float toggleWidth, ref float floatValue, float leftValue, float rightValue, ref bool toggleState) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUI.BeginChangeCheck ();
			bool toggle = GUILayout.Toggle (toggleState, label, EditorStyles.miniButton, GUILayout.Width (toggleWidth));
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (target, "IS Toggle" + label);
				toggleState = toggle;
			}
			EditorGUI.BeginDisabledGroup (!toggleState);

			EditorGUI.BeginChangeCheck ();
			float value = EditorGUILayout.Slider ("", floatValue, leftValue, rightValue);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (target, "IS Value" + label);
				floatValue = value;
			}

			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndHorizontal ();
		}

		private void DrawTitle (string content, FontStyle fontStyle, string style) {
			GUIStyle labelStyle = GUI.skin.GetStyle ("Label");
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.fontStyle = fontStyle;

			EditorGUILayout.BeginHorizontal (style);
			EditorGUILayout.LabelField (content, labelStyle);
			EditorGUILayout.EndHorizontal ();
		}

		private void BeginInspector () {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (-9);
			EditorGUILayout.BeginVertical ();
		}

		private void EndInspector () {
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
		}
	}
}