using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class DataHandler
{
    public static Dictionary<string, string> Settings = new Dictionary<string, string>
    {
        { "input.pause", "P" },
        { "input.camera", "L" },
        { "input.moveMapUp", "UpArrow" },
        { "input.moveMapDown", "DownArrow" },
        { "input.moveMapLeft", "LeftArrow" },
        { "input.moveMapRight", "RightArrow" },
        { "screen.resolution", "default" },
        { "screen.full", "1" },
        { "map.mini", "1" },
        { "map.alternative", "1" },
        { "input.edgeScrolling", "1" },
        { "input.scrollSpeed", "20" },
        { "input.zoomSpeed", "20" },
        { "input.scrollBoundary", "1" },
        { "input.fovMin", "4" },
        { "input.fovMax", "95" }
    };

    public delegate void SimpleBinaryWriterEventHandler(BinaryWriter writer);
    public static event SimpleBinaryWriterEventHandler WriteBinary;
    public delegate void SimpleBinaryReaderEventHandler(BinaryReader reader);
    public static event SimpleBinaryReaderEventHandler ReadBinary;

    /// <summary>
    /// Writes the given object instance to a binary file.
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    /// <param name="filePath">The file path to write the object instance to.</param>
    private static void WriteToBinaryFile(string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate)))
        {
            if (WriteBinary != null)
            {
                WriteBinary(writer);
            }
        }
    }

    /// <summary>
    /// Reads an object instance from a binary file.
    /// </summary>
    /// <param name="filePath">The file path to read the object instance from.</param>
    private static void ReadFromBinaryFile(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            if (ReadBinary != null)
            {
                ReadBinary(reader);
            }
        }
    }

    //Save Data
    public static void SaveData(string dataToSave, string dataFileName, bool isBinary = true)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, dataFileName);

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }

        try
        {
            if (isBinary)
            {
                WriteToBinaryFile(tempPath);
            }
            else
            {
                File.WriteAllText(tempPath, dataToSave);
            }

            Debug.Log("Saved Data to: " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data Data to: " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    //Load Data
    public static IOrderedEnumerable<FileInfo> ListSaveDataFiles()
    {
        string tempPath = Path.Combine(Application.persistentDataPath, "GameSaves");

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Debug.LogWarning("Directory does not exist");
            return null;
        }

        return new DirectoryInfo(tempPath).GetFiles("*.dat").OrderByDescending(f => f.LastWriteTime);
    }

    //Load Data
    public static string LoadData(string dataFileName, bool isBinary = true)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, dataFileName);

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Debug.LogWarning("Directory does not exist");
            return null;
        }

        if (!File.Exists(tempPath))
        {
            Debug.Log("File does not exist");
            return null;
        }

        //Load saved Json
        string jsonData = null;
        try
        {
            if (isBinary)
            {
                ReadFromBinaryFile(tempPath);
            }
            else
            {
                jsonData = File.ReadAllText(tempPath);
            }

            Debug.Log("Loaded Data from: " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Load Data from: " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
        
        return jsonData;   
    }

    public static bool DeleteData(string dataFileName)
    {
        bool success = false;

        //Load Data
        string tempPath = Path.Combine(Application.persistentDataPath, dataFileName);

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Debug.LogWarning("Directory does not exist");
            return false;
        }

        if (!File.Exists(tempPath))
        {
            Debug.Log("File does not exist");
            return false;
        }

        try
        {
            File.Delete(tempPath);
            Debug.Log("Data deleted from: " + tempPath.Replace("/", "\\"));
            success = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Delete Data: " + e.Message);
        }

        return success;
    }
}