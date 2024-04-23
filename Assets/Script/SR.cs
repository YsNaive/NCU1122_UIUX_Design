using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SR : RSLocalizationKeyProvider
{
    public const string page_main = "page.main";
    public const string page_main_title = "page.main.title";
    public const string page_userData = "page.userData";
    public const string page_editTheme = "page.editTheme";
    public const string page_editUser = "page.editUser";
    public const string page_editUser_edit = "page.editUser.editing";
    public const string page_editUser_create = "page.editUser.creating";
    public const string page_documentation = "page.documentation";
    public const string page_onlineHelp = "page.onlineHelp";


    public const string userData_name = "userData.name";
    public const string userData_base64Icon ="userData.base64Icon";
    public const string userData_major ="userData.major";
    public const string userData_career ="userData.career";
    public const string userData_gender ="userData.gender";
    public const string userData_phoneNumber ="userData.phoneNumber";
    public const string userData_researchTopic ="userData.researchTopic";
    public const string userData_contact ="userData.contact";
    public const string userData_skills ="userData.skills";
    public const string userData_hobbies ="userData.hobbies";
    public const string userData_graduatedSchool ="userData.graduatedSchool";
    public const string userData_specialExperience ="userData.specialExperience";
    public const string userData_favoriteClasses = "userData.favoriteClasses";

    public override IEnumerable<string> TextKeys
    {
        get
        {
            yield return page_main;
            yield return page_main_title;
            yield return page_userData;
            yield return page_editTheme;
            yield return page_editUser;
            yield return page_editUser_edit;
            yield return page_editUser_create;
            yield return page_documentation;
            yield return page_onlineHelp;
            yield return userData_name;
            yield return userData_name;
            yield return userData_base64Icon;
            yield return userData_major;
            yield return userData_career;
            yield return userData_gender;
            yield return userData_phoneNumber;
            yield return userData_researchTopic;
            yield return userData_contact;
            yield return userData_skills;
            yield return userData_hobbies;
            yield return userData_graduatedSchool;
            yield return userData_specialExperience;
            yield return userData_favoriteClasses;
        }
    }

    public override IEnumerable<string> ImageKeys => Enumerable.Empty<string>();
}
