using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class PlayerPrefs Manager of MAGNET, PAWPRINTS, PORKCHOP.
/// </summary>
public class GameItemsManager {

	public static int CHARACTER_LOCKED = 1;
	public static int CHARACTER_UNLOCKED = 2;

	#region Class members

	/// <summary>
	/// Numeber items availables on game.
	/// </summary>
	public enum Item {
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
        GameMode,

        //Variables of the Dialog Rate
        postPoneTime,
        rateState,
        globalCountRate,
        TodayCount,
        TodayDate

	}

	/// <summary>
	/// Characters availables in game.
	/// </summary>
	public enum Character {
		//Para personajes 
		//1 = BLOQUEADO
		//2 = DESBLOQUEADO
        PersonajeCielo,
        PersonajeTierra,
        PersonajeEnzo,
        PersonajeAchilles,
        PersonajeDharma,
        PersonajeKarma,
        PersonajeHan,
        PersonajeLove,
        PersonajeAli,
        PersonajeMatzo,

	}

    public enum Wallpaper
    {
        //wallpaper
        //1 = lock
        //2 = unlock
        WallpaperCielo,
        WallpaperTierra,
        WallpaperEnzo,
        WallpaperAchilles,
        WallpaperDharma,
        WallpaperKarma,
        WallpaperHan,
        WallpaperLove,
        WallpaperAli,
        WallpaperMatzo

    }

    /// <summary>
    /// Store products availables.
    /// </summary>
    public enum StoreProduct {
		Package1,
		Package2,
		Package3,
		Package4
	}

	public enum StorePower {
		PorkChop,
		Magnet,
		PawPrintMultiplier
	}

    public enum GameMode {
        DEBUG,
        RELEASE
    }

   
	#endregion

	#region Class implementation
	public static int GetPowerCount (StorePower power) {
		return PlayerPrefs.GetInt ("PowerCount" + power.ToString (), 0);
	}

	public static void SetPowerCount (StorePower power, int value) {
		PlayerPrefs.SetInt ("PowerCount" + power.ToString (), value);
	}

    public static void AddPowerCount(StorePower power, int value)
    {
        PlayerPrefs.SetInt("PowerCount" + power.ToString(), (GetPowerUpgradeLevel(power) + value));
    }

	public static int GetPowerUpgradeLevel (StorePower power) {
		return PlayerPrefs.GetInt ("PowerUpgrade" + power.ToString (), 1);
	}

	public static void SetUpgradeValue (StorePower power, int value) {
		PlayerPrefs.SetInt ("PowerUpgrade" + power.ToString (), value);
	}

    public static void AddUpgradeValue(StorePower power, int value)
    {
        PlayerPrefs.SetInt("PowerUpgrade" + power.ToString(), (GetPowerUpgradeLevel(power)+value));
    }

	/// <summary>
	/// Sets the lock character.
	/// </summary>
	/// <param name="ch">Ch.</param>
	public static void SetLockCharacter (Character ch) {
		PlayerPrefs.SetInt (ch.ToString (), CHARACTER_LOCKED);
	}

	/// <summary>
	/// Sets the unlock character.
	/// </summary>
	/// <param name="ch">Ch.</param>
	public static void SetUnlockCharacter (Character ch) {
		PlayerPrefs.SetInt (ch.ToString (), CHARACTER_UNLOCKED);
	}

	/// <summary>
	/// Checks status character.
	/// </summary>
	/// <returns><c>true</c>, if locked character, <c>false</c> otherwise.</returns>
	/// <param name="ch">Ch.</param>
	public static bool isLockedCharacter (Character ch) {
		int currStatus = PlayerPrefs.GetInt (ch.ToString (), 0);
		if (currStatus == CHARACTER_LOCKED || currStatus == 0) {
			return true;
		}
		else return false;
	}

    /// <summary>
	/// Sets the lock Wallpaper.
	/// </summary>
	/// <param name="wall">wallpaper.</param>
	public static void SetLockWallpaper(Wallpaper wall)
    {
        PlayerPrefs.SetInt(wall.ToString(), CHARACTER_LOCKED);
    }

    /// <summary>
    /// Sets the unlock wallpaper.
    /// </summary>
    /// <param name="wall">Wallpaper.</param>
    public static void SetUnlockWallpaper(Wallpaper wall)
    {
        PlayerPrefs.SetInt(wall.ToString(), CHARACTER_UNLOCKED);
    }

    /// <summary>
    /// Checks status Wallpaper.
    /// </summary>
    /// <returns><c>true</c>, if locked Wallpaper, <c>false</c> otherwise.</returns>
    /// <param name="wall">wall.</param>
    public static bool isLockedWallpaper(Wallpaper wall)
    {
        int currStatus = PlayerPrefs.GetInt(wall.ToString(), 0);
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
    public static void SetValueById (Item id, int value) {
		PlayerPrefs.SetInt (id.ToString (), value);
	}

	/// <summary>
	/// Gets the player prefs by identifier.
	/// </summary>
	/// <returns>The play prefs by identifier.</returns>
	/// <param name="id">Identifier.</param>
	public static int GetValueById (Item id) {
		return PlayerPrefs.GetInt (id.ToString (), 0);
	}
	/// <summary>
	/// Adds the value by identifier to PlayerPrefs.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="val">Value.</param>
	public static void addValueById (Item id, int val) {
		SetValueById (id, (GetValueById (id) + val));
	}

	/// <summary>
	/// Subtracts the value by identifier to PlayerPrefs.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="val">Value.</param>
	public static void subtractValueById (Item id, int val) {
		SetValueById (id, (GetValueById (id) - val));
	}

	/// <summary>
	/// Set tha value of the PlayerPrefs type String
	/// </summary>
	/// <param name="id">Item Id identifier</param>
	/// <param name="value">Value</param>
	public static void SetValueStringById (Item id, string value) {
		PlayerPrefs.SetString (id.ToString (), value);
	}

	/// <summary>
	/// Get tha value of the PlayerPrefs type String
	/// </summary>
	/// <param name="id">Item Id identifier</param>
	/// <param name="value">Value Default</param>
	public static string GetValueStringById (Item id, string val) {
		return PlayerPrefs.GetString (id.ToString (), val);
	}

	public static string GetValueStringById (Item id) {
		return PlayerPrefs.GetString (id.ToString ());
	}

    public static void SetLastChooseCharacter(Character character){
        PlayerPrefs.SetString("LastChooseCharacter",character.ToString());
    }

    public static Character GetLastChooseCharacter()
    {
        Character en = (Character)Enum.Parse(typeof(Character), PlayerPrefs.GetString("LastChooseCharacter","PersonajeCielo"));
        return en;
    }
    #endregion


}