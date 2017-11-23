﻿using System;
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

    public static int GetLevelWorl(string nameWorld)
    {
        char[] split = { '/', '-' };
        string[] progresslevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel, "Circus-1/Train-0/Zoo-0/Mansion-0").Split(split);

        int maxlevel = 0;

        for (int i = 0; i < progresslevels.Length; i++)
        {

            //Debug.Log(levelname);
            if (progresslevels[i] == nameWorld)
            {
                maxlevel = Int32.Parse(progresslevels[i + 1]);
                return maxlevel;
            }
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameWorld">Name of the world where the player is.</param>
    /// <param name="levelOfWorl">the level that the gamer to be playing in this moment</param>
    public static void CompleteLevel(string nameWorld, int levelOfWorl)
    {
        char[] split = { '/', '-' };
        string[] progresslevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel).Split(split);

        int maxlevel = 0;

        for (int i = 0; i < progresslevels.Length; i++)
        {

            //Debug.Log(levelname);
            if (progresslevels[i] == nameWorld)
            {
                maxlevel = Int32.Parse(progresslevels[i + 1]);
                if (levelOfWorl + 1 <= 5)
                {
                    int newlevel = levelOfWorl + 1;
                    if (newlevel >= maxlevel)
                    {
                        progresslevels[i + 1] = "" + newlevel;
                    }
                }
                else
                {
                    progresslevels[i + 1] = "5";
                }

            }
            //Debug.Log(progresslevels[i]);
        }

        newLevel= levelOfWorl + 1;
        CreateAndSaveLevel(progresslevels);
    }

    private static void CreateAndSaveLevel(string[] progresslevels)
    {
        string cadena = "";
        for (int i = 0; i < progresslevels.Length; i += 2)
        {
            if ((i + 1) < progresslevels.Length)
            {
                cadena += progresslevels[i] + "-" + progresslevels[i + 1] + "/";
            }

        }
        GameItemsManager.SetValueStringById(GameItemsManager.Item.GameProgressLevel, cadena);
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

    #endregion


}