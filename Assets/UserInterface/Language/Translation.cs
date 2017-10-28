using System.Xml.Serialization;

public class Translation {

    [XmlAttribute("key")]
    public string key;

    [XmlElement("Default")]
    public string Default;

    [XmlElement("English")]
    public string English;

    [XmlElement("Spanish")]
    public string Spanish;

    [XmlElement("Dutch")]
    public string Dutch;

    [XmlElement("French")]
    public string French;

    [XmlElement("Italian")]
    public string Italian;

    [XmlElement("Mandarin")]
    public string Mandarin;

    [XmlElement("German")]
    public string German;

    public Translation()
    {
        key = "New Key";
        Default = string.Empty;
        English = string.Empty;
        Spanish = string.Empty;
        Dutch = string.Empty;
        French = string.Empty;
        Italian = string.Empty;
        Mandarin = string.Empty;
        German = string.Empty;
    }

    public string GetValue(Language language)
    {
        switch (language)
        {
            case Language.Default:
                return Default;
            case Language.English:
                return English;
            case Language.Spanish:
                return Spanish;
            case Language.Dutch:
                return Dutch;
            case Language.French:
                return French;
            case Language.Italian:
                return Italian;
            case Language.Mandarin:
                return Mandarin;
            case Language.German:
                return German;
            default:
                return string.Empty;
        }
    }
}
