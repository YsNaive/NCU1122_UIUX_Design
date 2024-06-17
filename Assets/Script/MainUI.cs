using NaiveAPI.UITK;
using SFB;
using SingularityGroup.HotReload;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    public static string GetPageTextKey(Page page)
    {
        return page switch
        {
            Page.Main => SR.page_main,
            Page.ViewUser => SR.page_userData,
            Page.EditUser => SR.page_editUser,
            Page.EditTheme => SR.page_editTheme,
            Page.ColorPlayground => "Color\nTest",
            Page.Documnetation => SR.page_documentation,
            Page.OnlineHelp => SR.page_onlineHelp,
            _ => throw new System.NotImplementedException(),
        }; ;
    }

    VisualElement root;
    PageView<Page> pageView;
    VisualElement toolBarContainer, backgroundElement;
    Texture2D backgroundImage;
    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.flexDirection = FlexDirection.Row;
        backgroundImage = Resources.Load<Texture2D>("Image/dark_background");
        editDataDrawer = new UserDataDrawer();
        _InitUI();
    }
    [InvokeOnHotReload]
    void _InitUI()
    {
        root.Clear();
        root.style.backgroundImage = backgroundImage;
        toolBarContainer = new VisualElement();
        toolBarContainer.style.borderRightColor = RSTheme.Current.BackgroundColor3;
        toolBarContainer.style.borderRightWidth = 2f;
        pageView = new PageView<Page>();
        pageView.style.flexGrow = 1;
        pageView.style.backgroundColor = Color.clear;
        RSPadding.op_temp.any = RSTheme.Current.LineHeight/2f;
        RSPadding.op_temp.ApplyOn(pageView);

        _InitMainPage();
        _InitUserDataPage();
        _InitEditUserPage();
        _InitEditThemePage();
        //_InitColorPlayGround();
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


        //Color c1, c2;
        //c1 = RSTheme.Current.BackgroundColor2;
        //c2 = RSTheme.Current.BackgroundColor;
        //pageView.contentContainer.generateVisualContent += (MeshGenerationContext mgc) =>
        //{ UIElementExtensionUtils.FillElementMeshGeneration(mgc, c1, c2, c2, c1);};

    }
    void _InitUserDataPage()
    {
        pageView.OpenOrCreatePage(Page.ViewUser);
        pageView.Clear();
        var searchView = new UserDataSearchView();
        searchView.Search("");
        var scrollView = new RSScrollView();

        VisualElement topElement = new VisualElement();
        topElement.style.flexDirection = FlexDirection.Row;

        VisualElement searchContainer = new VisualElement();
        searchContainer.style.width = Length.Percent(30);
        searchContainer.style.flexDirection = FlexDirection.Row;
        searchContainer.style.backgroundColor = RSTheme.Current.NormalColorSet.BackgroundColor;
        searchContainer.style.alignItems = Align.Center;
        searchContainer.style.SetRS_Style(RSRadius.Pixel(5));
        searchContainer.style.left = StyleKeyword.Auto;
        searchContainer.style.right = 0;
        searchContainer.style.marginLeft = StyleKeyword.Auto;
        searchContainer.style.marginRight = 10;
        searchContainer.style.marginTop = 5;
        searchContainer.style.marginBottom = 5;

        RSTextField searchField = new RSTextField("", (evt) => { searchView.Search(evt.newValue); });
        searchField.value = "·j´M";
        searchField.style.flexGrow = 1;
        searchField.style.height = 25;
        searchField.style.marginLeft = 10;
        searchField.style.marginRight = 10;
        searchField[0].style.backgroundImage = null;
        searchField[0].style.SetRS_Style(new RSBorder(Color.clear, 0));
        searchField[0].style.fontSize = RSTheme.Current.TextSize * 1.2f;

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
        image.style.width = 25;
        image.style.height = 25;
        image.style.marginTop = 5;
        image.style.marginBottom = 5;
        image.style.marginLeft = 10;
        image.style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor;

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
    UserData editDataCopy;
    void _InitEditUserPage()
    {
        pageView.OpenOrCreatePage(Page.EditUser);
        var root = pageView.contentContainer;
        root.style.flexGrow = 1f;
        root.contentContainer.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            editDataDrawer = new();
            editDataDrawer.style.maxHeight = Length.Percent(80);
            RSTextElement createOrEditHint = new RSTextElement() { style = { fontSize = 24, minHeight = 40 } };
            createOrEditHint.text = RSLocalization.GetText(isCreating ? SR.page_editUser_create : SR.page_editUser_edit);
            PageView<bool> editUserPage = new(false);
            editUserPage.Add(new RSTextElement(RSLocalization.GetText(SR.userDataEdit_title)) { style = { fontSize = 24 } });
            var dataChoicesContainer = new RSScrollView();
            editUserPage.Add(dataChoicesContainer);
            dataChoicesContainer.Add(new RSButton(RSLocalization.GetText(SR.userDataEdit_createNew), RSTheme.Current.HintColorSet, () =>
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
                        width = 30,
                        height = 30,
                        backgroundImage = data.IconTexture,
                        marginRight = 5,
                    }
                };
                var name = new RSTextElement(data.Name)
                {
                    style =
                    {
                        fontSize = 20,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        paddingLeft  = 5,
                    }
                };
                hor.Add(icon);
                hor.Add(name);
                hor.style.borderBottomColor = RSTheme.Current.BackgroundColor2;
                hor.style.borderBottomWidth = 1f;
                hor.style.paddingTop = RSTheme.Current.VisualMargin;
                hor.style.paddingBottom = RSTheme.Current.VisualMargin;
                hor.style.paddingLeft = RSTheme.Current.VisualMargin;
                hor.RegisterCallback<PointerEnterEvent>(evt => { hor.style.backgroundColor = RSTheme.Current.BackgroundColor2; });
                hor.RegisterCallback<PointerLeaveEvent>(evt => { hor.style.backgroundColor = Color.clear; });
                hor.RegisterCallback<PointerDownEvent>(evt =>
                {
                    hor.style.backgroundColor = RSTheme.Current.BackgroundColor;
                    isCreating = false;
                    editDataDrawer.value = localData;
                    prevEditData = editDataDrawer.value;
                    editDataCopy = new UserData(prevEditData);
                    editUserPageState = true;
                    editUserPage.OpenPage(true);
                    createOrEditHint.text = RSLocalization.GetText(SR.page_editUser_edit);
                });
                RSButton deleteBtn = new RSButton(RSLocalization.GetText(SR._delete), RSTheme.Current.DangerColorSet, () =>
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

            RSButton saveBtn = new RSButton(RSLocalization.GetText(SR._save), RSTheme.Current.SuccessColorSet, () =>
            {
                if(isCreating)
                    UserDataHandler.Datas.Add(editDataDrawer.value);
                UserDataHandler.DeleteThenSaveAll();
                editUserPage.OpenPage(false);
                editUserPageState = false;
                _InitUI();
                _InitUserDataPage();
                pageView.OpenOrCreatePage(Page.EditUser);
            });
            RSButton cancelBtn = new RSButton(RSLocalization.GetText(SR._cancel), RSTheme.Current.DangerColorSet, () =>
            {
                if (!isCreating)
                {
                    UserDataHandler.Datas.Remove(editDataDrawer.value);
                    UserDataHandler.Datas.Add(editDataCopy);
                }
                editDataDrawer.value = new();
                editUserPage.OpenPage(false);
                editUserPageState = false;
                _InitEditUserPage();
            });
            var btnHor = new RSHorizontal(1f, saveBtn, cancelBtn) { style = { marginLeft = Length.Percent(50) } };
            btnHor.style.marginTop = StyleKeyword.Auto;
            editUserPage.Add(btnHor);
            editUserPage.contentContainer.style.flexGrow = 1;
            editUserPage.style.flexGrow = 1;

            editUserPage.OpenPage(editUserPageState);
            root.Clear();
            root.Add(editUserPage);
            editDataDrawer.value = prevEditData;
        });
    }
    void _InitEditThemePage()
    {
        pageView.OpenOrCreatePage(Page.EditTheme);
        RSHorizontal defaultHor = new RSHorizontal() { style = { marginBottom = 10 } };
        RSButton setToDefaultDark = new RSButton(RSLocalization.GetText(SR.theme_setToDefaultDark), RSTheme.Current.HintColorSet, () =>
        {
            RSTheme.Current = UIElementExtensionResource.Get.DarkTheme.Theme.DeepCopy();
            setBackgroundImage(Resources.Load<Texture2D>("Image/dark_background"));
            _InitUI();
            pageView.OpenPage(Page.EditTheme);
        });
        setToDefaultDark.style.marginRight = 10;
        setToDefaultDark.style.fontSize = RSTheme.Current.LabelTextSize;
        RSButton setToDefaultLight = new RSButton(RSLocalization.GetText(SR.theme_setToDefaultLight), RSTheme.Current.HintColorSet, () =>
        {
            RSTheme.Current = UIElementExtensionResource.Get.LightTheme.Theme.DeepCopy();
            setBackgroundImage(Resources.Load<Texture2D>("Image/light_background"));
            _InitUI();
            pageView.OpenPage(Page.EditTheme);
        });
        setToDefaultLight.style.fontSize = RSTheme.Current.LabelTextSize;
        defaultHor.Add(setToDefaultDark);
        defaultHor.Add(setToDefaultLight);
        pageView.Add(defaultHor);

        var scrollView = new RSScrollView();
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.NormalColorSet, RSLocalization.GetText(SR.theme_normalColorSet)));
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.SuccessColorSet, RSLocalization.GetText(SR.theme_successColorSet)));
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.DangerColorSet, RSLocalization.GetText(SR.theme_dangerColorSet)));
        scrollView.Add(RuntimeDrawer.Create(RSTheme.Current.HintColorSet, RSLocalization.GetText(SR.theme_hintColorSet)));
        VisualElement horizontal = new VisualElement();
        horizontal.style.flexDirection = FlexDirection.Row;
        horizontal.style.backgroundColor = RSTheme.Current.BackgroundColor;
        horizontal.style.paddingTop = 10;
        horizontal.style.paddingLeft = 10;
        horizontal.style.paddingBottom = 10;
        RSTextElement backgroundText = new RSTextElement(RSLocalization.GetText(SR.theme_backgroundImage));
        backgroundText.style.unityTextAlign = TextAnchor.MiddleCenter;
        backgroundText.style.marginRight = 5;
        backgroundElement = new VisualElement
        {
            style =
            {
                width = 48,
                height = 27,
                backgroundImage = backgroundImage,
                marginRight = 5,
            }
        };
        RSButton btSelect = new RSButton(RSLocalization.GetText(SR._select), RSTheme.Current.HintColorSet, () =>
        {
            var pick = StandaloneFileBrowser.OpenFilePanel(RSLocalization.GetText(SR._select), "", new ExtensionFilter[] { new ExtensionFilter("Image", "jpg", "jpeg", "png") }, false);
            if (pick.Length != 0)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(File.ReadAllBytes(pick[0]));
                backgroundImage = texture;
                backgroundElement.style.backgroundImage = texture;
            }
        });
        horizontal.Add(backgroundText);
        horizontal.Add(backgroundElement);
        horizontal.Add(btSelect);
        scrollView.Add(horizontal);
        foreach (var ve in scrollView.Children())
            ve.style.marginBottom = 5;
        pageView.Add(scrollView);
        var applyBtn = new RSButton(RSLocalization.GetText(SR._apply), RSTheme.Current.SuccessColorSet);
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
    RSButton _CreatePageButton(string key)
    {
        var textInfo = RSLocalization.GetTextAndFont(key);
        RSButton button = new RSButton(textInfo.text);
        if (textInfo.font != null)
            button.style.unityFontDefinition = new FontDefinition() { fontAsset = textInfo.font };
        button.style.unityTextAlign = TextAnchor.MiddleCenter;
        button.style.width = RSTheme.Current.LineHeight * 2.5f;
        button.style.height = RSTheme.Current.LineHeight * 2.5f;
        var margin = RSTheme.Current.LineHeight / 3;
        button.style.marginLeft = margin;
        button.style.marginTop = margin / 2f;
        button.style.marginRight = margin;
        button.style.marginBottom = margin / 2f;
        toolBarContainer.Add(button);
        return button;
    }
    void _InitPageButton()
    {
        foreach (var pair in pageView.pageTable)
        {
            var localKey = pair.Key;
            RSButton button = _CreatePageButton(GetPageTextKey(localKey));
            button.clicked += () => pageView.OpenPage(localKey);
            toolBarContainer.Add(button);
            button.name = localKey.ToString();
        }

        RSButton btOnlineHelp = _CreatePageButton(SR.page_onlineHelp);
        btOnlineHelp.clicked += () => Application.OpenURL("https://ysnaive.github.io/NCU1122_UIUX_Design/");

        var bottomArea = new VisualElement { style = { marginTop = StyleKeyword.Auto } };
        bottomArea.Add(btOnlineHelp);
        bottomArea.Add(toolBarContainer.Q(Page.Documnetation.ToString()));
        toolBarContainer.Add(bottomArea);
    }

    void setBackgroundImage(Texture2D texture)
    {
        backgroundImage = texture;
        if (backgroundElement != null)
        {
            backgroundElement.style.backgroundImage = texture;
        }
    }

    class UserDataSearchView : SearchView<string, UserData>
    {
        protected override VisualElement CreateItemVisual(UserData value)
        {
            var hor = new RSHorizontal();
            var icon = new VisualElement()
            {
                style =
                {
                    width = 75,
                    height = 75,
                    backgroundImage = value.IconTexture,
                    scale = new Scale(new Vector2(.95f,.95f)),
                }
            };
            RSRadius.op_temp.any = RSTheme.Current.LineHeight / 3f;
            RSRadius.op_temp.ApplyOn(icon);
            icon.style.marginRight = 20;

            VisualElement simpleInfoVisual = new VisualElement();
            simpleInfoVisual.style.justifyContent = Justify.SpaceAround;
            simpleInfoVisual.style.flexGrow = 1;

            RSTextElement nameElement = new RSTextElement(RSLocalization.GetText(SR.userData_name) + "¡G" + value.Name);
            RSTextElement majorElement = new RSTextElement(RSLocalization.GetText(SR.userData_major) + "¡G" + value.Major);
            RSTextElement skillsElement = new RSTextElement(RSLocalization.GetText(SR.userData_skills) + "¡G" + value.Skills);

            simpleInfoVisual.Add(nameElement);
            simpleInfoVisual.Add(majorElement);
            simpleInfoVisual.Add(skillsElement);

            hor.Add(icon);
            hor.Add(simpleInfoVisual);
            hor.RegisterCallback<PointerEnterEvent>(evt => hor.style.backgroundColor = RSTheme.Current.BackgroundColor2);
            hor.RegisterCallback<PointerDownEvent>(evt => UserDataPopupWindow.Open(value));
            hor.RegisterCallback<PointerLeaveEvent>(evt => hor.style.backgroundColor = Color.clear);
            hor.style.borderBottomColor = RSTheme.Current.BackgroundColor2;
            hor.style.borderBottomWidth = RSTheme.Current.VisualMargin / 2f;
            hor.style.marginBottom = RSTheme.Current.VisualMargin / 2f;
            return hor;
        }

        protected override IEnumerable<UserData> GetOrderedItem(string searchKey)
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
        }
    }
}
