using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class CreateNewUserWindow : DSRuntimeWindow
{
    public Action OnSaveOrClear;
    public UserData UserData => UserDataDrawer.value;
    public UserDataDrawer UserDataDrawer => userDataDrawer;
    private UserDataDrawer userDataDrawer;
    public CreateNewUserWindow()
    {
        PopupOnClick = false;
        Resizable = false;

        userDataDrawer = new UserDataDrawer();
        userDataDrawer.value ??= new UserData();
        var sc = new DSScrollView();
        sc.Add(userDataDrawer);

        VisualElement visualElement = new VisualElement();
        visualElement.style.flexDirection = FlexDirection.Row;

        DSButton btnSave = new DSButton("儲存", DocStyle.Current.SuccessColor, () =>
        {
            if (userDataDrawer.IsAllValid())
            {
                Action doSave = () =>
                {
                    DataHandler.SaveData(DataHandler.UserDataDir, UserData.Name + ".json", JsonUtility.ToJson(UserData), true);
                    UserDataHandler.LoadAll();
                    OnSaveOrClear?.Invoke();
                    Destory();
                };
                if(UserDataHandler.FindByName(UserData.Name) != null)
                {
                    DoubleCheckWindow.Open(new Rect(25, 30, 50, 40), string.Format("姓名 '{0}' 已被使用過了，你要覆寫嗎?", UserData.Name), (overwrite) =>
                    {
                        if (overwrite)
                            doSave();
                    });
                }
                else
                    doSave();
            }
            else
            {
                userDataDrawer.ShowAllInvalidMessage();
            }
        });

        DSButton btnCancel = new DSButton("清除", DocStyle.Current.DangerColor, () => DoubleCheckWindow.Open(new Rect(20, 30, 60, 40), "該動作會清除所有更改，確定要刪除嗎？", (wantLeave) =>
        {
            if (wantLeave)
            {
                OnSaveOrClear?.Invoke();
                Destory();
            }
        }));

        btnSave.style.marginRight = 50;
        btnCancel.style.marginLeft = 50;
        btnSave.style.width = Length.Percent(20);
        btnCancel.style.width = Length.Percent(20);

        visualElement.Add(btnSave);
        visualElement.Add(btnCancel);

        visualElement.style.flexGrow = 1;
        visualElement.style.alignItems = Align.Center;
        visualElement.style.justifyContent = Justify.Center;
        visualElement.style.top = StyleKeyword.Auto;
        visualElement.style.bottom = 0;
        visualElement.style.marginTop = StyleKeyword.Auto;

        sc.Add(visualElement);
        Add(sc);
    }
}

public class UI_index : MonoBehaviour
{
    public UIDocument UID;

    private DSScrollView userPageContainer;
    VisualElement rightContainer;
    VisualElement leftToolbar;
    Button btnHome, btnUserList, btnCreateNewUser;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        RuntimeWindow.ScreenElement = UID.rootVisualElement;

        UID.rootVisualElement.style.flexGrow = 1;

        VisualElement background = UID.rootVisualElement.Q<VisualElement>("background");
        background.style.backgroundColor = DocStyle.Current.BackgroundColor;

        leftToolbar = UID.rootVisualElement.Q<VisualElement>("left-toolbar");
        leftToolbar.style.backgroundColor = DocStyle.Current.SubBackgroundColor;

        userPageContainer = new DSScrollView();

        rightContainer = UID.rootVisualElement.Q<VisualElement>("right-container");
        rightContainer.style.backgroundColor = DocStyle.Current.BackgroundColor;
        rightContainer.style.flexGrow = 1;

        btnHome = new DSButton("主頁");
        btnHome.AddToClassList("left-toolbar-btn");
        btnHome.clicked += openMainPage;

        btnUserList = new DSButton("使用者");
        btnUserList.AddToClassList("left-toolbar-btn");
        btnUserList.clicked += openUserList;

        btnCreateNewUser = new DSButton("新增");
        btnCreateNewUser.AddToClassList("left-toolbar-btn");
        btnCreateNewUser.clicked += () => { openCreateNewUser(); };

