using NaiveAPI.UITK;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using SFB;
using System.IO;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

[CustomRuntimeDrawer(typeof(UserData))]
public class UserDataDrawer : RuntimeDrawer<UserData>
{
    public enum MemberCategory
    {
        BasicInfo = 0,
        Contact = 1,
        Qualities = 2,
    }
    static string GetCategoryText(MemberCategory category)
    {
        return category switch
        {
            MemberCategory.BasicInfo => RSLocalization.GetText(SR.userData_basicInfoGroup),
            MemberCategory.Contact => RSLocalization.GetText(SR.userData_contactGroup),
            MemberCategory.Qualities => RSLocalization.GetText(SR.userData_quilitiesGroup),
            _ => "None",
        };
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
    MemberCategory currentPage;
    StringDrawer[] stringDrawers = new StringDrawer[13];
    protected override void CreateGUI()
    {
        stageContainer = new VisualElement();
        stageContainer.style.flexDirection = FlexDirection.Row;
        stageContainer.style.marginLeft = StyleKeyword.Auto;
        stageContainer.style.marginBottom = RSTheme.Current.IndentStep;

        var img = Resources.Load<Texture2D>("Image/banner");
        foreach (var item in typeof(MemberCategory).GetEnumValues())
        {
            var category = (MemberCategory)item;
            RSTextElement btn = new RSTextElement(GetCategoryText(category));
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

        var orgWidth = RSTheme.Current.LabelWidth;
        RSTheme.Current.LabelWidth = 125;
        for (int i = 1; i < 13; i++)
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

        pageView = new ();
        pageView.style.flexGrow = 1f;
        pageView.OpenOrCreatePage(MemberCategory.BasicInfo);
        pageView.Add(stringDrawers[UserData.I_Name]);
        pageView.Add(stringDrawers[UserData.I_Major]);
        pageView.Add(stringDrawers[UserData.I_Career]);
        pageView.Add(stringDrawers[UserData.I_Gender]);
        pageView.Add(stringDrawers[UserData.I_GraduatedSchool]);

        pageView.OpenOrCreatePage(MemberCategory.Contact);
        pageView.Add(stringDrawers[UserData.I_PhoneNumber]);
        pageView.Add(stringDrawers[UserData.I_Contact]);

        pageView.OpenOrCreatePage(MemberCategory.Qualities);
        pageView.Add(stringDrawers[UserData.I_Hobbies]);
        pageView.Add(stringDrawers[UserData.I_FavoriteClasses]);
        pageView.Add(stringDrawers[UserData.I_ResearchTopic]);
        pageView.Add(stringDrawers[UserData.I_Skills]);
        pageView.Add(stringDrawers[UserData.I_SpecialExperience]);

        var hor = new RSHorizontal();
        hor.Add(iconViewContainer);
        hor.Add(pageView);
        Add(hor);
        pageView.OpenOrCreatePage(MemberCategory.BasicInfo);
        pageView.style.height = RSTheme.Current.LineHeight * 7f;
        currentPage = MemberCategory.BasicInfo;
        Action updatePageState = null;
        var nextPage = new RSButton(RSLocalization.GetText(SR.userDataEdit_nextPage), RSTheme.Current.SuccessColorSet, () =>
        {
            int i = (int)currentPage;
            currentPage = (MemberCategory)(i + 1);
            pageView.OpenOrCreatePage(currentPage);
            updatePageState();
        });
        var prevPage = new RSButton(RSLocalization.GetText(SR.userDataEdit_prevPage), RSTheme.Current.SuccessColorSet, () =>
        {
            int i = (int)currentPage;
            currentPage = (MemberCategory)(i - 1);
            pageView.OpenOrCreatePage(currentPage);
            updatePageState();
        });
        updatePageState = () =>
        {
            prevPage.SetEnabled(false);
            nextPage.SetEnabled(false);
            if ((int)currentPage != 0)
                prevPage.SetEnabled(true);
            if ((int)currentPage != 2)
                nextPage.SetEnabled(true);
        };
        updatePageState();
        var nextPrevPageHor = new RSHorizontal() { style = { marginLeft = StyleKeyword.Auto} };
        nextPrevPageHor.Add(prevPage);
        nextPrevPageHor.Add(nextPage);
        Add(nextPrevPageHor);
    }

}
