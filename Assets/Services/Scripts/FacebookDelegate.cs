using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EasyMobile;
using System.IO;

public class FacebookDelegate : MonoBehaviour {
    #region Class members
    private static FacebookDelegate _instance;

    public delegate void EventLoginFacebook(bool statusLogin);
    public event EventLoginFacebook OnLoginFacebookStatus;

    public delegate void EventProfileUsernameFacebook(string name);
    public event EventProfileUsernameFacebook OnProfileUsernameFacebook;

    public delegate void EventProfilePictureFacebook(Sprite picture);
    public event EventProfilePictureFacebook OnProfilePictureFacebook;

    public delegate void EventMessageFacebookProcess(string message, bool isError);
    public event EventMessageFacebookProcess OnMessageFacebookProgress;

    public delegate void EventPawsFacebookReward(int paws);
    public event EventPawsFacebookReward OnPawsFacebookReward;

    private string urlPictureProfile { get; set; }
    private string appLinkURL { get; set; }

    #endregion

    #region Class accesors
    public static FacebookDelegate Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject fbm = new GameObject("FacebookDelegate");
                fbm.AddComponent<FacebookDelegate>();
            }

            return _instance;
        }
    }

    #endregion

    #region MonoBehaviour overrides
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _instance = this;
    }

   
    #endregion


    #region Class implementation

    /// <summary>
    /// Start Initialize of the Facebook
    /// </summary>
    public void InitFB()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(this.SetInit, this.OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
            Debug.Log("SDK Facebook inizialized rigth");
        }
    }

    private void SetInit()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            Debug.Log("SDK Facebook inizialized rigth");
        }
        else
        {
            if (OnMessageFacebookProgress != null)//Se genero un error interno al tratar de inicializar Facebook.
                OnMessageFacebookProgress("msg_err_init_sdk_facebook", true);
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

    }

    public void LoginUserFacebook()
    {
        if (FB.IsLoggedIn) {

            if (OnLoginFacebookStatus != null)
                OnLoginFacebookStatus(FB.IsLoggedIn);
            GetProfileFacebook();
        }
        else
        {
            List<string> permissions = new List<string>();
            permissions.Add("public_profile");
            permissions.Add("user_friends");
            permissions.Add("email");
            FB.LogInWithReadPermissions(permissions, CallBackLoginFacebook);

        }       
    }

    private void CallBackLoginFacebook(ILoginResult result)
    {
        if (result.Error != null)
        {
            if (OnMessageFacebookProgress != null)//Surgió un error al tratar de iniciar sesión en Facebook.
                OnMessageFacebookProgress("msg_err_login_facebook", true);
        }
        else if (result.Cancelled)
        {
            //Dont show result on screen because the user launch cancel
            Debug.Log("Cancel Login Facebook!");
        }
        else
        {
            //verify login user in facebook and get data as username and profile image
            if (FB.IsLoggedIn)
            {
                if (OnLoginFacebookStatus != null)
                    OnLoginFacebookStatus(FB.IsLoggedIn);

                if (OnPawsFacebookReward != null) { 
                   OnPawsFacebookReward(50);
                }
                
                GetProfileFacebook();

            }
            else
            {
                if (OnMessageFacebookProgress != null)//Surgió un error al tratar de iniciar sesión en Facebook.
                    OnMessageFacebookProgress("msg_err_login_facebook", true);
            }

        }
    }

    private void GetProfileFacebook()
    {
        FB.API("/me?fields=first_name", HttpMethod.GET, CallBackNameUserProfileFacebook);
        FB.API("/me/picture?redirect=false", HttpMethod.GET, CallBackPictureProfileFacebook);
        FB.GetAppLink(GetAppLinkFacebook);
    }

    private void CallBackNameUserProfileFacebook(IResult result)
    {
        if (string.IsNullOrEmpty(result.Error))
        {
            if(OnProfileUsernameFacebook != null)
                OnProfileUsernameFacebook("Welcome "+result.ResultDictionary["first_name"].ToString());
        }
        else
        {
            if (OnMessageFacebookProgress != null)//No se pudo obtener el nombre del usuario de su cuenta de Facebook.
                OnMessageFacebookProgress("msg_err_get_account_info", true);
        }
    }

    private void CallBackPictureProfileFacebook(IGraphResult result)
    {
        Dictionary<string, object> dic = result.ResultDictionary["data"] as Dictionary<string, object>;
        foreach (string key in dic.Keys)
        {
            //Debug.Log(key + " : " + dic[key].ToString());
            if (key == "url")
            {
                urlPictureProfile = dic[key].ToString();
                StartCoroutine(GetProfilePictureFacebook());
            }
        }

    }


    IEnumerator GetProfilePictureFacebook()
    {
        WWW www = new WWW(urlPictureProfile);
        yield return www;
        if(OnProfilePictureFacebook != null)
            OnProfilePictureFacebook(Sprite.Create(www.texture, new Rect(0, 0, 50, 50), new Vector2()));

    }

    private void GetAppLinkFacebook(IAppLinkResult result)
    {
        if (!String.IsNullOrEmpty(result.Url))
        {
            appLinkURL = "" + result.Url + "";
        }
        else
        {
            appLinkURL = "https://fb.me/146125585987313"; //Link de la app publicada
        }
    }

    public void ShareFacebook()
    {
        if(FB.IsLoggedIn)
        {
            FB.FeedShare(
               string.Empty,
               new Uri("http://blackjaguarwhitetiger.org"),
               "Mira esta App!",
               "Es un excelente juego",
               "Revisa esto...",
               new Uri("http://iluminart.com.mx/img/portafolio-img/pic3.jpg"),
               string.Empty,
               CallBackShareFacebook
           );
        }
        else
        {
            if (OnMessageFacebookProgress != null)//Se requiere que se inicie sesión en Facebook.
                OnMessageFacebookProgress("msg_err_login_facebook", true);
        }
       
    }

    private void CallBackShareFacebook(IResult result)
    {
        if (result.Cancelled)
        {
            //Dont show this action on screen because user launches it.
            Debug.Log("Share cancel");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            if (OnMessageFacebookProgress != null)//Se produjo un error al tartar de compartir en Facebook.
                OnMessageFacebookProgress("msg_err_share_facebook", true);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            if (OnMessageFacebookProgress != null)//Se compartió exitosamente en Facebook.
                OnMessageFacebookProgress("msg_info_success_share_fb", false);
            if (OnPawsFacebookReward != null)
                OnPawsFacebookReward(50);
        }
    }

    public void InviteFriendsFacebook()
    {
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();
        if (FB.IsLoggedIn)
        {
            FB.AppRequest(lm.GetString("msg_info_invite_to_play"),
            null,
            null,
            null,
            null,
            null,
            "BJWT",
            CallBackInviteFriendsFacebook);
        }
        else
        {
            if (OnMessageFacebookProgress != null)//Se requiere que se inicie sesión en Facebook.
                OnMessageFacebookProgress("msg_err_login_req", true);
        }
        
    }

    private void CallBackInviteFriendsFacebook(IAppRequestResult result)
    {
        if (result.Cancelled)
        {
            //Dont show this action on screen because user launches it.
            Debug.Log("Invite cancel");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            if (OnMessageFacebookProgress != null)//Se genero un error al invitar a tus amigos en Facebook
                OnMessageFacebookProgress("msg_err_invite_friends", true);
        }
        else
        {
            //Debug.Log(result.RawResult);
            var responseObject = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;

            if (responseObject.ContainsKey("to"))
            {
                string[] tokens = responseObject["to"].ToString().Split(',');
                if (tokens.Length >= 5)
                {
                    if (OnMessageFacebookProgress != null)//Acaba de invitar a más de 5 personas, es acreedor a una recompensa.
                        OnMessageFacebookProgress("msg_info_invite_5friends", false);
                    if (OnPawsFacebookReward != null)
                        OnPawsFacebookReward(100);
                }
                else
                {
                    if (OnMessageFacebookProgress != null)//Es necesario invitar a 5 personas para recibir una recompensa.
                        OnMessageFacebookProgress("msg_info_invite_less_5friends", false);
                }
            }
            else
            {
                if (OnMessageFacebookProgress != null)//Se generó un error al invitar a tus amigos en Facebook
                    OnMessageFacebookProgress("msg_err_invite_friends", true);
            }


        }

    }

    public void ShareScoreScreenFacebook()
    {
        StartCoroutine(CourtineShareScoreScreenFacebook());
    }

    private IEnumerator CourtineShareScoreScreenFacebook()
    {
        yield return new WaitForEndOfFrame();
        //save screenshot and share picture
        string namePicture = "screenScore";
        string message = "I love this game!";
        MobileNativeShare.ShareScreenshot(namePicture, message);
    }

    private void GetSaveImgDeScore()
    {
        try
        {
            Debug.Log("Obteniendo img de carpeta Resources...");
            var texture = Resources.Load<Texture2D>("capturaPantalla3");
            Debug.Log("Guaradando en..." + Application.persistentDataPath);
            File.WriteAllBytes(Application.persistentDataPath + "/logro1.png", texture.EncodeToPNG());
            Debug.Log("Img de logro1 Guardada exitosamente !!");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public void ShareScorePathImageFacebook()
    {
        GetSaveImgDeScore();
        string path = Path.Combine(Application.persistentDataPath, "logro1.png");
        MobileNativeShare.ShareImage(path, "Logro superado !!", "BJWT");
    }

	public void LogOut()
	{
		FB.LogOut();
	}

    #endregion

    #region Interface implementation
    #endregion
}