        leftToolbar.Add(btnHome);
        leftToolbar.Add(btnUserList);
        leftToolbar.Add(btnCreateNewUser);

        openMainPage();

        DetailedUserDataWindow window = RuntimeWindow.GetWindow<DetailedUserDataWindow>();

        yield return null;

        window.SetLayoutPercentAnyway(new Rect(0, 0, 1, 1));
        window.Close();
    }
    (int newIndex, UserData userData) getRandData(int prevIndex)
    {
        var dataList = UserDataHandler.Datas;
        if (dataList.Count == 1)
            return (0, dataList[0]);
        if (dataList.Count == 0)
            return (-1, null);
        var resultIndex = prevIndex;
        while (resultIndex ==  prevIndex) {
            resultIndex = Random.Range(0, dataList.Count);
        }
        return (resultIndex, dataList[resultIndex]);
    }

    private void clearPage()
    {
        rightContainer.Clear();
        var border = new ISBorder(Color.clear, 0);
        foreach(var ve in leftToolbar.Children())
            ve.style.SetIS_Style(border);
    }
    private void openMainPage() 
    {
        clearPage();
        btnHome.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2.5f));
        VisualElement root = new VisualElement();
        root.style.alignItems = Align.Center;
        root.style. flexGrow = 1;
        DSLabel label = new DSLabel("使用者界面設計和評鑑");
        label.style.fontSize = label.style.fontSize.value.value * 2f;
        label.style.marginTop = DocStyle.Current.LineHeight;
        DSTextElement memberName = new DSTextElement("李睿穎　丘廷豪　許治暘"); memberName.style.fontSize = memberName.style.fontSize.value.value * 1.8f;
        memberName.style.marginBottom = 30;
        DSHorizontal randomUser = new DSHorizontal();
        VisualElement displayRandomUser = new VisualElement();
        displayRandomUser.style.justifyContent = Justify.Center;
        randomUser.Add(displayRandomUser);
        VisualElement userImage = new VisualElement();
        DSTextElement userName = new DSTextElement();
        userName.style.marginTop = 15;
        displayRandomUser.Add(userImage);
        displayRandomUser.Add(userName);
        userImage.style.width = 250;
        userImage.style.height = 250;
        userImage.style.SetIS_Style(ISRadius.Pixel(30));
        userName.style.fontSize = userName.style.fontSize.value.value * 1.8f;
        userName.style.unityTextAlign = TextAnchor.MiddleCenter;

        Button getRandomUser = new DSButton("抽獎");
        getRandomUser.style.unityTextAlign = TextAnchor.MiddleCenter;
        getRandomUser.style.fontSize = getRandomUser.style.fontSize.value.value * 1.8f;
        getRandomUser.style.SetIS_Style(ISRadius.Pixel(7));
        getRandomUser.style.marginBottom = 15;
        getRandomUser.style.maxHeight = 70;
        getRandomUser.style.minWidth = 200;
        getRandomUser.style.marginTop = StyleKeyword.Auto;
        var data = getRandData(-1);
        Action setRand = () =>
        {
            data = getRandData(data.newIndex);
            Texture2D texture;
            if (data.userData.Base64Icon != "")
            {
                texture = new Texture2D(1, 1);
                texture.LoadImage(Convert.FromBase64String(data.userData.Base64Icon));
            }
            else
            {
                texture = Resources.Load<Texture2D>("Image/default_icon");
            }
            userImage.style.backgroundImage = Background.FromTexture2D(texture);
            userName.text = data.userData.Name;
        };
        setRand();
        bool randing = false;
        getRandomUser.clicked += ()=>
        {
            int mili = 25;
            randing = true;
            displayRandomUser.style.opacity = 0.6f;
            displayRandomUser.pickingMode = PickingMode.Ignore;
            var item = getRandomUser.schedule.Execute(() =>
            {
                setRand();
            });
            item.Until(() =>
            {
                mili = (int)(mili * 1.2f);
                item.Every(mili);
                if (mili > 1250)
                {
                    item.Pause();
                    displayRandomUser.style.opacity = 1f;
                    displayRandomUser.pickingMode = PickingMode.Position;
                    randing = false;
                    return true;
                }
                return false;
            });
        };

        userImage.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (randing) return;
            DetailedUserDataWindow.Open(data.userData, true, false);
        });

        root.Add(label);
        root.Add(memberName);
        root.Add(randomUser);
        root.Add(getRandomUser);
        rightContainer.Add(root);
    }
    private void openUserList()
    {
        clearPage();
        btnUserList.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2.5f));
        userPageContainer.Clear();

        foreach (UserData data in UserDataHandler.Datas)
        {
            SimpleUserDataVisual simpleUserDataVisual = new SimpleUserDataVisual(data);
            simpleUserDataVisual.RegisterCallback<PointerDownEvent>((evt) => DetailedUserDataWindow.Open(data, true));
            userPageContainer.Add(simpleUserDataVisual);

            var localData = data;
            DSButton editBtn = new DSButton("編輯", DocStyle.Current.HintColor);
            editBtn.style.SetIS_Style(ISRadius.Pixel(10));
            editBtn.style.position = Position.Absolute;
            editBtn.style.top = 10;
            editBtn.style.right = 10;
            editBtn.style.left = StyleKeyword.Auto;
            editBtn.style.bottom = StyleKeyword.Auto;
            editBtn.style.marginTop = 0;
            editBtn.style.marginRight = 0;
            editBtn.style.marginLeft = StyleKeyword.Auto;
            editBtn.style.marginBottom = StyleKeyword.Auto;
            editBtn.clicked += () => { openCreateNewUser(localData); };
            simpleUserDataVisual.Add(editBtn);
        }

        rightContainer.Add(userPageContainer);
    }
    private void openCreateNewUser(UserData editData = null)
    {
        clearPage();
        btnCreateNewUser.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2.5f));

        CreateNewUserWindow window = RuntimeWindow.GetWindow<CreateNewUserWindow>();
        window.OnSaveOrClear = openMainPage;
        window.EnableTab = false;

        rightContainer.Add(window);

        window.style.width = Length.Percent(100);
        window.style.height = Length.Percent(100);
        if(editData != null)
        {
            window.UserDataDrawer.value = editData;
            window.UserDataDrawer.ShowAllInvalidMessage();
        }
    }

}

