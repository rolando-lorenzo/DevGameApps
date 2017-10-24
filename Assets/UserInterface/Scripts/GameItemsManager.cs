using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class PlayerPrefs Manager of MAGNET, PAWPRINTS, PORKCHOP.
/// </summary>
public class GameItemsManager  {

	#region Class members
	public enum Item
	{
		numMagnets,
		numPawprints,
		numPorkchop,
		numMultipliers,
		numYinYangs,

		//Para personajes 
		//1 = BLOQUEADO
		//2 = DESBLOQUEADO
		personajeJaguarNegro,
		personajeTigreBlanco,
		personajeJaguarMexicano,
		personajeLeon,
		personajeTigreBengala,
		personajePantera,
		personajeTigreSable,
		personajePuma,
		personajeGatoMontes,

        //Variables of the Scenes of Interface
        //public string gameMusic = "GameMusic";
        worldName,
        levelWorld,
        gameProgressLevel
    }
	#endregion

	#region Class implementation

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