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
        }
    }

    public override IEnumerable<string> ImageKeys => Enumerable.Empty<string>();
}
