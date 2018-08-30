using System.Collections.Generic;

public static class Localization
{
    static Localization()
    {
        // This constructor automatically runs at program start
        localizationDatabase = new Dictionary<string, string>();
        LoadLanguages();
    }

    private static Dictionary<string, string> localizationDatabase;

    static string currentLanguage = "EN";

    public static string CurrentLanguage
    {
        get
        {
            return currentLanguage;
        }

        set
        {
            currentLanguage = value;
        }
    }

    private static void LoadLanguages()
    {
        // TODO: read a directory of language files?

        LoadLanguage("EN");
    }

    static void LoadLanguage(string lang)
    {
        // probably read a file (XML? JSON? other?) with all the strings from a language

        localizationDatabase = new Dictionary<string, string>
        {
            { "menutype_menu", "Main Menu" },
            { "newgame", "New Game" },
            { "loadgame", "Load Game" },
            { "savegame", "Save Game" },
            { "load", "Load" },
            { "save", "Save" },
            { "cancel", "Cancel" },
            { "quit", "Quit" },
            { "build_mode_waypoint", "Waypoint placing mode" },
            { "build_mode_road", "Road placing mode" }
        };
        localizationDatabase["UI_ALERT_TEST"] = "This is a test alert.";
        localizationDatabase["ALERT_CITY_CAN_BOMBARD"] = "City can bombard an enemy!";

    }

    public static string GetString(string textId)
    {
        try
        {
            return localizationDatabase[textId];
        }
        catch (System.Exception)
        {
            UnityEngine.Debug.LogError("textid \"" + textId + "\" not found");
        }
        return "";
    }
}
