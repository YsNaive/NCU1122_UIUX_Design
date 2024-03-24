using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public string Base64Icon = "";
    public string Name = "";
    public string Major = "";
    public string Career = "";
    public string Gender = "";
    public string PhoneNumber = "";
    public string ResearchTopic = "";
    public string Contact = "";
    public string Skills = "";
    public string Hobbies = "";
    public string GraduatedSchool = "";
    public string SpecialExperience = "";

    public UserData() {}

    public UserData(UserData data)
    {
        Base64Icon = data.Base64Icon;
        Name = data.Name;
        Major = data.Major;
        Career = data.Career;
        Gender = data.Gender;
        PhoneNumber = data.PhoneNumber;
        ResearchTopic = data.ResearchTopic;
        Contact = data.Contact;
        Skills = data.Skills;
        Hobbies = data.Hobbies;
        GraduatedSchool = data.GraduatedSchool;
        SpecialExperience = data.SpecialExperience;
    }

    public override bool Equals(object obj)
    {
        return (obj is UserData data) && 
            data.Base64Icon == Base64Icon &&
            data.Name == Name && 
            data.Major == Major && 
            data.Career == Career && 
            data.Gender == Gender &&
            data.PhoneNumber == PhoneNumber &&
            data.ResearchTopic == ResearchTopic &&
            data.Contact == Contact &&
            data.Skills == Skills &&
            data.Hobbies == Hobbies &&
            data.GraduatedSchool == GraduatedSchool &&
            data.SpecialExperience == SpecialExperience;
    }

    public override int GetHashCode()
    {
        int hash = 17;

        hash = hash * 23 + Base64Icon.GetHashCode();
        hash = hash * 23 + Name.GetHashCode();
        hash = hash * 23 + Major.GetHashCode();
        hash = hash * 23 + Career.GetHashCode();
        hash = hash * 23 + Gender.GetHashCode();
        hash = hash * 23 + PhoneNumber.GetHashCode();
        hash = hash * 23 + ResearchTopic.GetHashCode();
        hash = hash * 23 + Contact.GetHashCode();
        hash = hash * 23 + Skills.GetHashCode();
        hash = hash * 23 + Hobbies.GetHashCode();
        hash = hash * 23 + GraduatedSchool.GetHashCode();
        hash = hash * 23 + SpecialExperience.GetHashCode();


        return hash;
    }

    public IEnumerable<string> GetAllValue()
    {
        yield return Name;
        yield return Major;
        yield return Career;
        yield return Gender;
        yield return PhoneNumber;
        yield return ResearchTopic;
        yield return Contact;
        yield return Skills;
        yield return Hobbies;
        yield return GraduatedSchool;
        yield return SpecialExperience;
    }
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
        Datas.Clear();
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
