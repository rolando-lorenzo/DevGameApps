using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PowerGUI {

	#region Class members
	public GameObject root;
	public Image icon;
	public Image backgroundIcon;
	public Button button;
	public CanvasGroup canvasGroup;

	public GameObject counterRoot;
	public CanvasGroup counterCanvasGroup;
	public Text counterText;
	#endregion

	#region Class accesors
	#endregion

	#region Class implementation
	public void SetUp () {
		icon = root.transform.GetChild (0).GetComponent<Image> ();
		CreateBackgroundIcon ();

		button = root.GetComponent<Button> ();
		canvasGroup = root.GetComponent<CanvasGroup> ();
		counterRoot = root.transform.GetChild (1).gameObject;
		counterCanvasGroup = counterRoot.GetComponent<CanvasGroup> ();
		counterText = counterRoot.GetComponentInChildren<Text> ();
	}

	public void SetState (bool value) {
		counterRoot.SetActive (value);
		canvasGroup.alpha = (value) ? 1 : 0.5f;
		canvasGroup.interactable = value;
		icon.fillAmount = 1;
	}

	public void UpdateCount (int count) {
		counterText.text = count.ToString ();
	}

	public void SetCounterAlpha (float alpha) {
		counterCanvasGroup.alpha = alpha;
	}

	public void UpdateIconProgress (float progress) {
		icon.fillAmount = progress;
	}

	private void CreateBackgroundIcon () {
		if (backgroundIcon != null)
			Object.DestroyImmediate (backgroundIcon.gameObject);

		GameObject biGO = new GameObject ("BackgroundIcon", typeof (RectTransform), typeof (Image));
		Transform biT = biGO.transform;
		RectTransform biRT = biGO.GetComponent<RectTransform> ();
		Image biI = biGO.GetComponent<Image> ();

		biT.SetParent (icon.transform);

		biRT.anchoredPosition = Vector2.zero;
		biRT.sizeDelta = Vector2.zero;
		biRT.anchorMin = Vector2.zero;
		biRT.anchorMax = Vector2.one;
		biRT.pivot = Vector2.one;

		biI.sprite = icon.sprite;
		biI.color = new Color (1, 1, 1, 0.25f);
		biI.raycastTarget = false;
		backgroundIcon = biI;
	}
	#endregion
}