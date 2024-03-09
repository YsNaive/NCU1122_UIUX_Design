using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public string Name = "";
    public string PhoneNumber = "";
    public string Degree = "";
    public string Career = "";
    public string Gender = "";
    public string ResearchTopic = "";
    public string Contact = "";
    public string Skills = "";
    public string Habits = "";
    public string GraduatedSchool = "";
}

public class UserDataHandler
{
    public static List<UserData> Datas = new List<UserData>();

    static UserDataHandler()
    {
        LoadAll();
    }

    public static void LoadAll()
    {
        foreach (string path in Directory.GetFiles(DataHandler.UserDataDir, "*.json"))
        {
            Datas.Add(JsonUtility.FromJson<UserData>(File.ReadAllText(path)));
        }
    }

    public static UserData FindByName(string name)
    {
        return Datas.Find((d) => d.Name == name);
    }
}
