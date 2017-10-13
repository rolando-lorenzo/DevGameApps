using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIAnimSystem : MonoBehaviour {
	private static GUIAnimSystem instance;
	public eAnimationMode m_AnimationMode = eAnimationMode.All;
	public bool m_AutoAnimation = true;
	[Range (0.5f, 10f)]
	public float m_GUISpeed = 1f;
	public float m_IdleTime = 8f;

	private void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this);
		}
		else if (this != instance) {
			Destroy (base.gameObject);
		}
	}

	private IEnumerator DelayEnableButton (bool Enable, float Seconds) {
		yield return new WaitForSeconds (Seconds);
		Button[] buttonArray = FindObjectsOfType<Button> ();
		for (int j = 0; j < buttonArray.Length; j++) {
			buttonArray[j].enabled = (Enable);
		}
	}

	private IEnumerator DelayEnableButton (Transform trans, bool Enable, float Seconds) {
		yield return new WaitForSeconds (Seconds);
		Button component = trans.gameObject.GetComponent<Button> ();
		if (component != null) {
			component.enabled = (Enable);
		}
		foreach (Transform transform in trans) {
			EnableButton (transform, Enable);
		}
	}

	private IEnumerator DelayInteracableButton (Transform trans, bool Interacable, float Seconds) {
		yield return new WaitForSeconds (Seconds);
		Button component = trans.gameObject.GetComponent<Button> ();
		if (component != null) {
			component.interactable = (Interacable);
		}
		foreach (Transform transform in trans) {
			InteracableButton (transform, Interacable);
		}
	}

	private IEnumerator DelayInteracableOfAllButton (bool Interacable, float Seconds) {
		yield return new WaitForSeconds (Seconds);
		Button[] buttonArray = FindObjectsOfType<Button> ();
		for (int j = 0; j < buttonArray.Length; j++) {
			buttonArray[j].interactable = (Interacable);
		}
	}

	public void DontDestroyParticleWhenLoadNewScene (ParticleSystem pParticleSystem, bool StopParticle) {
		if (base.gameObject.activeInHierarchy) {
			base.StartCoroutine (DontDestroyParticleWhenLoadNewSceneDelay (pParticleSystem, StopParticle, 0.1f));
		}
		else {
			UnityEngine.Debug.LogWarning (base.name + " is inactive");
		}
	}

	public void DontDestroyParticleWhenLoadNewScene (Transform trans, bool StopParticle) {
		if (base.gameObject.activeInHierarchy) {
			base.StartCoroutine (DontDestroyParticleWhenLoadNewSceneDelay (trans, StopParticle, 0.1f));
		}
		else {
			UnityEngine.Debug.LogWarning (base.name + " is inactive");
		}
	}

	private IEnumerator DontDestroyParticleWhenLoadNewSceneDelay (ParticleSystem pParticleSystem, bool StopParticle, float delay) {
		yield return new WaitForSeconds (delay);
		if ((pParticleSystem != null) && pParticleSystem.IsAlive ()) {
			if (StopParticle) {
				pParticleSystem.loop = (false);
				//pParticleSystem.get_emission ().enabled = (false);
				if (pParticleSystem.isPlaying) {
					pParticleSystem.Stop ();
				}
			}
			pParticleSystem.transform.SetParent (transform);
			Destroy (pParticleSystem.gameObject, 10f);
		}
	}

	private IEnumerator DontDestroyParticleWhenLoadNewSceneDelay (Transform trans, bool StopParticle, float delay) {
		yield return new WaitForSeconds (delay);
		foreach (Transform transform in trans) {
			ParticleSystem component = transform.gameObject.GetComponent<ParticleSystem> ();
			if (component != null) {
				if (component.IsAlive ()) {
					if (StopParticle) {
						//component.set_loop (false);
						//component.get_emission ().enabled = (false);
						if (component.isPlaying) {
							component.Stop ();
						}
					}
					component.transform.SetParent (transform);
					Destroy (component.gameObject, 10f);
				}
			}
			else {
				DontDestroyParticleWhenLoadNewScene (transform.transform, StopParticle);
			}
		}
	}

	public void EnableAllButtons (bool Enable) {
		Button[] buttonArray = FindObjectsOfType<Button> ();
		for (int i = 0; i < buttonArray.Length; i++) {
			buttonArray[i].enabled = (Enable);
		}
	}

	public void EnableAllGraphicRaycasters (bool Enable) {
		Canvas[] canvasArray = FindObjectsOfType<Canvas> ();
		for (int i = 0; i < canvasArray.Length; i++) {
			GraphicRaycaster component = canvasArray[i].gameObject.GetComponent<GraphicRaycaster> ();
			if (component != null) {
				component.enabled = (Enable);
			}
		}
	}

	public void EnableButton (bool Enable, float Seconds) {
		if (base.gameObject.activeInHierarchy) {
			base.StartCoroutine (DelayEnableButton (Enable, Seconds));
		}
		else {
			UnityEngine.Debug.LogWarning (base.name + " is inactive");
		}
	}

	public void EnableButton (Transform trans, bool Enable) {
		Button component = trans.gameObject.GetComponent<Button> ();
		if (component != null) {
			component.enabled = (Enable);
		}
		foreach (Transform transform in trans) {
			EnableButton (transform, Enable);
		}
	}

	public void EnableButton (Transform trans, bool Enable, float Seconds) {
		if (base.gameObject.activeInHierarchy) {
			base.StartCoroutine (DelayEnableButton (trans, Enable, Seconds));
		}
		else {
			UnityEngine.Debug.LogWarning (base.name + " is inactive");
		}
	}

	public void FocusOnlyThisCanvas (GameObject gOB) {
		foreach (Canvas canvas in FindObjectsOfType<Canvas> ()) {
			GraphicRaycaster component = canvas.gameObject.GetComponent<GraphicRaycaster> ();
			if (component != null) {
				if (canvas.gameObject == gOB) {
					component.enabled = (true);
				}
				else {
					component.enabled = (false);
				}
			}
		}
	}

	public void FocusTheseCanvas (Canvas[] pCanvases) {
		foreach (Canvas canvas in FindObjectsOfType<Canvas> ()) {
			if (canvas.gameObject.GetComponent<GraphicRaycaster> () != null) {
				canvas.enabled = (false);
			}
		}
		foreach (Canvas canvas2 in pCanvases) {
			if (canvas2.gameObject.GetComponent<GraphicRaycaster> () != null) {
				canvas2.enabled = (true);
			}
		}
	}

	public void FocusTheseCanvases (GameObject[] gOBs) {
		int num;
		Canvas[] canvasArray = FindObjectsOfType<Canvas> ();
		for (num = 0; num < canvasArray.Length; num++) {
			GraphicRaycaster component = canvasArray[num].gameObject.GetComponent<GraphicRaycaster> ();
			if (component != null) {
				component.enabled = (false);
			}
		}
		GameObject[] objArray = gOBs;
		for (num = 0; num < objArray.Length; num++) {
			GraphicRaycaster raycaster2 = objArray[num].GetComponent<GraphicRaycaster> ();
			if (raycaster2 != null) {
				raycaster2.enabled = (true);
			}
		}
	}

	public void FocusTheseCanvases (GraphicRaycaster[] pGraphicRaycasters) {
		int num;
		GraphicRaycaster[] raycasterArray = FindObjectsOfType<GraphicRaycaster> ();
		for (num = 0; num < raycasterArray.Length; num++) {
			raycasterArray[num].enabled = (false);
		}
		raycasterArray = pGraphicRaycasters;
		for (num = 0; num < raycasterArray.Length; num++) {
			raycasterArray[num].enabled = (true);
		}
	}

	public void FocusThisCanvas (Canvas pCanvas) {
		foreach (Canvas canvas in FindObjectsOfType<Canvas> ()) {
			if (canvas.gameObject.GetComponent<GraphicRaycaster> () != null) {
				if (canvas.gameObject == pCanvas) {
					canvas.enabled = (true);
				}
				else {
					canvas.enabled = (false);
				}
			}
		}
	}

	public void FocusThisCanvas (GraphicRaycaster pGraphicRaycaster) {
		foreach (GraphicRaycaster raycaster in FindObjectsOfType<GraphicRaycaster> ()) {
			if (raycaster == pGraphicRaycaster) {
				raycaster.enabled = (true);
			}
			else {
				raycaster.enabled = (false);
			}
		}
	}

	public GUIAnimSystem GetInstance () {
		return instance;
	}

	public Canvas GetParent_Canvas (Transform trans) {
		for (Transform transform = trans.parent; transform != null; transform = transform.parent) {
			Canvas component = transform.gameObject.GetComponent<Canvas> ();
			if (component != null) {
				return component;
			}
		}
		return null;
	}

	public void InteracableButton (Transform trans, bool Interacable) {
		Button component = trans.gameObject.GetComponent<Button> ();
		if (component != null) {
			component.interactable = (Interacable);
		}
		foreach (Transform transform in trans) {
			InteracableButton (transform, Interacable);
		}
	}

	public void InteracableButton (Transform trans, bool Interacable, float Seconds) {
		if (base.gameObject.activeInHierarchy) {
			base.StartCoroutine (DelayInteracableButton (trans, Interacable, Seconds));
		}
		else {
			UnityEngine.Debug.LogWarning (base.name + " is inactive");
		}
	}

	public void LoadLevel (string LevelName, float delay) {
		if (delay <= 0f) {
			SceneManager.LoadScene (LevelName);
		}
		else if (base.gameObject.activeInHierarchy) {
			base.StartCoroutine (LoadLevelDelay (LevelName, delay));
		}
		else {
			UnityEngine.Debug.LogWarning (base.name + " is inactive");
		}
	}

	private IEnumerator LoadLevelDelay (string LevelName, float delay) {
		yield return new WaitForSeconds (delay);
		SceneManager.LoadScene (LevelName);
	}

	public void MoveIn (Transform trans, bool EffectsOnChildren) {
		if (trans.gameObject.activeSelf) {
			GUIAnim component = trans.gameObject.GetComponent<GUIAnim> ();
			if ((component != null) && component.enabled) {
				component.MoveIn ();
			}
			Button button = trans.gameObject.GetComponent<Button> ();
			if (button != null) {
				button.interactable = (true);
			}
			if (EffectsOnChildren) {
				foreach (Transform transform in trans) {
					MoveIn (transform, EffectsOnChildren);
				}
			}
		}
	}

	public void MoveInAll () {
		if (base.gameObject.activeSelf) {
			GUIAnim[] mfreeArray = FindObjectsOfType<GUIAnim> ();
			for (int i = 0; i < mfreeArray.Length; i++) {
				mfreeArray[i].MoveIn (eGUIMove.Self);
			}
		}
	}

	public void MoveOut (Transform trans, bool EffectsOnChildren) {
		if (trans.gameObject.activeSelf) {
			GUIAnim component = trans.gameObject.GetComponent<GUIAnim> ();
			if ((component != null) && component.enabled) {
				component.MoveOut ();
			}
			Button button = trans.gameObject.GetComponent<Button> ();
			if (button != null) {
				button.interactable = (false);
			}
			if (EffectsOnChildren) {
				foreach (Transform transform in trans) {
					MoveOut (transform, EffectsOnChildren);
				}
			}
		}
	}

	public void MoveOutAll () {
		if (base.gameObject.activeSelf) {
			GUIAnim[] mfreeArray = FindObjectsOfType<GUIAnim> ();
			for (int i = 0; i < mfreeArray.Length; i++) {
				mfreeArray[i].MoveOut (eGUIMove.Self);
			}
		}
	}

	public void PlayParticle (Transform trans) {
		AudioSource component = trans.gameObject.GetComponent<AudioSource> ();
		if (component != null) {
			component.Play ();
		}
		foreach (Transform transform in trans) {
			ParticleSystem system = transform.gameObject.GetComponent<ParticleSystem> ();
			if (system != null) {
				//system.get_emission ().enabled = (true);
				system.Play (true);
			}
			else {
				PlayParticle (transform.transform);
			}
			AudioSource source2 = transform.gameObject.GetComponent<AudioSource> ();
			if (source2 != null) {
				source2.Play ();
			}
		}
	}

	public void SetGraphicRaycasterEnable (Canvas pCanvas, bool Enable) {
		GraphicRaycaster component = pCanvas.gameObject.GetComponent<GraphicRaycaster> ();
		if (component != null) {
			component.enabled = (Enable);
		}
	}

	public void SetGraphicRaycasterEnable (GameObject gOB, bool Enable) {
		GraphicRaycaster component = gOB.GetComponent<GraphicRaycaster> ();
		if (component != null) {
			component.enabled = (Enable);
		}
	}

	public void SetGraphicRaycasterEnable (GraphicRaycaster pGraphicRaycaster, bool Enable) {
		if (pGraphicRaycaster != null) {
			pGraphicRaycaster.enabled = (Enable);
		}
	}

	public void SetInteracableAllButtons (bool Interacable) {
		Button[] buttonArray = FindObjectsOfType<Button> ();
		for (int i = 0; i < buttonArray.Length; i++) {
			buttonArray[i].interactable = (Interacable);
		}
	}

	public void SetInteracableAllButtons (bool Interacable, float Seconds) {
		if (base.gameObject.activeInHierarchy) {
			base.StartCoroutine (DelayInteracableOfAllButton (Interacable, Seconds));
		}
		else {
			UnityEngine.Debug.LogWarning (base.name + " is inactive");
		}
	}

	private void Start () {
	}

	public void StopParticle (Transform trans) {
		foreach (Transform transform in trans) {
			ParticleSystem component = transform.gameObject.GetComponent<ParticleSystem> ();
			if (component != null) {
				component.Stop (true);
			}
			else {
				StopParticle (transform.transform);
			}
		}
	}

	public static GUIAnimSystem Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<GUIAnimSystem> ();
				if (Application.isPlaying && (instance != null)) {
					DontDestroyOnLoad (instance.gameObject);
				}
			}
			return instance;
		}
		set {
			instance = Instance;
			if (Application.isPlaying && (instance != null)) {
				DontDestroyOnLoad (instance.gameObject);
			}
		}
	}

	public enum eAnimationMode {
		None,
		In,
		Idle,
		Out,
		All
	}

	public enum eGUIMove {
		Self,
		Children,
		SelfAndChildren
	}
}