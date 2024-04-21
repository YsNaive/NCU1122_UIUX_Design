using SingularityGroup.HotReload;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class UserData
{
    public static readonly string[] MemberNames = new string[]
    {
        nameof(Base64Icon),
        nameof(Name),
        nameof(Major),
        nameof(Career),
        nameof(Gender),
        nameof(PhoneNumber),
        nameof(ResearchTopic),
        nameof(Contact),
        nameof(Skills),
        nameof(Hobbies),
        nameof(GraduatedSchool),
        nameof(SpecialExperience),
        nameof(FavoriteClasses),
    };
    public string[] stringValues = new string[13];
    public const int I_Base64Icon        = 0;
    public const int I_Name              = 1;
    public const int I_Major             = 2;
    public const int I_Career            = 3;
    public const int I_Gender            = 4;
    public const int I_PhoneNumber       = 5;
    public const int I_ResearchTopic     = 6;
    public const int I_Contact           = 7;
    public const int I_Skills            = 8;
    public const int I_Hobbies           = 9;
    public const int I_GraduatedSchool   = 10;
    public const int I_SpecialExperience = 11;
    public const int I_FavoriteClasses   = 12;
    #region getter setter
    public Texture2D IconTexture
    {
        get
        {
            if (m_IconTexture == null)
            {
                if (Base64Icon != "")
                {
                    m_IconTexture = new Texture2D(1, 1);
                    m_IconTexture.LoadImage(Convert.FromBase64String(Base64Icon));
                }
                m_IconTexture = Resources.Load<Texture2D>("Image/default_icon");
            }
            return m_IconTexture;
        }
    }
    Texture2D m_IconTexture = null;
    public string Base64Icon
    {
        get => stringValues[I_Base64Icon];
        set
        {
            m_IconTexture = null;
            stringValues[I_Base64Icon] = value;
        }
    }
    public string Name
    {
        get => stringValues[I_Name];
        set => stringValues[I_Name] = value;
    }
    public string Major
    {
        get => stringValues[I_Major];
        set => stringValues[I_Major] = value;
    }
    public string Career
    {
        get => stringValues[I_Career];
        set => stringValues[I_Career] = value;
    }
    public string Gender
    {
        get => stringValues[I_Gender];
        set => stringValues[I_Gender] = value;
    }
    public string PhoneNumber
    {
        get => stringValues[I_PhoneNumber];
        set => stringValues[I_PhoneNumber] = value;
    }
    public string ResearchTopic
    {
        get => stringValues[I_ResearchTopic];
        set => stringValues[I_ResearchTopic] = value;
    }
    public string Contact
    {
        get => stringValues[I_Contact];
        set => stringValues[I_Contact] = value;
    }
    public string Skills
    {
        get => stringValues[I_Skills];
        set => stringValues[I_Skills] = value;
    }
    public string Hobbies
    {
        get => stringValues[I_Hobbies];
        set => stringValues[I_Hobbies] = value;
    }
    public string GraduatedSchool
    {
        get => stringValues[I_GraduatedSchool];
        set => stringValues[I_GraduatedSchool] = value;
    }
    public string SpecialExperience
    {
        get => stringValues[I_SpecialExperience];
        set => stringValues[I_SpecialExperience] = value;
    }
    public string FavoriteClasses
    {
        get => stringValues[I_FavoriteClasses];
        set => stringValues[I_FavoriteClasses] = value;
    }
    #endregion
    public UserData() {
        for (int i = 0; i < stringValues.Length; i++)
            stringValues[i] = "";
    }
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
        FavoriteClasses = data.FavoriteClasses;
    }

    public VisualElement CreateUserIconElement()
    {
        VisualElement ret = new();
        ret.style.backgroundImage = Background.FromTexture2D(IconTexture);
        return ret;
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
            data.SpecialExperience == SpecialExperience &&
            data.FavoriteClasses == FavoriteClasses;
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
        hash = hash * 23 + FavoriteClasses.GetHashCode();

        return hash;
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
    public static void DeleteThenSaveAll()
    {
        foreach (string path in Directory.GetFiles(DataHandler.UserDataDir, "*.json"))
            File.Delete(path);
        foreach (string path in Directory.GetFiles(DataHandler.UserDataDir, "*.meta"))
            File.Delete(path);
        SaveAll();
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    public static void SaveAll()
    {
        int i=0;
        foreach (var data in Datas)
        {
            Debug.Log("Save " + data.Name);
            DataHandler.SaveData(DataHandler.UserDataDir, $"{i++}.json", JsonUtility.ToJson(data));
        }
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
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
