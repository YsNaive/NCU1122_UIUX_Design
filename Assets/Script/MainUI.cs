using NaiveAPI.UITK;
using SingularityGroup.HotReload;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MainUI : MonoBehaviour
{
    public enum Page
    {
        Main,
        ViewUser,
        EditUser,
        EditTheme,
    }
    public static string GetPageName(Page page)
    {
        return page switch
        {
            Page.Main => RSLocalization.GetText(SR.page_main),
            Page.ViewUser => "資料\n清單",
            Page.EditUser => RSLocalization.GetText(SR.page_editUser),
            Page.EditTheme => RSLocalization.GetText(SR.page_editTheme),
            _ => throw new System.NotImplementedException(),
        };
    }

    VisualElement root;
    PageView<Page> pageView;
    VisualElement toolBarContainer;
    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.flexDirection = FlexDirection.Row;
        editDataDrawer = new UserDataDrawer();
        _InitUI();
    }
    [InvokeOnHotReload]
    void _InitUI()
    {
        root.Clear();
        root.style.backgroundColor = RSTheme.Current.BackgroundColor;
        toolBarContainer = new VisualElement();
        toolBarContainer.style.borderRightColor = RSTheme.Current.BackgroundColor3;
        toolBarContainer.style.borderRightWidth = 2f;
        pageView = new PageView<Page>();
        pageView.style.flexGrow = 1;
        RSPadding.op_temp.any = RSTheme.Current.LineHeight/2f;
        RSPadding.op_temp.ApplyOn(pageView);

        _InitMainPage();
        _InitEditUserPage();
        _InitEditThemePage();
        _InitPageButton();

        root.Add(toolBarContainer);
        root.Add(pageView);

        pageView.OpenPage(Page.Main);
    }
    void _InitMainPage()
    {
        pageView.OpenOrCreatePage(Page.Main);
        var titleText = new RSLocalizeTextElement(RSLocalizeText.FromKey(SR.page_main_title));
        titleText.style.fontSize = 32;
        titleText.style.marginTop = 20;
        titleText.style.alignSelf = Align.Center;

        var previewUser = new VisualElement
        {
            style =
            {
                backgroundColor = Color.white,
                width = 200,
                height = 200,
                marginTop = 35,
                alignSelf = Align.Center,
            }
        };
        previewUser.schedule.Execute(() =>
        {
            RSTransition transition = new RSTransition();
            RSStyle state = new RSStyle();
            state.Transform = new();
            var trans = state.Transform;
            trans.rotateDeg = -2.5f;
            transition.AddTransition(1f, state.DeepCopy());
            trans.rotateDeg = 2.5f;
            transition.AddTransition(1f, state.DeepCopy());
            transition.AnimationMode = RSAnimationMode.PingPong;
            transition.ValidKeyframe();
            transition.MakePlayerByReference(previewUser).Start();
        }).ExecuteLater(1);
        pageView.contentContainer.style.flexGrow = 1;
        pageView.contentContainer.RegisterCallback<PointerMoveEvent>(evt =>
        {
            var center = pageView.contentRect.center;
            var oldCenter = (Vector2)previewUser.transform.position;
            var newCenter = ((Vector2)evt.localPosition - center) * 0.2f;
            previewUser.transform.position = oldCenter*0.95f + newCenter *0.05f;
        });
        UserData rngData = null;
        if (UserDataHandler.Datas.Count != 0)
            rngData = UserDataHandler.Datas[Random.Range(0, 10000000) % UserDataHandler.Datas.Count];
        rngData = new UserData();
        previewUser.style.backgroundImage = Background.FromTexture2D(rngData.IconTexture);
        pageView.Add(titleText);
        pageView.Add(previewUser);

        Color c1, c2;
        c1 = RSTheme.Current.BackgroundColor2;
        c2 = RSTheme.Current.BackgroundColor;
        pageView.contentContainer.generateVisualContent += (MeshGenerationContext mgc) =>
        { UIElementExtensionUtils.FillElementMeshGeneration(mgc, c1, c2, c2, c1);};

    }

    public bool isCreating = true;
    public bool editUserPageState = false;
    UserDataDrawer editDataDrawer;
    void _InitEditUserPage()
    {
        pageView.OpenOrCreatePage(Page.EditUser);
        var root = pageView.contentContainer;
        root.contentContainer.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            RSTextElement createOrEditHint = new RSTextElement();
            PageView<bool> editUserPage = new(false);
            editUserPage.Add(new RSTextElement("Select to Edit or Create a new data"));
            var dataChoicesContainer = new RSScrollView();
            editUserPage.Add(dataChoicesContainer);
            dataChoicesContainer.Add(new RSButton("Create New Data", RSTheme.Current.HintColorSet, () =>
            {
                isCreating = true;
                editDataDrawer.value = new();
                editUserPageState = true;
                editUserPage.OpenPage(editUserPageState);
                createOrEditHint.text = RSLocalization.GetText(SR.page_editUser_create);
            })
            { style = { width = 120 } });
            foreach (var data in UserDataHandler.Datas)
            {
                var localData = data;
                var hor = new RSHorizontal();
                var icon = new VisualElement
                {
                    style =
                {
                    width = 20,
                    height = 20,
                    backgroundImage = data.IconTexture,
                    marginRight=5,
                }
                };
                var name = new RSTextElement(data.Name);
                hor.Add(icon);
                hor.Add(name);
                hor.style.borderBottomColor = RSTheme.Current.BackgroundColor2;
                hor.style.borderBottomWidth = 1f;
                hor.style.paddingTop = RSTheme.Current.VisualMargin;
                hor.style.paddingBottom = RSTheme.Current.VisualMargin;
                hor.style.paddingLeft = RSTheme.Current.VisualMargin;
                hor.RegisterCallback<PointerEnterEvent>(evt => { hor.style.backgroundColor = RSTheme.Current.BackgroundColor2; });
                hor.RegisterCallback<PointerLeaveEvent>(evt => { hor.style.backgroundColor = RSTheme.Current.BackgroundColor; });
                hor.RegisterCallback<PointerDownEvent>(evt =>
                {
                    hor.style.backgroundColor = RSTheme.Current.BackgroundColor;
                    isCreating = false;
                    editDataDrawer.value = localData;
                    editUserPageState = true;
                    editUserPage.OpenPage(true);
                    createOrEditHint.text = RSLocalization.GetText(SR.page_editUser_edit);
                });
                RSButton deleteBtn = new RSButton("Delete", RSTheme.Current.DangerColorSet, () =>
                {
                    UserDataHandler.Datas.Remove(localData);
                    UserDataHandler.DeleteThenSaveAll();
                    _InitEditUserPage();
                })
                { style = { marginLeft = StyleKeyword.Auto } };
                hor.Add(deleteBtn);
                dataChoicesContainer.Add(hor);
            }
            editUserPage.OpenOrCreatePage(true);
            editUserPage.Add(createOrEditHint);
            editUserPage.Add(editDataDrawer);

            RSButton saveBtn = new RSButton("Save", RSTheme.Current.SuccessColorSet, () =>
            {
                if(isCreating)
                    UserDataHandler.Datas.Add(editDataDrawer.value);
                UserDataHandler.DeleteThenSaveAll();
                editUserPage.OpenPage(false);
                editUserPageState = false;
                _InitEditUserPage();
            });
            RSButton cancelBtn = new RSButton("Cancel", RSTheme.Current.DangerColorSet, () =>
            {
                editDataDrawer.value = new();
                editUserPage.OpenPage(false);
                editUserPageState = false;
                _InitEditUserPage();
            });
            editUserPage.Add(new RSHorizontal(1f,saveBtn, cancelBtn) { style = {marginLeft = Length.Percent(50)}});
            editUserPage.OpenPage(editUserPageState);
            root.Clear();
            root.Add(editUserPage);
        });
    }
    void _InitEditThemePage()
    {
        pageView.OpenOrCreatePage(Page.EditTheme);
        var drawer = RuntimeDrawer.Create(RSTheme.Current.NormalColorSet, "Main Color");
        pageView.Add(drawer);
        var applyBtn = new RSButton("Apply", RSTheme.Current.SuccessColorSet);
        applyBtn.clicked += _InitUI;
        pageView.Add(applyBtn);
    }
    void _InitPageButton()
    {
        foreach(var pair in pageView.pageTable)
        {
            var localKey = pair.Key;
            RSButton button = new RSButton(GetPageName(localKey), () => { pageView.OpenPage(localKey); });
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.width = RSTheme.Current.LineHeight * 2f;
            button.style.height = RSTheme.Current.LineHeight * 2f;
            var margin = RSTheme.Current.LineHeight / 3;
            button.style.marginLeft = margin;
            button.style.marginTop = margin;
            button.style.marginRight = margin;
            toolBarContainer.Add(button);
        }
    }
}
