using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Facebook.Unity;
using EasyMobile;

public class FacebookManager : MonoBehaviour {

	private static FacebookManager _instance;
	private string urlImg;
	int score = 10000;
	public delegate void ItemPurchasedAction (string algo);
	/// <summary>
	/// Called when current item is purchased
	/// </summary>
	public event ItemPurchasedAction OnFBSuccess;
	public event ItemPurchasedAction OnFBError;

	private Rect windowRect = new Rect ((Screen.width - 300)/2, (Screen.height - 200)/2, 300, 200);
	private bool dialogShow = false;
	private string dialogMensage;
	private bool dialogBtnOk = false;

	public static FacebookManager Instance
	{
		get {
			if (_instance == null) {
				GameObject fbm = new GameObject ("FBManager");
				fbm.AddComponent<FacebookManager> ();
			}

			return _instance;
		}
	}


	public bool IsLoggedIn { get; set; }
	public string ProfileName { get; set; }
	public Sprite ProfilePic { get; set; }
	public string AppLinkURL { get; set; }
	public Image FotoPerfil { get; set;}
	public int NumeroInvite { get; set; }

	void Awake()
	{
		DontDestroyOnLoad (this.gameObject);
		_instance = this;

	}

	public void InitFB()
	{
		if (!FB.IsInitialized) {
			FB.Init (this.SetInit, this.OnHideUnity);
		} else {
			IsLoggedIn = FB.IsLoggedIn;
		}
	}

	void SetInit()
	{

		if (FB.IsLoggedIn) {
			Debug.Log ("Se ha logueado a facebook correctamente");
			ObtenerPerfil ();
		} else {
			if(OnFBError != null)
				OnFBError ("Ocurrio un error al intentar loguearse");
			Debug.Log ("Ocurrio un error al intentar loguearse");
		}

		IsLoggedIn = FB.IsLoggedIn;

	}

	void OnHideUnity(bool isGameShown)
	{

		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

	}

	public void ObtenerPerfil()
	{
		FB.API ("/me?fields=first_name", HttpMethod.GET, MostrarUsuarioLogueado);
		FB.API ("/me/picture?redirect=false", HttpMethod.GET, MostrarImgPerfil);
		FB.GetAppLink (DealWithAppLink);
	}

	private void MostrarUsuarioLogueado(IResult result)
	{
		if (String.IsNullOrEmpty(result.Error)) {
			ProfileName = "" + result.ResultDictionary ["first_name"];
		} else {
			Debug.Log (result.Error);
		}
	}

	private void MostrarImgPerfil(IGraphResult result)
	{
		Dictionary<string,object> dic = result.ResultDictionary ["data"] as Dictionary<string,object>;
		foreach (string key in dic.Keys) {
			Debug.Log(key + " : " + dic[key].ToString());
			if (key == "url") {
				urlImg = dic [key].ToString ();
				StartCoroutine ("ObtenerImgPerfil");
			}
		}

	}


	IEnumerator ObtenerImgPerfil()
	{

		WWW www = new WWW(urlImg);
		yield return www;
		ProfilePic = Sprite.Create (www.texture, new Rect (0, 0, 50, 50), new Vector2 ());

	}

	void DealWithAppLink(IAppLinkResult result)
	{
		Debug.Log ("AppLinkURL "+result);
		if (!String.IsNullOrEmpty (result.Url)) {
			AppLinkURL = "" + result.Url + "";
			Debug.Log (AppLinkURL);
			MostrarDialogo ("El link es: "+AppLinkURL,true);
		}else {
			AppLinkURL = "https://fb.me/146125585987313"; //Link de la app publicada
		}
	}


	public void Compartir()
	{
		FB.FeedShare (
			string.Empty,
			new Uri("http://blackjaguarwhitetiger.org"),
			"Mira esta App!",
			"Es un excelente juego",
			"Revisa esto...",
			new Uri("http://iluminart.com.mx/img/portafolio-img/pic3.jpg"),
			string.Empty,
			CompartirCallback
		);
	}

