using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public string Base64Icon = "";
    public string Name = "";
    public string PhoneNumber = "";
    public string Career = "";
    public string Gender = "";
    public string ResearchTopic = "";
    public string Contact = "";
    public string Skills = "";
    public string Hobbies = "";
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

    public static UserData FindPrevData(UserData userData)
    {
        int index = Datas.IndexOf(userData);

        if (index == -1) return null;

        return Datas[index == 0 ? Datas.Count - 1 : index - 1];
    }

    public static UserData FindNextData(UserData userData)
    {
        int index = Datas.IndexOf(userData);

        if (index == -1) return null;

        return Datas[index == Datas.Count - 1 ? 0 : index + 1];
    }
}
