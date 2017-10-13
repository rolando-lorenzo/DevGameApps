using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GUIAnim : MonoBehaviour {
	[HideInInspector] public float m_CameraBottomEdge;
	[HideInInspector] public float m_CameraLeftEdge;
	[HideInInspector] public float m_CameraRightEdge;
	[HideInInspector] public float m_CameraTopEdge;
	private CanvasRenderer m_CanvasRenderer;
	[HideInInspector] public Vector3 m_CanvasWorldBottomCenter;
	[HideInInspector] public Vector3 m_CanvasWorldBottomLeft;
	[HideInInspector] public Vector3 m_CanvasWorldBottomRight;
	[HideInInspector] public Vector3 m_CanvasWorldMiddleCenter;
	[HideInInspector] public Vector3 m_CanvasWorldMiddleLeft;
	[HideInInspector] public Vector3 m_CanvasWorldMiddleRight;
	[HideInInspector] public Vector3 m_CanvasWorldTopCenter;
	[HideInInspector] public Vector3 m_CanvasWorldTopLeft;
	[HideInInspector] public Vector3 m_CanvasWorldTopRight;
	public cFade m_FadeIn;
	public cPingPongFade m_FadeLoop;
	[HideInInspector] public float m_FadeOriginal;
	[HideInInspector] public float m_FadeOriginalTextOutline;
	[HideInInspector] public float m_FadeOriginalTextShadow;
	public cFade m_FadeOut;
	[HideInInspector] public float m_FadeVariable;
	private GETweenItem m_GETweenFadeLoop;
	private GETweenItem m_GETweenScaleLoop;
	private Image m_Image;
	private bool m_InitialDone;
	private int m_MoveIdle_Attemp;
	private int m_MoveIdle_AttempMax = 5;
	private float m_MoveIdle_AttempMax_TimeInterval = 0.5f;
	private bool m_MoveIdle_StartSucceed;
	public cMoveIn m_MoveIn;
	[HideInInspector] public Vector3 m_MoveOriginal;
	public cMoveOut m_MoveOut;
	[HideInInspector] public float m_MoveVariable;
	private Canvas m_Parent_Canvas;
	private RectTransform m_ParentCanvasRectTransform;
	private RawImage m_RawImage;
	[HideInInspector] private RectTransform m_RectTransform;
	public cRotationIn m_RotationIn;
	[HideInInspector] public Quaternion m_RotationOriginal;
	public cRotationOut m_RotationOut;
	[HideInInspector] public float m_RotationVariable;
	public cScaleIn m_ScaleIn;
	public cPingPongScale m_ScaleLoop;
	[HideInInspector] public Vector3 m_ScaleOriginal;
	public cScaleOut m_ScaleOut;
	[HideInInspector] public float m_ScaleVariable;
	private Slider m_Slider;
	private Text m_Text;
	private Outline m_TextOutline;
	private Shadow m_TextShadow;
	private Toggle m_Toggle;
	[HideInInspector]
	public Bounds m_TotalBounds;

	private void AnimIn_FadeComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_FadeIn.Sounds.m_End != null) {
				PlaySoundOneShot (m_FadeIn.Sounds.m_End);
			}
			m_FadeIn.Began = false;
			m_FadeIn.Animating = false;
			m_FadeIn.Done = true;
			m_FadeOut.Began = false;
			m_FadeOut.Animating = false;
			m_FadeOut.Done = false;
			AnimIn_FadeUpdateByValue (1f);
			StartMoveIdle ();
		}
	}

	private void AnimIn_FadeUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float fade = m_FadeIn.Fade + ((m_FadeOriginal - m_FadeIn.Fade) * Value);
			RecursiveFade (base.transform, fade, m_FadeIn.FadeChildren);
			if (m_TextOutline != null) {
				float num2 = Value - 0.75f;
				if (num2 < 0f) {
					num2 = 0f;
				}
				if (num2 > 0f) {
					num2 *= 4f;
				}
				float fadeOutline = m_FadeOriginalTextOutline * num2;
				FadeTextOutline (base.transform, fadeOutline);
			}
			if (m_TextShadow != null) {
				float num4 = Value - 0.75f;
				if (num4 < 0f) {
					num4 = 0f;
				}
				if (num4 > 0f) {
					num4 *= 4f;
				}
				float fadeShadow = m_FadeOriginalTextShadow * num4;
				FadeTextShadow (base.transform, fadeShadow);
			}
			if (!m_FadeIn.Animating && m_FadeIn.Began) {
				m_FadeIn.Animating = true;
				if (m_FadeIn.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_FadeIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimIn_MoveComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_MoveIn.Sounds.m_End != null) {
				PlaySoundOneShot (m_MoveIn.Sounds.m_End);
			}
			m_MoveIn.Began = false;
			m_MoveIn.Animating = false;
			m_MoveIn.Done = true;
			m_MoveOut.Began = false;
			m_MoveOut.Animating = false;
			m_MoveOut.Done = false;
			StartMoveIdle ();
		}
	}

	private void AnimIn_MoveUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			m_RectTransform.anchoredPosition = (new Vector3 (m_MoveIn.BeginPos.x + ((m_MoveIn.EndPos.x - m_MoveIn.BeginPos.x) * Value), m_MoveIn.BeginPos.y + ((m_MoveIn.EndPos.y - m_MoveIn.BeginPos.y) * Value), m_MoveIn.BeginPos.z + ((m_MoveIn.EndPos.z - m_MoveIn.BeginPos.z) * Value)));
			if (!m_MoveIn.Animating && m_MoveIn.Began) {
				m_MoveIn.Animating = true;
				if (m_MoveIn.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_MoveIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimIn_RotationComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_RotationIn.Sounds.m_End != null) {
				PlaySoundOneShot (m_RotationIn.Sounds.m_End);
			}
			m_RotationIn.Began = false;
			m_RotationIn.Animating = false;
			m_RotationIn.Done = true;
			m_RotationOut.Began = false;
			m_RotationOut.Animating = false;
			m_RotationOut.Done = false;
			AnimIn_RotationUpdateByValue (1f);
			StartMoveIdle ();
		}
	}

	private void AnimIn_RotationUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float num = m_RotationIn.BeginRotation.x + ((m_RotationIn.EndRotation.x - m_RotationIn.BeginRotation.x) * Value);
			float num2 = m_RotationIn.BeginRotation.y + ((m_RotationIn.EndRotation.y - m_RotationIn.BeginRotation.y) * Value);
			float num3 = m_RotationIn.BeginRotation.z + ((m_RotationIn.EndRotation.z - m_RotationIn.BeginRotation.z) * Value);
			m_RectTransform.localRotation = (Quaternion.Euler (num, num2, num3));
			if (!m_RotationIn.Animating && m_RotationIn.Began) {
				m_RotationIn.Animating = true;
				if (m_RotationIn.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_RotationIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimIn_ScaleComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_ScaleIn.Sounds.m_End != null) {
				PlaySoundOneShot (m_ScaleIn.Sounds.m_End);
			}
			m_ScaleIn.Began = false;
			m_ScaleIn.Animating = false;
			m_ScaleIn.Done = true;
			m_ScaleOut.Began = false;
			m_ScaleOut.Animating = false;
			m_ScaleOut.Done = false;
			AnimIn_ScaleUpdateByValue (1f);
			StartMoveIdle ();
		}
	}

	private void AnimIn_ScaleUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			base.transform.localScale = (new Vector3 ((m_ScaleIn.ScaleBegin.x * m_ScaleOriginal.x) + ((m_ScaleOriginal.x - (m_ScaleIn.ScaleBegin.x * m_ScaleOriginal.x)) * Value), (m_ScaleIn.ScaleBegin.y * m_ScaleOriginal.y) + ((m_ScaleOriginal.y - (m_ScaleIn.ScaleBegin.y * m_ScaleOriginal.y)) * Value), (m_ScaleIn.ScaleBegin.z * m_ScaleOriginal.z) + ((m_ScaleOriginal.z - (m_ScaleIn.ScaleBegin.z * m_ScaleOriginal.z)) * Value)));
			if (!m_ScaleIn.Animating && m_ScaleIn.Began) {
				m_ScaleIn.Animating = true;
				if (m_ScaleIn.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_ScaleIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_FadeComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_FadeOut.Sounds.m_End != null) {
				PlaySoundOneShot (m_FadeOut.Sounds.m_End);
			}
			m_FadeIn.Began = false;
			m_FadeIn.Animating = false;
			m_FadeIn.Done = false;
			m_FadeOut.Began = false;
			m_FadeOut.Animating = false;
			m_FadeOut.Done = true;
			AnimOut_FadeUpdateByValue (1f);
		}
	}

	private void AnimOut_FadeUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float fade = m_FadeOriginal + ((m_FadeOut.Fade - m_FadeOriginal) * Value);
			RecursiveFade (base.transform, fade, m_FadeOut.FadeChildren);
			if (m_TextOutline != null) {
				float num2 = Value * 3f;
				if (num2 > 1f) {
					num2 = 1f;
				}
				float fadeOutline = m_FadeOriginalTextOutline * (1f - num2);
				FadeTextOutline (base.transform, fadeOutline);
			}
			if (m_TextShadow != null) {
				float num4 = Value * 3f;
				if (num4 > 1f) {
					num4 = 1f;
				}
				float fadeShadow = m_FadeOriginalTextShadow * (1f - num4);
				FadeTextShadow (base.transform, fadeShadow);
			}
			if (!m_FadeOut.Animating && m_FadeOut.Began) {
				m_FadeOut.Animating = true;
				if (m_FadeOut.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_FadeOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_MoveComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_MoveOut.Sounds.m_End != null) {
				PlaySoundOneShot (m_MoveOut.Sounds.m_End);
			}
			m_MoveIn.Began = false;
			m_MoveIn.Animating = false;
			m_MoveIn.Done = false;
			m_MoveOut.Began = false;
			m_MoveOut.Animating = false;
			m_MoveOut.Done = true;
		}
	}

	private void AnimOut_MoveUpdateByValue (float Value) {
		if (((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) && (m_RectTransform != null)) {
			m_RectTransform.anchoredPosition = (new Vector3 (m_MoveOut.BeginPos.x + ((m_MoveOut.EndPos.x - m_MoveOut.BeginPos.x) * Value), m_MoveOut.BeginPos.y + ((m_MoveOut.EndPos.y - m_MoveOut.BeginPos.y) * Value), m_MoveOut.BeginPos.z + ((m_MoveOut.EndPos.z - m_MoveOut.BeginPos.z) * Value)));
			if (!m_MoveOut.Animating && m_MoveOut.Began) {
				m_MoveOut.Animating = true;
				if (m_MoveOut.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_MoveOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_RotationComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_RotationOut.Sounds.m_End != null) {
				PlaySoundOneShot (m_RotationOut.Sounds.m_End);
			}
			m_RotationIn.Began = false;
			m_RotationIn.Animating = false;
			m_RotationIn.Done = false;
			m_RotationOut.Began = false;
			m_RotationOut.Animating = false;
			m_RotationOut.Done = true;
			AnimOut_RotationUpdateByValue (1f);
		}
	}

	private void AnimOut_RotationUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float num = m_RotationOut.BeginRotation.x + ((m_RotationOut.EndRotation.x - m_RotationOut.BeginRotation.x) * Value);
			float num2 = m_RotationOut.BeginRotation.y + ((m_RotationOut.EndRotation.y - m_RotationOut.BeginRotation.y) * Value);
			float num3 = m_RotationOut.BeginRotation.z + ((m_RotationOut.EndRotation.z - m_RotationOut.BeginRotation.z) * Value);
			m_RectTransform.localRotation = (Quaternion.Euler (num, num2, num3));
			if (!m_RotationOut.Animating && m_RotationOut.Began) {
				m_RotationOut.Animating = true;
				if (m_RotationOut.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_RotationOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_ScaleComplete () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_ScaleOut.Sounds.m_End != null) {
				PlaySoundOneShot (m_ScaleOut.Sounds.m_End);
			}
			m_ScaleIn.Began = false;
			m_ScaleIn.Animating = false;
			m_ScaleIn.Done = false;
			m_ScaleOut.Began = false;
			m_ScaleOut.Animating = false;
			m_ScaleOut.Done = true;
			AnimOut_ScaleUpdateByValue (1f);
		}
	}

	private void AnimOut_ScaleUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_ScaleOut.ScaleBegin == m_ScaleOriginal) {
				base.transform.localScale = (new Vector3 (m_ScaleOut.ScaleBegin.x + ((m_ScaleOut.ScaleEnd.x - m_ScaleOut.ScaleBegin.x) * Value), m_ScaleOut.ScaleBegin.y + ((m_ScaleOut.ScaleEnd.y - m_ScaleOut.ScaleBegin.y) * Value), m_ScaleOut.ScaleBegin.z + ((m_ScaleOut.ScaleEnd.z - m_ScaleOut.ScaleBegin.z) * Value)));
			}
			else {
				float num = Mathf.Lerp (m_ScaleOut.ScaleBegin.x, m_ScaleOriginal.x, Value);
				float num2 = Mathf.Lerp (m_ScaleOut.ScaleBegin.y, m_ScaleOriginal.y, Value);
				float num3 = Mathf.Lerp (m_ScaleOut.ScaleBegin.z, m_ScaleOriginal.z, Value);
				Vector3 vector = new Vector3 (num, num2, num3);
				base.transform.localScale = (new Vector3 (vector.x + ((m_ScaleOut.ScaleEnd.x - vector.x) * Value), vector.y + ((m_ScaleOut.ScaleEnd.y - vector.y) * Value), vector.z + ((m_ScaleOut.ScaleEnd.z - vector.z) * Value)));
			}
			if (!m_ScaleOut.Animating && m_ScaleOut.Began) {
				m_ScaleOut.Animating = true;
				if (m_ScaleOut.Sounds.m_AfterDelay != null) {
					PlaySoundOneShot (m_ScaleOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void Awake () {
		if ((this != null) && (base.gameObject != null)) {
			GETween.Init (0x640);
			m_RectTransform = base.transform.GetComponent<RectTransform> ();
			m_MoveOriginal = m_RectTransform.anchoredPosition;
			m_RotationOriginal = m_RectTransform.localRotation;
			m_ScaleOriginal = base.transform.localScale;
			m_FadeOriginal = 1f;
			m_FadeOriginalTextOutline = 0.5f;
			m_FadeOriginalTextShadow = 0.5f;
			m_Image = base.gameObject.GetComponent<Image> ();
			m_Toggle = base.gameObject.GetComponent<Toggle> ();
			m_Text = base.gameObject.GetComponent<Text> ();
			m_TextOutline = base.gameObject.GetComponent<Outline> ();
			m_TextShadow = base.gameObject.GetComponent<Shadow> ();
			m_RawImage = base.gameObject.GetComponent<RawImage> ();
			m_Slider = base.gameObject.GetComponent<Slider> ();
			m_CanvasRenderer = base.gameObject.GetComponent<CanvasRenderer> ();
			m_FadeOriginal = GetFadeValue (base.transform);
			m_FadeOriginalTextOutline = GetFadeTextOutlineValue (base.transform);
			m_FadeOriginalTextShadow = GetFadeTextShadowValue (base.transform);
		}
	}

	private void CalculateCameraArea () {
		if ((this != null) && (base.gameObject != null)) {
			if (m_Parent_Canvas == null) {
				m_Parent_Canvas = GUIAnimSystem.Instance.GetParent_Canvas (base.transform);
			}
			if (m_Parent_Canvas != null) {
				m_ParentCanvasRectTransform = m_Parent_Canvas.GetComponent<RectTransform> ();
				m_CameraRightEdge = (m_ParentCanvasRectTransform.rect.width / 2f) + m_TotalBounds.size.x;
				m_CameraLeftEdge = -m_CameraRightEdge;
				m_CameraTopEdge = (m_ParentCanvasRectTransform.rect.height / 2f) + m_TotalBounds.size.y;
				m_CameraBottomEdge = -m_CameraTopEdge;
				m_CanvasWorldTopLeft = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (m_CameraLeftEdge, m_CameraTopEdge, 0f));
				m_CanvasWorldTopCenter = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (0f, m_CameraTopEdge, 0f));
				m_CanvasWorldTopRight = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (m_CameraRightEdge, m_CameraTopEdge, 0f));
				m_CanvasWorldMiddleLeft = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (m_CameraLeftEdge, 0f, 0f));
				m_CanvasWorldMiddleCenter = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (0f, 0f, 0f));
				m_CanvasWorldMiddleRight = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (m_CameraRightEdge, 0f, 0f));
				m_CanvasWorldBottomLeft = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (m_CameraLeftEdge, m_CameraBottomEdge, 0f));
				m_CanvasWorldBottomCenter = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (0f, m_CameraBottomEdge, 0f));
				m_CanvasWorldBottomRight = m_ParentCanvasRectTransform.TransformPoint (new Vector3 (m_CameraRightEdge, m_CameraBottomEdge, 0f));
			}
		}
	}

	private void CalculateTotalBounds () {
		m_TotalBounds = new Bounds (Vector3.zero, Vector3.zero);
		if (((m_Slider != null) || (m_Toggle != null)) || (m_CanvasRenderer != null)) {
			RectTransform component = base.gameObject.GetComponent<RectTransform> ();
			m_TotalBounds.size = (new Vector3 (component.rect.width, component.rect.height, 0f));
		}
	}

	private IEnumerator CoroutineMoveIdle (float Delay) {
		yield return new WaitForSeconds (Delay);
		m_MoveIdle_Attemp++;
		if (m_MoveIdle_Attemp <= m_MoveIdle_AttempMax) {
			MoveIdle ();
		}
	}

	private IEnumerator CoroutineMoveOut (float Delay) {
		yield return new WaitForSeconds (Delay);
		MoveOut ();
	}

	private void FadeLoopComplete () {
	}

	private void FadeLoopStart (float delay) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			m_FadeLoop.Began = true;
			m_FadeLoop.Animating = false;
			m_FadeLoop.IsOverriding = false;
			m_FadeLoop.IsOverridingDelayTimeCount = 0f;
			m_FadeLoop.Done = false;
			m_FadeLoop.m_FadeLast = GetFadeValue (base.transform);
			m_GETweenFadeLoop = GETween.TweenValue (base.gameObject, new Action<float> (FadeLoopUpdateByValue), 0f, 1f, m_FadeLoop.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_FadeLoop.EaseType)).SetLoopPingPong ().SetOnComplete (new Action (FadeLoopComplete)).SetUseEstimatedTime (true);
		}
	}

	private void FadeLoopUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float fadeValue = GetFadeValue (base.transform);
			if (m_FadeLoop.m_FadeLast != fadeValue) {
				StopFadeLoop ();
			}
			else {
				if (m_FadeLoop.IsOverriding) {
					m_FadeLoop.IsOverridingDelayTimeCount += Time.deltaTime;
					if (m_FadeLoop.IsOverridingDelayTimeCount > 2f) {
						m_FadeLoop.IsOverridingDelayTimeCount = 0f;
						m_FadeLoop.IsOverriding = false;
					}
				}
				if (!m_FadeLoop.IsOverriding) {
					if (!m_FadeLoop.Animating) {
						StartFadeLoopSound ();
					}
					fadeValue = m_FadeLoop.Min + ((m_FadeLoop.Max - m_FadeLoop.Min) * Value);
					RecursiveFade (base.transform, fadeValue, m_FadeLoop.FadeChildren);
					if (m_TextOutline != null) {
						float num2 = m_FadeOriginalTextOutline * Value;
						float fadeOutline = 0f;
						if (num2 > m_TextOutline.effectColor.a) {
							num2 = Value - 0.75f;
							if (num2 < 0f) {
								num2 = 0f;
							}
							if (num2 > 0f) {
								num2 *= 4f;
							}
							fadeOutline = m_FadeOriginalTextOutline * num2;
							FadeTextOutline (base.transform, fadeOutline);
						}
						if (num2 < m_TextOutline.effectColor.a) {
							num2 = Value * 2f;
							if (num2 > 1f) {
								num2 = 1f;
							}
							fadeOutline = m_FadeOriginalTextOutline * (1f - num2);
							FadeTextOutline (base.transform, fadeOutline);
						}
					}
					if (m_TextShadow != null) {
						float num4 = m_FadeOriginalTextShadow * Value;
						float fadeShadow = 0f;
						if (num4 > m_TextShadow.effectColor.a) {
							num4 = Value - 0.75f;
							if (num4 < 0f) {
								num4 = 0f;
							}
							if (num4 > 0f) {
								num4 *= 4f;
							}
							fadeShadow = m_FadeOriginalTextShadow * num4;
							FadeTextShadow (base.transform, fadeShadow);
						}
						if (num4 < m_TextShadow.effectColor.a) {
							num4 = Value * 2f;
							if (num4 > 1f) {
								num4 = 1f;
							}
							fadeShadow = m_FadeOriginalTextShadow * (1f - num4);
							FadeTextShadow (base.transform, fadeShadow);
						}
					}
				}
			}
			m_FadeLoop.m_FadeLast = fadeValue;
		}
	}

	private void FadeTextOutline (Transform trans, float FadeOutline) {
		if (m_TextOutline != null) {
			m_TextOutline.effectColor = (new Color (m_TextOutline.effectColor.r, m_TextOutline.effectColor.g, m_TextOutline.effectColor.b, FadeOutline));
		}
	}

	private void FadeTextShadow (Transform trans, float FadeShadow) {
		if (m_TextShadow != null) {
			m_TextShadow.effectColor = (new Color (m_TextShadow.effectColor.r, m_TextShadow.effectColor.g, m_TextShadow.effectColor.b, FadeShadow));
		}
	}

	private float GetFadeTextOutlineValue (Transform trans) {
		float a = 1f;
		if (m_TextOutline != null) {
			a = m_TextOutline.effectColor.a;
		}
		return a;
	}

	private float GetFadeTextShadowValue (Transform trans) {
		float a = 1f;
		if (m_TextShadow != null) {
			a = m_TextShadow.effectColor.a;
		}
		return a;
	}

	private float GetFadeValue (Transform trans) {
		float a = 1f;
		if (m_Image != null) {
			a = m_Image.color.a;
		}
		if (m_Toggle != null) {
			a = m_Toggle.GetComponentInChildren<Image> ().color.a;
		}
		if (m_Text != null) {
			a = m_Text.color.a;
		}
		if (m_RawImage != null) {
			a = m_RawImage.color.a;
		}
		if (m_Slider != null) {
			a = m_Slider.colors.normalColor.a;
		}
		return a;
	}

	public GETween.GETweenType GETweenEaseType (eEaseType easeType) {
		switch (easeType) {
			case eEaseType.InQuad:
				return GETween.GETweenType.easeInQuad;

			case eEaseType.OutQuad:
				return GETween.GETweenType.easeOutQuad;

			case eEaseType.InOutQuad:
				return GETween.GETweenType.easeInOutQuad;

			case eEaseType.InCubic:
				return GETween.GETweenType.easeOutCubic;

			case eEaseType.OutCubic:
				return GETween.GETweenType.easeOutCubic;

			case eEaseType.InOutCubic:
				return GETween.GETweenType.easeInOutCubic;

			case eEaseType.InQuart:
				return GETween.GETweenType.easeInQuart;

			case eEaseType.OutQuart:
				return GETween.GETweenType.easeOutQuart;

			case eEaseType.InOutQuart:
				return GETween.GETweenType.easeInOutQuart;

			case eEaseType.InQuint:
				return GETween.GETweenType.easeInQuint;

			case eEaseType.OutQuint:
				return GETween.GETweenType.easeOutQuint;

			case eEaseType.InOutQuint:
				return GETween.GETweenType.easeInOutQuint;

			case eEaseType.InSine:
				return GETween.GETweenType.easeInSine;

			case eEaseType.OutSine:
				return GETween.GETweenType.easeOutSine;

			case eEaseType.InOutSine:
				return GETween.GETweenType.easeInOutSine;

			case eEaseType.InExpo:
				return GETween.GETweenType.easeInExpo;

			case eEaseType.OutExpo:
				return GETween.GETweenType.easeOutExpo;

			case eEaseType.InOutExpo:
				return GETween.GETweenType.easeInOutExpo;

			case eEaseType.InCirc:
				return GETween.GETweenType.easeInCirc;

			case eEaseType.OutCirc:
				return GETween.GETweenType.easeOutCirc;

			case eEaseType.InOutCirc:
				return GETween.GETweenType.easeInOutCirc;

			case eEaseType.linear:
				return GETween.GETweenType.linear;

			case eEaseType.InBounce:
				return GETween.GETweenType.easeInBounce;

			case eEaseType.OutBounce:
				return GETween.GETweenType.easeOutBounce;

			case eEaseType.InOutBounce:
				return GETween.GETweenType.easeInOutBounce;

			case eEaseType.InBack:
				return GETween.GETweenType.easeInBack;

			case eEaseType.OutBack:
				return GETween.GETweenType.easeOutBack;

			case eEaseType.InOutBack:
				return GETween.GETweenType.easeInOutBack;

			case eEaseType.InElastic:
				return GETween.GETweenType.easeInElastic;

			case eEaseType.OutElastic:
				return GETween.GETweenType.easeOutElastic;

			case eEaseType.InOutElastic:
				return GETween.GETweenType.easeInOutElastic;
		}
		return GETween.GETweenType.linear;
	}

	private void InitFadeIn () {
		if (((this != null) && (base.gameObject != null)) && m_FadeIn.Enable) {
			RecursiveFade (base.transform, m_FadeIn.Fade, m_FadeIn.FadeChildren);
		}
	}

	private void InitMoveIn () {
		if (((this != null) && (base.gameObject != null)) && (m_MoveIn.Enable && !m_MoveIn.Done)) {
			CalculateTotalBounds ();
			CalculateCameraArea ();
			RectTransform component = base.transform.parent.GetComponent<RectTransform> ();
			switch (m_MoveIn.MoveFrom) {
				case ePosMove.ParentPosition:
					if (base.transform.parent != null) {
						m_MoveIn.BeginPos = new Vector3 (0f, 0f, m_RectTransform.localPosition.z);
					}
					break;

				case ePosMove.LocalPosition:
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.Position.x, m_MoveIn.Position.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.UpperScreenEdge:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldTopCenter);
					m_MoveIn.BeginPos = new Vector3 (m_RectTransform.localPosition.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.LeftScreenEdge:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldMiddleLeft);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_RectTransform.localPosition.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.RightScreenEdge:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldMiddleRight);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_RectTransform.localPosition.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.BottomScreenEdge:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldBottomCenter);
					m_MoveIn.BeginPos = new Vector3 (m_RectTransform.localPosition.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.UpperLeft:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldTopLeft);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.UpperCenter:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldTopCenter);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.UpperRight:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldTopRight);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.MiddleLeft:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldMiddleLeft);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.MiddleCenter:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldMiddleCenter);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.MiddleRight:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldMiddleRight);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.BottomLeft:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldBottomLeft);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.BottomCenter:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldBottomCenter);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.BottomRight:
					m_MoveIn.BeginPos = component.InverseTransformPoint (m_CanvasWorldBottomRight);
					m_MoveIn.BeginPos = new Vector3 (m_MoveIn.BeginPos.x, m_MoveIn.BeginPos.y, m_RectTransform.localPosition.z);
					break;

				case ePosMove.SelfPosition:
					m_MoveIn.BeginPos = m_MoveOriginal;
					break;
			}
			m_RectTransform.anchoredPosition = (m_MoveIn.BeginPos);
			m_MoveIn.EndPos = m_MoveOriginal;
		}
	}

	private void InitMoveOut () {
		if (((this != null) && (base.gameObject != null)) && m_MoveOut.Enable) {
			CalculateTotalBounds ();
			CalculateCameraArea ();
			RectTransform component = base.transform.parent.GetComponent<RectTransform> ();
			switch (m_MoveOut.MoveTo) {
				case ePosMove.ParentPosition:
					if (base.transform.parent != null) {
						m_MoveOut.EndPos = new Vector3 (0f, 0f, m_RectTransform.localPosition.z);
					}
					return;

				case ePosMove.LocalPosition:
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.Position.x, m_MoveOut.Position.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.UpperScreenEdge:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldTopCenter);
					m_MoveOut.EndPos = new Vector3 (m_RectTransform.localPosition.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.LeftScreenEdge:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldMiddleLeft);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_RectTransform.localPosition.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.RightScreenEdge:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldMiddleRight);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_RectTransform.localPosition.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.BottomScreenEdge:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldBottomCenter);
					m_MoveOut.EndPos = new Vector3 (m_RectTransform.localPosition.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.UpperLeft:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldTopLeft);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.UpperCenter:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldTopCenter);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.UpperRight:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldTopRight);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.MiddleLeft:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldMiddleLeft);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.MiddleCenter:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldMiddleCenter);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.MiddleRight:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldMiddleRight);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.BottomLeft:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldBottomLeft);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.BottomCenter:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldBottomCenter);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.BottomRight:
					m_MoveOut.EndPos = component.InverseTransformPoint (m_CanvasWorldBottomRight);
					m_MoveOut.EndPos = new Vector3 (m_MoveOut.EndPos.x, m_MoveOut.EndPos.y, m_RectTransform.localPosition.z);
					return;

				case ePosMove.SelfPosition:
					m_MoveOut.EndPos = m_MoveOriginal;
					return;
			}
		}
	}

	private void InitRotationIn () {
		if (((this != null) && (base.gameObject != null)) && m_RotationIn.Enable) {
			m_RotationIn.BeginRotation = m_RotationIn.Rotation;
			m_RotationIn.EndRotation = m_RotationOriginal.eulerAngles;
		}
	}

	private void InitScaleIn () {
		if (((this != null) && (base.gameObject != null)) && m_ScaleIn.Enable) {
			base.transform.localScale = (m_ScaleIn.ScaleBegin);
		}
	}

	private IEnumerator InitScaleIn (float Delay) {
		yield return new WaitForSeconds (Delay);
		InitScaleIn ();
	}

	public bool IsAnimating () {
		if (this == null) {
			return false;
		}
		if (base.gameObject == null) {
			return false;
		}
		if (!base.gameObject.activeSelf) {
			return false;
		}
		if (!base.enabled) {
			return false;
		}
		if (((!m_MoveIn.Began && !m_MoveOut.Began) && (!m_RotationIn.Began && !m_RotationOut.Began)) && ((!m_ScaleIn.Began && !m_ScaleOut.Began) && (!m_FadeIn.Began && !m_FadeOut.Began))) {
			return false;
		}
		return true;
	}

	private void MoveIdle () {
		if (((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) && !m_MoveIdle_StartSucceed) {
			if (!m_MoveIn.Began && !m_RotationIn.Began) {
				if (m_ScaleLoop.Enable && !m_ScaleIn.Began) {
					m_MoveIdle_StartSucceed = true;
					ScaleLoopStart (m_ScaleLoop.Delay);
				}
				if (m_FadeLoop.Enable && !m_FadeIn.Began) {
					m_MoveIdle_StartSucceed = true;
					FadeLoopStart (m_FadeLoop.Delay);
				}
			}
			if (!m_MoveIdle_StartSucceed) {
				base.StartCoroutine (CoroutineMoveIdle (m_MoveIdle_AttempMax_TimeInterval));
			}
		}
	}

	public void MoveIn () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		}
	}

	public void MoveIn (GUIAnimSystem.eGUIMove GUIMoveType) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Self) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				if (m_MoveIn.Enable && !m_MoveIn.Began) {
					m_MoveIn.Began = true;
					m_MoveIn.Animating = false;
					m_MoveIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimIn_MoveUpdateByValue), 0f, 1f, m_MoveIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_MoveIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_MoveIn.EaseType)).SetOnComplete (new Action (AnimIn_MoveComplete)).SetUseEstimatedTime (true);
					if (m_MoveIn.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_MoveIn.Sounds.m_Begin);
					}
				}
				if (m_RotationIn.Enable && !m_RotationIn.Began) {
					m_RotationIn.Began = true;
					m_RotationIn.Animating = false;
					m_RotationIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimIn_RotationUpdateByValue), 0f, 1f, m_RotationIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_RotationIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_RotationIn.EaseType)).SetOnComplete (new Action (AnimIn_RotationComplete)).SetUseEstimatedTime (true);
					if (m_RotationIn.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_RotationIn.Sounds.m_Begin);
					}
				}
				if (m_ScaleIn.Enable && !m_ScaleIn.Began) {
					m_ScaleIn.Began = true;
					m_ScaleIn.Animating = false;
					m_ScaleIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimIn_ScaleUpdateByValue), 0f, 1f, m_ScaleIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_ScaleIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_ScaleIn.EaseType)).SetOnComplete (new Action (AnimIn_ScaleComplete)).SetUseEstimatedTime (true);
					if (m_ScaleIn.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_ScaleIn.Sounds.m_Begin);
					}
				}
				if (m_FadeIn.Enable && !m_FadeIn.Began) {
					m_FadeIn.Began = true;
					m_FadeIn.Animating = false;
					m_FadeIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimIn_FadeUpdateByValue), 0f, 1f, m_FadeIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_FadeIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_FadeIn.EaseType)).SetOnComplete (new Action (AnimIn_FadeComplete)).SetUseEstimatedTime (true);
					if (m_FadeIn.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_FadeIn.Sounds.m_Begin);
					}
				}
				if (((!m_MoveIn.Enable && !m_RotationIn.Enable) && (!m_ScaleIn.Enable && !m_FadeIn.Enable)) && (m_ScaleLoop.Enable || m_FadeLoop.Enable)) {
					StartMoveIdle ();
				}
			}
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Children) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				RecuresiveMoveIn (base.transform);
			}
		}
	}

	public void MoveOut () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
		}
	}

	public void MoveOut (GUIAnimSystem.eGUIMove GUIMoveType) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Self) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				if (m_ScaleLoop.Enable && m_ScaleLoop.Began) {
					m_ScaleLoop.Began = false;
					m_ScaleLoop.Done = true;
					StopScaleLoop ();
				}
				if (m_FadeLoop.Enable && m_FadeLoop.Began) {
					m_FadeLoop.Began = false;
					m_FadeLoop.Done = true;
					StopFadeLoop ();
				}
				if (m_MoveOut.Enable && !m_MoveOut.Began) {
					m_MoveOut.Began = true;
					m_MoveOut.Animating = false;
					m_MoveOut.Done = false;
					m_MoveOut.BeginPos = m_RectTransform.anchoredPosition;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimOut_MoveUpdateByValue), 0f, 1f, m_MoveOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_MoveOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_MoveOut.EaseType)).SetOnComplete (new Action (AnimOut_MoveComplete)).SetUseEstimatedTime (true);
					if (m_MoveOut.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_MoveOut.Sounds.m_Begin);
					}
				}
				if (m_RotationOut.Enable && !m_RotationOut.Began) {
					m_RotationOut.Began = true;
					m_RotationOut.Animating = false;
					m_RotationOut.Done = false;
					m_RotationOut.BeginRotation = m_RectTransform.localRotation.eulerAngles;
					m_RotationOut.EndRotation = m_RotationOut.Rotation;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimOut_RotationUpdateByValue), 0f, 1f, m_RotationOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_RotationOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_RotationOut.EaseType)).SetOnComplete (new Action (AnimOut_RotationComplete)).SetUseEstimatedTime (true);
					if (m_RotationOut.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_RotationOut.Sounds.m_Begin);
					}
				}
				if (m_ScaleOut.Enable && !m_ScaleOut.Began) {
					m_ScaleOut.Began = true;
					m_ScaleOut.Animating = false;
					m_ScaleOut.Done = false;
					m_ScaleOut.ScaleBegin = base.transform.localScale;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimOut_ScaleUpdateByValue), 0f, 1f, m_ScaleOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_ScaleOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_ScaleOut.EaseType)).SetOnComplete (new Action (AnimOut_ScaleComplete)).SetUseEstimatedTime (true);
					if (m_ScaleOut.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_ScaleOut.Sounds.m_Begin);
					}
				}
				if (m_FadeOut.Enable && !m_FadeOut.Began) {
					m_FadeOut.Began = true;
					m_FadeOut.Animating = false;
					m_FadeOut.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (AnimOut_FadeUpdateByValue), 0f, 1f, m_FadeOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (m_FadeOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_FadeOut.EaseType)).SetOnComplete (new Action (AnimOut_FadeComplete)).SetUseEstimatedTime (true);
					if (m_FadeOut.Sounds.m_Begin != null) {
						PlaySoundOneShot (m_FadeOut.Sounds.m_Begin);
					}
				}
			}
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Children) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				RecuresiveMoveOut (base.transform);
			}
		}
	}

	private AudioSource PlaySoundLoop (AudioClip pAudioClip) {
		if (this == null) {
			return null;
		}
		if (base.gameObject == null) {
			return null;
		}
		if (!base.gameObject.activeSelf) {
			return null;
		}
		if (!base.enabled) {
			return null;
		}
		AudioSource source = null;
		AudioListener listener = FindObjectOfType<AudioListener> ();
		if (listener != null) {
			bool flag = false;
			AudioSource[] components = listener.gameObject.GetComponents<AudioSource> ();
			if (components.Length != 0) {
				for (int i = 0; i < components.Length; i++) {
					if (!components[i].isPlaying) {
						flag = true;
						source = components[i];
						source.clip = (pAudioClip);
						source.loop = (true);
						source.Play ();
						break;
					}
				}
			}
			if (!flag && (components.Length < 0x10)) {
				source = listener.gameObject.AddComponent<AudioSource> ();
				source.rolloffMode = AudioRolloffMode.Linear;
				source.playOnAwake = (false);
				source.clip = (pAudioClip);
				source.loop = (true);
				source.Play ();
			}
		}
		return source;
	}

	private void PlaySoundOneShot (AudioClip pAudioClip) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			AudioListener listener = FindObjectOfType<AudioListener> ();
			if (listener != null) {
				bool flag = false;
				AudioSource[] components = listener.gameObject.GetComponents<AudioSource> ();
				if (components.Length != 0) {
					for (int i = 0; i < components.Length; i++) {
						if (!components[i].isPlaying) {
							flag = true;
							components[i].PlayOneShot (pAudioClip);
							break;
						}
					}
				}
				if (!flag && (components.Length < 0x20)) {
					AudioSource local1 = listener.gameObject.AddComponent<AudioSource> ();
					local1.rolloffMode = AudioRolloffMode.Linear; ;
					local1.playOnAwake = (false);
					local1.PlayOneShot (pAudioClip);
				}
			}
		}
	}

	private IEnumerator PlaySoundOneShotWithDelay (AudioClip pAudioClip, float Delay) {
		yield return new WaitForSeconds (Delay);
		PlaySoundOneShot (pAudioClip);
	}

	private void RecuresiveMoveIn (Transform tf) {
		foreach (Transform transform in tf) {
			if (transform.gameObject.activeSelf) {
				GUIAnim component = transform.gameObject.GetComponent<GUIAnim> ();
				if ((component != null) && component.enabled) {
					component.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
				}
				RecuresiveMoveIn (transform);
			}
		}
	}

	private void RecuresiveMoveOut (Transform tf) {
		foreach (Transform transform in tf) {
			if (transform.gameObject.activeSelf) {
				GUIAnim component = transform.gameObject.GetComponent<GUIAnim> ();
				if ((component != null) && component.enabled) {
					component.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
				}
				RecuresiveMoveOut (transform);
			}
		}
	}

	private void RecursiveFade (Transform trans, float Fade, bool IsFadeChildren) {
		bool flag = false;
		if (base.transform != trans) {
			GUIAnim component = trans.GetComponent<GUIAnim> ();
			if (component != null) {
				if (component.m_FadeIn.Enable && component.m_FadeIn.Began) {
					flag = true;
				}
				else if (component.m_FadeOut.Enable && component.m_FadeOut.Began) {
					flag = true;
				}
			}
		}
		if (!flag) {
			Image image = trans.gameObject.GetComponent<Image> ();
			if (image != null) {
				image.color = (new Color (image.color.r, image.color.g, image.color.b, Fade));
			}
			Text text = trans.gameObject.GetComponent<Text> ();
			if (text != null) {
				text.color = (new Color (text.color.r, text.color.g, text.color.b, Fade));
			}
			RawImage image2 = trans.gameObject.GetComponent<RawImage> ();
			if (image2 != null) {
				image2.color = (new Color (image2.color.r, image2.color.g, image2.color.b, Fade));
			}
		}
		if (IsFadeChildren) {
			foreach (Transform transform in trans) {
				if (transform.gameObject.activeSelf) {
					RecursiveFade (transform.transform, Fade, IsFadeChildren);
				}
			}
		}
	}

	public void Reset () {
		if ((this != null) && (base.gameObject != null)) {
			if (m_MoveIn == null) {
				if (FindObjectOfType<GUIAnimSystem> () == null) {
					GameObject obj1 = new GameObject ();
					obj1.transform.localPosition = (new Vector3 (0f, 0f, 0f));
					obj1.name = ("GUIAnimSystem");
					obj1.AddComponent<GUIAnimSystem> ();
					GUIAnimSystem.Instance = FindObjectOfType<GUIAnimSystem> ();
					DontDestroyOnLoad (obj1);
				}
			}
			else {
				InitMoveIn ();
				InitMoveOut ();
				InitRotationIn ();
				InitFadeIn ();
				if (m_ScaleIn.Enable) {
					InitScaleIn ();
				}
			}
		}
	}

	public void ResetAllChildren () {
		if ((this != null) && (base.gameObject != null)) {
			InitMoveIn ();
			InitMoveOut ();
			InitRotationIn ();
			InitFadeIn ();
			foreach (Transform transform in base.transform) {
				if (transform.gameObject.activeSelf) {
					GUIAnim component = transform.gameObject.GetComponent<GUIAnim> ();
					if ((component != null) && component.enabled) {
						component.ResetAllChildren ();
					}
				}
			}
			if (m_ScaleIn.Enable) {
				InitScaleIn ();
			}
		}
	}

	private void ScaleLoopComplete () {
	}

	private void ScaleLoopStart (float delay) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			m_ScaleLoop.Began = true;
			m_ScaleLoop.Animating = false;
			m_ScaleLoop.IsOverriding = false;
			m_ScaleLoop.IsOverridingDelayTimeCount = 0f;
			m_ScaleLoop.Done = false;
			m_ScaleLoop.m_ScaleLast = base.transform.localScale;
			m_GETweenScaleLoop = GETween.TweenValue (base.gameObject, new Action<float> (ScaleLoopUpdateByValue), 0f, 1f, m_ScaleLoop.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (GETweenEaseType (m_ScaleLoop.EaseType)).SetLoopPingPong ().SetOnComplete (new Action (ScaleLoopComplete)).SetUseEstimatedTime (true);
		}
	}

	private void ScaleLoopUpdateByValue (float Value) {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (m_ScaleLoop.m_ScaleLast != base.transform.localScale) {
				StopScaleLoop ();
			}
			else {
				if (m_ScaleLoop.IsOverriding) {
					m_ScaleLoop.IsOverridingDelayTimeCount += Time.deltaTime;
					if (m_ScaleLoop.IsOverridingDelayTimeCount > 2f) {
						m_ScaleLoop.IsOverridingDelayTimeCount = 0f;
						m_ScaleLoop.IsOverriding = false;
					}
				}
				if (!m_ScaleLoop.IsOverriding) {
					if (!m_ScaleLoop.Animating) {
						StartScaleLoopSound ();
					}
					base.transform.localScale = (new Vector3 ((m_ScaleLoop.Min.x * m_ScaleOriginal.x) + (((m_ScaleLoop.Max.x * m_ScaleOriginal.x) - (m_ScaleLoop.Min.x * m_ScaleOriginal.x)) * Value), (m_ScaleLoop.Min.y * m_ScaleOriginal.y) + (((m_ScaleLoop.Max.y * m_ScaleOriginal.y) - (m_ScaleLoop.Min.y * m_ScaleOriginal.y)) * Value), (m_ScaleLoop.Min.z * m_ScaleOriginal.z) + (((m_ScaleLoop.Max.z * m_ScaleOriginal.z) - (m_ScaleLoop.Min.z * m_ScaleOriginal.z)) * Value)));
				}
			}
			m_ScaleLoop.m_ScaleLast = base.transform.localScale;
		}
	}

	public void ScreenResolutionChange () {
		if ((this != null) && (base.gameObject != null)) {
			InitMoveIn ();
			InitMoveOut ();
		}
	}

	private void Start () {
		if (((this != null) && (base.gameObject != null)) && !m_InitialDone) {
			m_InitialDone = true;
			bool flag = true;
			if (GUIAnimSystem.Instance.m_AutoAnimation) {
				if ((GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.In) || (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.All)) {
					if ((m_MoveIn.Enable || m_RotationIn.Enable) || (m_ScaleIn.Enable || m_FadeIn.Enable)) {
						MoveIn ();
					}
					else if (m_ScaleLoop.Enable || m_FadeLoop.Enable) {
						flag = false;
						StartMoveIdle ();
					}
				}
				else if (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.Idle) {
					flag = false;
					StartMoveIdle ();
				}
				else if (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.Out) {
					flag = false;
					InitMoveOut ();
				}
				else if (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.None) {
					flag = false;
				}
			}
			if (flag) {
				Reset ();
			}
			if (GUIAnimSystem.Instance.m_AutoAnimation && ((GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.Out) || (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.All))) {
				float delay = 1f;
				if ((GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.In) || (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.All)) {
					delay = GUIAnimSystem.Instance.m_IdleTime;
				}
				base.StartCoroutine (CoroutineMoveOut (delay));
			}
		}
	}

	private void StartFadeLoopSound () {
		if (!m_FadeLoop.Animating && (m_FadeLoop.Sound.m_AudioClip != null)) {
			m_FadeLoop.Animating = true;
			m_FadeLoop.Sound.m_AudioSource = null;
			if (m_FadeLoop.Sound.m_Loop) {
				m_FadeLoop.Sound.m_AudioSource = PlaySoundLoop (m_FadeLoop.Sound.m_AudioClip);
			}
			else {
				PlaySoundOneShot (m_FadeLoop.Sound.m_AudioClip);
			}
		}
	}

	private void StartMoveIdle () {
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			m_MoveIdle_StartSucceed = false;
			m_MoveIdle_Attemp = 0;
			MoveIdle ();
		}
	}

	private void StartScaleLoopSound () {
		if (((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) && (!m_ScaleLoop.Animating && (m_ScaleLoop.Sound.m_AudioClip != null))) {
			m_ScaleLoop.Animating = true;
			m_ScaleLoop.Sound.m_AudioSource = null;
			if (m_ScaleLoop.Sound.m_Loop) {
				m_ScaleLoop.Sound.m_AudioSource = PlaySoundLoop (m_ScaleLoop.Sound.m_AudioClip);
			}
			else {
				PlaySoundOneShot (m_ScaleLoop.Sound.m_AudioClip);
			}
		}
	}

	private void StopFadeLoop () {
		if ((this != null) && (base.gameObject != null)) {
			m_FadeLoop.Animating = false;
			m_FadeLoop.IsOverriding = true;
			m_FadeLoop.IsOverridingDelayTimeCount = 0f;
			m_FadeLoop.Began = false;
			m_FadeLoop.Done = true;
			if (m_GETweenFadeLoop != null) {
				m_GETweenFadeLoop.Cancel ();
				m_GETweenFadeLoop = null;
			}
			StopFadeLoopSound ();
		}
	}

	private void StopFadeLoopSound () {
		if ((m_FadeLoop.Sound.m_AudioClip != null) && (m_FadeLoop.Sound.m_AudioSource != null)) {
			m_FadeLoop.Sound.m_AudioSource.Stop ();
			m_FadeLoop.Sound.m_AudioSource = null;
		}
	}

	private void StopScaleLoop () {
		if ((this != null) && (base.gameObject != null)) {
			m_ScaleLoop.Animating = false;
			m_ScaleLoop.IsOverriding = true;
			m_ScaleLoop.IsOverridingDelayTimeCount = 0f;
			m_ScaleLoop.Began = false;
			m_ScaleLoop.Done = true;
			if (m_GETweenScaleLoop != null) {
				m_GETweenScaleLoop.Cancel ();
				m_GETweenScaleLoop = null;
			}
			StopScaleLoopSound ();
		}
	}

	private void StopScaleLoopSound () {
		if (((this != null) && (base.gameObject != null)) && ((m_ScaleLoop.Sound.m_AudioClip != null) && (m_ScaleLoop.Sound.m_AudioSource != null))) {
			m_ScaleLoop.Sound.m_AudioSource.Stop ();
			m_ScaleLoop.Sound.m_AudioSource = null;
		}
	}

	private void Update () {
	}





	[Serializable]
	public class cFade {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		public bool Enable;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public GUIAnim.cSounds Sounds;
		public float Time = 1f;
	}

	[Serializable]
	public class cMoveIn {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginPos;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.OutBack;
		public bool Enable;
		[HideInInspector]
		public Vector3 EndPos;
		public GUIAnim.ePosMove MoveFrom = GUIAnim.ePosMove.UpperScreenEdge;
		public Vector3 Position;
		public GUIAnim.cSounds Sounds;
		public float Time = 1f;
	}

	[Serializable]
	public class cMoveOut {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginPos;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.InBack;
		public bool Enable;
		[HideInInspector]
		public Vector3 EndPos;
		public GUIAnim.ePosMove MoveTo = GUIAnim.ePosMove.UpperScreenEdge;
		public Vector3 Position;
		public GUIAnim.cSounds Sounds;
		public float Time = 1f;
	}

	[Serializable]
	public class cPingPongFade {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		public bool Enable;
		public bool FadeChildren;
		[HideInInspector]
		public bool IsOverriding;
		[HideInInspector]
		public float IsOverridingDelayTimeCount;
		[HideInInspector]
		public float m_FadeLast;
		public float Max = 1f;
		public float Min;
		public GUIAnim.cSoundsForPingPongAnim Sound;
		public float Time = 1f;
	}

	[Serializable]
	public class cPingPongScale {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		public bool Enable;
		[HideInInspector]
		public bool IsOverriding;
		[HideInInspector]
		public float IsOverridingDelayTimeCount;
		[HideInInspector]
		public Vector3 m_ScaleLast;
		public Vector3 Max = new Vector3 (1.05f, 1.05f, 1.05f);
		public Vector3 Min = new Vector3 (1f, 1f, 1f);
		public GUIAnim.cSoundsForPingPongAnim Sound;
		public float Time = 1f;
	}

	[Serializable]
	public class cRotationIn {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginRotation;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.OutBack;
		public bool Enable;
		[HideInInspector]
		public Vector3 EndRotation;
		public Vector3 Rotation;
		public GUIAnim.cSounds Sounds;
		public float Time = 1f;
	}

	[Serializable]
	public class cRotationOut {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginRotation;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.InBack;
		public bool Enable;
		[HideInInspector]
		public Vector3 EndRotation;
		public Vector3 Rotation;
		public GUIAnim.cSounds Sounds;
		public float Time = 1f;
	}

	[Serializable]
	public class cScaleIn {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.OutBack;
		public bool Enable;
		public Vector3 ScaleBegin = new Vector3 (0f, 0f, 0f);
		public GUIAnim.cSounds Sounds;
		public float Time = 1f;
	}

	[Serializable]
	public class cScaleOut {
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.InBack;
		public bool Enable;
		[HideInInspector]
		public Vector3 ScaleBegin = new Vector3 (1f, 1f, 1f);
		public Vector3 ScaleEnd = new Vector3 (0f, 0f, 0f);
		public GUIAnim.cSounds Sounds;
		public float Time = 1f;
	}

	[Serializable]
	public class cSounds {
		public AudioClip m_AfterDelay;
		public AudioClip m_Begin;
		public AudioClip m_End;
	}

	[Serializable]
	public class cSoundsForPingPongAnim {
		public AudioClip m_AudioClip;
		[HideInInspector]
		public AudioSource m_AudioSource;
		public bool m_Loop;
	}

	public enum eAlignment {
		Current,
		TopLeft,
		TopCenter,
		TopRight,
		LeftCenter,
		Center,
		RightCenter,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	public enum eEaseType {
		InQuad,
		OutQuad,
		InOutQuad,
		InCubic,
		OutCubic,
		InOutCubic,
		InQuart,
		OutQuart,
		InOutQuart,
		InQuint,
		OutQuint,
		InOutQuint,
		InSine,
		OutSine,
		InOutSine,
		InExpo,
		OutExpo,
		InOutExpo,
		InCirc,
		OutCirc,
		InOutCirc,
		linear,
		spring,
		InBounce,
		OutBounce,
		InOutBounce,
		InBack,
		OutBack,
		InOutBack,
		InElastic,
		OutElastic,
		InOutElastic
	}

	public enum ePosMove {
		ParentPosition,
		LocalPosition,
		UpperScreenEdge,
		LeftScreenEdge,
		RightScreenEdge,
		BottomScreenEdge,
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
		SelfPosition
	}
}

