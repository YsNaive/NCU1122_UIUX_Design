using NaiveAPI.UITK;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

[CustomRuntimeDrawer(typeof(UserData))]
public class UserDataDrawer : RuntimeDrawer<UserData>
{
    public enum MemberCategory
    {
        BasicInfo,
        Contact,
        Qualities,
    }
    public override void RepaintDrawer()
    {
        for (int i = 1; i < value.stringValues.Length; i++)
            stringDrawers[i].SetValueWithoutNotify(value.stringValues[i]);
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

        pageView = new ();
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
        Add(pageView);
    }
}
