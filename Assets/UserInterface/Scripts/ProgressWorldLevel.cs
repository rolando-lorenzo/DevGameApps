using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressWorldLevel {

    public enum WorldsNames
    {
        Circus = 1,
        Train = 2,
        Zoo = 3,
        Manssion = 4
    }

    #region Class implementation
    public static WorldsNames GetWorldsEnum(string nameWorld)
    {
        return (WorldsNames) Enum.Parse(typeof(WorldsNames), nameWorld);
    }

    public static WorldsNames GetWorldsEnum(int nameWorld)
    {
        return (WorldsNames) nameWorld;
    }

    public static int GetLevelWorl(WorldsNames nameWorld)
    {
        char[] split = { '/', '-' };
        string[] progresslevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel, GetLevelWorlBase()).Split(split);

        int maxlevel = 0;

        for (int i = 0; i < progresslevels.Length; i++)
        {
            if (progresslevels[i] == nameWorld.ToString())
            {
                maxlevel = Int32.Parse(progresslevels[i + 1]);
                return maxlevel;
            }
        }

        return 0;
    }

    public static string GetLevelWorlBase()
    {
        return "Circus-1/Train-0/Zoo-0/Mansion-0";
    }

    public static void CompleteLevel(string nameWorld, int levelOfWorl)
    {
        CompleteWorldsLevel(GetWorldsEnum(nameWorld).ToString(), levelOfWorl);
    }

    public static void CompleteLevel(int nameWorld, int levelOfWorl)
    {
        CompleteWorldsLevel(GetWorldsEnum(nameWorld).ToString(), levelOfWorl);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameWorld">Name of the world where the player is.</param>
    /// <param name="levelOfWorl">the level that the gamer to be playing in this moment</param>
    private static void CompleteWorldsLevel(string nameWorld, int levelOfWorl)
    {
        char[] split = { '/', '-' };
        string[] progresslevels = GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel).Split(split);

        int maxlevel = 0;

        for (int i = 0; i < progresslevels.Length; i++)
        {

            Debug.Log(nameWorld);
            if (progresslevels[i] == nameWorld)
            {
                Debug.Log("here");
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
                    int baselevel = Int32.Parse(progresslevels[i + 3]);
                    if (baselevel == 0)
                    {
                        progresslevels[i + 3] = "1";
                    }
                }

            }
            //Debug.Log(progresslevels[i]);
        }

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
        Debug.Log(cadena);
        GameItemsManager.SetValueStringById(GameItemsManager.Item.GameProgressLevel, cadena);
    }
    #endregion
}