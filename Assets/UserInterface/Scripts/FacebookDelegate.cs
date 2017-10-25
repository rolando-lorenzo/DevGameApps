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

    public delegate void EventPawsFacebookReward(int paws);
    public event EventPawsFacebookReward OnPawsFacebookReward;

    private string urlPictureProfile { get; set; }
    private string appLinkURL { get; set; }
    public bool loginFacebook { get; set; }
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
            if (OnMessageFacebookProgress != null)//Se genero un error interno al tratar de inicializar Facebook.
                OnMessageFacebookProgress("An internal error is generated when trying to initialize Facebook.", false);
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
                OnMessageFacebookProgress("There was an error trying to log in to Facebook.", false);
        }
        else if (result.Cancelled)
        {
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
                    OnMessageFacebookProgress("There was an error trying to log in to Facebook.", false);
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
            if (OnMessageFacebookProgress != null)//No se pudo obtener el nombre del usuario de su cuenta de Facebook.
                OnMessageFacebookProgress("The user name of your Facebook account could not be obtained.", false);
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
                OnMessageFacebookProgress("You are required to log in to Facebook.", false);
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
            if (OnMessageFacebookProgress != null)//Se produjo un error al tartar de compartir en Facebook.
                OnMessageFacebookProgress("There was an error while trying to share on Facebook.", false);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("No bueno");
            if (OnMessageFacebookProgress != null)//Se compartió exitosamente en Facebook.
                OnMessageFacebookProgress("It was successfully shared on Facebook.", true);
            if (OnPawsFacebookReward != null)
                OnPawsFacebookReward(50);
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
            if (OnMessageFacebookProgress != null)//Se requiere que se inicie sesión en Facebook.
                OnMessageFacebookProgress("You are required to log in to Facebook.", false);
        }
        
    }

    private void CallBackInviteFriendsFacebook(IAppRequestResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Invite cancel");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            if (OnMessageFacebookProgress != null)//Se genero un error al invitar a tus amigos en Facebook
                OnMessageFacebookProgress("An error occurred while inviting your friends on Facebook.", false);
        }
        else
        {
            //Debug.Log(result.RawResult);
            var responseObject = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as Dictionary<string, object>;
            /*if (responseObject.ContainsKey("request"))
            {
                string request = responseObject["request"].ToString();
                PlayerPrefs.SetString("invitekey", request);
                PlayerPrefs.Save();
            }
            else
            {
                numberInvite = 0;
            }*/

            if (responseObject.ContainsKey("to"))
            {
                string[] tokens = responseObject["to"].ToString().Split(',');
                if (tokens.Length >= 5)
                {
                    if (OnMessageFacebookProgress != null)//Acaba de invitar a más de 5 personas, es acreedor a una recompensa.
                        OnMessageFacebookProgress("Just invited more than 5 people, is entitled to a reward.", true);
                    if (OnPawsFacebookReward != null)
                        OnPawsFacebookReward(100);
                }
                else
                {
                    if (OnMessageFacebookProgress != null)//Es necesario invitar a 5 personas para recibir una recompensa.
                        OnMessageFacebookProgress("It is necessary to invite 5 people to receive a reward.", false);
                }
            }
            else
            {
                if (OnMessageFacebookProgress != null)//Se generó un error al invitar a tus amigos en Facebook
                    OnMessageFacebookProgress("An error occurred while inviting your friends on Facebook", false);
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

    public void LogOut()
    {
        FB.LogOut();
    }
    #endregion

    #region Interface implementation
    #endregion
}
