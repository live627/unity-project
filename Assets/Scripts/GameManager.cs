using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameManager
{
    //Create Keycodes that will be associated with each of our commands.
    //These can be accessed by any other script in our game
    public KeyCode Pause { get; set; }
    public KeyCode Up { get; set; }
    public KeyCode Down { get; set; }
    public KeyCode Left { get; set; }
    public KeyCode Right { get; set; }

    public GameManager()
    {
        Dictionary<string, string> settings = ParseJSON(DataHandler.LoadData("Settings.json", false));
        if (settings != null)
        {
            foreach (KeyValuePair<string, string> setting in settings)
            {
        Debug.Log(setting.Key+" "+setting.Value);
                DataHandler.Settings[setting.Key] = setting.Value;
            }
        }

        /*
         * Assign each keycode when the game starts.
         * Loads data from PlayerPrefs so if a user quits the game,
         * their bindings are loaded next time. 
         */
        Pause = (KeyCode)Enum.Parse(typeof(KeyCode), DataHandler.Settings["input.pause"]);
        Up = (KeyCode)Enum.Parse(typeof(KeyCode), DataHandler.Settings["input.moveMapUp"]);
        Down = (KeyCode)Enum.Parse(typeof(KeyCode), DataHandler.Settings["input.moveMapDown"]);
        Left = (KeyCode)Enum.Parse(typeof(KeyCode), DataHandler.Settings["input.moveMapLeft"]);
        Right = (KeyCode)Enum.Parse(typeof(KeyCode), DataHandler.Settings["input.moveMapRight"]);
    }

    public void SaveSettings()
    {
        // Parse the settings down to strings.
        DataHandler.Settings["input.moveMapUp"] = Up.ToString();
        DataHandler.Settings["input.moveMapDown"] = Down.ToString();
        DataHandler.Settings["input.moveMapLeft"] = Left.ToString();
        DataHandler.Settings["input.moveMapRight"] = Right.ToString();
        DataHandler.Settings["input.pause"] = Pause.ToString();

        // Write them to the file.
        DataHandler.SaveData(MyDictionaryToJson(DataHandler.Settings), "Settings.json", false);
    }

    string MyDictionaryToJson(Dictionary<string, string> dict)
    {
        return "{\r\n    " + string.Join(",\r\n    ", dict.Select(d =>
            string.Format("\"{0}\": \"{1}\"", d.Key, d.Value)).ToArray()) + "\r\n}";
    }

    public Dictionary<string, string> ParseJSON(string json)
    {
        int end;
        return ParseJSON(json, 0, out end);
    }

    private Dictionary<string, string> ParseJSON(string json, int start, out int end)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        bool escbegin = false;
        bool escend = false;
        bool inquotes = false;
        string key = null;
        StringBuilder sb = new StringBuilder();
        List<object> arraylist = null;
        Regex regex = new Regex(@"\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);

        foreach (char c in json.ToCharArray())
        {
            if (c == '\\')
            {
                escbegin = !escbegin;
            }

            if (!escbegin)
            {
                if (c == '"')
                {
                    inquotes = !inquotes;
                    if (!inquotes && arraylist != null)
                    {
                        arraylist.Add(DecodeString(regex, sb.ToString()));
                        sb.Length = 0;
                    }
                    continue;
                }
                if (!inquotes)
                {
                    switch (c)
                    {
                        case '{':
                        case '}':
                        case '[':
                        case ']':
                            continue;
                        case ',':
                            if (arraylist == null && key != null)
                            {
                                dict.Add(key, DecodeString(regex, sb.ToString()));
                                key = null;
                                sb.Length = 0;
                            }
                            if (arraylist != null && sb.Length > 0)
                            {
                                arraylist.Add(sb.ToString());
                                sb.Length = 0;
                            }
                            continue;
                        case ':':
                            key = DecodeString(regex, sb.ToString().Trim());
                            sb.Length = 0;
                            continue;
                    }
                }
            }
            sb.Append(c);
            if (escend)
            {
                escbegin = false;
            }

            if (escbegin)
            {
                escend = true;
            }
            else
            {
                escend = false;
            }
        }
        end = json.Length - 1;
        return dict; //theoretically shouldn't ever get here
    }

    private string DecodeString(Regex regex, string str)
    {
        return Regex.Unescape(regex.Replace(str, match => char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber))));
    }
}
