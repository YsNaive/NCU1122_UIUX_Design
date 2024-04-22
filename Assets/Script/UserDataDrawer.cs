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
    public enum MemberCategory
    {
        BasicInfo,
        Contact,
        Qualities,
    }
    VisualElement iconView;
    public override void RepaintDrawer()
    {
        if (value == null) return;
        for (int i = 1; i < value.stringValues.Length; i++)
            stringDrawers[i].SetValueWithoutNotify(value.stringValues[i]);
        iconView.style.backgroundImage = value.IconTexture;
    }

    VisualElement stageContainer;
    PageView<MemberCategory> pageView;
    StringDrawer[] stringDrawers = new StringDrawer[13];
    protected override void CreateGUI()
    {
        stageContainer = new VisualElement();
        stageContainer.style.flexDirection = FlexDirection.Row;
        stageContainer.style.marginLeft = StyleKeyword.Auto;
        stageContainer.style.marginBottom = RSTheme.Current.IndentStep;
        Add(stageContainer);
        var img = Resources.Load<Texture2D>("Image/banner");
        foreach (var item in typeof(MemberCategory).GetEnumValues())
        {
            var category = (MemberCategory)item;
            RSTextElement btn = new RSTextElement(category.ToString());
            btn.style.width = 120;
            btn.style.height = 30;
            btn.style.unityTextAlign = TextAnchor.MiddleCenter;
            btn.style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor;
            btn.style.color = RSTheme.Current.BackgroundColor;
            btn.style.fontSize = 18;
            btn.style.backgroundImage = img;
            btn.RegisterCallback<PointerLeaveEvent>(evt => { btn.style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor; });
            btn.RegisterCallback<PointerEnterEvent>(evt => { btn.style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor2; });
            btn.RegisterCallback<PointerDownEvent>(evt =>
            {
                pageView.OpenPage(category);
            });
            stageContainer.Add(btn);
        }
        Add(stageContainer);


        for (int i = 1; i < 13; i++)
        {
            var localIndex = i;
            stringDrawers[localIndex] = new StringDrawer() { label = UserData.MemberNames[localIndex] };
            stringDrawers[localIndex].style.marginBottom = RSTheme.Current.VisualMargin;
            stringDrawers[localIndex].OnValueChanged += () =>
            {
                value.stringValues[localIndex] = stringDrawers[localIndex].value;
                InvokeMemberValueChange(stringDrawers[localIndex]);
            };
        }
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
        var selectIconBtn = new RSButton("Select", RSTheme.Current.HintColorSet, () =>
        {
            var pick = StandaloneFileBrowser.OpenFilePanel("Select Icon", "", new ExtensionFilter[] { new ExtensionFilter("Image", "jpg", "jpeg", "png") }, false);
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

        pageView = new ();
        pageView.style.flexGrow = 1f;
        pageView.OpenOrCreatePage(MemberCategory.BasicInfo);
        pageView.Add(stringDrawers[UserData.I_Name]);
        pageView.Add(stringDrawers[UserData.I_Major]);
        pageView.Add(stringDrawers[UserData.I_Gender]);
        pageView.Add(stringDrawers[UserData.I_GraduatedSchool]);
        pageView.Add(stringDrawers[UserData.I_ResearchTopic]);

        pageView.OpenOrCreatePage(MemberCategory.Contact);
        pageView.Add(stringDrawers[UserData.I_PhoneNumber]);
        pageView.Add(stringDrawers[UserData.I_Contact]);

        pageView.OpenOrCreatePage(MemberCategory.Qualities);
        pageView.Add(stringDrawers[UserData.I_Hobbies]);
        pageView.Add(stringDrawers[UserData.I_FavoriteClasses]);
        pageView.Add(stringDrawers[UserData.I_Skills]);
        pageView.Add(stringDrawers[UserData.I_SpecialExperience]);

        var hor = new RSHorizontal();
        hor.Add(iconViewContainer);
        hor.Add(pageView);
        Add(hor);
        pageView.OpenOrCreatePage(MemberCategory.BasicInfo);
    }

}
