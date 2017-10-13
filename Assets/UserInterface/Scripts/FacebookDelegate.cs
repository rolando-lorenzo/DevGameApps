using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EasyMobile;

public class FacebookDelegate : MonoBehaviour {
    #region Class members
    private static FacebookDelegate _instance;

    public delegate void EventLoginFacebook(bool statusLogin);
    public event EventLoginFacebook OnLoginFacebookStatus;

    public delegate void EventProfileUsernameFacebook(string name);
    public event EventProfileUsernameFacebook OnProfileUsernameFacebook;

    public delegate void EventProfilePictureFacebook(Sprite picture);
    public event EventProfilePictureFacebook OnProfilePictureFacebook;

    public delegate void EventMessageFacebookProcess(string message, bool status);
    public event EventMessageFacebookProcess OnMessageFacebookProgress;

    public delegate void EventInviteFacebook(string message, int paws);
    public event EventInviteFacebook OnInviteFacebook;

    private string urlPictureProfile { get; set; }
    private string appLinkURL { get; set; }
    private int numberInvite { get; set; }
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

    #region Super class overrides

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
            if (OnMessageFacebookProgress != null)
                OnMessageFacebookProgress("Se genero un error interno al tratar de inicializar Facebook.", false);
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
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("user_friends");
        permissions.Add("email");
        FB.LogInWithReadPermissions(permissions, CallBackLoginFacebook);
    }

    private void CallBackLoginFacebook(ILoginResult result)
    {
        if (result.Error != null)
        {
            if (OnMessageFacebookProgress != null)
                OnMessageFacebookProgress("Surgió un error al tratar de iniciar sesión en Facebook.", false);
        }
        else
        {
            //verify login user in facebook and get data as username and profile image
            if (FB.IsLoggedIn)
            {
                if (OnLoginFacebookStatus != null)
                    OnLoginFacebookStatus(FB.IsLoggedIn);
                
                GetProfileFacebook();
            }
            else
            {
                if (OnMessageFacebookProgress != null)
                    OnMessageFacebookProgress("Surgió un error al tratar de iniciar sesión en Facebook.", false);
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
                OnProfileUsernameFacebook(result.ResultDictionary["first_name"].ToString());
        }
        else
        {
            if (OnMessageFacebookProgress != null)
                OnMessageFacebookProgress("No se pudo obtener el nombre del usuario.", false);
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
            if (OnMessageFacebookProgress != null)
                OnMessageFacebookProgress("Se requiere que se inicie sesión en Facebook.", false);
        }
       
    }

    private void CallBackShareFacebook(IResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Share cancel");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            if (OnMessageFacebookProgress != null)
                OnMessageFacebookProgress("Se produjo un error al tartar de compartir en Facebook.", false);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            if (OnMessageFacebookProgress != null)
                OnMessageFacebookProgress("Se compartió exitosamente en Facebook.", true);
        }
    }

    public void InviteFriendsFacebook()
    {
        if (FB.IsLoggedIn)
        {
            FB.AppRequest("Te invito a probar este juego !!",
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
            if (OnMessageFacebookProgress != null)
                OnMessageFacebookProgress("Se requiere que se inicie sesión en Facebook.", false);
        }
        
    }

    private void CallBackInviteFriendsFacebook(IAppRequestResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Invite cancel");
            numberInvite = 0;
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            numberInvite = 0;
            if (OnInviteFacebook != null)
                OnInviteFacebook("Se genero un error al invitar a tus amigos en Facebook", numberInvite);
        }
        else
        {
            //Debug.Log(result.RawResult);
            var responseObject = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;
            if (responseObject.ContainsKey("request"))
            {
                string request = responseObject["request"].ToString();
                PlayerPrefs.SetString("invitekey", request);
                PlayerPrefs.Save();
            }
            else
            {
                numberInvite = 0;
            }

            if (responseObject.ContainsKey("to"))
            {
                string[] tokens = responseObject["to"].ToString().Split(',');
                numberInvite = tokens.Length;
            }
            else
            {
                numberInvite = 0;
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
    #endregion

    #region Interface implementation
    #endregion
}
