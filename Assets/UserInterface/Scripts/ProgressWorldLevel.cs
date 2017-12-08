using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressWorldLevel {

    public delegate void EventProgressCompleteWorld(string infoMessage, bool isUnlock);
    public static event EventProgressCompleteWorld OnProgressCompleteWorld;
    GameItemsManager.GameMode gameMode;

    public enum WorldsNames
    {
        Circus = 1,
        Train = 2,
        Zoo = 3,
        Mansion = 4
    }

    #region Class implementation
    /*public static GameItemsManager.GameMode GetGameModeEnum(string gamemode)
    {
        return (GameItemsManager.GameMode)Enum.Parse(typeof(WorldsNames), gamemode);
    }

    public static void SetGameMode(GameItemsManager.GameMode gameMode)
    {
        //GameItemsManager.SetValueStringById(GameItemsManager.Item.GameMode, gameMode.ToString());
    }

    public static GameItemsManager.GameMode GetGameMode()
    {
        //return GetGameModeEnum(GameItemsManager.GetValueStringById(GameItemsManager.Item.GameMode, "RELEASE"));
    }*/

    public static WorldsNames GetWorldsEnum(string nameWorld)
    {
        return (WorldsNames)Enum.Parse(typeof(WorldsNames), nameWorld);
    }

    public static WorldsNames GetWorldsEnum(int nameWorld)
    {
        return (WorldsNames)nameWorld;
    }

    public static int GetLevelWorl(WorldsNames nameWorld)
    {
        char[] split = { '/', '-' };

        string[] progresslevels = ProgressString().Split(split);

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

    public static string ProgressString()
    {
        return GameItemsManager.GetValueStringById(GameItemsManager.Item.GameProgressLevel, GetLevelWorlBase());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameWorld">Name of the world where the player is.</param>
    /// <param name="levelOfWorl">the level that the gamer to be playing in this moment</param>
    private static void CompleteWorldsLevel(string nameWorld, int levelOfWorl)
    {
        bool state = false;
        string message = "";
        char[] split = { '/', '-' };
        Debug.Log(ProgressString());
        string[] progresslevels = ProgressString().Split(split);
        LanguagesManager lm = MenuUtils.BuildLeanguageManagerTraslation();

        int maxlevel = 0;

        for (int i = 0; i < progresslevels.Length; i++)
        {

            Debug.Log(nameWorld);
            if (progresslevels[i] == nameWorld)
            {
                maxlevel = Int32.Parse(progresslevels[i + 1]);
                if (levelOfWorl + 1 <= 5)
                {
                    int newlevel = levelOfWorl + 1;
                    //Debug.Log(newlevel + " - " + levelOfWorl + "-" + nameWorld);
                    if (newlevel > maxlevel)
                    {
                        progresslevels[i + 1] = "" + newlevel;
                        state = true;
                        message = lm.GetString("progress_info_success_newlevel");
                        break;
                    }else if (newlevel == maxlevel)
                    {
                        state = false;
                        message = lm.GetString("progress_info_error_newlevel");
                        break;
                    }
                    else
                    {
                        state = false;
                        message = lm.GetString("progress_info_error_newlevel");
                        break;
                    }
                }
                else
                {
                    progresslevels[i + 1] = "5";
                    int limitAux = i + 3;
                    if (limitAux >= progresslevels.Length)
                    {
                        state = false;
                        message = "Limit Array";
                        break;
                    }
                    else
                    {
                        int baselevel = Int32.Parse(progresslevels[i + 3]);
                        if (baselevel == 0)
                        {
                            int yiyangs = GameItemsManager.GetValueById(GameItemsManager.Item.NumYinYangs);
                            if (nameWorld == WorldsNames.Circus.ToString() && yiyangs >= 2)//unlocked Train
                            {
                                progresslevels[i + 3] = "1";
                                state = true;
                                message = string.Format(lm.GetString("progress_info_success_newworld"), WorldsNames.Train.ToString());
                            }
                            else if (nameWorld == WorldsNames.Train.ToString() && yiyangs >= 5)//unlocked Zoo
                            {
                                progresslevels[i + 3] = "1";
                                state = true;
                                message = string.Format(lm.GetString("progress_info_success_newworld"), WorldsNames.Zoo.ToString());
                            }
                            else if (nameWorld == WorldsNames.Zoo.ToString() && yiyangs >= 8)//unlocked Mansion
                            {
                                progresslevels[i + 3] = "1";
                                state = true;
                                message = string.Format(lm.GetString("progress_info_success_newworld"), WorldsNames.Mansion.ToString());
                            }
                            else
                            {
                                Debug.Log(yiyangs);
                                int restYangs = 0;
                                if (nameWorld == WorldsNames.Circus.ToString()) //Train will require yinyangs
                                {
                                    restYangs = 2 - yiyangs;

                                }
                                else if (nameWorld == WorldsNames.Train.ToString())//Zoo will require yinyangs
                                {
                                    restYangs = 5 - yiyangs;
                                }
                                else if (nameWorld == WorldsNames.Zoo.ToString())//Mansion will require yinyangs
                                {
                                    restYangs = 8 - yiyangs;
                                }
                                else
                                {
                                    Debug.Log("I have to error in progress file");
                                }

                                // textDilogAux = "Para entrar al mundo {0} necesita {1} Yin Yang";
                                message = string.Format(lm.GetString("progress_info_error_yinyangs"), restYangs.ToString());
                                state = false;
                                break;
                            }

                            break;
                        }
                        else
                        {
                            state = false;
                            message = lm.GetString("progress_info_error_nextworld");
                            break;
                        }
                    }
                }

            }
            else
            {
                state = false;
                message = lm.GetString("progress_info_error_world");
            }
            //Debug.Log(progresslevels[i]);
        }

        CreateAndSaveLevel(progresslevels);

        if (OnProgressCompleteWorld != null)
        {
            Debug.Log("Into Event");
            OnProgressCompleteWorld(message, state);
        }
        else
        {
            Debug.LogError("Error");
            Debug.Log(OnProgressCompleteWorld);
        }
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