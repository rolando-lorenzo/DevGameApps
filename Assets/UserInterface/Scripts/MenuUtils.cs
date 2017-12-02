using System;
using UnityEngine;
using System.Collections;

public class MenuUtils  {

    internal static int newLevel { get; set; }

    #region Class implementation

    /// <summary>
    /// Formats the price products.
    /// </summary>
    /// <returns>The price products.</returns>
    /// <param name="val">Value.</param>
    public static string FormatPriceProducts (float val){
        return "$ " + val;
	}

	/// <summary>
	/// Formats the pawprints products.
	/// </summary>
	/// <returns>The pawprints products.</returns>
	/// <param name="val">Value.</param>
	public static string FormatPawprintsProducts (float val){
		return  val + " Huellas";
	}


   

    /// <summary>
    /// Builds the leanguage manager traslation.
    /// </summary>
    /// <returns>The leanguage manager traslation.</returns>
    public static LanguagesManager BuildLeanguageManagerTraslation()
    {

        string deviceLanguage = Application.systemLanguage.ToString();

        if (LanguagesManager.Instance.VerifyLanguage(deviceLanguage))
        {
            Language languageEnum = LanguagesManager.Instance.GetLanguageEnum(deviceLanguage);
            LanguagesManager.Instance.SetLanguage(languageEnum);
        }
        else
        {
            LanguagesManager.Instance.SetLanguage(Language.Default);
        }
        return LanguagesManager.Instance;
    }

    public static IEnumerator MostrarRate()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("aqui");
        if(ManagerRate.instance != null)
            ManagerRate.instance.ShowAPIRaterRandom();
    }

    public static void CanvasSortingOrderShow()
    {
        Canvas main = GameObject.FindObjectOfType<Canvas>();
        main.sortingOrder = 2;
    }

    public static void CanvasSortingOrderHiden()
    {
        Canvas main = GameObject.FindObjectOfType<Canvas>();
        main.sortingOrder = 0;
    }

    #endregion


}