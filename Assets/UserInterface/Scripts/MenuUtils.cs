using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUtils  {

    internal static int newLevel { get; set; }

    #region Class implementation

    /// <summary>
    /// Formats the price products.
    /// </summary>
    /// <returns>The price products.</returns>
    /// <param name="val">Value.</param>
    public static string FormatPriceProducts (float val){
		return "$"+ val + " USD";
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
    /// 
    /// </summary>
    /// <param name="nameWorld">Name of the world where the player is.</param>
    /// <param name="levelOfWorl">the level that the gamer to be playing in this moment</param>
    public static void CompleteLevel(string nameWorld, int levelOfWorl)
    {
        char[] split = { '/', '-' };
        string[] progresslevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.gameProgressLevel).Split(split);

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
        GameItemsManager.SetValueStringById(GameItemsManager.Item.gameProgressLevel, cadena);
    }



    #endregion


}