public class SimpleUserDataVisual : VisualElement
{
    private Texture2D texture;
    public SimpleUserDataVisual(UserData data)
    {
        style.backgroundColor = DocStyle.Current.BackgroundColor;
        style.SetIS_Style(ISMargin.Pixel(7));
        style.marginBottom = 0;
        style.paddingLeft = 20;
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.Center;
        RegisterCallback<PointerEnterEvent>(evt => style.backgroundColor = DocStyle.Current.SubBackgroundColor);
        RegisterCallback<PointerLeaveEvent>(evt => style.backgroundColor = Color.clear);
        VisualElement preview = new VisualElement();
        preview.style.marginRight = 20;
        preview.style.SetIS_Style(ISRadius.Pixel(10));

        texture = new Texture2D(1, 1);
        if (data.Base64Icon != "")
            texture.LoadImage(Convert.FromBase64String(data.Base64Icon));
        else
            texture = Resources.Load<Texture2D>("Image/default_icon");

        preview.style.backgroundImage = new StyleBackground(texture);

        preview.style.width = 100;
        preview.style.height = 100;

        VisualElement rightContainer = new VisualElement();
        rightContainer.style.flexGrow = 1;

        DSLabel nameElement = new DSLabel(data.Name);
        DSLabel contactElement = new DSLabel("聯絡方式：" + data.Contact);
        DSLabel researchTopicElement = new DSLabel("研究主題：" + data.ResearchTopic);

        rightContainer.Add(nameElement);
        rightContainer.Add(contactElement);
        rightContainer.Add(researchTopicElement);

        Add(preview);
        Add(rightContainer);
    }
}