	private void CompartirCallback(IResult result)
	{
		if (result.Cancelled) {
			Debug.Log ("Compartir cancelada");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error al Compartir!");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Compartir Correcto!!!");
		}
	}

	public void InvitarAmigos()
	{
		FB.Mobile.AppInvite (
			new Uri(AppLinkURL),
			new Uri("http://iluminart.com.mx/img/portafolio-img/pic3.jpg"),
			InvitarAmigosCallback
		);
	}

	void InvitarAmigosCallback(IResult result)
	{
		MostrarDialogo (result.RawResult.ToString(),true);
		if (result.Cancelled) {
			Debug.Log ("Invitacion cancelada");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error al invitar!");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Exito al invitar");
		}
	}

	public void CompartirConAppUsuarios()
	{

		FB.AppRequest (
			"Texto a compartir...",
			null,
			new List<object> (){ "app_users" },
			null,
			null,
			null,
			null,
			CompartirConAppUsuariosCallback
		);

	}

	private void CompartirConAppUsuariosCallback(IAppRequestResult result)
	{
		if (result.Cancelled) {
			Debug.Log ("Desafio cancelado");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error en el desafio!");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Exito en el desafio");
		}
	}

	public void QueryScores()
	{
		FB.API ("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, QueryScoresCallback);
	}

	private void QueryScoresCallback(IGraphResult result)
	{
		Debug.Log (result);

	}

	private IEnumerator TakeScreenshot() 
	{
		yield return new WaitForEndOfFrame();

		var width = Screen.width;
		var height = Screen.height;
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();

		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "Screenshot.png");

		FB.API("me/photos", HttpMethod.POST, QueryScoresCallback, wwwForm);
	}

	private void ObtenerGuardarImgDeLogro(){
		try
		{
			Debug.Log ("Obteniendo img de carpeta Resources...");
			var texture = Resources.Load<Texture2D>("capturaPantalla3");
			Debug.Log ("Guaradando en..."+Application.persistentDataPath);
			File.WriteAllBytes(Application.persistentDataPath + "/logro1.png", texture.EncodeToPNG());
			Debug.Log ("Img de logro1 Guardada exitosamente !!");
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}

	}

	public void CompartirLogro() {
		ObtenerGuardarImgDeLogro ();
		string path = Path.Combine (Application.persistentDataPath,"logro1.png");
		MobileNativeShare.ShareImage (path,"Logro superado !!","BJWT");
	}

	void OnGUI () 
	{
		if(dialogShow)
			windowRect = GUI.Window (0, windowRect, DialogWindow, dialogMensage);
	}

	void DialogWindow (int windowID)
	{
		if(GUI.Button(new Rect(5,100, windowRect.width - 10, 20), "Aceptar"))
		{
			dialogShow = false;
		}
	}

	public void MostrarDialogo(string mensaje, bool btnOk)
	{
		dialogMensage = mensaje;
		dialogBtnOk = btnOk;
		dialogShow = true;
	}

	public void SetScore()
	{
		var scoreData = new Dictionary<string,string> ();
		scoreData ["score"] = "27";
		FB.API ("/me/scores", HttpMethod.POST, delegate(IGraphResult result) {
			Debug.Log ("Score submit result: " + result);
		}, scoreData);
	}
		
	public void InvitarAmigosV()
	{
		NumeroInvite = -1;
		FB.AppRequest("Te invito a probar este juego !!",
			null,
			null,
			null,
			null,
			null,
			"BJWT",
			InvitarAmigosCallbackV);
	}

	private void InvitarAmigosCallbackV(IAppRequestResult result)
	{
		if (result.Cancelled)
		{
			Debug.Log("Invitacion cancelada");
			NumeroInvite = 0;
		}
		else if (!string.IsNullOrEmpty(result.Error))
		{
			Debug.Log("Error al invitar!");
			NumeroInvite = 0;
		}
		else {
			Debug.Log(result.RawResult);
			var responseObject = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;
			if (responseObject.ContainsKey("request"))
			{
				string request = responseObject["request"].ToString();
				PlayerPrefs.SetString("invitekey", request);
				PlayerPrefs.Save();
			}
			else
			{
				NumeroInvite = 0;
			}

			if (responseObject.ContainsKey("to"))
			{
				string[] tokens = responseObject["to"].ToString().Split(',');
				//MostrarDialogo("Has invitado a "+tokens.Length.ToString()+" Amigos !!", true);
				NumeroInvite = tokens.Length;
			}
			else
			{
				NumeroInvite = 0;
			}


		}
	}

	public void LogOut()
	{
		FB.LogOut();
		IsLoggedIn = false;
	}
}
