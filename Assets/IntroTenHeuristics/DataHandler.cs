using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataHandler
{
    public static readonly string RootDir;
    public static readonly string UserDataDir;

    static DataHandler()
    {
        RootDir = Application.dataPath + "/gameData";
        UserDataDir = RootDir + "/userData";

        if (!Directory.Exists(RootDir))
            Directory.CreateDirectory(RootDir);

        if (!Directory.Exists(UserDataDir))
            Directory.CreateDirectory(UserDataDir);
    }

    public static string LoadData(string dir, string fileName)
    {
        string completePath = dir + "/" + fileName;

        if (!File.Exists(completePath)) return "";

        return File.ReadAllText(completePath);
    }

    public static bool SaveData(string dir, string fileName, string data, bool overwrite = true)
    {
        string completePath = dir + "/" + fileName;

        if (!overwrite && File.Exists(completePath)) return false;

        File.WriteAllText(completePath, data);

        return true;
    }

    public static bool FileExists(string dir, string fileName)
    {
        return File.Exists(dir + "/" + fileName);
    }
}
