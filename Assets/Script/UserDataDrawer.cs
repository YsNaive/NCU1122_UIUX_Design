using NaiveAPI.UITK;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using SFB;
using System.IO;
using System;

[CustomRuntimeDrawer(typeof(UserData))]
public class UserDataDrawer : RuntimeDrawer<UserData>
{
    VisualElement iconView;
    public override void RepaintDrawer()
    {
        if (value == null) return;
        for (int i = 1; i < value.stringValues.Length; i++)
            stringDrawers[i].SetValueWithoutNotify(value.stringValues[i]);
        iconView.style.backgroundImage = value.IconTexture;
    }

    StringDrawer[] stringDrawers = new StringDrawer[UserData.dataNum];
    protected override void CreateGUI()
    {
        var orgWidth = RSTheme.Current.LabelWidth;
        RSTheme.Current.LabelWidth = 125;
        for (int i = 1; i < UserData.dataNum; i++)
        {
            var localIndex = i;
            stringDrawers[localIndex] = new StringDrawer() { label = RSLocalization.GetText(UserData.MemberNameKeys[localIndex]) };
            stringDrawers[localIndex].style.marginBottom = RSTheme.Current.VisualMargin;
            stringDrawers[localIndex].OnValueChanged += () =>
            {
                value.stringValues[localIndex] = stringDrawers[localIndex].value;
                InvokeMemberValueChange(stringDrawers[localIndex]);
            };
        }
        RSTheme.Current.LabelWidth = orgWidth;
        var iconViewContainer = new VisualElement();
        iconView = new VisualElement()
        {
            style =
            {
                width = 75,
                height = 75,
            }
        };
        iconView.style.backgroundColor = Color.white;
        var selectIconBtn = new RSButton(RSLocalization.GetText(SR._select), RSTheme.Current.HintColorSet, () =>
        {
            var pick = StandaloneFileBrowser.OpenFilePanel(RSLocalization.GetText(SR._select), "", new ExtensionFilter[] { new ExtensionFilter("Image", "jpg", "jpeg", "png") }, false);
            if (pick.Length != 0)
            {
                var pickedPath = pick[0];
                value.Base64Icon = Convert.ToBase64String(File.ReadAllBytes(pickedPath));
                iconView.style.backgroundImage = value.IconTexture;
            }
        })
        { style = { marginTop = RSTheme.Current.VisualMargin, } };
        iconViewContainer.Add(iconView);
        iconViewContainer.Add(selectIconBtn);

        RSScrollView dataElement = new RSScrollView();
        dataElement.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        dataElement.style.flexGrow = 1;

        dataElement.Add(stringDrawers[UserData.I_Name]);
        dataElement.Add(stringDrawers[UserData.I_Major]);
        dataElement.Add(stringDrawers[UserData.I_Career]);
        dataElement.Add(stringDrawers[UserData.I_Gender]);
        dataElement.Add(stringDrawers[UserData.I_GraduatedSchool]);
        dataElement.Add(stringDrawers[UserData.I_PhoneNumber]);
        dataElement.Add(stringDrawers[UserData.I_Contact]);
        dataElement.Add(stringDrawers[UserData.I_Hobbies]);
        dataElement.Add(stringDrawers[UserData.I_FavoriteClasses]);
        dataElement.Add(stringDrawers[UserData.I_ResearchTopic]);
        dataElement.Add(stringDrawers[UserData.I_Skills]);
        dataElement.Add(stringDrawers[UserData.I_SpecialExperience]);
        dataElement.Add(stringDrawers[UserData.I_FB]);
        dataElement.Add(stringDrawers[UserData.I_IG]);

        var hor = new RSHorizontal();
        hor.Add(iconViewContainer);
        hor.Add(dataElement);
        Add(hor);
        //RSButton nextPage = new RSButton(RSLocalization.GetText(SR.userDataEdit_nextPage), RSTheme.Current.SuccessColorSet, () =>
        //{
        //});
        //RSButton prevPage = new RSButton(RSLocalization.GetText(SR.userDataEdit_prevPage), RSTheme.Current.SuccessColorSet, () =>
        //{
        //});
        //RSHorizontal controlElement = new RSHorizontal() { style = { marginLeft = StyleKeyword.Auto} };
        //controlElement.Add(prevPage);
        //controlElement.Add(nextPage);
        //Add(controlElement);
    }

}
