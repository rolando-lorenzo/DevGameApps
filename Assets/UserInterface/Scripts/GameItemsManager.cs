using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class PlayerPrefs Manager of MAGNET, PAWPRINTS, PORKCHOP.
/// </summary>
public class GameItemsManager  {

    public static int CHARACTER_LOCKED = 1;
    public static int CHARACTER_UNLOCKED = 2;

	#region Class members

    /// <summary>
    /// Numeber items availables on game.
    /// </summary>
	public enum Item
	{
		NumMagnets,
		NumPawprints,
		NumPorkchop,
		NumMultipliers,
		NumYinYangs,

        //Variables of the Scenes of Interface
        //public string gameMusic = "GameMusic";
        WorldName,
        LevelWorld,
        GameProgressLevel,

        //Variables of the Dialog Rate
        postPoneTime,
        rateState,
        dateTimePostponeExecution,
        globalCountRate

    }

    /// <summary>
    /// Characters availables in game.
    /// </summary>
    public enum Character{
        //Para personajes 
        //1 = BLOQUEADO
        //2 = DESBLOQUEADO
        personaje_enzo,
        personaje_love,
        personaje_ali,
        personaje_achilles,
        personaje_matzo
    }

    /// <summary>
    /// Store products availables.
    /// </summary>
    public enum StoreProduct{
        Package1,
        Package2,
        Package3,
        Package4,
        PorkChopPowerUp,
        MagnetPowerUp,
        PawPrintMultiplierPowerUp,
        PorkChopUpgrade,
        MagnetUpgrade,
        PawPrintMultiplierUpgrade,
    }


	#endregion

	#region Class implementation
    /// <summary>
    /// Gets the power up value.
    /// </summary>
    /// <returns>The power up value.</returns>
    /// <param name="pu">Pu.</param>
    public static int GetPowerUpValue(StoreProduct pu){
        return PlayerPrefs.GetInt(pu.ToString(),0);
    }

    /// <summary>
    /// Sets the power up value.
    /// </summary>
    /// <param name="pu">Pu.</param>
    /// <param name="value">Value.</param>
    public static void SetPowerUpValue(StoreProduct pu, int value){
        PlayerPrefs.SetInt(pu.ToString(), value + (PlayerPrefs.GetInt(pu.ToString(), 0)));
    }


    /// <summary>
    /// Gets the Upgrade value.
    /// </summary>
    /// <returns>The power up value.</returns>
    /// <param name="pu">Pu.</param>
    public static int GetUpgradeValue(StoreProduct pu)
    {
        return PlayerPrefs.GetInt(pu.ToString(), 0);
    }

    /// <summary>
    /// Sets the Upgrade value.
    /// </summary>
    /// <param name="pu">Pu.</param>
    /// <param name="value">Value.</param>
    public static void SetUpgradeValue(StoreProduct pu, int value)
    {
        PlayerPrefs.SetInt(pu.ToString(), value + (PlayerPrefs.GetInt(pu.ToString(), 0)));
    }
  

    /// <summary>
    /// Sets the lock character.
    /// </summary>
    /// <param name="ch">Ch.</param>
    public static void SetLockCharacter(Character ch){
        PlayerPrefs.SetInt(ch.ToString(), CHARACTER_LOCKED);
    }

    /// <summary>
    /// Sets the unlock character.
    /// </summary>
    /// <param name="ch">Ch.</param>
    public static void SetUnlockCharacter(Character ch){
        PlayerPrefs.SetInt(ch.ToString(), CHARACTER_UNLOCKED);
    }

    /// <summary>
    /// Checks status character.
    /// </summary>
    /// <returns><c>true</c>, if locked character, <c>false</c> otherwise.</returns>
    /// <param name="ch">Ch.</param>
    public static bool isLockedCharacter(Character ch){
        int currStatus = PlayerPrefs.GetInt(ch.ToString(), 0);
        if (currStatus == CHARACTER_LOCKED || currStatus == 0)
        {
            return true;
        }
        else return false;
    }

	/// <summary>
	/// Saves the player prefs by identifier.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public static void SetValueById(Item id, int value){
		PlayerPrefs.SetInt (id.ToString(), value);
	}

	/// <summary>
	/// Gets the player prefs by identifier.
	/// </summary>
	/// <returns>The play prefs by identifier.</returns>
	/// <param name="id">Identifier.</param>
	public static int GetValueById(Item id){
		return PlayerPrefs.GetInt (id.ToString(),0);
	}
    /// <summary>
    /// Adds the value by identifier to PlayerPrefs.
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="val">Value.</param>
    public static void addValueById(Item id, int val){
		SetValueById (id, (GetValueById (id)+val));
	}

	/// <summary>
	/// Subtracts the value by identifier to PlayerPrefs.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="val">Value.</param>
	public static void subtractValueById(Item id, int val){
		SetValueById (id, (GetValueById (id)-val));
	}

    /// <summary>
    /// Set tha value of the PlayerPrefs type String
    /// </summary>
    /// <param name="id">Item Id identifier</param>
    /// <param name="value">Value</param>
    public static void SetValueStringById(Item id, string value)
    {
        PlayerPrefs.SetString(id.ToString(), value);
    }

    /// <summary>
    /// Get tha value of the PlayerPrefs type String
    /// </summary>
    /// <param name="id">Item Id identifier</param>
    /// <param name="value">Value Default</param>
    public static string GetValueStringById(Item id, string val)
    {
        return PlayerPrefs.GetString(id.ToString(), val);
    }

    public static string GetValueStringById(Item id)
    {
        return PlayerPrefs.GetString(id.ToString());
    }
    #endregion


}