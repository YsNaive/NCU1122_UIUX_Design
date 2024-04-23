using NaiveAPI.UITK;
using SingularityGroup.HotReload;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
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
        ColorPlayground,
        Documnetation,
        OnlineHelp
    }
    public static (string text, FontAsset font) GetPageTextInfo(Page page)
    {
        return page switch
        {
            Page.Main => RSLocalization.GetTextAndFont(SR.page_main),
            Page.ViewUser => RSLocalization.GetTextAndFont(SR.page_userData),
            Page.EditUser => RSLocalization.GetTextAndFont(SR.page_editUser),
            Page.EditTheme => RSLocalization.GetTextAndFont(SR.page_editTheme),
            Page.ColorPlayground => ("Color\nTest", null),
            Page.Documnetation => RSLocalization.GetTextAndFont(SR.page_documentation),
            Page.OnlineHelp => RSLocalization.GetTextAndFont(SR.page_onlineHelp),
            _ => throw new System.NotImplementedException(),
        }; ;
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
        _InitUserDataPage();
        _InitEditUserPage();
        _InitEditThemePage();
        _InitColorPlayGround();
        _InitDocumentation();
        _InitPageButton();

        root.Add(toolBarContainer);
        root.Add(pageView);
        pageView.OpenPage(Page.Main);
        RuntimeWindow.ScreenElement = root;
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
        RSRadius.op_temp.any = RSTheme.Current.LineHeight / 2f;
        RSRadius.op_temp.ApplyOn(previewUser);
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
        rngData ??= new UserData() { Name = "N/A"};
        previewUser.style.backgroundImage = Background.FromTexture2D(rngData.IconTexture);
        previewUser.Add(new RSTextElement(rngData.Name)
        {
            style =
            {
                alignSelf = Align.Center,
                fontSize = 20,
                position = Position.Absolute,
                top = 210,
                paddingLeft = 15,
            }
        });
        RSBorder hightLightBorder = new RSBorder();
        hightLightBorder.anyWidth = 2f;
        hightLightBorder.anyColor = RSTheme.Current.FrontgroundColor3;
        RSBorder normalBorder = hightLightBorder.DeepCopy();
        normalBorder.anyColor = Color.clear;
        previewUser.RegisterCallback<PointerEnterEvent>(evt => hightLightBorder.ApplyOn(previewUser));
        previewUser.RegisterCallback<PointerLeaveEvent>(evt => normalBorder.ApplyOn(previewUser));
        previewUser.RegisterCallback<PointerDownEvent>(evt =>
        {
            UserDataPopupWindow.Open(rngData);
        });
        pageView.Add(titleText);
        pageView.Add(previewUser);


        Color c1, c2;
        c1 = RSTheme.Current.BackgroundColor2;
        c2 = RSTheme.Current.BackgroundColor;
        pageView.contentContainer.generateVisualContent += (MeshGenerationContext mgc) =>
        { UIElementExtensionUtils.FillElementMeshGeneration(mgc, c1, c2, c2, c1);};

    }
    void _InitUserDataPage()
    {
        pageView.OpenOrCreatePage(Page.ViewUser);
        SearchView<string, UserData> searchView = new((searchKey) =>
        {
            return UserDataHandler.Datas.OrderBy((v) =>
            {
                int distance = int.MaxValue;

                foreach (string s in v.stringValues)
                {
                    if (!s.Contains(searchKey))
                    {
                        distance++;
                    }
                }

                return distance;
            });
        },
        data =>
        {
            var hor = new RSHorizontal();
            var icon = new VisualElement()
            {
                style =
                {
                    width = 75,
                    height = 75,
                    backgroundImage = data.IconTexture,
                }
            };
            icon.style.marginRight = 20;

            VisualElement simpleInfoVisual = new VisualElement();
            simpleInfoVisual.style.justifyContent = Justify.SpaceAround;
            simpleInfoVisual.style.flexGrow = 1;

            RSTextElement nameElement = new RSTextElement(RSLocalization.GetText(SR.userData_name) + "¡G" + data.Name);
            RSTextElement majorElement = new RSTextElement(RSLocalization.GetText(SR.userData_major) + "¡G" + data.Major);
            RSTextElement skillsElement = new RSTextElement(RSLocalization.GetText(SR.userData_skills) + "¡G" + data.Skills);

            simpleInfoVisual.Add(nameElement);
            simpleInfoVisual.Add(majorElement);
            simpleInfoVisual.Add(skillsElement);

            hor.Add(icon);
            hor.Add(simpleInfoVisual);
            hor.RegisterCallback<PointerEnterEvent>(evt => hor.style.backgroundColor = RSTheme.Current.BackgroundColor2);
            hor.RegisterCallback<PointerDownEvent>(evt => UserDataPopupWindow.Open(data));
            hor.RegisterCallback<PointerLeaveEvent>(evt => hor.style.backgroundColor = RSTheme.Current.BackgroundColor);
            return hor;
        });
        searchView.Search("");
        var scrollView = new RSScrollView();

        VisualElement topElement = new VisualElement();
        topElement.style.flexDirection = FlexDirection.Row;

        VisualElement searchContainer = new VisualElement();
        searchContainer.style.width = Length.Percent(30);
        searchContainer.style.flexDirection = FlexDirection.Row;
        searchContainer.style.backgroundColor = RSTheme.Current.FieldStyle.Background.tintColor;
        searchContainer.style.alignItems = Align.Center;
        searchContainer.style.SetRS_Style(RSRadius.Pixel(10));
        searchContainer.style.left = StyleKeyword.Auto;
        searchContainer.style.right = 0;
        searchContainer.style.marginLeft = StyleKeyword.Auto;
        searchContainer.style.marginRight = 10;
        searchContainer.style.marginTop = 5;
        searchContainer.style.marginBottom = 5;

        RSTextField searchField = new RSTextField("", (evt) =>
        {
            searchView.Search(evt.newValue);
        });
        searchField.value = "·j´M";
        searchField.style.flexGrow = 1;
        searchField.style.height = 40;
        searchField.style.marginLeft = 10;
        searchField.style.marginRight = 10;
        searchField[0].style.backgroundImage = null;
        searchField[0].style.SetRS_Style(new RSBorder(Color.clear, 0));
        searchField[0].style.fontSize = RSTheme.Current.FieldStyle.Text.size * 1.5f;

        searchField.RegisterCallback<FocusInEvent>((evt) =>
        {
            if (searchField.value == "·j´M")
            {
                searchField.SetValueWithoutNotify("");
            }
        });

        searchField.RegisterCallback<FocusOutEvent>((evt) =>
        {
            if (searchField.value == "")
            {
                searchField.SetValueWithoutNotify("·j´M");
            }
        });

        VisualElement image = new VisualElement();

        image.style.backgroundImage = Resources.Load<Texture2D>("Image/search_white");
        image.style.width = 30;
        image.style.height = 30;
        image.style.marginTop = 5;
        image.style.marginBottom = 5;
        image.style.marginLeft = 10;

        searchContainer.Add(image);
        searchContainer.Add(searchField);

        topElement.Add(searchContainer);

        pageView.Add(topElement);

        scrollView.Add(searchView);
        pageView.Add(scrollView);
    }

    public bool isCreating = true;
    public bool editUserPageState = false;
    UserDataDrawer editDataDrawer;
    UserData prevEditData;
    void _InitEditUserPage()
    {
        pageView.OpenOrCreatePage(Page.EditUser);
        var root = pageView.contentContainer;
        root.contentContainer.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            editDataDrawer = new();
            RSTextElement createOrEditHint = new RSTextElement();
            createOrEditHint.style.fontSize = 24;
            createOrEditHint.text = RSLocalization.GetText(isCreating ? SR.page_editUser_create : SR.page_editUser_edit);
            PageView<bool> editUserPage = new(false);
            editUserPage.Add(new RSTextElement("Select to Edit or Create a new data") { style = { fontSize = 24 } });
            var dataChoicesContainer = new RSScrollView();
            editUserPage.Add(dataChoicesContainer);
            dataChoicesContainer.Add(new RSButton("Create New Data", RSTheme.Current.HintColorSet, () =>
            {
                isCreating = true;
                editDataDrawer.value = new();
                prevEditData = editDataDrawer.value;
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
                    prevEditData = editDataDrawer.value;
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

            editDataDrawer.value = prevEditData;
        });
    }
    void _InitEditThemePage()
    {
        pageView.OpenOrCreatePage(Page.EditTheme);
        var scrollView = new RSScrollView();
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.NormalColorSet, "Main Color"));
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.SuccessColorSet, "Success Color"));
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.DangerColorSet, "Danger Color"));
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.HintColorSet, "Hint Color"));
        foreach (var ve in scrollView.Children())
            ve.style.marginBottom = 5;
        pageView.Add(scrollView);
        var applyBtn = new RSButton("Apply", RSTheme.Current.SuccessColorSet);
        applyBtn.clicked += _InitUI;
        applyBtn.style.marginTop = StyleKeyword.Auto;
        pageView.contentContainer.style.flexGrow = 1;
        pageView.Add(applyBtn);
    }
    void _InitColorPlayGround()
    {
        pageView.OpenOrCreatePage(Page.ColorPlayground);

        Action rapaintShowElement = null;
        RSStyle showStyle = new()
        {
            Size = new()
            {
                height = 25,
            },
            Margin = new()
            {
                top = 5,
                left = RSTheme.Current.LineHeight,
            }
        };
        VisualElement[] showIndent = new VisualElement[4];
        for(int i = 0; i < 4; i++)
        {
            showIndent[i] = new VisualElement()
            {
                style =
                {
                    width = 50,
                    height = 50,
                    marginRight = 0,
                }
            };
        }
        VisualElement showNormal = new();
        VisualElement showAfter = new();
        showStyle.ApplyOn(showNormal);
        showStyle.ApplyOn(showAfter);
        showNormal.generateVisualContent += (mgc) => { UIElementExtensionUtils.FillElementMeshGeneration(mgc, Color.black, Color.black, Color.white, Color.white); };

        BoolDrawer showMidLine = new BoolDrawer() { label ="Show Mid Line"};

        FloatRangeDrawer exponentsDrawer = new FloatRangeDrawer();
        exponentsDrawer.label = "Exponents";
        exponentsDrawer.slider.lowValue = 0.5f;
        exponentsDrawer.slider.highValue = 3f;
        exponentsDrawer.value = 1f;
        rapaintShowElement = () =>
        {
            Texture2D showTexture = new Texture2D(256, 1);
            for (int i = 0; i < 256; i++)
            {
                var v = Mathf.Pow(i / 255f, exponentsDrawer.value);
                if (showMidLine.value && Mathf.Abs(v - 0.5f) < 0.005)
                    showTexture.SetPixel(i, 0, Color.green);
                else
                    showTexture.SetPixel(i, 0, Color.HSVToRGB(0, 0, v));
            }
            for (int i = 0; i < 4; i++)
            {
                var v = Mathf.Pow(i*85f / 255f, exponentsDrawer.value);
                showIndent[i].style.backgroundColor = Color.HSVToRGB(0, 0, v);
            }
            showTexture.wrapMode = TextureWrapMode.Clamp;
            showTexture.Apply();
            showAfter.style.backgroundImage = showTexture;
        };
        rapaintShowElement();
        exponentsDrawer.OnValueChanged += rapaintShowElement;
        showMidLine.OnValueChanged += rapaintShowElement;

        pageView.Add(showMidLine);
        pageView.Add(exponentsDrawer);
        pageView.Add(showNormal);
        pageView.Add(showAfter);

        RSHorizontal hor = new()
        {
            style = 
            {
                marginTop  = 20,
                marginLeft = RSTheme.Current.LineHeight,

            }
        };
        foreach (var item in showIndent)
            hor.Add(item);
        pageView.Add(hor);
    }
    void _InitDocumentation()
    {
        pageView.OpenOrCreatePage(Page.Documnetation);
    }
    void _InitPageButton()
    {
        foreach (var pair in pageView.pageTable)
        {
            var localKey = pair.Key;
            var textInfo = GetPageTextInfo(localKey);
            RSButton button = new RSButton(textInfo.text, () => { pageView.OpenPage(localKey); });
            if (textInfo.font != null)
                button.style.unityFontDefinition = new FontDefinition() { fontAsset = textInfo.font };
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.width = RSTheme.Current.LineHeight * 2f;
            button.style.height = RSTheme.Current.LineHeight * 2f;
            var margin = RSTheme.Current.LineHeight / 3;
            button.style.marginLeft = margin;
            button.style.marginTop = margin;
            button.style.marginRight = margin;
            toolBarContainer.Add(button);
        }


        var textAndFont = RSLocalization.GetTextAndFont(SR.page_onlineHelp);

        RSButton btOnlineHelp = new RSButton(textAndFont.text, () => Application.OpenURL("https://ysnaive.github.io/NCU1122_UIUX_Design/"));
        if (textAndFont.font != null)
            btOnlineHelp.style.unityFontDefinition = new FontDefinition() { fontAsset = textAndFont.font };
        btOnlineHelp.style.unityTextAlign = TextAnchor.MiddleCenter;
        btOnlineHelp.style.width = RSTheme.Current.LineHeight * 2f;
        btOnlineHelp.style.height = RSTheme.Current.LineHeight * 2f;
        var margin1 = RSTheme.Current.LineHeight / 3;
        btOnlineHelp.style.marginLeft = margin1;
        btOnlineHelp.style.marginTop = margin1;
        btOnlineHelp.style.marginRight = margin1;
        toolBarContainer.Add(btOnlineHelp);
    }
}
