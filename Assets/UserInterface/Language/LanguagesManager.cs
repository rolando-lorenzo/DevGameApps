﻿using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System;

[XmlRoot("LanguagesManager")]
public class LanguagesManager{
    private static string globalfilePath = Path.Combine(Application.dataPath, "Resources/Xml/Language.xml");
    
    // Current language - set at the begining of the game(1st time)
    [XmlEnum("CurrentLanguage")]
    public Language CurrentLanguage;

    // List of Translations
    [XmlArray("Translations"), XmlArrayItem("Translation")]
    public List<Translation> Translations;

    public LanguagesManager()
    {
        CurrentLanguage = Language.English;
        Translations = new List<Translation>();
    }

    public void SetLanguage(Language language)
    {
        CurrentLanguage = language;
    }

    public bool VerifyLanguage(string language)
    {
        return Enum.IsDefined(typeof(Language), language);
    }

    public Language GetLanguageEnum(String language)
    {
        if (language.Equals(Language.English.ToString()))
        {
            return Language.English;
        }else if (language.Equals(Language.Spanish.ToString()))
        {
            return Language.Spanish;
        }
        else if (language.Equals(Language.Dutch.ToString()))
        {
            return Language.Dutch;
        }
        else if (language.Equals(Language.French.ToString()))
        {
            return Language.French;
        }
        else if (language.Equals(Language.Italian.ToString()))
        {
            return Language.Italian;
        }
        else if (language.Equals(Language.Mandarin.ToString()))
        {
            return Language.Mandarin;
        }
        else if (language.Equals(Language.German.ToString()))
        {
            return Language.German;
        }

        return Language.Default;
    }

    public bool ContainsKey(string key)
    {
        return FindEntry(key) >= 0;
    }

    public void Add(Translation translation)
    {
        Insert(translation, true);
    }

    public void Update(Translation translation)
    {
        Insert(translation, false);
    }

    public void Remove(Translation translation)
    {
        Translations.Remove(translation);
    }

    public void Remove(string key)
    {
        int positionEntry = FindEntry(key);
        if (positionEntry >= 0)
            //Translations.RemoveAt(positionEntry);
            Remove(positionEntry);
    }

    public void Remove(int index)
    {
        Translations.RemoveAt(index);
    }

    public string GetString(string key)
    {
        Debug.Log(key);
        int positionEntry = FindEntry(key);
        if (positionEntry >= 0)
            return Translations[positionEntry].GetValue(CurrentLanguage);
        return string.Empty;
    }

    public string GetString(string key, Language language)
    {
        int positionEntry = FindEntry(key);
        if (positionEntry >= 0)
            return Translations[positionEntry].GetValue(language);
        return string.Empty;
    }

    private void Insert(Translation translation, bool add)
    {
        int positionEntry = FindEntry(translation.key);
        if (positionEntry >= 0)
        {
            if (add)
                return;

            Translations[positionEntry] = translation;
        }

        if(add){
            Translations.Add(translation);
        }
    }

    private int FindEntry(string key)
    {
        for(int i = 0; i< Translations.Count; i++)
        {
            if (Translations[i].key.Equals(key))
                return i;
        }

        return -1;
    }

    public void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(LanguagesManager));
        using (FileStream stream = new FileStream(globalfilePath, FileMode.Create))
        {
            Debug.Log(globalfilePath);
            serializer.Serialize(stream, this);
        }
    }

    public static LanguagesManager Load()
    {
        //Debug.Log(File.Exists(globalfilePath));
        if (File.Exists(globalfilePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LanguagesManager));
            using (FileStream stream = new FileStream(globalfilePath, FileMode.Open))
            {
                return serializer.Deserialize (stream) as LanguagesManager;
            }
        }

        return new LanguagesManager();
    }
}

public enum Language
{
    Default,
    English,
    Spanish,
    Dutch,
    French,
    Italian,
    Mandarin,
    German
}
